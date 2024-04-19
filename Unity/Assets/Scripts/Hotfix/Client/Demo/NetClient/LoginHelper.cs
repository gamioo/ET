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
           string token = response.Token;

            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}