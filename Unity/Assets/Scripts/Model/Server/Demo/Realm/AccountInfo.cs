namespace ET.Server
{
    [ChildOf(typeof(AccountInfosComponent))]
    public class AccountInfo:Entity,IAwake
    {

        private string _account;
        private string _password;

        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

    }
}

