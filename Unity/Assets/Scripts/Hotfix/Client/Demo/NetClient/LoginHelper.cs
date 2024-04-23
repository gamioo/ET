namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string account, string password)
        {

            root.RemoveComponent<ClientSenderComponent>();
            ClientSenderComponent clientSenderComponent = root.AddComponent<ClientSenderComponent>();

            NetClient2Main_Login response = await clientSenderComponent.LoginAsync(account, password);

            if (response.Error!=ErrorCode.ERR_Success)
            {
                Log.Error($"response.Error: {response.Error}");
                return;
            }
            Log.Debug("登录请求成功！！！");
           string Token = response.Token;
           // 获取服务器列表
           C2R_GetServerInfos c2RGetServerInfos = C2R_GetServerInfos.Create();
           c2RGetServerInfos.Account = account;
           c2RGetServerInfos.Token = response.Token;

           R2C_GetServerInfos r2CGetServerInfos = await clientSenderComponent.Call(c2RGetServerInfos) as R2C_GetServerInfos;
           if (r2CGetServerInfos.Error != ErrorCode.ERR_Success)
           {
               Log.Error("请求服务器列表失败!");
               return;
           }

           ServerInfoProto serverInfoProto = r2CGetServerInfos.ServerInfosList[0];
           Log.Debug($"请求服务器列表成功, 区服名称：{serverInfoProto.ServerName} 区服ID：{serverInfoProto.Id}");
           // 获取区服角色列表
           C2R_GetRoles c2RGetRoles = C2R_GetRoles.Create();
           c2RGetRoles.Token = Token;
           c2RGetRoles.Account = account;
           c2RGetRoles.ServerId = serverInfoProto.Id;
           R2C_GetRoles r2CGetRoles = await clientSenderComponent.Call(c2RGetRoles) as R2C_GetRoles;
           if (r2CGetRoles.Error != ErrorCode.ERR_Success)
           {
               Log.Error("请求区服角色列表失败!");
               return;
           }

           // 角色信息
           RoleInfoProto roleInfoProto = default;
           if (r2CGetRoles.RoleInfo.Count <= 0)
           {
               // 无角色信息，则创建角色信息
               C2R_CreateRole c2RCreateRole = C2R_CreateRole.Create();
               c2RCreateRole.Token = Token;
               c2RCreateRole.Account = account;
               c2RCreateRole.ServerId = serverInfoProto.Id;
               c2RCreateRole.Name = account;
               R2C_CreateRole r2CCreateRole = await clientSenderComponent.Call(c2RCreateRole) as R2C_CreateRole;
               if (r2CCreateRole.Error != ErrorCode.ERR_Success)
               {
                   Log.Error("创建角色失败!");
                   return;
               }
               roleInfoProto = r2CCreateRole.RoleInfo;
           }


            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}