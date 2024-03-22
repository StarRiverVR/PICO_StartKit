using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace com.vivo.codelibrary
{
    public sealed partial class VLog
    {
        public struct LogConfigData
        {
            // 日志模式
            public bool Debug;

            public string Tag;// 日志TAG
            public LogLevel LogLevel;// 日志等级
            public bool Cache;// 缓存日志
            public string CachePath;// 缓存路径
        }

        public static LogConfigData s_logConfigData = new LogConfigData()
        {
            // 模式
#if DEBUG_LOG
            Debug = true,
#else
            Debug = false,
#endif

            // 日志TAG
            Tag = k_tag,

            // 日志等级
            LogLevel = LogLevel.Info,

            // 缓存开关
#if DEBUG_CACHE
            Cache = true,
#else
            Cache = false, 
#endif

            // 缓存路径
            CachePath = FilePath.DebugLogDir
        };
    }
}