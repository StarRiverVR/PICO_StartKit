using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    public class TimeHelper
    {

        /// <summary>
        /// 当前游戏时间
        /// </summary>
        public static float NowGameTime
        {
            get
            {
                return MonoUpdateManager.Instance.NowGameTime;
            }
        }

        /// <summary>
        /// 当前物理时间
        /// </summary>
        public static float NowRealtimeTime
        {
            get
            {
                return MonoUpdateManager.Instance.NowRealtimeTime;
            }
        }

        /// <summary>  
        /// 获取时间戳  毫秒
        /// </summary>  
        /// <returns></returns> 
        public static long GetTimeStamp_MS()
        {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static int GetTimeStamp(DateTime dateTime)
        {
            DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt32((dateTime - DateStart).TotalSeconds);
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStampLong(DateTime dateTime)
        {
            DateTime DateStart = new DateTime(1970, 1, 1, 8, 0, 0);
            return Convert.ToInt64((dateTime - DateStart).TotalSeconds);
        }

        //时间戳转换成日期
        public static DateTime TimeToTimestamp(int xx)//xx时间戳
        {
            DateTime theTime = DateTime.Now;
            DateTime startTime =TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            int theT = (int)(theTime - startTime).TotalSeconds;
            theT = theT - xx;
            theTime = theTime.AddSeconds(-theT);
            return theTime;
        }

        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static string GetTimeStampString()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static UInt32 GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return (UInt32)Convert.ToInt64(ts.TotalSeconds);
        }

        /// <summary>
        /// 将字符串强制转换成DateTime，转换失败则返回DateTime.MinValue
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static DateTime ParseToDateTime(string str)
        {
            DateTime returndate = DateTime.MinValue;
            if (str.Trim().Length < 1)
            {
                return returndate;
            }

            if (DateTime.TryParse(str, out returndate))
            {
                return returndate;
            }
            else
            {
                Debug.LogWarning("string数据" + str + "转换DateTime失败，返回默认值" + returndate);
                return returndate;
            }
        }
    }
}

