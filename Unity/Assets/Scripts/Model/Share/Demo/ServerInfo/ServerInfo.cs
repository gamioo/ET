namespace ET
{
    public enum ServerStatus
    {
        Normal=0,
        Stop=1,
    }

    [ChildOf]
    public class ServerInfo : Entity, IAwake
    {
        private int status;
        private string serverName;
        private string ip;
        private int port;

        public int Status
        {
            get => this.status;
            set => this.status = value;
        }

        public string ServerName
        {
            get => this.serverName;
            set => this.serverName = value;
        }

        public string IP
        {
            get => this.ip;
            set => this.ip = value;
        }

        public int Port
        {
            get => this.port;
            set => this.port = value;
        }
    }
}