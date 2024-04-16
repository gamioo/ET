using System;
using System.Net;

namespace ET.Client
{
    public static partial class RouterHelper
    {
        // 注册router
        public static async ETTask<Session> CreateRouterSession(this NetComponent netComponent, IPEndPoint address, string account, string password)
        {
            uint localConn = (uint)(account.GetLongHashCode() ^ password.GetLongHashCode() ^ RandomGenerator.RandUInt32());
            (uint recvLocalConn, IPEndPoint routerAddress) = await GetRouterAddress(netComponent, address, localConn, 0);

            if (recvLocalConn == 0)
            {
                throw new Exception($"get router fail: {netComponent.Root().Id} {address}");
            }
            
            Log.Info($"get random router: recvLocalConn:{recvLocalConn} routerAddress:{routerAddress}");
            //router连接可以重用
            Session routerSession = netComponent.Create(routerAddress, address, recvLocalConn);
            routerSession.AddComponent<PingComponent>();
            routerSession.AddComponent<RouterCheckComponent>();
            
            return routerSession;
        }
        
        public static async ETTask<(uint, IPEndPoint)> GetRouterAddress(this NetComponent netComponent, IPEndPoint address, uint localConn, uint remoteConn)
        {
            Log.Info($"start get router address: id:{netComponent.Root().Id} address:{address} localConn:{localConn} remoteConn:{remoteConn}");
            //return (RandomHelper.RandUInt32(), address);
            RouterAddressComponent routerAddressComponent = netComponent.Root().GetComponent<RouterAddressComponent>();
            IPEndPoint routerInfo = routerAddressComponent.GetAddress();
            
            uint recvLocalConn = await netComponent.Connect(routerInfo, address, localConn, remoteConn);
            
            Log.Info($"finish get router address: id:{netComponent.Root().Id} address:{address} localConn:{localConn} remoteConn:{remoteConn} recvLocalConn:{recvLocalConn} routerInfo:{routerInfo}");
            return (recvLocalConn, routerInfo);
        }

        // 向router申请
        private static async ETTask<uint> Connect(this NetComponent netComponent, IPEndPoint routerAddress, IPEndPoint realAddress, uint localConn, uint remoteConn)
        {
            uint synFlag = remoteConn == 0? KcpProtocalType.RouterSYN : KcpProtocalType.RouterReconnectSYN;

            // 注意，session也以localConn作为id，所以这里不能用localConn作为id
            long id = (long)(((ulong)localConn << 32) | remoteConn);
            using RouterConnector routerConnector = netComponent.AddChildWithId<RouterConnector>(id);
            
            int count = 20;
            byte[] sendCache = new byte[512];

            uint connectId = RandomGenerator.RandUInt32();
            sendCache.WriteTo(0, synFlag);
            sendCache.WriteTo(1, localConn);
            sendCache.WriteTo(5, remoteConn);
            sendCache.WriteTo(9, connectId);
            byte[] addressBytes = realAddress.ToString().ToByteArray();
            Array.Copy(addressBytes, 0, sendCache, 13, addressBytes.Length);
            TimerComponent timerComponent = netComponent.Root().GetComponent<TimerComponent>();
            Log.Info($"router connect,local:{localConn} remote:{remoteConn} router:{routerAddress} real:{realAddress}");
            long lastSendTimer = 0;

            while (true)
            {
                long timeNow = TimeInfo.Instance.ClientFrameTime();
                if (timeNow - lastSendTimer > 300)
                {
                    if (--count < 0)
                    {
                        Log.Error($"router connect timeout fail! {localConn} {remoteConn} {routerAddress} {realAddress}");
                        return 0;
                    }

                    lastSendTimer = timeNow;
                    // 发送
                    routerConnector.Connect(sendCache, 0, addressBytes.Length + 13, routerAddress);
                }

                await timerComponent.WaitFrameAsync();
                
                if (routerConnector.Flag == 0)
                {
                    continue;
                }

                return localConn;
            }
        }
    }
}