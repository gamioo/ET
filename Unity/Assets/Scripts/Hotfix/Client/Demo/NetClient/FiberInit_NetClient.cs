namespace ET.Client
{
    [Invoke((long)SceneType.NetClient)]
    public class FiberInit_NetClient: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            //看不懂，为什么是UnOrderedMessage，不是OrderedMessage
            root.AddComponent<MailBoxComponent, MailBoxType>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<FiberParentComponent>();
            await ETTask.CompletedTask;
        }
    }
}