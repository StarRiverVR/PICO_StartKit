using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Threading;

using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.codelibrary
{
    public class TextureHelper
    {

        /// <summary>
        /// 获得图片路径
        /// </summary>
        /// <param name="textureDir"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetTexture2DPaths(string textureDir, SearchOption searchOption = SearchOption.AllDirectories)
        {
            string lockKey = textureDir.PathToLower();
            lock (FileLock.GetStringLock(lockKey))
            {
                IEnumerable<string> res = Directory.GetFiles(textureDir, string.Intern("*.*"), searchOption).Where(s => s.EndsWith(string.Intern(".png")) || s.EndsWith(string.Intern(".jpg")) || s.EndsWith(string.Intern(".jpeg"))).ToArray();
                return res;
            }
        }

        /// <summary>
        /// 图片名称
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="textuewName"></param>
        public static void SetTextureName(Texture2D texture,string textuewName)
        {
            if (ThreadHelper.UnitySynchronizationContext!= SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    texture.name = textuewName;
                },null);
            }
            else
            {
                texture.name = textuewName;
            }
        }

        /// <summary>
        /// 图片名称
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="textureB"></param>
        public static void SetTextureName(Texture2D texture, Texture2D textureB)
        {
            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    texture.name = textureB.name;
                }, null);
            }
            else
            {
                texture.name = textureB.name;
            }
        }

        public static int GetColorIndex(int widthOiriginal, int indexU, int indexV)
        {
            int index = indexU + widthOiriginal * indexV;
            return index;
        }

        public static Color GetColor(Color[] colorsOiriginal, int widthOiriginal, int indexU, int indexV)
        {
            int index = indexU + widthOiriginal * indexV;
            return colorsOiriginal[index];
        }

        public static void SetColor(Color[] colorsOiriginal, int widthOiriginal, int indexU, int indexV, Color color)
        {
            int index = indexU + widthOiriginal * indexV;
            colorsOiriginal[index] = color;
        }

        #region//读取图片

        /// <summary>
        /// 本地读取图片 默认开启可读写 主线程
        /// </summary>
        /// <param name="filePath">图片路径</param>
        /// <returns></returns>
        public static Texture2D Texture2DRead(string filePath)
        {
            byte[] datas = null;
            bool bl = FileReadHelper.Instance.ReadByteArray(filePath, (p,d,par) => {
                datas = d;
            }, System.Text.Encoding.UTF8);
            if (bl && datas!=null)
            {
                if (ThreadHelper.UnitySynchronizationContext!= SynchronizationContext.Current)
                {
                    Texture2D texRes = null;
                    ThreadHelper.UnitySynchronizationContext.Send((o) => {
                        Texture2D tex = new Texture2D(32, 32);
                        try
                        {
                            tex.LoadImage(datas);
                            SetTextureName(tex, filePath.GetNameDeleteSuffix());
                            texRes = tex;
                        }
                        catch (System.Exception ex)
                        {
                            if (tex != null)
                            {
                                if (Application.isPlaying)
                                {
                                    Texture2D.Destroy(tex);
                                }
                                else
                                {
                                    Texture2D.DestroyImmediate(tex);
                                }
                            }
                            tex = null;
                            VLog.Error(ex.Message);
                            texRes = null;
                        }
                    },null);
                    return texRes;
                }
                else
                {
                    Texture2D tex = new Texture2D(32, 32);
                    try
                    {
                        tex.LoadImage(datas);
                        SetTextureName(tex, filePath.GetNameDeleteSuffix());
                        return tex;
                    }
                    catch (System.Exception ex)
                    {
                        if (tex != null)
                        {
                            if (Application.isPlaying)
                            {
                                Texture2D.Destroy(tex);
                            }
                            else
                            {
                                Texture2D.DestroyImmediate(tex);
                            }
                        }
                        tex = null;
                        VLog.Error(ex.Message);
                        return null;
                    }
                }
            }
            else
            {
                VLog.Error($"图片读取失败:{filePath}");
                return null;
            }
        }

        /// <summary>
        /// 本地读取图片 主线程
        /// </summary>
        /// <param name="fileDir"></param>
        /// <param name="callBack"></param>
        /// <param name="searchOption"></param>
        public static List<Texture2D> Texture2DsRead(string fileDir, SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<Texture2D> list = new List<Texture2D>();
            IEnumerable<string> imags = GetTexture2DPaths(fileDir, searchOption);
            IEnumerator<string> iEnumerator = imags.GetEnumerator();
            while (iEnumerator.MoveNext())
            {
                string imagePath = iEnumerator.Current;
                list.Add(Texture2DRead(imagePath));
            }
            return list;
        }

        /// <summary>
        /// 本地读取图片 主线程
        /// </summary>
        /// <param name="imageList"></param>
        /// <returns></returns>
        public static List<Texture2D> Texture2DsRead(List<string> imageFilePaths)
        {
            IEnumerable<string> imags = imageFilePaths;
            List<Texture2D> list = new List<Texture2D>();
            IEnumerator<string> iEnumerator = imags.GetEnumerator();
            while (iEnumerator.MoveNext())
            {
                string imagePath = iEnumerator.Current;
                list.Add(Texture2DRead(imagePath));
            }
            return list;
        }

        /// <summary>
        /// 异步 url下载图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="callBack">下载的图片，err错误信息</param>
        /// <param name="reLoadCount">错误尝试重新下载次数</param>
        public static void Texture2DLoadAsyn(string url, bool readable, System.Action<Texture2D, string> callBack, int reLoadCount = 25)
        {
            if (string.IsNullOrEmpty(url))
            {
                callBack(null, "Texture2D Url Is Null !");
                return;
            }
            List<TextureHttpLoadeData> datas = TextureHttpLoader.GetOneList();
            TextureHttpLoadeData data = TextureHttpLoader.GetOneData(url, 0, readable);
            datas.Add(data);
            TextureHttpLoader textureHttpLoader = new TextureHttpLoader(datas, (texture2D, textureHttpLoadData) => {
                callBack(texture2D, String.Format("TextureErr={0} \nLoadErr={1} \nCallBackErr={2}", textureHttpLoadData.TextureErr, textureHttpLoadData.LoadErr, textureHttpLoadData.CallBackErr));
            }, (bl, obj, textureHttpLoader) => {

            }, null, null, reLoadCount);
        }

        /// <summary>
        /// 异步 下载图片
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="readable"></param>
        /// <param name="callBack">返回图片,路径</param>
        /// <param name="finishCallBack">结果，纹理</param>
        /// <param name="reLoadCount">错误尝试重新下载次数</param>
        public static void Texture2DLoadAsyn(List<string> urls, bool readable, System.Action<Texture2D, string> callBack, System.Action<bool,List<Texture2D>> finishCallBack,int reLoadCount=25)
        {
            bool find = false;
            for (int i = 0; i < urls.Count; ++i)
            {
                if (!string.IsNullOrEmpty(urls[i]))
                {
                    find = true;
                    break;
                }
            }
            if (!find)
            {
                callBack(null, "Texture2D Url Is Null !");
                return;
            }
            List<TextureHttpLoadeData> datas = TextureHttpLoader.GetOneList();
            for (int i = 0; i < urls.Count; ++i)
            {
                if (!string.IsNullOrEmpty(urls[i]))
                {
                    TextureHttpLoadeData data = TextureHttpLoader.GetOneData(urls[i], 0, readable);
                    datas.Add(data);
                }
            }
            Dictionary<string, Texture2D> res = new Dictionary<string, Texture2D>();
            for (int i=0;i< urls.Count;++i)
            {
                res.Add(urls[i],null);
            }
            TextureHttpLoader textureHttpLoader = new TextureHttpLoader(datas, (texture2D, textureHttpLoadData) => {
                res[textureHttpLoadData.Url] = texture2D;
                try
                {
                    callBack(texture2D, textureHttpLoadData.Url);
                }
                catch (System.Exception ex)
                {
                    VLog.Exception(ex);
                }
            }, (bl, obj, textureHttpLoader) => {
                try
                {
                    List<Texture2D> texList = new List<Texture2D>();
                    Dictionary<string, Texture2D>.Enumerator enumerator = res.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        if (enumerator.Current.Value!=null)
                        {
                            texList.Add(enumerator.Current.Value);
                        }
                    }
                    if (finishCallBack != null)
                    {
                        finishCallBack(bl, texList);
                    }
                }
                catch (System.Exception ex)
                {
                    VLog.Exception(ex);
                }
            }, null, null, reLoadCount);
        }

        /// <summary>
        /// 读取像素
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Color[] GetColors(Texture2D texture)
        {
            Color[] res = null;
            bool bl = false;
            TextureHelper.TextureReadableOn(texture, (b, c, w, h, b2) => {
                bl = b;
                res = c;
            }, true);
            if (!bl)
            {
                TextureHelper.TextureReadableOff(texture);
            }
            return res;
        }

        /// <summary>
        /// 获得图片尺寸
        /// </summary>
        /// <param name="textureFilePath"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        public static bool GetSizeFromHeader(string textureFilePath, out int w, out int h)
        {
            w = 0;
            h = 0;
            string lockKey = textureFilePath.PathToLower();
            lock (FileLock.GetStringLock(lockKey))
            {
                if (!File.Exists(textureFilePath))
                {
                    return false;
                }
                FileWriteHelper.Instance.Close(textureFilePath, lockKey);
                try
                {
                    FileStream stream = new FileStream(textureFilePath, FileMode.Open);
                    return GetSizeFromHeader(stream,out w,out h);
                }
                catch (System.Exception ex)
                {
                    VLog.Exception(ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 获得图片尺寸
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        static bool GetSizeFromHeader(Stream stream, out int w, out int h)
        {
            w = -1;
            h = -1;
            Exception j = null, p = null;
            try
            {
                JPGReader reader = new JPGReader(stream);
                reader.ReadHeader();
                w = reader.Width;
                h = reader.Height;
                return true;
            }
            catch (Exception e)
            {
                j = e;
                stream.Position = 0;
            }
            try
            {
                PNGReader reader = new PNGReader(stream);
                reader.ReadInfo();
                w = (int)reader.Width;
                h = (int)reader.Height;
                return true;
            }
            catch (Exception e)
            {
                p = e;
            }
            if (j != null)
            {
                VLog.Error($"[ImageHelper] Parse Jpg Error : {j}");
            }
            if (p != null)
            {
                VLog.Error($"[ImageHelper] Parse Png Error : {p}");
            }
            return false;
        }

        /// <summary>
        /// 获得图片尺寸
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        static bool GetSizeFromHeader(byte[] bytes, out int w, out int h)
        {
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return GetSizeFromHeader(stream, out w, out h);
            }
        }

        #endregion

        #region//保存图片

        /// <summary>
        /// 获取图片字节数据
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="textureType"></param>
        /// <param name="jpgQuality"></param>
        /// <returns></returns>
        public static byte[] GetTextureDatas(Texture2D texture, TextureType textureType = TextureType.PNG,int jpgQuality=75)
        {
            if(texture==null)
            {
                return null;
            }

            bool isReadable = false;
            Color[] colors = null;
            int width = 0;
            int height = 0;
            bool isSet = false;
            TextureReadableOn(texture, (b, c, w, h,s) => {
                isReadable = b;
                colors = c;
                width = w;
                height = h;
                isSet = s;
            },false);

            byte[] datas = null;
            if (ThreadHelper.UnitySynchronizationContext!= SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    try
                    {
                        switch (textureType)
                        {
                            case TextureType.TGA:
                                {
                                    datas = texture.EncodeToTGA();
                                }
                                break;
                            case TextureType.PNG:
                                {
                                    datas = texture.EncodeToPNG();
                                }
                                break;
                            case TextureType.JPG:
                                {
                                    datas = texture.EncodeToJPG(jpgQuality);
                                }
                                break;
                            case TextureType.EXR:
                                {
                                    datas = texture.EncodeToEXR();
                                }
                                break;
                        }
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Exception(ex);
                    }
                    
                },null);
            }
            else
            {
                try
                {
                    switch (textureType)
                    {
                        case TextureType.TGA:
                            {
                                datas = texture.EncodeToTGA();
                            }
                            break;
                        case TextureType.PNG:
                            {
                                datas = texture.EncodeToPNG();
                            }
                            break;
                        case TextureType.JPG:
                            {
                                datas = texture.EncodeToJPG(jpgQuality);
                            }
                            break;
                        case TextureType.EXR:
                            {
                                datas = texture.EncodeToEXR();
                            }
                            break;
                    }
                    if (isSet)
                    {
                        TextureReadableOff(texture);
                    }
                }
                catch (System.Exception ex)
                {
                    VLog.Exception(ex);
                }
            }
            if (isSet)
            {
                TextureReadableOff(texture);
            }
            return datas;
        }

        /// <summary>
        /// 图片存储路径
        /// </summary>
        /// <param name="savePath"></param>
        /// <param name="textureType"></param>
        /// <returns></returns>
        public static string GetSavePath(string savePath, TextureType textureType = TextureType.PNG)
        {
            string res = savePath;
            string suffix = savePath.GetSuffix();
            switch (textureType)
            {
                case TextureType.TGA:
                    {
                        if (string.IsNullOrEmpty(suffix) || suffix.CompareTo("tga")!=0)
                        {
                            res = savePath.DeleteSuffix() + ".tga";
                        }
                    }
                    break;
                case TextureType.PNG:
                    {
                        if (string.IsNullOrEmpty(suffix) || suffix.CompareTo("png") != 0)
                        {
                            res = savePath.DeleteSuffix() + ".png";
                        }
                    }
                    break;
                case TextureType.JPG:
                    {
                        if (string.IsNullOrEmpty(suffix) || suffix.CompareTo("jpg") != 0)
                        {
                            res = savePath.DeleteSuffix() + ".jpg";
                        }
                    }
                    break;
                case TextureType.EXR:
                    {
                        if (string.IsNullOrEmpty(suffix) || suffix.CompareTo("exr") != 0)
                        {
                            res = savePath.DeleteSuffix() + ".exr";
                        }
                    }
                    break;
            }
            return res;
        }

        /// <summary>
        /// 图片存储
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="savePath"></param>
        /// <param name="textureType"></param>
        public static void SaveTexture(Texture2D texture,string savePath, TextureType textureType= TextureType.PNG)
        {
            SaveTextureAsynData asynData = saveTextureAsynDataPool.Spawn();
            asynData.texture = texture;
            asynData.savePath = savePath;
            asynData.textureType = textureType;
            SaveTextureThread(asynData);
        }

        /// <summary>
        /// 异步图片存储
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="savePath"></param>
        /// <param name="callBack"></param>
        /// <param name="textureType"></param>
        public static void SaveTextureAsyn(Texture2D texture, string savePath,System.Action callBack, TextureType textureType = TextureType.PNG)
        {
            SaveTextureAsynData asynData = saveTextureAsynDataPool.Spawn();
            asynData.texture = texture;
            asynData.savePath = savePath;
            asynData.textureType = textureType;
            //
            ThreadHelper.StartTask<SaveTextureAsynData>(SaveTextureThread, asynData, (res,par,ex) => {
                try
                {
                    callBack();
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                }
                if (ex!=null)
                {
                    VLog.Exception(ex);
                }
            });
        }

        static object SaveTextureThread(object obj)
        {
            SaveTextureAsynData asynData = (SaveTextureAsynData)obj;
            Texture2D texture = asynData.texture;
            string savePath = asynData.savePath; 
            TextureType textureType = asynData.textureType;
            saveTextureAsynDataPool.Recycle(asynData);
            //
            savePath = GetSavePath(savePath, textureType);
            byte[] datas = GetTextureDatas(texture, textureType);
            if (datas == null)
            {
                VLog.Error($"图片存储失败：{savePath}");
                return null;
            }
            FileWriteHelper.Instance.Write(savePath, datas, false, true, true);
            return null;
        }

        static SimplePool<SaveTextureAsynData> saveTextureAsynDataPool = new SimplePool<SaveTextureAsynData>();

        class SaveTextureAsynData : ISimplePoolData, IThreadHelperPar
        {

            public Texture2D texture;
            public string savePath;
            public TextureType textureType;

            public SynchronizationContext Context { get; set; }

            bool isDispose = false;

            public void Dispose()
            {
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
                texture = null;
                Context = null;
                savePath = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        public enum TextureType
        {
            TGA,
            PNG,
            JPG,//Quality=1..100 (default 75).
            EXR,
        }

        #endregion

        #region//创建

        /// <summary>
        /// 创建新图片
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="colors"></param>
        /// <param name="callBack"></param>
        /// <param name="mipMap"></param>
        /// <param name="readable"></param>
        public static void CreateTexture(int width, int height, Color[] colors, System.Action<Texture2D> callBack, bool mipMap = true, bool readable = true)
        {
            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    CreateTextureRun(width, height, colors, callBack, mipMap, readable);
                }, null);
            }
            else
            {
                CreateTextureRun(width, height, colors, callBack, mipMap, readable);
            }
        }

        static void CreateTextureRun(int width, int height, Color[] colors, System.Action<Texture2D> callBack, bool mipMap = true, bool readable = true)
        {
            Texture2D res = new Texture2D(width, height);
            res.SetPixels(colors);
            res.Apply(mipMap, !readable);
            callBack(res);
        }

        #endregion

        #region//可读写

        /// <summary>
        /// 打开Readable
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="callBack">图片原始可读写状态/像素/宽度/高度/是否经过设置</param>
        public static void TextureReadableOn(Texture2D texture, System.Action<bool, Color[], int, int, bool> callBack, bool getColors = true)
        {
            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    OpenTextureReadableRun(texture, callBack, getColors);
                }, null);
            }
            else
            {
                OpenTextureReadableRun(texture, callBack, getColors);
            }
        }

        static void OpenTextureReadableRun(Texture2D texture, System.Action<bool, Color[], int, int, bool> callBack, bool getColors = true)
        {
            if (texture==null)
            {
                VLog.Error("图片为Null:" + texture);
                callBack(false, null, 0, 0, false);
                return;
            }
            bool isReadable = texture.isReadable;
            if (!isReadable)
            {
#if UNITY_EDITOR
                string texAssetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);
                if (texAssetPath.Contains("."))
                {
                    UnityEditor.TextureImporter textureImporter = (UnityEditor.TextureImporter)UnityEditor.TextureImporter.GetAtPath(texAssetPath);
                    if (textureImporter == null)
                    {
                        VLog.Error("图片不可读:" + texture);
                        callBack(false, null, 0, 0, false);
                        return;
                    }
                    textureImporter.isReadable = true;
                    UnityEditor.AssetDatabase.ImportAsset(texAssetPath);
                    texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(texAssetPath);
                }
                else
                {
                    VLog.Error("图片不可读:" + texture);
                    callBack(false, null, 0, 0, false);
                    return;
                }
#else
                VLog.Error("图片不可读:"+ texture);
                callBack(false,null,0,0,false);
                return;
#endif
            }

            Color[] colors = null;
            if (getColors)
            {
                colors = texture.GetPixels();
            }
            if (!isReadable)
            {
                callBack(isReadable, colors, texture.width, texture.height, true);
            }
            else
            {
                callBack(isReadable, colors, texture.width, texture.height, false);
            }
        }

        /// <summary>
        /// 设置可读写
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="isReadable"></param>
        public static void TextureReadableOff(Texture2D texture)
        {
            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    SetTextureReadableRun(texture, false);
                }, null);
            }
            else
            {
                SetTextureReadableRun(texture, false);
            }
        }

        static void SetTextureReadableRun(Texture2D texture, bool isReadable)
        {
            if (texture==null ||!texture.isReadable)
            {
                return;
            }
#if UNITY_EDITOR
            string texAssetPath = UnityEditor.AssetDatabase.GetAssetPath(texture);
            UnityEditor.TextureImporter textureImporter = (UnityEditor.TextureImporter)UnityEditor.TextureImporter.GetAtPath(texAssetPath);
            if (textureImporter != null)
            {
                textureImporter.isReadable = isReadable;
                UnityEditor.AssetDatabase.ImportAsset(texAssetPath);
            }
#endif
        }

        #endregion


        static SimplePool<CountData> countDataPool = new SimplePool<CountData>();

        class CountData:ISimplePoolData
        {
            int count = 0;

            public void AddCount()
            {
                lock (lockObj)
                {
                    count = count + 1;
                }
            }

            public void SubCount()
            {
                lock (lockObj)
                {
                    count = count - 1;
                }
            }

            public void SetCount(int v)
            {
                lock (lockObj)
                {
                    count = v;
                }
            }

            public int GetCount()
            {
                lock (lockObj)
                {
                    return count;
                }
            }

            public Dictionary<string, CountTextureData> CountTextureDatas=new Dictionary<string, CountTextureData> ();

            public object parData;

            public System.Action<Dictionary<string, ChannelFinishData>, object> finishCallBack;

            public Dictionary<Texture2D, ChannelFinishData> Texture2DDic;

            object lockObj = new object();

            bool isDispose = false;

            public void Dispose()
            {
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
                CountTextureDatas.Clear();
                lock (lockObj)
                {
                    count = 0;
                }
                Texture2DDic = null;
                parData = null;
                finishCallBack = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        static SimplePool<CountTextureData> countTextureDataPool = new SimplePool<CountTextureData>();

        class CountTextureData : ISimplePoolData
        {
            public string TextureFilePath;

            public Texture2D Texture;

            public string SavePath;

            bool isDispose = false;

            public void Dispose()
            {
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
                SavePath = null;
                TextureFilePath = null;
                Texture = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        public class ChannelFinishData
        {
            /// <summary>
            /// 原始纹理路径
            /// </summary>
            public string TextureFilePath;

            /// <summary>
            /// 新纹理保存路径
            /// </summary>
            public string TextureSavePath;

            /// <summary>
            /// 旧纹理
            /// </summary>
            public Texture2D OldTexture;

            /// <summary>
            /// 新纹理
            /// </summary>
            public Texture2D NewTexture;
        }

        //libpng 1.6.30
        private class PNGReader
        {
            //#define png_IDAT PNG_U32( 73,  68,  65,  84)
            private const uint IDAT = (73 << 24) + (68 << 16) + (65 << 8) + 84;
            //#define png_IHDR PNG_U32( 73,  72,  68,  82)
            private const uint IHDR = (73 << 24) + (72 << 16) + (68 << 8) + 82;

            private Stream m_Stream;

            private uint m_ChunkName;

            public uint Width
            {
                get;
                private set;
            }

            public uint Height
            {
                get;
                private set;
            }

            public PNGReader(Stream stream)
            {
                m_Stream = stream;
            }

            //png_read_info
            public void ReadInfo()
            {
                ReadSig();

                //limit to 512 times
                int i;
                for (i = 0; i < 512; ++i)
                {
                    uint length = ReadChunkHeader();
                    uint chunkName = m_ChunkName;

                    if (chunkName == IDAT)
                    {
                        break;
                    }
                    else if (chunkName == IHDR)
                    {
                        HandleIHDR(length);
                    }
                    else
                    {
                        HandleUnknown(length);
                    }
                }
                if (i == 512)
                {
                    throw new Exception("read info loop too long");
                }
            }

            //png_get_uint_31
            private static uint GetUInt31(byte[] bytes, uint offset)
            {
                uint num = 0;
                for (int i = 0; i < 4; ++i)
                {
                    num = (num << 8) + bytes[i + offset];
                }
                return num;
            }

            //png_read_data
            private void ReadData(byte[] buffer, uint length)
            {
                if (m_Stream.Read(buffer, 0, (int)length) <= 0)
                {
                    throw new Exception("EOF reached");
                }
            }

            //png_crc_read
            private void CRCRead(byte[] buffer, uint length)
            {
                ReadData(buffer, length);
            }

            //png_crc_finish
            private void CRCFinish(uint skip)
            {
                m_Stream.Seek(skip, SeekOrigin.Current);
                CRCError();
            }

            //png_crc_error
            private void CRCError()
            {
                byte[] buffer = new byte[4];
                ReadData(buffer, 4);
            }

            //png_read_sig
            private void ReadSig()
            {
                byte[] buffer = new byte[8];
                ReadData(buffer, 8);
            }

            //png_read_chunk_header
            private uint ReadChunkHeader()
            {
                byte[] buffer = new byte[8];
                ReadData(buffer, 8);
                uint length = GetUInt31(buffer, 0);
                m_ChunkName = GetUInt31(buffer, 4);
                return length;
            }

            //png_handle_IHDR
            private void HandleIHDR(uint length)
            {
                if (length != 13)
                {
                    throw new Exception("invalid");
                }

                byte[] buffer = new byte[13];
                CRCRead(buffer, 13);
                CRCFinish(0);
                Width = GetUInt31(buffer, 0);
                Height = GetUInt31(buffer, 4);
            }

            //png_handle_unknown
            private void HandleUnknown(uint length)
            {
                CRCFinish(length);
            }
        }

        //libjpeg 6b
        private class JPGReader
        {
            //#define JPEG_SUSPENDED		0 /* Suspended due to lack of input data */
            private const int JPEG_SUSPENDED = 0;
            //#define JPEG_REACHED_SOS	1 /* Reached start of new scan */
            private const int JPEG_REACHED_SOS = 1;
            //#define JPEG_REACHED_EOI	2 /* Reached end of image */
            private const int JPEG_REACHED_EOI = 2;

            //#define DSTATE_START	200	/* after create_decompress */
            private const int DSTATE_START = 200;
            //#define DSTATE_INHEADER	201	/* reading header markers, no SOS yet */
            private const int DSTATE_INHEADER = 201;
            //#define DSTATE_READY	202	/* found SOS, ready for start_decompress */
            private const int DSTATE_READY = 202;
            //#define DSTATE_PRELOAD	203	/* reading multiscan file in start_decompress*/
            private const int DSTATE_PRELOAD = 203;
            //#define DSTATE_PRESCAN	204	/* performing dummy pass for 2-pass quant */
            private const int DSTATE_PRESCAN = 204;
            //#define DSTATE_SCANNING	205	/* start_decompress done, read_scanlines OK */
            private const int DSTATE_SCANNING = 205;
            //#define DSTATE_RAW_OK	206	/* start_decompress done, read_raw_data OK */
            private const int DSTATE_RAW_OK = 206;
            //#define DSTATE_BUFIMAGE	207	/* expecting jpeg_start_output */
            private const int DSTATE_BUFIMAGE = 207;
            //#define DSTATE_BUFPOST	208	/* looking for SOS/EOI in jpeg_finish_output */
            private const int DSTATE_BUFPOST = 208;
            //#define DSTATE_STOPPING	210	/* looking for EOI in jpeg_finish_decompress */
            private const int DSTATE_STOPPING = 210;

            /* JPEG marker codes */
            private enum JPEGMarker
            {
                M_SOF0 = 0xc0,
                M_SOF1 = 0xc1,
                M_SOF2 = 0xc2,
                M_SOF3 = 0xc3,

                M_SOF5 = 0xc5,
                M_SOF6 = 0xc6,
                M_SOF7 = 0xc7,

                M_JPG = 0xc8,
                M_SOF9 = 0xc9,
                M_SOF10 = 0xca,
                M_SOF11 = 0xcb,

                M_SOF13 = 0xcd,
                M_SOF14 = 0xce,
                M_SOF15 = 0xcf,

                M_DHT = 0xc4,

                M_DAC = 0xcc,

                M_RST0 = 0xd0,
                M_RST1 = 0xd1,
                M_RST2 = 0xd2,
                M_RST3 = 0xd3,
                M_RST4 = 0xd4,
                M_RST5 = 0xd5,
                M_RST6 = 0xd6,
                M_RST7 = 0xd7,

                M_SOI = 0xd8,
                M_EOI = 0xd9,
                M_SOS = 0xda,
                M_DQT = 0xdb,
                M_DNL = 0xdc,
                M_DRI = 0xdd,
                M_DHP = 0xde,
                M_EXP = 0xdf,

                M_APP0 = 0xe0,
                M_APP1 = 0xe1,
                M_APP2 = 0xe2,
                M_APP3 = 0xe3,
                M_APP4 = 0xe4,
                M_APP5 = 0xe5,
                M_APP6 = 0xe6,
                M_APP7 = 0xe7,
                M_APP8 = 0xe8,
                M_APP9 = 0xe9,
                M_APP10 = 0xea,
                M_APP11 = 0xeb,
                M_APP12 = 0xec,
                M_APP13 = 0xed,
                M_APP14 = 0xee,
                M_APP15 = 0xef,

                M_JPG0 = 0xf0,
                M_JPG13 = 0xfd,
                M_COM = 0xfe,

                M_TEM = 0x01,

                M_ERROR = 0x100
            }

            private Stream m_Stream;

            private int m_GlobalState;

            private bool m_EOIReached;

            private bool m_InHeaders;

            private int m_UnreadMarker;

            private bool m_SawSOI;

            private bool m_SawSOF;

            public int Width
            {
                get;
                private set;
            }

            public int Height
            {
                get;
                private set;
            }

            public JPGReader(Stream stream)
            {
                m_Stream = stream;
                m_GlobalState = DSTATE_START;
                m_EOIReached = false;
                m_InHeaders = true;
                m_UnreadMarker = 0;
                m_SawSOI = false;
                m_SawSOF = false;
            }

            //jpeg_read_header
            public void ReadHeader()
            {
                ConsumeInput();
            }

            //INPUT_BYTE
            private int InputByte()
            {
                int b = m_Stream.ReadByte();
                if (b < 0)
                {
                    throw new Exception("EOF reached");
                }
                return b;
            }

            //INPUT_2BYTES
            private int Input2Bytes()
            {
                int v = m_Stream.ReadByte();
                v = (v << 8) + m_Stream.ReadByte();
                return (ushort)v;
            }

            //jpeg_consume_input
            private int ConsumeInput()
            {
                int retcode = JPEG_SUSPENDED;

                switch (m_GlobalState)
                {
                    case DSTATE_START:
                        m_GlobalState = DSTATE_INHEADER;
                        ConsumeInput();
                        break;
                    case DSTATE_INHEADER:
                        retcode = ConsumeMakers();
                        if (retcode == JPEG_REACHED_SOS)
                        {
                            m_GlobalState = DSTATE_READY;
                        }
                        break;
                    case DSTATE_READY:
                        retcode = JPEG_REACHED_SOS;
                        break;
                    case DSTATE_PRELOAD:
                    case DSTATE_PRESCAN:
                    case DSTATE_SCANNING:
                    case DSTATE_RAW_OK:
                    case DSTATE_BUFIMAGE:
                    case DSTATE_BUFPOST:
                    case DSTATE_STOPPING:
                        retcode = ConsumeMakers();
                        break;
                    default:
                        throw new Exception("bad state");
                }
                return retcode;
            }

            //consume_markers
            private int ConsumeMakers()
            {
                if (m_EOIReached)
                {
                    return JPEG_REACHED_EOI;
                }

                int val = ReadMarkers();
                switch (val)
                {
                    case JPEG_REACHED_SOS:
                        if (m_InHeaders)
                        {
                            m_InHeaders = false;
                        }
                        else
                        {
                            throw new Exception("EOI expected");
                        }
                        break;
                    case JPEG_REACHED_EOI:
                        m_EOIReached = true;
                        if (m_InHeaders)
                        {
                            if (m_SawSOF)
                            {
                                throw new Exception("SOF no SOS");
                            }
                        }
                        break;
                    case JPEG_SUSPENDED:
                        break;
                }
                return val;
            }

            //read_markers
            private int ReadMarkers()
            {
                for (int i = 0; i < 512; ++i)
                {
                    if (m_UnreadMarker == 0)
                    {
                        if (!m_SawSOI)
                        {
                            if (!FirstMarker())
                                return JPEG_SUSPENDED;
                        }
                        else
                        {
                            if (!NextMarker())
                                return JPEG_SUSPENDED;
                        }
                    }

                    switch (m_UnreadMarker)
                    {
                        case (int)JPEGMarker.M_SOI:
                            if (!GetSOI())
                            {
                                return JPEG_SUSPENDED;
                            }
                            break;
                        case (int)JPEGMarker.M_SOF0:
                        case (int)JPEGMarker.M_SOF1:
                        case (int)JPEGMarker.M_SOF2:
                        case (int)JPEGMarker.M_SOF9:
                        case (int)JPEGMarker.M_SOF10:
                            if (!GetSOF())
                            {
                                return JPEG_SUSPENDED;
                            }
                            break;
                        case (int)JPEGMarker.M_SOS:
                            SkipVariable();
                            m_UnreadMarker = 0;
                            return JPEG_REACHED_SOS;
                        case (int)JPEGMarker.M_EOI:
                            m_UnreadMarker = 0;
                            return JPEG_REACHED_EOI;
                        case (int)JPEGMarker.M_DAC:
                        case (int)JPEGMarker.M_DHT:
                        case (int)JPEGMarker.M_DQT:
                        case (int)JPEGMarker.M_DRI:
                            SkipVariable();
                            break;
                        case (int)JPEGMarker.M_SOF3:
                        case (int)JPEGMarker.M_SOF5:
                        case (int)JPEGMarker.M_SOF6:
                        case (int)JPEGMarker.M_SOF7:
                        case (int)JPEGMarker.M_JPG:
                        case (int)JPEGMarker.M_SOF11:
                        case (int)JPEGMarker.M_SOF13:
                        case (int)JPEGMarker.M_SOF14:
                        case (int)JPEGMarker.M_SOF15:
                            throw new Exception("SOF unsupported");
                        case (int)JPEGMarker.M_APP0:
                        case (int)JPEGMarker.M_APP1:
                        case (int)JPEGMarker.M_APP2:
                        case (int)JPEGMarker.M_APP3:
                        case (int)JPEGMarker.M_APP4:
                        case (int)JPEGMarker.M_APP5:
                        case (int)JPEGMarker.M_APP6:
                        case (int)JPEGMarker.M_APP7:
                        case (int)JPEGMarker.M_APP8:
                        case (int)JPEGMarker.M_APP9:
                        case (int)JPEGMarker.M_APP10:
                        case (int)JPEGMarker.M_APP11:
                        case (int)JPEGMarker.M_APP12:
                        case (int)JPEGMarker.M_APP13:
                        case (int)JPEGMarker.M_APP14:
                        case (int)JPEGMarker.M_APP15:
                            SkipVariable();
                            break;
                        default:
                            throw new Exception("unknown marker");
                    }

                    m_UnreadMarker = 0;
                }

                throw new Exception("loop too long");
            }

            //first_marker
            private bool FirstMarker()
            {
                int c1 = InputByte(), c2 = InputByte();
                if (c1 != 0xFF || c2 != (byte)JPEGMarker.M_SOI)
                {
                    throw new Exception("no SOI");
                }
                m_UnreadMarker = c2;
                return true;
            }

            //next_marker
            private bool NextMarker()
            {
                int c = 0;
                //limit to 512 times
                int i;
                for (i = 0; i < 512; ++i)
                {
                    c = InputByte();
                    int j;
                    for (j = 0; j < 512 && c != 0xFF; ++j)
                    {
                        c = InputByte();
                    }
                    if (j == 512)
                    {
                        throw new Exception("read mark loop too long");
                    }
                    c = InputByte();
                    for (j = 0; j < 512 && c == 0xFF; ++j)
                    {
                        c = InputByte();
                    }
                    if (j == 512)
                    {
                        throw new Exception("skip mark loop too long");
                    }
                    if (c != 0)
                    {
                        break;
                    }
                }
                if (i == 512)
                {
                    throw new Exception("next mark loop too long");
                }
                m_UnreadMarker = c;
                return true;
            }

            //get_soi
            private bool GetSOI()
            {
                if (m_SawSOI)
                {
                    throw new Exception("SOI duplicate");
                }

                m_SawSOI = true;

                return true;
            }

            //get_sof
            private bool GetSOF()
            {
                int length = Input2Bytes();

                InputByte();
                Height = Input2Bytes();
                Width = Input2Bytes();
                int numComponents = InputByte();

                length -= 8;

                if (m_SawSOF)
                {
                    throw new Exception("SOF duplicate");
                }

                if (length != numComponents * 3)
                {
                    throw new Exception("bad length");
                }

                //limit to 512 times
                int i;
                for (i = 0; i < numComponents && i < 512; ++i)
                {
                    InputByte();
                    InputByte();
                    InputByte();
                }
                if (i == 512)
                {
                    throw new Exception("comp loop too long");
                }

                m_SawSOF = true;

                return true;
            }

            //skip_variable
            private bool SkipVariable()
            {
                int length = Input2Bytes();
                length -= 2;

                m_Stream.Seek(length, SeekOrigin.Current);

                return true;
            }
        }
    }

}

