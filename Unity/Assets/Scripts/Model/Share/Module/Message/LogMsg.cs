using System.Collections.Generic;

namespace ET
{
    /**对消息的打印*/
    public class LogMsg: Singleton<LogMsg>, ISingletonAwake
    {
        private readonly HashSet<ushort> ignore = new()
        {
            OuterMessage.C2G_Ping, 
            OuterMessage.G2C_Ping, 
            OuterMessage.C2G_Benchmark, 
            OuterMessage.G2C_Benchmark,
        };

        public void Awake()
        {
        }

        public void Recv(Fiber fiber, object msg)
        {
            ushort opcode = OpcodeType.Instance.GetOpcode(msg.GetType());
            if (this.ignore.Contains(opcode))
            {
                return;
            }
            Log.Debug($"recv type:{opcode},name:{msg.GetType()},args:{msg}");
            fiber.Log.Trace(msg.ToString());
        }
        public void Send(Fiber fiber, object msg)
        {
            ushort opcode = OpcodeType.Instance.GetOpcode(msg.GetType());
            if (this.ignore.Contains(opcode))
            {
                return;
            }
            Log.Debug($"send type:{opcode},name:{msg.GetType()},args:{msg}");
           fiber.Log.Trace(msg.ToString());
        }
    }
}