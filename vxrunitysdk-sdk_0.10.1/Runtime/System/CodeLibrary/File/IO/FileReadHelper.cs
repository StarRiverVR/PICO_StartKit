using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 读文件
    /// </summary>
    public class FileReadHelper : CSingleton<FileReadHelper>, IDisposable
    {

        #region// ReadLine

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineCallBack">《文件路径，line内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        /// <returns></returns>
        public bool ReadLine(string filePath, System.Action<string, string,object> lineCallBack, System.Text.Encoding encoding, long startOffset =0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, filePath.PathToLower(), lineCallBack, null, null, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadLine, encoding, false, startOffset, countLimit, decryptKey);
            return res;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="lineCallBack">《文件路径，line内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        /// <returns></returns>
        public bool ReadLine(string filePath, string lockKey, System.Action<string, string, object> lineCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, lockKey, lineCallBack, null, null, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadLine, encoding, false, startOffset, countLimit, decryptKey);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lineCallBack">《文件路径，line内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadLineAsyn(string filePath, System.Action<string, string, object> lineCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding,
            long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, filePath.PathToLower(), lineCallBack, null, null, null, null, finishReadCallBack, parObj, ReadType.ReadLine, encoding, true, startOffset, countLimit, decryptKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="lineCallBack">《文件路径，line内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadLineAsyn(string filePath, string lockKey, System.Action<string, string, object> lineCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding,
            long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, lockKey, lineCallBack, null, null, null, null, finishReadCallBack, parObj, ReadType.ReadLine, encoding, true, startOffset, countLimit, decryptKey);
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="lineCallBack">《文件路径，line内容，null》</param>
        /// <param name="finishReadCallBack">《是否成功》</param>
        /// <param name="encoding"></param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadLineMutAsyn(List<string> filePaths, System.Action<string, string, object> lineCallBack, System.Action<bool> finishReadCallBack, System.Text.Encoding encoding,
            long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath))
                {
                    tempList.Add(filePath);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            ReadThreadCountData readThreadCountData = readThreadCountDataPool.Spawn();
            readThreadCountData.Count = tempList.Count;
            readThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePath = tempList[i];
                ReadLineAsyn(filePath, lineCallBack, (bl, resPath, obj) =>
                {
                    ReadThreadCountData countData = (ReadThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        readThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件读取错误!");
                            }
                            else
                            {
                                VLog.Info("多文件读取完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件读取完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, readThreadCountData, encoding, startOffset, countLimit, decryptKey);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        #endregion

        #region// ReadText

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="textCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        /// <returns></returns>
        public bool ReadText(string filePath, System.Action<string, string, object> textCallBack, System.Text.Encoding encoding, long startOffset = 0, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, filePath.PathToLower(), textCallBack, null, null, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadStrEnd, encoding, false, startOffset,-1, decryptKey);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="textCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        /// <returns></returns>
        public bool ReadText(string filePath, string lockKey, System.Action<string, string, object> textCallBack, System.Text.Encoding encoding, long startOffset = 0, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, lockKey, textCallBack, null, null, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadStrEnd, encoding, false, startOffset,-1, decryptKey);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="textCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadTextAsyn(string filePath, System.Action<string, string, object> textCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding
            , long startOffset = 0, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, filePath.PathToLower(), textCallBack, null, null, null, null, finishReadCallBack, parObj, ReadType.ReadStrEnd, encoding, true, startOffset,-1, decryptKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="textCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadTextAsyn(string filePath, string lockKey, System.Action<string, string, object> textCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding
             , long startOffset = 0, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, lockKey, textCallBack, null, null, null, null, finishReadCallBack, parObj, ReadType.ReadStrEnd, encoding, true, startOffset,-1, decryptKey);
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="textCallBack">《文件路径，内容，null》</param>
        /// <param name="finishReadCallBack">《是否成功》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadTextAsyn(List<string> filePaths, System.Action<string, string, object> textCallBack, System.Action<bool> finishReadCallBack , System.Text.Encoding encoding
             , long startOffset = 0, string decryptKey = null)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath))
                {
                    tempList.Add(filePath);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            ReadThreadCountData readThreadCountData = readThreadCountDataPool.Spawn();
            readThreadCountData.Count = tempList.Count;
            readThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePath = tempList[i];
                ReadTextAsyn(filePath, textCallBack, (bl, resPath, obj) =>
                {
                    ReadThreadCountData countData = (ReadThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        readThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件读取错误!");
                            }
                            else
                            {
                                VLog.Info("多文件读取完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件读取完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, readThreadCountData, encoding, startOffset, decryptKey);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        #endregion

        #region// ReadChar

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="charCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <returns></returns>
        public bool ReadChar(string filePath, System.Action<string, char, object> charCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, filePath.PathToLower(), null, charCallBack, null, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadChar, encoding, false, startOffset, countLimit);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="charCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <returns></returns>
        public bool ReadChar(string filePath, string lockKey, System.Action<string, char, object> charCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, lockKey, null, charCallBack, null, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadChar, encoding, false, startOffset, countLimit);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="charCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        public void ReadCharAsyn(string filePath, System.Action<string, char, object> charCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, filePath.PathToLower(), null, charCallBack, null, null, null, finishReadCallBack, parObj, ReadType.ReadChar, encoding, true, startOffset, countLimit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="charCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        public void ReadCharAsyn(string filePath, string lockKey, System.Action<string, char, object> charCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, lockKey, null, charCallBack, null, null, null, finishReadCallBack, parObj, ReadType.ReadChar, encoding, true, startOffset, countLimit);
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="charCallBack">《文件路径，内容，null》</param>
        /// <param name="finishReadCallBack">《是否成功》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        public void ReadCharAsyn(List<string> filePaths, System.Action<string, char, object> charCallBack, System.Action<bool> finishReadCallBack, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath))
                {
                    tempList.Add(filePath);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            ReadThreadCountData readThreadCountData = readThreadCountDataPool.Spawn();
            readThreadCountData.Count = tempList.Count;
            readThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePath = tempList[i];
                ReadCharAsyn(filePath, charCallBack, (bl, resPath, obj) =>
                {
                    ReadThreadCountData countData = (ReadThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        readThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件读取错误!");
                            }
                            else
                            {
                                VLog.Info("多文件读取完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件读取完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, readThreadCountData, encoding, startOffset, countLimit);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        #endregion

        #region// ReadCharArray

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="charArrayCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <returns></returns>
        public bool ReadCharArray(string filePath, System.Action<string, char[], object> charArrayCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, filePath.PathToLower(), null, null, charArrayCallBack, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadCharArray, encoding, false, startOffset, countLimit);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="charArrayCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <returns></returns>
        public bool ReadCharArray(string filePath, string lockKey, System.Action<string, char[], object> charArrayCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, lockKey, null, null, charArrayCallBack, null, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadCharArray, encoding, false, startOffset, countLimit);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="charArrayCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        public void ReadCharArrayAsyn(string filePath, System.Action<string, char[], object> charArrayCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, filePath.PathToLower(), null, null, charArrayCallBack, null, null, finishReadCallBack, parObj, ReadType.ReadCharArray, encoding, true, startOffset, countLimit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="charArrayCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        public void ReadCharArrayAsyn(string filePath, string lockKey, System.Action<string, char[], object> charArrayCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj,
            System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, lockKey, null, null, charArrayCallBack, null, null, finishReadCallBack, parObj, ReadType.ReadCharArray, encoding, true, startOffset, countLimit);
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="charArrayCallBack">《文件路径，内容，null》</param>
        /// <param name="finishReadCallBack">《是否成功》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        public void ReadCharArrayAsyn(List<string> filePaths, System.Action<string, char[], object> charArrayCallBack, System.Action<bool> finishReadCallBack, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath))
                {
                    tempList.Add(filePath);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            ReadThreadCountData readThreadCountData = readThreadCountDataPool.Spawn();
            readThreadCountData.Count = tempList.Count;
            readThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePath = tempList[i];
                ReadCharArrayAsyn(filePath, charArrayCallBack, (bl, resPath, obj) =>
                {
                    ReadThreadCountData countData = (ReadThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        readThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件读取错误!");
                            }
                            else
                            {
                                VLog.Info("多文件读取完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件读取完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, readThreadCountData, encoding, startOffset, countLimit);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        #endregion

        #region// ReadByteArray

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="byteArrayCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        /// <returns></returns>
        public bool ReadByteArray(string filePath, System.Action<string, byte[], object> byteArrayCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, filePath.PathToLower(), null, null, null, byteArrayCallBack, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadByteArray, encoding, false, startOffset, countLimit, decryptKey);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="byteArrayCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        /// <returns></returns>
        public bool ReadByteArray(string filePath, string lockKey, System.Action<string, byte[], object> byteArrayCallBack, System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, lockKey, null, null, null, byteArrayCallBack, null, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadByteArray, encoding, false, startOffset, countLimit, decryptKey);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="byteArrayCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadByteArrayAsyn(string filePath, System.Action<string, byte[], object> byteArrayCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, filePath.PathToLower(), null, null, null, byteArrayCallBack, null, finishReadCallBack, parObj, ReadType.ReadByteArray, encoding, true, startOffset, countLimit, decryptKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="byteArrayCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadByteArrayAsyn(string filePath, string lockKey, System.Action<string, byte[], object> byteArrayCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, 
            System.Text.Encoding encoding, long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, lockKey, null, null, null, byteArrayCallBack, null, finishReadCallBack, parObj, ReadType.ReadByteArray, encoding, true, startOffset, countLimit, decryptKey);
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="byteArrayCallBack">《文件路径，内容，null》</param>
        /// <param name="finishReadCallBack">《是否成功》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <param name="countLimit">读取数量限制, 小于等于0表示没有数量限制</param>
        /// <param name="decryptKey">解密密钥 8位 如："%o-!_d5@"</param>
        public void ReadByteArrayAsyn(List<string> filePaths, System.Action<string, byte[], object> byteArrayCallBack, System.Action<bool> finishReadCallBack, System.Text.Encoding encoding
            , long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath))
                {
                    tempList.Add(filePath);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            ReadThreadCountData readThreadCountData = readThreadCountDataPool.Spawn();
            readThreadCountData.Count = tempList.Count;
            readThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePath = tempList[i];
                ReadByteArrayAsyn(filePath, byteArrayCallBack, (bl, resPath, obj) =>
                {
                    ReadThreadCountData countData = (ReadThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        readThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件读取错误!");
                            }
                            else
                            {
                                VLog.Info("多文件读取完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件读取完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, readThreadCountData, encoding, startOffset, countLimit, decryptKey);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        #endregion

        #region// ReadSerializ

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <returns></returns>
        public bool ReadSerializ(string filePath, System.Action<string, object, object> serializCallBack, System.Text.Encoding encoding, long startOffset = 0)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, filePath.PathToLower(), null, null, null, null, serializCallBack, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadSerializ, encoding, false, startOffset);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="serializCallBack">《文件路径，内容，null》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        /// <returns></returns>
        public bool ReadSerializ(string filePath, string lockKey, System.Action<string, object, object> serializCallBack, System.Text.Encoding encoding, long startOffset = 0)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return false;
            }
            bool res = false;
            Read(filePath, lockKey, null, null, null, null, serializCallBack, (bl, path, obj) => {
                res = bl;
            }, null, ReadType.ReadByteArray, encoding, false, startOffset);
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="serializCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        public void ReadSerializAsyn(string filePath, System.Action<string, object, object> serializCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, 
            System.Text.Encoding encoding, long startOffset = 0)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, filePath.PathToLower(), null, null, null, null, serializCallBack, finishReadCallBack, parObj, ReadType.ReadByteArray, encoding, true, startOffset);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lockKey">filePath.PathToLower()</param>
        /// <param name="serializCallBack">《文件路径，内容，传入的参数parObj》</param>
        /// <param name="finishReadCallBack">《是否成功，文件路径，传入的参数parObj》</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        public void ReadSerializAsyn(string filePath, string lockKey, System.Action<string, object, object> serializCallBack, System.Action<bool, string, object> finishReadCallBack, object parObj, 
            System.Text.Encoding encoding, long startOffset = 0)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            Read(filePath, lockKey, null, null, null, null, serializCallBack, finishReadCallBack, parObj, ReadType.ReadByteArray, encoding, true, startOffset);
        }

        /// <summary>
        /// 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="serializCallBack">《文件路径，内容，null》</param>
        /// <param name="finishReadCallBack">《是否成功》</param>
        /// <param name="encoding"></param>
        /// <param name="startOffset">读取偏移量</param>
        public void ReadSerializAsyn(List<string> filePaths, System.Action<string, object, object> serializCallBack, System.Action<bool> finishReadCallBack, System.Text.Encoding encoding, long startOffset = 0)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishReadCallBack(false);
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath))
                {
                    tempList.Add(filePath);
                }
            }
            if (tempList.Count==0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                finishReadCallBack(false);
                return;
            }
            ReadThreadCountData readThreadCountData = readThreadCountDataPool.Spawn();
            readThreadCountData.Count = tempList.Count;
            readThreadCountData.finishReadCallBack = finishReadCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string filePath = tempList[i];
                ReadSerializAsyn(filePath, serializCallBack, (bl, resPath, obj) =>
                {
                    ReadThreadCountData countData = (ReadThreadCountData)obj;
                    if (!bl)
                    {
                        countData.AllIsFinish = false;
                    }
                    countData.CountSub();
                    if (countData.Count == 0)
                    {
                        bool isBl = countData.AllIsFinish;
                        System.Action<bool> readCallBack = countData.finishReadCallBack;
                        readThreadCountDataPool.Recycle(countData);
                        try
                        {
                            if (isBl)
                            {
                                VLog.Error("多文件读取错误!");
                            }
                            else
                            {
                                VLog.Info("多文件读取完成!");
                            }
                            readCallBack.Invoke(isBl);
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error("多文件读取完成回调错误!");
                            VLog.Exception(ex);
                        }
                    }

                }, readThreadCountData, encoding, startOffset);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        #endregion

        void Read(string filePath, string lockKey, System.Action<string, string, object> lineCallBack, System.Action<string, char, object> charCallBack, 
            System.Action<string, char[], object> charArrayCallBack, System.Action<string, byte[], object> byteArrayCallBack,
            System.Action<string, object, object> serializCallBack,System.Action<bool, string, object> finishReadCallBack,
            object parObj, ReadType readType, System.Text.Encoding encoding, bool useThread = true, long startOffset = 0, long countLimit = -1, string decryptKey = null)
        {
            if (IsRelease) return;
            if (useThread)
            {
                ReadLineThreadData readLineThreadData = readLineThreadDataPool.Spawn();
                readLineThreadData.parObj = parObj;
                readLineThreadData.Encoding = encoding;
                readLineThreadData.FilePath = filePath;
                readLineThreadData.LockKey = lockKey;
                readLineThreadData.StrCallBack = lineCallBack;
                readLineThreadData.CharCallBack = charCallBack;
                readLineThreadData.CharArrayCallBack = charArrayCallBack;
                readLineThreadData.ByteArrayCallBack = byteArrayCallBack;
                readLineThreadData.SerializCallBack = serializCallBack;
                readLineThreadData.FinishReadCallBack = finishReadCallBack;
                readLineThreadData.UseThread = true;
                readLineThreadData.ReadType = readType;
                readLineThreadData.countLimit = countLimit;
                readLineThreadData.startOffset = startOffset;
                readLineThreadData.decryptKey = decryptKey;
                if (SynchronizationContext.Current== ThreadHelper.UnitySynchronizationContext)
                {
                    readLineThreadData.Context = ThreadHelper.UnitySynchronizationContext;
                    AddReadThread(readLineThreadData);
                }
                else
                {
                    ThreadHelper.StartTask(ReadLine, readLineThreadData, (res, par, ex) => {
                        ReadLineThreadData tData = (ReadLineThreadData)par;
                        System.Action<bool, string, object> cBack = tData.FinishReadCallBack;
                        object callObj = tData.parObj;
                        string resPath = tData.FilePath;
                        bool finish = false;
                        if (ex != null)
                        {
                            VLog.Error($"文件读取错误:{resPath}");
                            VLog.Exception(ex);
                        }
                        else
                        {
                            finish = (bool)res;
                        }
                        try
                        {
                            cBack.Invoke(finish, resPath, callObj);
                        }
                        catch (System.Exception e)
                        {
                            VLog.Exception(e);
                        }
                        readLineThreadDataPool.Recycle(tData);
                    });
                }
            }
            else
            {
                ReadLineThreadData readLineThreadData = readLineThreadDataPool.Spawn();
                readLineThreadData.parObj = parObj;
                readLineThreadData.Encoding = encoding;
                readLineThreadData.FilePath = filePath;
                readLineThreadData.LockKey = lockKey;
                readLineThreadData.StrCallBack = lineCallBack;
                readLineThreadData.CharCallBack = charCallBack;
                readLineThreadData.CharArrayCallBack = charArrayCallBack;
                readLineThreadData.ByteArrayCallBack = byteArrayCallBack;
                readLineThreadData.SerializCallBack = serializCallBack;
                readLineThreadData.FinishReadCallBack = finishReadCallBack;
                readLineThreadData.Context = ThreadHelper.UnitySynchronizationContext;
                readLineThreadData.UseThread = false;
                readLineThreadData.ReadType = readType;
                readLineThreadData.countLimit = countLimit;
                readLineThreadData.startOffset = startOffset;
                readLineThreadData.decryptKey = decryptKey;
                //
                object res = ReadLine(readLineThreadData);
                bool finish = (bool)res;
                readLineThreadData.FinishReadCallBack.Invoke(finish, readLineThreadData.FilePath, readLineThreadData.parObj);
                readLineThreadDataPool.Recycle(readLineThreadData);
            }
        }

        /// <summary>
        /// 同时读取数量限制
        /// </summary>
        int maxThreadReadCount = 64;

        int curThreadReadCount = 0;

        object curThreadReadCountLock = new object();

        void AddCurThreadReadCount()
        {
            lock (curThreadReadCountLock)
            {
                curThreadReadCount = curThreadReadCount + 1;
            }
        }

        void SubCurThreadReadCount()
        {
            lock (curThreadReadCountLock)
            {
                curThreadReadCount = curThreadReadCount - 1;
                RunReadThreadList();
            }
        }

        int GetCurThreadReadCount()
        {
            lock (curThreadReadCountLock)
            {
                return curThreadReadCount;
            }
        }

        Queue<ReadLineThreadData> readAddThreads = new Queue<ReadLineThreadData>();

        SimpleListPool<List<ReadLineThreadData>, ReadLineThreadData> readOutThreadPool = new SimpleListPool<List<ReadLineThreadData>, ReadLineThreadData>();

        void AddReadThread(ReadLineThreadData readThreadData)
        {

            lock (readAddThreads)
            {
                if (IsRelease)
                {
                    return;
                }
                readAddThreads.Enqueue(readThreadData);
            }

            RunReadThreadList();
        }

        void RunReadThreadList()
        {
            int count = maxThreadReadCount - GetCurThreadReadCount();
            if (count > 0)
            {
                Task task = Task.Factory.StartNew((obj) => {
                    int addCount = (int)obj;
                    List<ReadLineThreadData> readOutThreads = readOutThreadPool.Spawn();
                    lock (readAddThreads)
                    {
                        int num = 0;
                        while (readAddThreads.Count > 0)
                        {
                            ReadLineThreadData data = readAddThreads.Dequeue();
                            readOutThreads.Add(data);
                            num++;
                            if (num >= addCount)
                            {
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < readOutThreads.Count; ++i)
                    {
                        ReadLineThreadData tData = readOutThreads[i];
                        System.Action<bool, string, object> cBack = tData.FinishReadCallBack;
                        object callObj = tData.parObj;
                        string resPath = tData.FilePath;
                        bool finish =(bool)ReadLine(tData);
                        if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
                        {
                            ThreadHelper.UnitySynchronizationContext.Send((o) => {
                                ErrLockData errLockDataA = null;
                                if (ErrLock.ErrLockOpen)
                                {
                                    errLockDataA = ErrLock.LockStart("FileReadHelper.cs-->RunReadThreadList-->1093");
                                }
                                try
                                {
                                    cBack.Invoke(finish, resPath, callObj);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Exception(e);
                                }
                                if (ErrLock.ErrLockOpen)
                                {
                                    ErrLock.LockEnd(errLockDataA);
                                }
                            }, null);
                        }
                        else
                        {
                            try
                            {
                                cBack.Invoke(finish, resPath, callObj);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Exception(e);
                            }
                        }
                        readLineThreadDataPool.Recycle(tData);
                    }
                    readOutThreadPool.Recycle(readOutThreads);
                }, count);
            }
        }

        SimpleListPool<List<char[]>, char[]> cahrArrayList = new SimpleListPool<List<char[]>, char[]>();

        object ReadLine(object obj)
        {
            if (IsRelease) return false;
            AddCurThreadReadCount();
            ReadLineThreadData readLineThreadData = (ReadLineThreadData)obj;
            string decryptKey = readLineThreadData.decryptKey;
            StreamReader streamReader = null;
            FileStream fileStream = null;
            string lockKey = readLineThreadData.LockKey;
            bool finish = false;
            try
            {
                lock (FileLock.GetStringLock(lockKey))
                {
                    //
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("FileReadHelper.cs -- > 1013 --> {0}", lockKey));
                    }
                    //
                    FileWriteHelper.Instance.Close(readLineThreadData.FilePath);
                    if (File.Exists(readLineThreadData.FilePath))
                    {
                        fileStream = new FileStream(readLineThreadData.FilePath, FileMode.Open);
                        if (readLineThreadData.startOffset > 0)
                        {
                            fileStream.Position = System.Math.Min(readLineThreadData.startOffset, fileStream.Length);
                        }
                        switch (readLineThreadData.ReadType)
                        {
                            case ReadType.ReadLine:
                                {
                                    streamReader = new StreamReader(fileStream, readLineThreadData.Encoding, true);
                                    string strLine = streamReader.ReadLine();
                                    while (strLine != null)
                                    {
                                        if (decryptKey != null)
                                        {
                                            strLine=EncryptHelper.DecryptDES(strLine, decryptKey);
                                        }
                                        ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                        readLineThreadStrData.Str = strLine;
                                        readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                        readLineThreadStrData.StrCallBack = readLineThreadData.StrCallBack;
                                        readLineThreadStrData.parObj = readLineThreadData.parObj;
                                        readLineThreadStrData.countLimit= readLineThreadData.countLimit-1;
                                        readLineThreadData.countLimit = readLineThreadData.countLimit - 1;
                                        if (readLineThreadData.UseThread)
                                        {
                                            readLineThreadData.Context.Post((obj) => {
                                                ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                                data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                                readLineThreadStrDataPool.Recycle(data);
                                            }, readLineThreadStrData);
                                        }
                                        else
                                        {
                                            readLineThreadData.Context.Send((obj) => {
                                                ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                                data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                                readLineThreadStrDataPool.Recycle(data);
                                            }, readLineThreadStrData);
                                        }
                                        if (readLineThreadStrData.countLimit == 0)
                                        {
                                            break;
                                        }
                                        strLine = streamReader.ReadLine();
                                    }
                                }
                                break;
                            case ReadType.ReadStrEnd:
                                {
                                    streamReader = new StreamReader(fileStream, readLineThreadData.Encoding, true);
                                    string str = streamReader.ReadToEnd();
                                    if (decryptKey != null)
                                    {
                                        str = EncryptHelper.DecryptDES(str, decryptKey);
                                    }
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.Str = str;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.StrCallBack = readLineThreadData.StrCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadChar:
                                {
                                    streamReader = new StreamReader(fileStream, readLineThreadData.Encoding, true);
                                    while (streamReader.Peek() >= 0)
                                    {
                                        char cr = (char)streamReader.Read();
                                        ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                        readLineThreadStrData.Char = cr;
                                        readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                        readLineThreadStrData.CharCallBack = readLineThreadData.CharCallBack;
                                        readLineThreadStrData.parObj = readLineThreadData.parObj;
                                        readLineThreadStrData.countLimit = readLineThreadData.countLimit - 1;
                                        readLineThreadData.countLimit = readLineThreadData.countLimit - 1;

                                        if (readLineThreadData.UseThread)
                                        {
                                            readLineThreadData.Context.Post((obj) => {
                                                ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                                data.CharCallBack.Invoke(data.FilePath, data.Char, data.parObj);
                                                readLineThreadStrDataPool.Recycle(data);
                                            }, readLineThreadStrData);
                                        }
                                        else
                                        {
                                            readLineThreadData.Context.Send((obj) => {
                                                ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                                data.CharCallBack.Invoke(data.FilePath, data.Char, data.parObj);
                                                readLineThreadStrDataPool.Recycle(data);
                                            }, readLineThreadStrData);
                                        }
                                        if (readLineThreadStrData.countLimit == 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                break;
                            case ReadType.ReadCharArray:
                                {
                                    streamReader = new StreamReader(fileStream, readLineThreadData.Encoding, true);

                                    long countLimit = readLineThreadData.countLimit;
                                    long charLenght = 0;
                                    if (countLimit >= 0)
                                    {
                                        charLenght = System.Math.Min(fileStream.Length - fileStream.Position, countLimit);
                                    }
                                    else
                                    {
                                        charLenght = fileStream.Length;
                                    }
                                    char[] resoultArray = new char[charLenght];
                                    streamReader.Read(resoultArray, 0, resoultArray.Length);

                                    //long countLimit = readLineThreadData.countLimit;
                                    //int size = 1024;
                                    //List<char[]> charArrayList = cahrArrayList.Spawn();
                                    //long lenght = 0;
                                    //int setSize = size;
                                    //while (streamReader.Peek() >= 0)
                                    //{
                                    //    if (countLimit>0)
                                    //    {
                                    //        long lastCount = countLimit - lenght;
                                    //        if (lastCount<=0)
                                    //        {
                                    //            break;
                                    //        }
                                    //        if (size> lastCount)
                                    //        {
                                    //            setSize = (int)lastCount;
                                    //        }
                                    //    }
                                    //    char[] charArray = new char[setSize];
                                    //    int count = streamReader.Read(charArray, 0, charArray.Length);
                                    //    lenght = lenght + count;
                                    //    charArrayList.Add(charArray);
                                    //}
                                    //char[] resoultArray = null;
                                    //if (lenght > 0)
                                    //{
                                    //    if (countLimit>0)
                                    //    {
                                    //        resoultArray = new char[System.Math.Min(lenght, countLimit)];
                                    //    }
                                    //    else
                                    //    {
                                    //        resoultArray = new char[lenght];
                                    //    }
                                    //    lenght = resoultArray.LongLength;
                                    //}

                                    //long index = 0;
                                    //for (int i = 0; i < charArrayList.Count; ++i)
                                    //{
                                    //    char[] charArray = charArrayList[i];
                                    //    long copyLenght = Math.Min(lenght, charArray.LongLength);
                                    //    lenght = lenght - copyLenght;
                                    //    Array.Copy(charArray, 0, resoultArray, index, copyLenght);
                                    //    index = index + copyLenght;
                                    //}

                                    //cahrArrayList.Recycle(charArrayList);
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.CharArray = resoultArray;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.CharArrayCallBack = readLineThreadData.CharArrayCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.CharArrayCallBack.Invoke(data.FilePath, data.CharArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.CharArrayCallBack.Invoke(data.FilePath, data.CharArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadByteArray:
                                {
                                    long countLimit = readLineThreadData.countLimit;
                                    long byteLenght = 0;
                                    if (countLimit>=0)
                                    {
                                        byteLenght = System.Math.Min(fileStream.Length- fileStream.Position, countLimit);
                                    }
                                    else
                                    {
                                        byteLenght = fileStream.Length;
                                    }
                                    byte[] byteArray = new byte[byteLenght];
                                    fileStream.Read(byteArray, 0, byteArray.Length);
                                    if (decryptKey != null)
                                    {
                                        byteArray = EncryptHelper.DecryptDES_B(byteArray, decryptKey);
                                    }
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.ByteArray = byteArray;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.ByteArrayCallBack = readLineThreadData.ByteArrayCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.ByteArrayCallBack.Invoke(data.FilePath, data.ByteArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.ByteArrayCallBack.Invoke(data.FilePath, data.ByteArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadSerializ:
                                {
                                    object serializObj = FileHelper.BinaryFormatter.Deserialize(fileStream);
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.SerializObject = serializObj;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.SerializCallBack = readLineThreadData.SerializCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.SerializCallBack.Invoke(data.FilePath, data.SerializObject, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.SerializCallBack.Invoke(data.FilePath, data.SerializObject, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                        }

                        if (fileStream != null)
                        {
                            fileStream.Close();
                        }
                        if (streamReader != null)
                        {
                            streamReader.Close();
                        }
                        finish = true;
                    }
                    else
                    {
                        switch (readLineThreadData.ReadType)
                        {
                            case ReadType.ReadLine:
                                {
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.Str = null;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.StrCallBack = readLineThreadData.StrCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadStrEnd:
                                {
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.Str = null;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.StrCallBack = readLineThreadData.StrCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadChar:
                                {
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.Char = '\0';
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.CharCallBack = readLineThreadData.CharCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.CharCallBack.Invoke(data.FilePath, data.Char, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.CharCallBack.Invoke(data.FilePath, data.Char, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadCharArray:
                                {
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.CharArray = null;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.CharArrayCallBack = readLineThreadData.CharArrayCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.CharArrayCallBack.Invoke(data.FilePath, data.CharArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.CharArrayCallBack.Invoke(data.FilePath, data.CharArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadByteArray:
                                {
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.ByteArray = null;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.ByteArrayCallBack = readLineThreadData.ByteArrayCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.ByteArrayCallBack.Invoke(data.FilePath, data.ByteArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.ByteArrayCallBack.Invoke(data.FilePath, data.ByteArray, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                            case ReadType.ReadSerializ:
                                {
                                    ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                                    readLineThreadStrData.SerializObject = null;
                                    readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                                    readLineThreadStrData.SerializCallBack = readLineThreadData.SerializCallBack;
                                    readLineThreadStrData.parObj = readLineThreadData.parObj;
                                    if (readLineThreadData.UseThread)
                                    {
                                        readLineThreadData.Context.Post((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.SerializCallBack.Invoke(data.FilePath, data.SerializObject, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                    else
                                    {
                                        readLineThreadData.Context.Send((obj) => {
                                            ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                            data.SerializCallBack.Invoke(data.FilePath, data.SerializObject, data.parObj);
                                            readLineThreadStrDataPool.Recycle(data);
                                        }, readLineThreadStrData);
                                    }
                                }
                                break;
                        }
                    }
                    //
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    //
                }
            }
            catch (System.Exception e)
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                if (streamReader != null)
                {
                    streamReader.Close();
                }
                switch (readLineThreadData.ReadType)
                {
                    case ReadType.ReadLine:
                        {
                            ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                            readLineThreadStrData.Str = null;
                            readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                            readLineThreadStrData.StrCallBack = readLineThreadData.StrCallBack;
                            readLineThreadStrData.parObj = readLineThreadData.parObj;
                            if (readLineThreadData.UseThread)
                            {
                                readLineThreadData.Context.Post((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                            else
                            {
                                readLineThreadData.Context.Send((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                        }
                        break;
                    case ReadType.ReadStrEnd:
                        {
                            ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                            readLineThreadStrData.Str = null;
                            readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                            readLineThreadStrData.StrCallBack = readLineThreadData.StrCallBack;
                            readLineThreadStrData.parObj = readLineThreadData.parObj;
                            if (readLineThreadData.UseThread)
                            {
                                readLineThreadData.Context.Post((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                            else
                            {
                                readLineThreadData.Context.Send((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.StrCallBack.Invoke(data.FilePath, data.Str, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                        }
                        break;
                    case ReadType.ReadChar:
                        {
                            ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                            readLineThreadStrData.Char = '\0';
                            readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                            readLineThreadStrData.CharCallBack = readLineThreadData.CharCallBack;
                            readLineThreadStrData.parObj = readLineThreadData.parObj;
                            if (readLineThreadData.UseThread)
                            {
                                readLineThreadData.Context.Post((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.CharCallBack.Invoke(data.FilePath, data.Char, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                            else
                            {
                                readLineThreadData.Context.Send((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.CharCallBack.Invoke(data.FilePath, data.Char, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                        }
                        break;
                    case ReadType.ReadCharArray:
                        {
                            ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                            readLineThreadStrData.CharArray = null;
                            readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                            readLineThreadStrData.CharArrayCallBack = readLineThreadData.CharArrayCallBack;
                            readLineThreadStrData.parObj = readLineThreadData.parObj;
                            if (readLineThreadData.UseThread)
                            {
                                readLineThreadData.Context.Post((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.CharArrayCallBack.Invoke(data.FilePath, data.CharArray, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                            else
                            {
                                readLineThreadData.Context.Send((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.CharArrayCallBack.Invoke(data.FilePath, data.CharArray, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                        }
                        break;
                    case ReadType.ReadByteArray:
                        {
                            ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                            readLineThreadStrData.ByteArray = null;
                            readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                            readLineThreadStrData.ByteArrayCallBack = readLineThreadData.ByteArrayCallBack;
                            readLineThreadStrData.parObj = readLineThreadData.parObj;
                            if (readLineThreadData.UseThread)
                            {
                                readLineThreadData.Context.Post((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.ByteArrayCallBack.Invoke(data.FilePath, data.ByteArray, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                            else
                            {
                                readLineThreadData.Context.Send((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.ByteArrayCallBack.Invoke(data.FilePath, data.ByteArray, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                        }
                        break;
                    case ReadType.ReadSerializ:
                        {
                            ReadLineThreadStrData readLineThreadStrData = readLineThreadStrDataPool.Spawn();
                            readLineThreadStrData.SerializObject = null;
                            readLineThreadStrData.FilePath = readLineThreadData.FilePath;
                            readLineThreadStrData.SerializCallBack = readLineThreadData.SerializCallBack;
                            readLineThreadStrData.parObj = readLineThreadData.parObj;
                            if (readLineThreadData.UseThread)
                            {
                                readLineThreadData.Context.Post((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.SerializCallBack.Invoke(data.FilePath, data.SerializObject, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                            else
                            {
                                readLineThreadData.Context.Send((obj) => {
                                    ReadLineThreadStrData data = (ReadLineThreadStrData)obj;
                                    data.SerializCallBack.Invoke(data.FilePath, data.SerializObject, data.parObj);
                                    readLineThreadStrDataPool.Recycle(data);
                                }, readLineThreadStrData);
                            }
                        }
                        break;
                }
                VLog.Exception(e);
            }
            SubCurThreadReadCount();
            return finish;
        }

        class ReadThreadData : ISimplePoolData
        {
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

            public virtual void PutIn()
            {
                isUsed = false;
            }

            public virtual void PutOut()
            {
                isUsed = true;
            }
        }

        SimplePool<ReadLineThreadData> readLineThreadDataPool = new SimplePool<ReadLineThreadData>();

        class ReadLineThreadData : ReadThreadData, IThreadHelperPar
        {
            public string FilePath;

            public string LockKey;

            public object parObj;

            public System.Text.Encoding Encoding;

            public System.Action<string, string, object> StrCallBack;

            public System.Action<string, char, object> CharCallBack;

            public System.Action<string, char[], object> CharArrayCallBack;

            public System.Action<string, byte[], object> ByteArrayCallBack;

            public System.Action<string, object, object> SerializCallBack;

            public System.Action<bool, string, object> FinishReadCallBack;

            public bool UseThread;

            public ReadType ReadType;

            public long countLimit =-1;

            public long startOffset = 0;

            public string decryptKey;

            public SynchronizationContext Context { get; set; }

            public override void PutIn()
            {
                decryptKey = null;
                startOffset = 0;
                countLimit = -1;
                parObj = null;
                SerializCallBack = null;
                ByteArrayCallBack = null;
                CharArrayCallBack = null;
                CharCallBack = null;
                FilePath = null;
                StrCallBack = null;
                FinishReadCallBack = null;
                Context = null;
                base.PutIn();
            }
        }

        SimplePool<ReadLineThreadStrData> readLineThreadStrDataPool = new SimplePool<ReadLineThreadStrData>();

        class ReadLineThreadStrData : ReadThreadData
        {
            public System.Action<string, string, object> StrCallBack;

            public System.Action<string, char, object> CharCallBack;

            public System.Action<string, char[], object> CharArrayCallBack;

            public System.Action<string, byte[], object> ByteArrayCallBack;

            public System.Action<string, object, object> SerializCallBack;

            public string Str;

            public object parObj;

            public char Char;

            public char[] CharArray;

            public byte[] ByteArray;

            public object SerializObject;

            public string FilePath;

            public string LockKey;

            public long countLimit = -1;

            public override void PutIn()
            {
                countLimit = -1;
                parObj = null;
                ByteArrayCallBack = null;
                SerializCallBack = null;
                CharArray = null;
                CharArrayCallBack = null;
                CharCallBack = null;
                StrCallBack = null;
                ByteArray = null;
                Str = null;
                SerializObject = null;
                FilePath = null;
                LockKey = null;
                base.PutIn();
            }
        }

        enum ReadType
        {
            ReadLine,
            ReadStrEnd,
            ReadChar,
            ReadCharArray,
            ReadByteArray,
            ReadSerializ,
        }

        SimplePool<ReadThreadCountData> readThreadCountDataPool = new SimplePool<ReadThreadCountData>();

        class ReadThreadCountData : ISimplePoolData
        {

            public System.Action<bool> finishReadCallBack;

            bool allIsFinish = true;

            public bool AllIsFinish
            {
                get
                {
                    lock (lockObj)
                    {
                        return allIsFinish;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        allIsFinish = value;
                    }
                }
            }

            int count = 0;

            public int Count
            {
                get
                {
                    lock (lockObj)
                    {
                        return count;
                    }
                }
                set
                {
                    lock (lockObj)
                    {
                        count = value;
                    }
                }
            }

            public void CountSub()
            {
                lock (lockObj)
                {
                    count = count - 1;
                }
            }

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
                finishReadCallBack = null;
                AllIsFinish = true;
                Count = 0;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        public void Dispose()
        {
            readLineThreadDataPool.Clear();
            readLineThreadStrDataPool.Clear();
            readThreadCountDataPool.Clear();
            cahrArrayList.Clear();
        }
    }
}
