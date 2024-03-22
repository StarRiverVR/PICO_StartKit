using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public class XmlHelper
    {
        /// <summary>
        /// 读取xml文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static SecurityParser CreateXmlDocFromFile(string filePath)
        {
            byte[] datas=null;
            FileReadHelper.Instance.ReadByteArray(filePath, (p, d,o) =>
            {
                datas = d;
            }, System.Text.Encoding.UTF8);
            if (datas != null)
            {
                return CreateXmlDocFromBytes(datas);
            }
            return null;
        }

        /// <summary>
        /// 转byte[] 为 XML
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static SecurityParser CreateXmlDocFromBytes(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                VLog.Error($"_CreateXmlDocFromBytes data {data} fail");
                return null;
            }

            string text = System.Text.Encoding.UTF8.GetString(data);
            SecurityParser doc = new SecurityParser();
            doc.LoadXml(text);

            return doc;
        }
    }
}


