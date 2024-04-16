namespace ET
{
    /**
     * <summary>
     * 初始化服务器与客户端通用组件的入口事件处理器
     * </summary>
     */
    [Event(SceneType.Main)]
    public class EntryEvent1_InitShare : AEvent<Scene, EntryEvent1>
    {
        /**
         * <summary>
         * 在场景初始化阶段执行，用于添加通用组件到场景中
         * </summary>
         * <param name="root">场景根节点</param>
         * <param name="args">入口事件参数</param>
         * <returns>异步任务</returns>
         */
        protected override async ETTask Run(Scene root, EntryEvent1 args)
        {
            // 添加定时器组件
            root.AddComponent<TimerComponent>();

            // 添加协程锁组件
            root.AddComponent<CoroutineLockComponent>();

            // 添加对象等待组件
            root.AddComponent<ObjectWait>();

            // 添加消息邮箱组件（无序消息类型）
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);

            // 添加内部发送者处理组件
            root.AddComponent<ProcessInnerSender>();

            // 等待任务完成
            await ETTask.CompletedTask;
        }
    }

}