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

            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoIsNull;
                session.Disconnect().Coroutine();
                return;
            }

            if (!Regex.IsMatch(request.Account.Trim(), @"^(?=,*[0-9].*)(?=,*[A-Z].*)(?=,*[a-z].*).{6,15}$"))
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
                using (await coroutineLockComponent.Wait(CoroutineLockType.LoginAccount, request.Account.GetLongHashCode()))
                {
                    DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    List<Account> list = await dbComponent.Query<Account>(entity => entity.AccountName == request.Account);
                    Account account = null;
                    if (list.Count == 0)
                    {
                        account = session.AddChild<Account>();
                        account.AccountName = request.Account.Trim();
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
                    r2LLoginAccountRequest.AccountName = request.Account;
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    L2R_LoginAccountRequest loginAccountResponse =
                            await session.Fiber().Root.GetComponent<MessageSender>().Call(loginCenterConfig.ActorId, r2LLoginAccountRequest) as
                                    L2R_LoginAccountRequest;

                    if (loginAccountResponse.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = loginAccountResponse.Error;
                        session?.Disconnect().Coroutine();
                        account?.Dispose();
                        return;
                    }

                    Session otherSession = session.Root().GetComponent<AccountSessionsComponent>().Get(request.Account);
                    otherSession?.Send(A2C_Disconnect.Create());
                    otherSession?.Disconnect().Coroutine();
                    session.Root().GetComponent<AccountSessionsComponent>().Add(request.Account, session);
                    session.AddComponent<AccountCheckOutTimeComponent, string>(request.Account);
                    string Token = TimeInfo.Instance.ServerNow().ToString() + RandomGenerator.RandomNumber(int.MinValue, int.MaxValue);
                    session.Root().GetComponent<TokenComponent>().Remove(request.Account);
                    session.Root().GetComponent<TokenComponent>().Add(request.Account, Token);
                }
            }
        }
    }
}