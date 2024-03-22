using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace com.vivo.codelibrary
{
    [Serializable]
    /// <summary>
    /// 可序列化字典
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class SerializableDictionary<TKey, TValue>  : Dictionary<TKey, TValue>,ISerializ
    {
        public SerializableDictionary()
        {

        }

        public SerializableDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        private List<TKey> keysList;

        private List<TValue> valuesList;

        /// <summary>
        /// 序列化调用
        /// </summary>
        public void OnSerialization()
        {
            KeyCollection keys = Keys;
            if (keys!=null)
            {
                if (keysList==null)
                {
                    keysList = new List<TKey>();
                }
                Dictionary<TKey, TValue>.KeyCollection.Enumerator enumerator = keys.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    keysList.Add(enumerator.Current);
                }
            }
            else
            {
                keysList = null;
            }
            ValueCollection values = Values;
            if (values != null)
            {
                if (valuesList == null)
                {
                    valuesList = new List<TValue>();
                }
                Dictionary<TKey, TValue>.ValueCollection.Enumerator enumerator = values.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    valuesList.Add(enumerator.Current);
                }
            }
            else
            {
                valuesList = null;
            }
        }

        /// <summary>
        /// 反序列化调用
        /// </summary>
        public void OnDeserialization()
        {
            if (keysList != null && valuesList!=null && keysList.Count== valuesList.Count)
            {
                for (int i=0;i< keysList.Count;++i)
                {
                    Add(keysList[i], valuesList[i]);
                }
            }
            keysList = null;
            valuesList = null;
        }
    }
}

