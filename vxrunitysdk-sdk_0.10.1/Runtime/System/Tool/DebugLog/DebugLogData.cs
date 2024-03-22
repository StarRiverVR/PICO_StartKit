using System;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error,
        Max
    }
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum DebugLogType
    {
        Log,
        LogWarning,
        LogError,
        LogException,
        JsonData,
    }

    enum LogTimeType
    {
        All,
        Log,
        WarningLog,
        ErrLog,
    }

    public enum LogState
    {
        Log,
        WarningLog,
        ErrLog,
    }

    public class IDebugLogData : ISimplePoolData
    {
        public LogState LogState;
        public StringBuilder SB;
        bool isUsed = false;

        public bool IsUsed
        {
            get { return isUsed; }
        }

        public virtual void PutIn()
        {
            isUsed = false;
        }

        public virtual void PutOut()
        {
            isUsed = true;
        }

        bool disposed = false;

        public bool Disposed
        {
            get { return disposed; }
        }

        public virtual void Dispose()
        {
            disposed = true;
        }
    }

    public class LogData_Log : IDebugLogData
    {
        public LogData_Log()
        {
            LogState = LogState.Log;
            SB = new StringBuilder(256);
        }

        public override void PutOut()
        {
            base.PutOut();
            SB.Clear();
            if (SB.Length > 256)
            {
                SB.EnsureCapacity(256);
                SB.Capacity = 256;
            }
        }
    }

    public class LogData_Warning : IDebugLogData
    {
        public LogData_Warning()
        {
            LogState = LogState.WarningLog;
            SB = new StringBuilder(512);
        }

        public override void PutOut()
        {
            base.PutOut();
            SB.Clear();
            if (SB.Length > 512)
            {
                SB.EnsureCapacity(512);
                SB.Capacity = 512;
            }
        }
    }

    public class LogData_Error : IDebugLogData
    {
        public LogData_Error()
        {
            LogState = LogState.ErrLog;
            SB = new StringBuilder(512);
        }

        public override void PutOut()
        {
            base.PutOut();
            SB.Clear();
            if (SB.Length > 512)
            {
                SB.EnsureCapacity(512);
                SB.Capacity = 512;
            }
        }
    }
}
