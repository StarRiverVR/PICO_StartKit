using UnityEngine;
using System;
using System.Reflection;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 剪贴板
    /// </summary>
    public class ClipboardHelper
    {
        private static PropertyInfo m_systemCopyBufferProperty = null;
        private static PropertyInfo GetSystemCopyBufferProperty()
        {
            if (m_systemCopyBufferProperty == null)
            {
                Type T = typeof(GUIUtility);
                m_systemCopyBufferProperty = T.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
                if (m_systemCopyBufferProperty == null)
                    throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
            }
            return m_systemCopyBufferProperty;
        }

        public static void SendMessage(string msg)
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            P.SetValue(null, msg, null);
        }

        public static string GetMessage()
        {
            PropertyInfo P = GetSystemCopyBufferProperty();
            return (string)P.GetValue(null, null);
        }
    }
}
