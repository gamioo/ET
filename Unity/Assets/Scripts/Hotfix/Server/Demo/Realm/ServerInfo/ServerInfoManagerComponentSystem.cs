namespace ET.Server
{
    [EntitySystemOf(typeof(ServerInfoManagerComponent))]
    [FriendOf(typeof(ET.Server.ServerInfoManagerComponent))]
    [FriendOf(typeof(ET.ServerInfo))]
    public static partial class ServerInfoManagerComponentSystem
    {
        [EntitySystem]
        public static void Awake(this ET.Server.ServerInfoManagerComponent self)
        {
            self.Load();
        }

        [EntitySystem]
        public static void Destroy(this ET.Server.ServerInfoManagerComponent self)
        {
            foreach (var serverInfoRef in self.ServerInfos)
            {
                ServerInfo serverInfo = serverInfoRef;
                serverInfo?.Dispose();
            }
            self.ServerInfos.Clear();
        }
        public static void Load(this ServerInfoManagerComponent self)
        {
            // 清空原有的服务器信息
            foreach (EntityRef<ServerInfo> serverInfoRef in self.ServerInfos)
            {
                ServerInfo serverInfo = serverInfoRef;
                serverInfo?.Dispose();
            }
            self.ServerInfos.Clear();

            // 获取所有服务器配置信息
            var serverInfoConfigs = StartZoneConfigCategory.Instance.GetAll();

            // 添加符合要求的服务器信息
            foreach (var info in serverInfoConfigs.Values)
            {
                if (info.ZoneType != 1)
                {
                    continue;
                }

                // 创建新的服务器信息对象
                var newServerInfo = self.AddChildWithId<ServerInfo>(info.Id);
                newServerInfo.ServerName = info.DBName;
                newServerInfo.Status = (int)ServerStatus.Normal;

                // 将新的服务器信息添加到集合中
                self.ServerInfos.Add(newServerInfo);
            }
        }
    }
}