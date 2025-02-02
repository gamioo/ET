﻿using System;
using System.Net;
using System.Net.Sockets;

namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginHandler: MessageHandler<Scene, Main2NetClient_Login, NetClient2Main_Login>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_Login request, NetClient2Main_Login response)
        {
            string account = request.Account;
            string password = request.Password;
            // 创建一个ETModel层的Session
            root.RemoveComponent<RouterAddressComponent>();
            // 获取路由跟realmDispatcher地址
            RouterAddressComponent routerAddressComponent =
                    root.AddComponent<RouterAddressComponent, string, int>(ConstValue.RouterHttpHost, ConstValue.RouterHttpPort);
            await routerAddressComponent.Init();
            //看不懂，为什么是UDP
            //创建和router的连接,KCP里包含TCP,和UDP模式
            root.AddComponent<NetComponent, AddressFamily, NetworkProtocol>(routerAddressComponent.RouterManagerIPAddress.AddressFamily, NetworkProtocol.UDP);
            root.GetComponent<FiberParentComponent>().ParentFiberId = request.OwnerFiberId;

            NetComponent netComponent = root.GetComponent<NetComponent>();
            
            IPEndPoint realmAddress = routerAddressComponent.GetRealmAddress(account);


            // 创建一个router Session,包括连接，并且保存到SessionComponent中
            Session session = await netComponent.CreateRouterSession(realmAddress, account, password);

                //这个消息通过router 走向realm server
                C2R_LoginAccount c2RLogin = C2R_LoginAccount.Create();
                c2RLogin.AccountName = account;
                c2RLogin.Password = password;
                R2C_LoginAccount   r2CLogin = (R2C_LoginAccount)await session.Call(c2RLogin);

            if (r2CLogin.Error== ErrorCode.ERR_Success)
            {
                root.AddComponent<SessionComponent>().Session = session;
            }
            else
            {
                session?.Dispose();
            }

            response.Token=r2CLogin.Token;
            response.Error=r2CLogin.Error;
            // // 创建一个gate Session,并且保存到SessionComponent中,还是通过router去连接gate
            // Session gateSession = await netComponent.CreateRouterSession(NetworkHelper.ToIPEndPoint(r2CLogin.Address), account, password);
            // gateSession.AddComponent<ClientSessionErrorComponent>();
            // root.AddComponent<SessionComponent>().Session = gateSession;
            // //发起登录请求
            // C2G_LoginGate c2GLoginGate = C2G_LoginGate.Create();
            // c2GLoginGate.Key = r2CLogin.Key;
            // c2GLoginGate.GateId = r2CLogin.GateId;
            // G2C_LoginGate g2CLoginGate = (G2C_LoginGate)await gateSession.Call(c2GLoginGate);
            //
            // Log.Debug("登陆gate成功!");
            //
            // response.PlayerId = g2CLoginGate.PlayerId;
        }
    }
}