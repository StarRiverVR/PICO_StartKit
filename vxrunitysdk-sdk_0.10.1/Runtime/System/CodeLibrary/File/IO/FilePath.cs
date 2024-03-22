using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

namespace com.vivo.codelibrary
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class FilePath
    {

        public static string DataPath;

        public static string PersistentDataPath;

        public static string TemporaryCachePath;

        /// <summary>
        /// 本地可读可写得一个路径
        /// 对于Editor以及Standalone版本，为了区分不同分支，使用了相对与工程Assets的一个路径
        /// </summary>
        /// <returns></returns>
        public static string LocalStorageDir;

        public static string DebugLogDir;

#if UNITY_EDITOR
        static FilePath()
        {
            if (!Application.isPlaying)
            {
                PathInit();
            }
            UnityEditor.EditorApplication.delayCall += DoSomethingPrepare;
        }

        static void DoSomethingPrepare()
        {
            if (!Application.isPlaying)
            {
                PathInit();
            }
        }
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            PathInit();
        }

        static void PathInit()
        {
            DataPath = Application.dataPath;
            PersistentDataPath = Application.persistentDataPath;
            TemporaryCachePath = Application.temporaryCachePath;
#if UNITY_EDITOR || UNITY_STANDALONE
            LocalStorageDir = DataPath + "/../Caches/";
#else
#if UNITY_ANDROID
            LocalStorageDir = PersistentDataPath+"/";
#else
            //LocalStorageDir = TemporaryCachePath+"/";
            LocalStorageDir = PersistentDataPath+"/";
#endif
            LocalStorageDir = PersistentDataPath+"/";
#endif
            lock (FileLock.GetStringLock(LocalStorageDir.PathToLower()))
            {
                if (!Directory.Exists(LocalStorageDir))
                {
                    Directory.CreateDirectory(LocalStorageDir);
                }
            }
            DebugLogDir = LocalStorageDir + "Log/";
            VLog.Warning(string.Format("[DataPath]={0}", DataPath));
            VLog.Warning(string.Format("[PersistentDataPath]={0}", PersistentDataPath));
            VLog.Warning(string.Format("[TemporaryCachePath]={0}", TemporaryCachePath));
            VLog.Warning(string.Format("[LocalStorageDir]={0}", LocalStorageDir));
            VLog.Warning(string.Format("[DebugLogDir]={0}", DebugLogDir));
        }



    }
}


