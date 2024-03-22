using System;

namespace com.vivo.codelibrary
{
    public sealed partial class DebugLogCache
    {
        public static string s_cachePath = FilePath.DebugLogDir;//log保存路径

        private static int saveLogDay = 90;//保存多天以内的日志

        private static int sizeLimit = 0; //log的size限制

        private static int sizeMinLimit = 1024;//Log最小的size，单位bit

        private static int sizeMaxLimitMB = 20;//log的最大size，单位MB

        private static bool isDelOldDir = false;//是否删除？

        private static object isDelOldDirLock = new object();//log删除时的线程锁

        /// <summary>
        /// 设置缓存配置
        /// </summary>
        public static void SetCacheConfig(string path)
        {
            s_cachePath = path;
            UnityEngine.Debug.Log($"[Log]  SetCacheConfig s_cachePath {path}");
        }

        static int SizeLimit
        {
            get
            {
                if (sizeLimit < sizeMinLimit)
                {
                    sizeLimit = (int)FileWriteHelper.Instance.GetSizeByMB(sizeMaxLimitMB);
                }

                return sizeLimit;
            }
            set { sizeLimit = value; }
        }

        private static string GetNowTimeString()
        {
            DateTime now = DateTime.Now;
            string timeString = string.Format("{0:00}:{1:00}:{2:00}.{3:000}", now.Hour, now.Minute, now.Second,
                now.Millisecond);
            return timeString;
        }

        /// <summary>
        /// 日志缓存本地
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public static void CacheLog(string baseTag, string logString, int level)
        {
            CacheLog(baseTag, logString, (LogLevel)level);
        }

        /// <summary>
        /// 日志缓存本地
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        public static void CacheLog(string baseTag, string logString, LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    CacheLogDebug(logString, baseTag, true);
                    break;
                case LogLevel.Info:
                    CacheLogInfo(logString, baseTag, true);
                    break;
                case LogLevel.Warning:
                    CacheLogWarning(logString, baseTag, true);
                    break;
                case LogLevel.Error:
                    CacheLogError(logString, baseTag, true);
                    break;
                case LogLevel.Max:
                    break;
                default:
                    CacheLogInfo(logString, baseTag, true);
                    break;
            }
        }

        static void CacheLogDebug(string str, string baseTag, bool clientSend)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            LogData_Log logData = DebugLogGUI.LogPool.Spawn();
            logData.SB.Clear();
            logData.SB.Append($"[{baseTag}] ");
            logData.SB.Append($"[{GetNowTimeString()}] [Debug] ");
            logData.SB.Append(str);
            //if (clientSend && !string.IsNullOrEmpty(str) && !str.Contains("SocketNotSend"))
            //{
            //    DebugLogClient.Log(str);
            //}
            //DebugLogGUI.AddInGuiLogList(logData);

            //1.写全文件
            WriteAll(logData);
            //2.写Log文件
            WriteLog(logData);
        }

        static void CacheLogInfo(string str, string baseTag, bool clientSend)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            LogData_Log logData = DebugLogGUI.LogPool.Spawn();
            logData.SB.Clear();
            logData.SB.Append($"[{baseTag}] ");
            logData.SB.Append($"[{GetNowTimeString()}] [Info] ");
            logData.SB.Append(str);
            //if (clientSend && !string.IsNullOrEmpty(str) && !str.Contains("SocketNotSend"))
            //{
            //    DebugLogClient.Log(str);
            //}
            //DebugLogGUI.AddInGuiLogList(logData);

            //1.写全文件
            WriteAll(logData);
            //2.写Log文件
            WriteLog(logData);
        }

        static void CacheLogWarning(string str, string baseTag, bool clientSend)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            LogData_Warning logData = DebugLogGUI.WarningPool.Spawn();
            logData.SB.Clear();
            logData.SB.Append($"[{baseTag}] ");
            logData.SB.Append($"[{GetNowTimeString()}] [Warning] ");
            logData.SB.Append(str);
            //if (clientSend && !string.IsNullOrEmpty(str) && !str.Contains("SocketNotSend"))
            //{
            //    DebugLogClient.LogWarning(str);
            //}
            //DebugLogGUI.AddInGuiLogList(logData);

            //1.写全文件
            WriteAll(logData);
            //2.写Warning文件
            WriteWarning(logData);
        }

        static void CacheLogError(string str, string baseTag, bool clientSend)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            LogData_Error logData = DebugLogGUI.ErrorPool.Spawn();
            logData.SB.Clear();
            logData.SB.Append($"[{baseTag}] ");
            logData.SB.Append($"[{GetNowTimeString()}] [Error] ");
            logData.SB.Append(str);
            //if (clientSend && !string.IsNullOrEmpty(str) && !str.Contains("SocketNotSend"))
            //{
            //    DebugLogClient.LogError(str);
            //}
            //DebugLogGUI.AddInGuiLogList(logData);

            //1.写全文件
            WriteAll(logData);
            //2.写Error文件
            WriteError(logData);
        }

        static DebugLogCacheData logTimeData_All = new DebugLogCacheData();

        static void WriteAll(IDebugLogData logData)
        {
            logTimeData_All.Form(s_cachePath, LogTimeType.All);
            Write(logTimeData_All, logData.SB.ToString());
        }

        static DebugLogCacheData logTimeData_Log = new DebugLogCacheData();

        static void WriteLog(LogData_Log logData)
        {
            logTimeData_Log.Form(s_cachePath, LogTimeType.Log);
            Write(logTimeData_Log, logData.SB.ToString());
        }

        static DebugLogCacheData logTimeData_Warning = new DebugLogCacheData();

        static void WriteWarning(LogData_Warning logData)
        {
            logTimeData_Warning.Form(s_cachePath, LogTimeType.WarningLog);
            Write(logTimeData_Warning, logData.SB.ToString());
        }

        static DebugLogCacheData logTimeData_Error = new DebugLogCacheData();

        static void WriteError(LogData_Error logData)
        {
            logTimeData_Error.Form(s_cachePath, LogTimeType.ErrLog);
            Write(logTimeData_Error, logData.SB.ToString());
        }

        static void Write(DebugLogCacheData logTimeData, string str)
        {
            DelOldDir();
            FileWriteHelper.Instance.WriteLogAsyn(logTimeData.WritePath, logTimeData.LockKey, str, SizeLimit, null);
        }

        /// <summary>
        /// 删除以前的Log
        /// </summary>
        static void DelOldDir()
        {
            lock (isDelOldDirLock)
            {
                if (isDelOldDir)
                {
                    return;
                }
                isDelOldDir = true;
                int nowY = DateTime.Now.Year;
                int nowM = DateTime.Now.Month;
                int nowD = DateTime.Now.Day;
                int now = nowY * 12 * 30 + nowM * 30 + nowD;
                string dir = s_cachePath;
                string dirLockKey = dir.PathToLower();
                lock (FileLock.GetStringLock(dirLockKey))
                {
                    FileWriteHelper.Instance.Close(dir, dirLockKey);
                    if (System.IO.Directory.Exists(dir))
                    {
                        string[] strs = System.IO.Directory.GetDirectories(dir);
                        for (int i = 0; i < strs.Length; ++i)
                        {
                            try
                            {
                                string dirName = strs[i].GetNameDeleteSuffix();
                                string[] times = dirName.Split('-');
                                if (times.Length == 3)
                                {
                                    int y = int.Parse(times[0]);
                                    int m = int.Parse(times[1]);
                                    int d = int.Parse(times[2]);
                                    int old = y * 12 * 30 + m * 30 + d;
                                    if ((now - old) > saveLogDay)
                                    {
                                        string dirPath = strs[i];
                                        string dirPathLockKey = dirPath.PathToLower();
                                        lock (FileLock.GetStringLock(dirPathLockKey))
                                        {
                                            FileWriteHelper.Instance.Close(dirPath, dirPathLockKey);
                                            System.IO.Directory.Delete(dirPath, true);
                                        }
                                    }
                                }
                            }
                            catch (System.Exception ex)
                            {
                                UnityEngine.Debug.LogException(ex);
                            }
                        }
                    }
                }
            }
        }
    }
}