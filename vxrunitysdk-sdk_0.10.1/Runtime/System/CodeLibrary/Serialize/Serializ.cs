using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    public class Serializ<T> where T : ISerializ
    {

        public static void SaveData(T data, string filePath)
        {
            try
            {
                data.OnSerialization();
                FileWriteHelper.Instance.WriteSerializ(filePath, data, false, true, true);
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                throw (e);
            }
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <param name="oneFileCallBack">参数为文件路径和null</param>
        public static void SaveDataAsyn(T data, string filePath, System.Action<string,object> oneFileCallBack)
        {
            try
            {
                data.OnSerialization();
                FileWriteHelper.Instance.WriteSerializAsyn(filePath, data, false, oneFileCallBack,null, true, true, true);
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                throw (e);
            }
        }

        public static T ReadData(string filePath, long startOffset = 0)
        {
            try
            {
                T data=default(T);
                //如果路径上有文件，就读取文件
                if (File.Exists(filePath))
                {
                    bool find = false;
                    //读取数据
                    FileReadHelper.Instance.ReadSerializ(filePath, (p, obj,par) =>
                    {
                        if (obj!=null)
                        {
                            find = true;
                            data = (T)obj;
                        }
                    }, System.Text.Encoding.UTF8, startOffset);
                    if (find)
                    {
                        data.OnDeserialization();
                    }
                }
                return data;
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                return default(T);
            }
        }

        public static T ReadDataAsyn(string filePath,System.Action<T> callBack, long startOffset = 0)
        {
            try
            {
                T data = default(T);
                //如果路径上有文件，就读取文件
                if (File.Exists(filePath))
                {
                    //读取数据
                    FileReadHelper.Instance.ReadSerializAsyn(filePath, (p, obj, par) =>
                    {
                        if (obj!=null)
                        {
                            T readData = (T)obj;
                            readData.OnDeserialization();
                            callBack(readData);
                        }
                        else
                        {
                            callBack(default(T));
                        }
                    }, (isbl,p,par)=> {

                    }, null,System.Text.Encoding.UTF8, startOffset);
                }
                return data;
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                throw (e);
            }
        }

        public static List<T> ReadAllData(string fileDir, string fileter = "*.byte", long startOffset = 0)
        {
            string[] paths = null;
            lock (FileLock.GetStringLock(fileDir.PathToLower()))
            {
                if (!Directory.Exists(fileDir)) return null;
                paths = Directory.GetFiles(fileDir, fileter, SearchOption.AllDirectories);
            }
            List<T> list = new List<T>();
            try
            {
                for (int i = 0, listCount = paths.Length; i < listCount; ++i)
                {
                    T data = ReadData(paths[i], startOffset);
                    if (data != null)
                    {
                        list.Add(data);
                    }
                }
            }
            catch (System.Exception e)
            {
                VLog.Error(e.Message);
                throw (e);
            }
            return list;
        }

        public static void ReadAllDataAsyn(string fileDir, System.Action<string, T> serializCallBack, System.Action<List<T>> finishReadCallBack, string fileter= "*.byte", long startOffset = 0)
        {
            string[] paths = null;
            lock (FileLock.GetStringLock(fileDir.PathToLower()))
            {
                if (!Directory.Exists(fileDir))
                {
                    if (finishReadCallBack!=null)
                    {
                        finishReadCallBack(null);
                    }
                    return;
                }
                List<T> list = new List<T>();
                paths = Directory.GetFiles(fileDir, fileter, SearchOption.AllDirectories);
                FileReadHelper.Instance.ReadSerializAsyn(paths.ToList<string>(), (p,d,par) => {
                    if (serializCallBack!=null)
                    {
                        if (d != null)
                        {
                            serializCallBack(p, (T)d);
                            list.Add((T)d);
                        }
                        else
                        {
                            serializCallBack(p, default(T));
                        }
                    }
                }, (bl) => {
                    if (finishReadCallBack != null)
                    {
                        finishReadCallBack(list);
                    }
                }, System.Text.Encoding.UTF8, startOffset);
            }

        }
    }
}

