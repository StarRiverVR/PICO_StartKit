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
    public class UIHelper
    {
        /// <summary>
        /// 鼠标是否在UI上
        /// </summary>
        /// <returns></returns>
        public static bool IsTouchedUI()
        {
            if (EventSystem.current == null)
            {
                return false;
            }
            if (Application.isMobilePlatform)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return true;
                }
            }
            else if (EventSystem.current.IsPointerOverGameObject())
            {
                return true;
            }
            return false;
        }
    }
}


