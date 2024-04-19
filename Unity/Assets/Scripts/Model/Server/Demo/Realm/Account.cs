using System;

namespace ET.Server
{
    public enum AccountType
    {
        GENERAL =0,
        BLACKLIST =1,
    }
    [ChildOf(typeof(Session))]
    public class Account:Entity,IAwake
    {
        private string _accountName;//账户名
        private string _password;
        private DateTime _addTime;
        private AccountType _accountType;
        private DateTime _lastLoginTime;
        private DateTime _lastLogoutTime;

        private string _lastLoginIp;

        public string AccountName
        {
            get => this._accountName;
            set => this._accountName = value;
        }

        public string Password
        {
            get => this._password;
            set => this._password = value;
        }

        public DateTime AddTime
        {
            get => this._addTime;
            set => this._addTime = value;
        }

        public AccountType AccountType
        {
            get => this._accountType;
            set => this._accountType = value;
        }

        public DateTime LastLoginTime
        {
            get => this._lastLoginTime;
            set => this._lastLoginTime = value;
        }

        public DateTime LastLogoutTime
        {
            get => this._lastLogoutTime;
            set => this._lastLogoutTime = value;
        }


        public string LastLoginIp
        {
            get => this._lastLoginIp;
            set => this._lastLoginIp = value;
        }
    }

}



