namespace ET.Server
{
    [MessageHandler(SceneType.LoginCenter)]
    public class R2L_LoginAccountHandler : MessageHandler<Scene, R2L_LoginAccountRequest, L2R_LoginAccountRequest>
    {
        protected override async ETTask Run(Scene scene, R2L_LoginAccountRequest request, L2R_LoginAccountRequest response)
        {

            await ETTask.CompletedTask;
        }
    }
}