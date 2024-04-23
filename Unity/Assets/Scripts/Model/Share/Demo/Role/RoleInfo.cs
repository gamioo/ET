namespace ET
{
    public enum RoleInfoState
    {
        Normal = 0,
        Freeze,
    }


    public class RoleInfo : Entity, IAwake
    {
        private string _Name;
        private int _ServerId;
        private RoleInfoState _State;
        private string _Account;
        private long _LastLoginTime;
        private long _CreateTime;

        public string Name
        {
            get => this._Name;
            set => this._Name = value;
        }

        public int ServerId
        {
            get => this._ServerId;
            set => this._ServerId = value;
        }

        public RoleInfoState State
        {
            get => this._State;
            set => this._State = value;
        }

        public string Account
        {
            get => this._Account;
            set => this._Account = value;
        }

        public long LastLoginTime
        {
            get => this._LastLoginTime;
            set => this._LastLoginTime = value;
        }

        public long CreateTime
        {
            get => this._CreateTime;
            set => this._CreateTime = value;
        }
    }
}