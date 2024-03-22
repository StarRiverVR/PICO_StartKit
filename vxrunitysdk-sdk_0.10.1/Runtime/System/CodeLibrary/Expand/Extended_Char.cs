using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public static class Extended_Char
    {
        /// <summary>
        /// 判断是否是中文
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool isCHS(this char character)
        {
            //  中文表意字符的范围 4E00-9FA5
            int charVal = (int)character;
            return (charVal >= 0x4e00 && charVal <= 0x9fa5);
        }

        /// <summary>
        /// 判断是否是数字
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool isNum(this char character)
        {
            int charVal = (int)character;
            return (charVal >= 48 && charVal <= 57);
        }

        /// <summary>
        /// 判断是否是字母
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static bool isAlphabet(this char character)
        {
            int charVal = (int)character;
            return ((charVal >= 97 && charVal <= 122) || (charVal >= 65 && charVal <= 90));
        }
    }
}

