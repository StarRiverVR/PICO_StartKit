using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 深拷贝工具
    /// </summary>
    public class DeepCloneHelper
    {
        /// <summary>
        /// 深拷贝
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="data">实例对象</param>
        /// <returns></returns>
        public static T Clone<T>(T data) where T : class
        {
            try
            {
                using (Stream objectStream = new MemoryStream())
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(objectStream, data);
                    objectStream.Seek(0, SeekOrigin.Begin);
                    return formatter.Deserialize(objectStream) as T;
                }
            }
            catch (Exception e)
            {
                VLog.Error($"深拷贝发生错误:{e.Message},data:{data}");
                return null;
            }
        }
    }
}


