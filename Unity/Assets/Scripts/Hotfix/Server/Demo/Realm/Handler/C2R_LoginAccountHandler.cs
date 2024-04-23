using System;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    public class C2R_LoginAccountHandler : MessageSessionHandler<C2R_LoginAccount,
        R2C_LoginAccount>
    {
        protected override async ETTask Run(Session session, C2R_LoginAccount request, R2C_LoginAccount response)
        {
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = ErrorCode.ERR_RequestRepeatedly;
                session.Disconnect().Coroutine();
                return;
            }

            if (string.IsNullOrEmpty(request.AccountName) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoIsNull;
                session.Disconnect().Coroutine();
                return;
            }

            if (!Regex.IsMatch(request.AccountName.Trim(), "^[A-Za-z0-9]$"))
            {
                response.Error = ErrorCode.ERR_AccountNameFormError;
                session.Disconnect().Coroutine();
                return;
            }

            if (!Regex.IsMatch(request.Password.Trim(), @"^[A-Za-z0-9]+$"))
            {
                response.Error = ErrorCode.ERR_PasswordFormError;
                session.Disconnect().Coroutine();
                return;
            }

            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.LoginAccount, request.AccountName.GetLongHashCode()))
                {
                    DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    List<Account> list = await dbComponent.Query<Account>(entity => entity.AccountName == request.AccountName);
                    Account account = null;
                    if (list.Count == 0)
                    {
                        account = session.AddChild<Account>();
                        account.AccountName = request.AccountName.Trim();
                        account.Password = request.Password;
                        account.AddTime = TimeInfo.Instance.Now();
                        account.AccountType = AccountType.GENERAL;

                        await dbComponent.Save(account);
                    }
                    else
                    {
                        account = list[0];
                        session.AddChild(account);
                        if (account.AccountType == AccountType.BLACKLIST)
                        {
                            response.Error = ErrorCode.ERR_AccountInBlackListError;
                            session.Disconnect().Coroutine();
                            account?.Dispose();
                            return;
                        }

                        if (account.Password != request.Password)
                        {
                            response.Error = ErrorCode.ERR_LoginPasswordError;
                            //   CloseSession(session).Coroutine();
                            return;
                        }
                    }

                    account.LastLoginTime = TimeInfo.Instance.Now();
                    account.LastLoginIp = session.RemoteAddress.ToString();
                    await dbComponent.Save(account);
                    R2L_LoginAccountRequest r2LLoginAccountRequest = R2L_LoginAccountRequest.Create();
                    r2LLoginAccountRequest.AccountName = request.AccountName;
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    MessageSender messageSender = session.Fiber().Root.GetComponent<MessageSender>();
                    L2R_LoginAccountRequest loginAccountResponse =
                            await messageSender.Call(loginCenterConfig.ActorId, r2LLoginAccountRequest) as
                                    L2R_LoginAccountRequest;

                    if (loginAccountResponse.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = loginAccountResponse.Error;
                        session?.Disconnect().Coroutine();
                        account?.Dispose();
                        return;
                    }

                    Session otherSession = session.Root().GetComponent<AccountSessionsComponent>().Get(request.AccountName);
                    otherSession?.Send(A2C_Disconnect.Create());
                    otherSession?.Disconnect().Coroutine();
                    session.Root().GetComponent<AccountSessionsComponent>().Add(request.AccountName, session);
                    session.AddComponent<AccountCheckOutTimeComponent, string>(request.AccountName);
                    string Token = TimeInfo.Instance.ServerNow().ToString() + RandomGenerator.RandomNumber(int.MinValue, int.MaxValue);
                    session.Root().GetComponent<TokenComponent>().Remove(request.AccountName);
                    session.Root().GetComponent<TokenComponent>().Add(request.AccountName, Token);
                    response.Token = Token;
                    account?.Dispose();
                }
            }
        }
    }
}