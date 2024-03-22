using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_0_2
        {
            /// <summary>
            /// 设置注视点等级
            /// </summary>
            /// <param name="level">注视点等级</param>
            /// <returns>函数调用状态</returns>
            [DllImport(pluginName, EntryPoint = "vxr_SetFoveationRenderingLevel")]
            public static extern Result vxr_SetFoveationRenderingLevel(FoveationLevel level);

            /// <summary>
            /// 获取注视点等级
            /// </summary>
            /// <param name="level">注视点等级</param>
            /// <returns>函数调用状态</returns>
            [DllImport(pluginName, EntryPoint = "vxr_GetFoveationRenderingLevel")]
            public static extern Result vxr_GetFoveationRenderingLevel(out FoveationLevel level);

            /// <summary>
            /// 设置动态匹配静态注视点
            /// </summary>
            /// <param name="dynamic">动态匹配状态</param>
            /// <returns>函数调用状态</returns>
            [DllImport(pluginName, EntryPoint = "vxr_SetFoveationRenderingDynamic")]
            public static extern Result vxr_SetFoveationRenderingDynamic(FoveationDynamic dynamic);

            /// <summary>
            /// 获取动态匹配静态注视点
            /// </summary>
            /// <param name="dynamic">动态匹配状态</param>
            /// <returns>函数调用状态</returns>
            [DllImport(pluginName, EntryPoint = "vxr_GetFoveationRenderingDynamic")]
            public static extern Result vxr_GetFoveationRenderingDynamic(out FoveationDynamic dynamic);
        }
    }
}
