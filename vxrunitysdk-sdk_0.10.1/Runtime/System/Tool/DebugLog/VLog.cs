using UnityEngine;
using System;
using com.vivo.openxr;

namespace com.vivo.codelibrary
{
    public sealed partial class VLog
    {
        const string k_tag = "VXRSDK";

        internal static string s_tag = k_tag;//一级标签

        internal static LogLevel s_level = LogLevel.Info; //默认打印的最低log等级

        internal static bool s_cache = false;//是否保存日志

        /// <summary>
        /// 打印Debug级别日志
        /// </summary>
        /// <param name="str">日志信息</param>
        public static void Debug(string str)
        {
            Debug("", str);
        }
        /// <summary>
        ///  打印Debug级别日志
        /// </summary>
        /// <param name="msgTag">日志二级标签</param>
        /// <param name="str">日志信息</param>
        public static void Debug(string msgTag, string str)
        {
#if !UNITY_EDITOR
            if (LogLevel.Debug < s_level)
            {
                return;
            }
#endif
            string msg = msgTag == "" ? $"[{str}]" : $"[{msgTag}] [{str}]";
            UnityEngine.Debug.Log(msg);
        }

        /// <summary>
        ///  打印Info级别日志
        /// </summary>
        /// <param name="str">日志信息</param>
        public static void Info(string str)
        {
            Info("", str);
        }

        /// <summary>
        /// 打印Info级别日志
        /// </summary>
        /// <param name="msgTag">日志二级标签</param>
        /// <param name="str">日志信息</param>
        public static void Info(string msgTag, string str)
        {
#if !UNITY_EDITOR
            if (LogLevel.Info < s_level)
            {
                return;
            }
#endif
            string msg = msgTag == "" ? $"[{str}]" : $"[{msgTag}] [{str}]";
            UnityEngine.Debug.Log(msg);
        }

        /// <summary>
        /// 打印Warning级别日志
        /// </summary>
        /// <param name="str">日志信息</param>
        public static void Warning(string str)
        {
            Warning("", str);
        }
        /// <summary>
        /// 打印Warning级别日志
        /// </summary>
        /// <param name="msgTag">日志二级标签</param>
        /// <param name="str">日志信息</param>
        public static void Warning(string msgTag, string str)
        {
#if !UNITY_EDITOR
            if (LogLevel.Warning < s_level)
            {
                return;
            }
#endif
            string msg = msgTag == "" ? $"[{str}]" : $"[{msgTag}] [{str}]";
            UnityEngine.Debug.LogWarning(msg);
        }

        /// <summary>
        /// 打印Error级别日志
        /// </summary>
        /// <param name="str">日志信息</param>
        public static void Error(string str)
        {
            Error("", str);
        }

        /// <summary>
        /// 打印Error级别日志
        /// </summary>
        /// <param name="msgTag">日志二级标签</param>
        /// <param name="str">日志信息</param>
        public static void Error(string msgTag, string str)
        {
#if !UNITY_EDITOR
            if (LogLevel.Error < s_level)
            {
                return;
            }
#endif
            string msg = msgTag == "" ? $"[{str}]" : $"[{msgTag}] [{str}]";
            UnityEngine.Debug.LogError(msg);
        }

        /// <summary>
        ///  打印Error(Exception)级别日志
        /// </summary>
        /// <param name="exception">日志信息</param>
        public static void Exception(Exception exception)
        {
#if !UNITY_EDITOR
            if (LogLevel.Error < s_level)
            {
                return;
            }
#endif
            UnityEngine.Debug.LogException(exception);
        }


#if !UNITY_EDITOR
             static bool isInitUnityLogMSG = false;
#endif
        /// <summary>
        /// 非编辑器状态的LOG信息
        /// </summary>
        static void InitializedApplicationLogMessage()
        {
#if !UNITY_EDITOR
            if (isInitUnityLogMSG) return;
            isInitUnityLogMSG = true;
            Application.logMessageReceivedThreaded -= LogMessageReceivedThreaded;
            Application.logMessageReceivedThreaded += LogMessageReceivedThreaded;
#endif

        }

        private static void LogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            CacheUnityLog(logString, stackTrace, type);
            SendToInformationManage(logString, stackTrace, type);                
        }

        public static void CacheUnityLog(string logString, string stackTrace, LogType level)
        {
            string extractStackTrace = StackTraceUtility.ExtractStackTrace();
            string str;
            LogLevel l;
            switch (level)
            {
                case LogType.Log:
                    l = LogLevel.Info;
                    str = string.Format(string.Intern("{0} \n [stackTrace]={1}"), logString, stackTrace);
                    break;
                case LogType.Warning:
                    l = LogLevel.Warning;
                    str = string.Format(string.Intern("{0} \n [stackTrace]={1} [ExtractStackTrace]={2}"), logString, stackTrace, extractStackTrace);
                    break;
                case LogType.Assert:
                    l = LogLevel.Error;
                    str = string.Format(string.Intern("{0} \n [stackTrace]={1} [ExtractStackTrace]={2}"), logString, stackTrace, extractStackTrace);
                    break;
                case LogType.Error:
                    l = LogLevel.Error;
                    str = string.Format(string.Intern("{0} \n [stackTrace]={1} [ExtractStackTrace]={2}"), logString, stackTrace, extractStackTrace);
                    break;
                case LogType.Exception:
                    l = LogLevel.Error;
                    str = string.Format(string.Intern("{0} \n [stackTrace]={1} [ExtractStackTrace]={2}"), logString, stackTrace, extractStackTrace);
                    break;
                default:
                    l = LogLevel.Info;
                    str = string.Format(string.Intern("{0} \n [stackTrace]={1}"), logString, stackTrace);
                    break;
            }
            DebugLogCache.CacheLog(s_tag, str, l);
        }

        public static void SendToInformationManage(string logString,string stackTrace, LogType type)
        {
            string extractStackTrace = string.Empty;
            extractStackTrace = StackTraceUtility.ExtractStackTrace();
            string str = string.Format(string.Intern("{0} [stackTrace]={1} [ExtractStackTrace]={1}"), logString, stackTrace, extractStackTrace);
            switch (type)
            {
                case LogType.Log:
                    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.Log, true, str);
                    break;
                case LogType.Warning:
                    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.LogWarning, true, str);
                    break;
                case LogType.Error:
                    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.LogError, true, str);
                    break;
                case LogType.Exception:
                    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.LogException, true, str);
                    break;
                case LogType.Assert:
                    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.LogError, true, str);
                    break;
                default:
                    InformationManager.Instance.GameInformationCenter.Send<DebugLogType>((int)DebugLogType.Log, true, str);
                    break;
            }
        }
    }
}