using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public class SystemQuit
    {
        /// <summary>
        /// 关闭APP
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}


