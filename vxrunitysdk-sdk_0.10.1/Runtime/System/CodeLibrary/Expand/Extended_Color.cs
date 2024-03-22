
using UnityEngine;
using System.Text;

namespace com.vivo.codelibrary
{
    public static class Extended_Color
    {
        /// <summary>
        /// 获取16进制
        /// </summary>
        /// <param name="c"></param>
        /// <returns>"#FECEE1"</returns>
        public static string GetString16FromColor(this Color c)
        {
            return ColorUtility.ToHtmlStringRGB(c);
        }

    }
}

