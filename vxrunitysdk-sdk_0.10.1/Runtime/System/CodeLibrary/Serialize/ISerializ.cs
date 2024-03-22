using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    public interface ISerializ
    {
        /// <summary>
        /// 序列化调用
        /// </summary>
        public void OnSerialization();

        /// <summary>
        /// 反序列化调用
        /// </summary>
        public void OnDeserialization();
    }
}


