namespace ET.Server
{
    [EntitySystemOf(typeof(AccountSessionsComponent))]
    [FriendOf(typeof(AccountSessionsComponent))]
    public static partial class AccountSessionsComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.AccountSessionsComponent self)
        {
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.AccountSessionsComponent self)
        {
            self.AccountSessionDictionary.Clear();
        }

        public static Session Get(this AccountSessionsComponent self, string accountName)
        {
            if (!self.AccountSessionDictionary.TryGetValue(accountName, out EntityRef<Session> session))
            {
                return null;
            }

            return session;
        }
        public static void Add(this AccountSessionsComponent self, string accountName, EntityRef<Session> session)
        {
             self.AccountSessionDictionary.TryAdd(accountName, session);
        }

        public static void Remove(this AccountSessionsComponent self, string accountName)
        {
             self.AccountSessionDictionary.Remove(accountName);
        }
    }
}