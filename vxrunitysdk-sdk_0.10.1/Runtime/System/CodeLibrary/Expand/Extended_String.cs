using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System;

namespace com.vivo.codelibrary
{

    public static class Extended_String
    {

        static char[] charIAr = new char[] { '/', '\\' };

        static char[] CharIAr
        {
            get
            {
                if (charIAr == null)
                {
                    charIAr = new char[] { '/', '\\' };
                }
                return charIAr;
            }
        }

        const char pointChar = '.';

        const string assetsStr = "Assets";

        const string emptyStr = "";

        /// <summary>
        /// 转半角小写的函数(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        ///<remarks>
        ///全角空格为12288，半角空格为32
        ///其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
        ///</remarks>
        private static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c).ToLower();
        }

        /// <summary>
        /// 路径小写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string PathToLower(this string str)
        {
            str = str.Replace("\\","/").ToLower();
            return str;
        }

        /// <summary>
        /// 将数组转换为字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="strs"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string LinkStrings(this string str, string[] strArray, string c = "|")
        {
            if (strArray == null || strArray.Length == 0)
            {
                return "";
            }
            StringBuilder sb = ExtendedPool.GetOneStringBuilder();

            for (int i = 0; i < strArray.Length; ++i)
            {
                if (i == (strArray.Length - 1))
                {
                    sb.Append(strArray[i]);
                }
                else
                {
                    sb.Append(strArray[i]);
                    sb.Append(c);
                }
            }
            string res = sb.ToString();
            ExtendedPool.PutBackOneStringBuilder(sb);
            return res;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为bool
        /// </summary>
        /// <param name="str"> 第一个参数必须用this指定 此方法是对于string类的拓展</param>
        /// <returns></returns>
        public static bool ToBool(this string str)
        {
            if (str == null) return false;
            int l = str.Length;
            if (l == 1)
            {
                try
                {
                    int res = int.Parse(str);
                    if (res == 1)
                    {
                        return true;
                    }
                }
                catch (System.Exception e)
                {
                   VLog.Exception(e);
                    return false;
                }
            }
            else if (l == 4)
            {
                str = str.ToLower();
                if (str.CompareTo("true")==0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为int
        /// </summary>
        /// <param name="str"> 第一个参数必须用this指定 此方法是对于string类的拓展</param>
        /// 用法 string str = "124"; int i = str.ToInt();
        /// <returns></returns>
        public static int ToInt(this string str)
        {
            if (str == null) return 0;
            try
            {
                return int.Parse(str);
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return -1;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为float
        /// </summary>
        /// <param name="str"> 第一个参数必须用this指定 此方法是对于string类的拓展</param>
        /// <returns></returns>
        public static float ToFloat(this string str)
        {
            if (str == null || str.Length == 0) return 0;
            try
            {
                return  float.Parse(str);
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return -1;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为double
        /// </summary>
        /// <param name="str"> 第一个参数必须用this指定 此方法是对于string类的拓展</param>
        /// <returns></returns>
        public static double ToDouble(this string str)
        {
            if (str == null || str.Length == 0) return 0;
            try
            {
                return double.Parse(str);
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return -1;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为Long
        /// </summary>
        /// <param name="str"> 第一个参数必须用this指定 此方法是对于string类的拓展</param>
        /// <returns></returns>
        public static long ToLong(this string str)
        {
            if (str == null || str.Length == 0) return 0;
            try
            {
                return long.Parse(str);
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return -1;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为Vector4
        /// Vector4.ToString()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector4 ToVector4(this string str)
        {
            if (str == null || str.Length < 13) return Vector4.zero;
            try
            {
                str = str.Substring(1, str.Length - 2);
                //
                int index = str.IndexOf(string.Intern(","));
                string xStr = str.Substring(0, index);
                //
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                index = str.IndexOf(string.Intern(","));
                string yStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                index = str.IndexOf(string.Intern(","));
                string zStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);

                Vector4 res = Vector4.zero;
                res.x = float.Parse(xStr);
                res.y = float.Parse(yStr);
                res.z = float.Parse(zStr);
                res.w = float.Parse(str);
                return res;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return Vector4.zero;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为Vector3
        /// Vector3.ToString()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(this string str)
        {
            if (str == null || str.Length < 7) return Vector3.zero;
            try
            {
                str = str.Substring(1, str.Length - 2);
                int index = str.IndexOf(string.Intern(","));
                string xStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                index = str.IndexOf(string.Intern(","));
                string yStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                Vector3 res = Vector3.zero;
                res.x = float.Parse(xStr);
                res.y = float.Parse(yStr);
                res.z = float.Parse(str);
                return res;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为Vector2
        /// Vector2.ToString()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(this string str)
        {
            if (str == null || str.Length < 5) return Vector3.zero;
            try
            {
                str = str.Substring(1, str.Length - 2);
                int index = str.IndexOf(string.Intern(","));
                string xStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                Vector2 res = Vector2.zero;
                res.x = float.Parse(xStr);
                res.y = float.Parse(str);
                return res;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return Vector3.zero;
        }

        /// <summary>
        /// 拓展 string 类新功能 解析为Color
        /// 解析为Color.ToString()
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static Color ToColor(this string str)
        {
            if (str == null || str.Length < 13) return Color.black;
            try
            {
                str = str.Substring(5, str.Length - 6);
                int index = str.IndexOf(string.Intern(","));
                string xStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                index = str.IndexOf(string.Intern(","));
                string yStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);
                index = str.IndexOf(string.Intern(","));
                string zStr = str.Substring(0, index);
                index = index + 2;
                str = str.Substring(index, str.Length - index);

                Color res = Color.black;
                res.r = float.Parse(xStr);
                res.g = float.Parse(yStr);
                res.b = float.Parse(zStr);
                res.a= float.Parse(str);
                return res;
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
            }
            return Color.black;
        }

        /// <summary>
        /// 将16进制字符串"#FECEE1"转换成颜色
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static Color GetColorFromString16(this string str16)
        {
            Color res;
            ColorUtility.TryParseHtmlString(str16, out res);
            return res;
        }

        // <summary>
        /// 由连字符分隔的32位数字
        /// </summary>
        /// <returns></returns>
        public static string GetGuid(this string str)
        {
            System.Guid guid = new System.Guid();
            guid = System.Guid.NewGuid();
            return guid.ToString();
        }

        /// <summary>
        /// 根据GUID获取16位的唯一字符串  
        /// </summary>
        /// <returns></returns>
        public static string GuidTo16String(this string str)
        {
            long i = 1;
            byte[] bytes = System.Guid.NewGuid().ToByteArray();
            for (int j = 0; j < bytes.Length; ++j)
            {
                i *= ((int)bytes[j] + 1);
            }
            return string.Format("{0:x}", i - System.DateTime.Now.Ticks);
        }

        /// <summary>
        /// 字符串拆分后转换成泛型列表
        /// List<int> intList = str.ToList<int>(',', s => int.Parse(s));
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="split">拆分字符</param>
        /// <param name="convertHandler"></param>
        /// <returns></returns>
        static List<T> ToTList<T>(this string str, char split, System.Converter<string, T> convertHandler)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<T>();
            }
            else
            {
                T[] Tarr = System.Array.ConvertAll(str.Split(split), convertHandler);
                return new List<T>(Tarr);
            }
        }

        /// <summary>
        /// 字符串拆分后转换成泛型列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="str"></param>
        /// <param name="split">拆分字符</param>
        /// <param name="toType">拆分类型 0:float 1:int 2:double 3:byte 4:short 5:string</param>
        /// <returns></returns>
        public static List<T> StringToTList<T>(this string str, char split, int toType)
        {
            if (string.IsNullOrEmpty(str))
            {
                return new List<T>();
            }
            else
            {
                switch (toType)
                {
                    case 0:
                        {
                            return (List<T>)(object)(ToTList<float>(str, split, s => float.Parse(s)));
                        }
                    case 1:
                        {
                            return (List<T>)(object)(ToTList<int>(str, split, s => int.Parse(s)));
                        }
                    case 2:
                        {
                            return (List<T>)(object)(ToTList<double>(str, split, s => double.Parse(s)));
                        }
                    case 3:
                        {
                            return (List<T>)(object)(ToTList<byte>(str, split, s => byte.Parse(s)));
                        }
                    case 4:
                        {
                            return (List<T>)(object)(ToTList<short>(str, split, s => short.Parse(s)));
                        }
                    case 5:
                        {
                            return (List<T>)(object)(ToTList<string>(str, split, s => s.ToString()));
                        }
                }
                return new List<T>();
            }
        }

        /// <summary>
        /// 打开一个网页
        /// url = "http://www.baidu.com";
        /// </summary>
        public static void OpenWebPage(this string url)
        {
            if (url == null || url == "")
            {
                return;
            }
            Application.OpenURL(url);
        }

#if UNITY_EDITOR
        /// <summary>
        /// 加载层
        /// 运行时添加无效
        /// </summary>
        /// <param name="layer"></param>
        public static void AddLayer(this string layer)
        {
            if (!IsHasLayer(layer))
            {
                //加载项目设置层以及tag值管理 资源
                UnityEditor.SerializedObject tagManager = new UnityEditor.SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                UnityEditor.SerializedProperty it = tagManager.GetIterator();//获取层或tag值所有列表信息
                while (it.NextVisible(true))//判断向后是否还有信息，如果没有则返回false
                {
                    if (it.name == "layers")
                    {
                        //层默认是32个，只能从第8个开始写入自己的层
                        for (int i = 8; i < it.arraySize; ++i)
                        {
                            UnityEditor.SerializedProperty dataPoint = it.GetArrayElementAtIndex(i);//获取层信息
                            if (string.IsNullOrEmpty(dataPoint.stringValue))//如果制定层内为空，则可以填写自己的层名称
                            {
                                dataPoint.stringValue = layer;//设置名字
                                tagManager.ApplyModifiedProperties();//保存修改的属性
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否已经有层
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static bool IsHasLayer(this string layer)
        {
            for (int i = 0; i < UnityEditorInternal.InternalEditorUtility.layers.Length; ++i)
            {
                if (UnityEditorInternal.InternalEditorUtility.layers[i].Contains(layer))
                {
                    return true;
                }
            }
            return false;
        }
#endif

        /// <summary>
        /// 获取资源路径
        /// </summary>
        /// <param name="path">目录 路径</param>
        /// <param name="bundleFileFormat">{"*.txt","*.json", "*.png","*.prefab"}</param>
        /// <returns></returns>
        public static List<string> GetAllAssetPath(string path, string[] bundleFileFormat)
        {
            lock (FileLock.GetStringLock(path.PathToLower()))
            {
                List<string> list = new List<string>();
                for (int i = 0; i < bundleFileFormat.Length; ++i)
                {
                    string[] paths = Directory.GetFiles(path, bundleFileFormat[i], SearchOption.AllDirectories);
                    for (int j = 0; j < paths.Length; ++j)
                    {
                        list.Add(paths[j].Replace("\\", "/"));
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// assetPath到文件路径
        /// </summary>
        /// <param name="assetPath"></param>
        /// <returns></returns>
        public static string AssetPathToFilePath(this string assetPath)
        {
            int index =FilePath.DataPath.LastIndexOf(CharIAr[0]);
            string path = FilePath.DataPath.Substring(0, index + 1) + assetPath;
            return path;
        }

        /// <summary>
        /// 路径转换为assetPath
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FilePathToAssetPath(this string filePath)
        {
            filePath = filePath.Replace(CharIAr[1], CharIAr[0]);
            filePath = filePath.Replace(FilePath.DataPath, assetsStr);
            return filePath;
        }

        /// <summary>
        /// 获得后缀名 没有'.'  如: "Abc/file.txt" -> "txt"
        /// </summary>
        /// <param name="pathOrName"></param>
        /// <returns></returns>
        public static string GetSuffix(this string pathOrName)
        {
            int endIndex = pathOrName.LastIndexOf(pointChar);
            string endStr = pathOrName.Substring(endIndex + 1, pathOrName.Length - endIndex - 1);
            return endStr;
        }

        /// <summary>
        /// 获得去掉后缀的路径 如: "Abc/file.txt" -> "Abc/file"
        /// </summary>
        /// <param name="pathOrName"></param>
        /// <returns></returns>
        public static string DeleteSuffix(this string pathOrName)
        {
            int start = pathOrName.LastIndexOfAny(CharIAr);
            int endIndex = pathOrName.LastIndexOf(pointChar);
            if (endIndex> start)
            {
                return pathOrName.Substring(0, endIndex);
            }
            return pathOrName;
        }

        /// <summary>
        /// 文件名 去掉后缀 如: "Abc/file.txt" -> "file"
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string GetNameDeleteSuffix(this string pathOrName)
        {
            int start = pathOrName.LastIndexOfAny(CharIAr);
            int end = pathOrName.LastIndexOf(pointChar);
            if (start >=0)
            {
                if (end >= 0 && end> start)
                {
                    return pathOrName.Substring(start + 1, end - start - 1);
                }
                else
                {
                    return pathOrName.Substring(start + 1);
                }
            }
            else
            {
                return pathOrName;
            }
        }

        /// <summary>
        /// 文件名 如: "Abc/file.txt" -> "file.txt"
        /// </summary>
        /// <param name="pathOrName"></param>
        /// <returns></returns>
        public static string GetNameWithSuffix(this string pathOrName)
        {
            int start = pathOrName.LastIndexOfAny(CharIAr);
            return pathOrName.Substring(start + 1, pathOrName.Length - start - 1);
        }

        static void CreateDir(string dir)
        {
            lock (FileLock.GetStringLock(dir.PathToLower()))
            {
                try
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                }
            }
        }

        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirFromPath(this string path)
        {
            string dir = Path.GetDirectoryName(path);
            CreateDir(dir);
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="txt"></param>
        public static void WriteTxt(this string filePath, string txt)
        {
            FileWriteHelper.Instance.Write(filePath, txt, false, true, true);
        }

        /// <summary>
        /// 读文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadTxt(this string filePath)
        {
            string res = null;
            FileReadHelper.Instance.ReadText(filePath, (p, t,par) =>
            {
                res = t;
            }, System.Text.Encoding.UTF8);
            return res;
        }

        /// <summary>
        /// 将文件pathA拷贝到pathB
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="pathDes"></param>
        /// <param name="errList">错误列表</param>
        /// <param name="thread"></param>
        public static void CopyTo(this string filePath, string pathDes)
        {
            string dir = Path.GetDirectoryName(pathDes);
            CreateDir(dir);
            lock (FileLock.GetStringLock(filePath.PathToLower()))
            {
                FileWriteHelper.Instance.Close(filePath);
                lock (FileLock.GetStringLock(pathDes.PathToLower()))
                {
                    FileWriteHelper.Instance.Close(pathDes);
                    try
                    {
                        if (File.Exists(filePath))
                        {
                            File.Copy(filePath, pathDes);
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                }
            }
        }


        /// <summary>
        /// 删除指定路径的文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeletFile(this string filePath)
        {
            lock (FileLock.GetStringLock(filePath.PathToLower()))
            {
                FileWriteHelper.Instance.Close(filePath);
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                }
            }
        }

        const string x2Str = "X2";

        /// <summary>
        /// 字符串的MD5
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string CreateStringMD5(this string input)
        {
            if (string.IsNullOrEmpty(input)) return emptyStr;
            try
            {
                using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
                {
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hashBytes = md5.ComputeHash(inputBytes);
                    StringBuilderPollData data = stringBuilderPollDataPool.Spawn();
                    for (int i = 0; i < hashBytes.Length; i++)
                    {
                        data.SB.Append(hashBytes[i].ToString(x2Str));
                    }
                    string str= data.SB.ToString();
                    stringBuilderPollDataPool.Recycle(data);
                    return str;
                }
            }
            catch (System.Exception e)
            {
                VLog.Exception(e);
                return null;
            }
        }

        static SimplePool<StringBuilderPollData> stringBuilderPollDataPool = new SimplePool<StringBuilderPollData>();

        class StringBuilderPollData : ISimplePoolData
        {
            public StringBuilder SB=new StringBuilder (32);

            bool isDispose = false;

            public void Dispose()
            {
                SB.Capacity = 0;
                SB.EnsureCapacity(0);
                isDispose = true;
            }

            bool isUsed = false;

            public bool IsUsed
            {
                get
                {
                    return isUsed;
                }
            }

            public bool Disposed
            {
                get
                {
                    return isDispose;
                }
            }

            public void PutIn()
            {
                SB.Clear();
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        /// <summary>
        /// 指定文件的md5
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string CreateFileMD5(this string filePath)
        {
            return CreateStringMD5(filePath.ReadTxt());
        }

        /// <summary>
        /// 存储byte数组
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bytes"></param>
        public static void SaveByte(this string filePath, byte[] bytes)
        {
            FileWriteHelper.Instance.Write(filePath, bytes, false, true, true);
        }

        /// <summary>
        /// 读取byte数组 在读取移动StreamingAssets文件夹的时候会读取失败
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] ReadByte(this string filePath)
        {
            byte[] res = null;
            FileReadHelper.Instance.ReadByteArray(filePath, (p, datas,par) =>
            {
                res = datas;
            }, System.Text.Encoding.UTF8);
            return res;
        }
    }
}

