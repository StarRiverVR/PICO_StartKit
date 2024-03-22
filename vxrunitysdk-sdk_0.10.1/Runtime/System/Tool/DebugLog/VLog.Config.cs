
using UnityEngine;

namespace com.vivo.codelibrary
{
    public sealed partial class VLog
    {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialized()
        {
            // 初始化工程日志信息
            InitializedApplicationLogMessage();

            // 初始化日志配置
            InitializedLogConfig();
        }

        /// <summary>
        /// 初始化日志配置
        /// </summary>
        /// <param name="data">LogConfigData</param>
        private static void InitializedLogConfig()
        {
            UnityEngine.Debug.Log("[Log] Log init: set config");

            // 初始化日志数据
            InitializedLogData();

            // 初始化Native日志配置
            VLogPlugin.SetNativeLogConfig(false, s_cache, s_tag, (int)s_level);

            // 打印日志配置
            UnityEngine.Debug.Log($"[Log] Log Config tag[{s_tag}] level[{s_level}] cache[{s_cache}] path[{DebugLogCache.s_cachePath}]");
        }

        private static void InitializedLogData()
        {
            // TAG
            s_tag = s_logConfigData.Tag;
            // 日志等级
            if (s_logConfigData.Debug)
            {
                s_level = LogLevel.Debug;
            }
            else
            {
                s_level = s_logConfigData.LogLevel;
            }
            // 保存日志
            s_cache = s_logConfigData.Cache;
            // 日志路径
            DebugLogCache.s_cachePath = s_logConfigData.CachePath;
        }
    }
}
