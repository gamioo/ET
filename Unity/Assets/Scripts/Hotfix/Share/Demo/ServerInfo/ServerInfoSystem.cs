namespace ET
{
    [EntitySystemOf(typeof(ServerInfo))]
    [FriendOf(typeof(ServerInfo))]
    public static partial class ServerInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.ServerInfo self)
        {

        }

        public static void FromMessage(this ET.ServerInfo self, ServerInfoProto serverInfoProto)
        {
            self.Status = serverInfoProto.Status;
            self.ServerName = serverInfoProto.ServerName;
        }

        public static ServerInfoProto ToMessage(this ET.ServerInfo self)
        {
            ServerInfoProto serverInfoProto = ServerInfoProto.Create();
            serverInfoProto.Id = (int)self.Id;
            serverInfoProto.ServerName = self.ServerName;
            serverInfoProto.Status = self.Status;
            return serverInfoProto;
        }
    }
}

