using System.Collections.Generic;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(RoleInfo))]
    public class C2R_CreateRoleHandler : MessageSessionHandler<C2R_CreateRole, R2C_CreateRole>
    {

        protected override async ETTask Run(Session session, C2R_CreateRole request, R2C_CreateRole response)
        {
            await ETTask.CompletedTask;
        }
    }
}