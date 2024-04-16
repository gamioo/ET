using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEditor.PackageManager;

namespace ET
{
    public class UnityLogger: ILog
    {
        public void Trace(string msg)
        {
            UnityEngine.Debug.Log(GetMessage(msg, LogLevel.Verbose));
        }

        public void Debug(string msg)
        {
            UnityEngine.Debug.Log(GetMessage(msg, LogLevel.Debug));
        }

        public void Info(string msg)
        {
            UnityEngine.Debug.Log(GetMessage(msg, LogLevel.Info));
        }

        public void Warning(string msg)
        {
            UnityEngine.Debug.LogWarning(GetMessage(msg, LogLevel.Warn));
        }

        public void Error(string msg)
        {
            UnityEngine.Debug.LogError(GetMessage(msg, LogLevel.Error));
        }

        public void Error(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        public void Trace(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(GetMessage(message, LogLevel.Verbose), args);
        }

        public void Warning(string message, params object[] args)
        {
            UnityEngine.Debug.LogWarningFormat(GetMessage(message, LogLevel.Warn), args);
        }

        public void Info(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(GetMessage(message, LogLevel.Info), args);
        }

        public void Debug(string message, params object[] args)
        {
            UnityEngine.Debug.LogFormat(GetMessage(message, LogLevel.Debug), args);
        }

        public void Error(string message, params object[] args)
        {
            UnityEngine.Debug.LogErrorFormat(GetMessage(message, LogLevel.Error), args);
        }

        private static string GetMessage(string msg, LogLevel level)
        {
            // #if !RELEASE  DEVELOP || UNITY_STANDALONE||DEVELOP
#if UNITY_EDITOR
            StackTrace stackTrace = new(true);
            StackFrame frame = stackTrace.GetFrame(3); // 获取调用堆栈的第二层
            string filePath = frame.GetFileName();
            string className = Path.GetFileNameWithoutExtension(filePath); // 获取文件名（不包含扩展名）
            int lineNumber = frame.GetFileLineNumber();
            string threadName = Thread.CurrentThread.Name;
            msg = $"{level.ToString()} [{threadName}][{className}:{lineNumber}] - {msg}";
#endif
            return msg;
        }
    }
}