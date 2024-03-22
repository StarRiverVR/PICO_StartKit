using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if USING_RENDER_URP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;
using System.Text;
using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace com.vivo.codelibrary
{
    public class UnityHelper
    {

#if DEVELOPMENT_BUILD
        static bool isDevelopmentBuild = true;
#else
        static bool isDevelopmentBuild = false;
#endif

        public static bool IsDevelopmentBuild
        {
            get
            {
                return isDevelopmentBuild;
            }
        }

        /// <summary>
        /// 设置帧率
        /// rate==-1：关闭帧率设置
        /// 手机一般设置为30帧
        /// 设置帧率的同时会关闭垂直同步
        /// 用法 this.SetFrameRate(0);
        /// </summary>
        public static void SetFrameRate(int rate = 30)
        {
            if (rate <= 0)
            {
                rate = -1;
            }
            Application.targetFrameRate = rate;
        }

        /// <summary>
        /// 设置垂直同步
        /// 0:关闭垂直同步
        /// 开启垂直同步的时候 自定义帧率将失去作用
        /// 用法 this.SetVSyncCount(0);
        /// </summary>
        /// <param name="count"></param>
        public static void SetVSyncCount(int count)
        {
            if (count < 0)
            {
                count = 0;
            }
            if (count > 2)
            {
                count = 2;
            }
            QualitySettings.vSyncCount = count;
        }
    }
}

