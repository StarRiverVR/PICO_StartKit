using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.openxr
{

    public class VXRFoveationRendering
    {
        /// <summary>
        /// 注视点渲染等级
        /// 等级越高，模糊范围越大。
        /// </summary>
        public enum Level
        {
            None = VXRPlugin.FoveationLevel.None,
            Low = VXRPlugin.FoveationLevel.Low,
            Medinum = VXRPlugin.FoveationLevel.Medinum,
            High = VXRPlugin.FoveationLevel.High,
        }

        /// <summary>
        /// 设置注视点渲染等级。
        /// </summary>
        /// <param name="level">注视点渲染等级</param>
        public static void SetFoveationLevel(Level level)
        {
            VXRPlugin.SetFoveationRenderingLevel((VXRPlugin.FoveationLevel)level);
        }

        /// <summary>
        /// 获取注视点渲染等级。
        /// </summary>
        /// <returns>当前注视点渲染等级</returns>
        public static Level GetFoveationLevel()
        {
            return (Level)VXRPlugin.GetFoveationRenderingLevel();
        }

        /// <summary>
        /// 设置是否动态匹配注视点渲染等级。
        /// 注意：开启动态匹配注视点等级后，当前前注视点等级不再是手动设置的注视点渲染等级<see cref="SetFoveationLevel"/>。
        /// 系统会根据运行环境动态匹配注视点等级，关闭动态匹配后会恢复到最后一次手动设置的注视点渲染等级。
        /// </summary>
        /// <param name="use">是否开启动态匹配</param>
        public static void SetUseDynamicFoveationLevel(bool use)
        {
            VXRPlugin.SetFoveationRenderingDynamic(use);
        }

        /// <summary>
        /// 获取当前是否开启动态匹配注视点渲染等级
        /// </summary>
        /// <returns>当前是否开启动态匹配注视点等级</returns>
        public static bool GetUseDynamicFoveationLevel()
        {
            return VXRPlugin.GetFoveationRenderingDynamic();
        }
    }
}