using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
#if UNITY_EDITOR
    [InitializeOnLoad]
    public class DebugLogToolInitialize
    {
        static DebugLogToolInitialize()
        {
            EditorApplication.update += Update;
        }

        static int isFirst = 0;

        static void Update()
        {
            if (isFirst==3)
            {
                if (!Application.isPlaying)
                {
                    DebugLogTool.Initialize();
                }
            }
            isFirst++;
            if (isFirst==int.MaxValue-1)
            {
                isFirst = 4;
            }
        }
    }
#endif


    public class DebugLogTool
    {

        [RuntimeInitializeOnLoadMethod]
        public static void Initialize()
        {

            //DebugLogCoroutine.StopCoroutine();
            //DebugLogCoroutine.StartCoroutine();
        }

    }

}

