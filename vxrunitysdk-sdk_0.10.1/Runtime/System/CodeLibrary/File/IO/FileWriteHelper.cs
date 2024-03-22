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
    /// 写文件
    /// </summary>
    public class FileWriteHelper : CSingleton<FileWriteHelper>, IDisposable
    {

        public long GetSizeByMB(int mb)
        {
            long b = mb * 1024 * 1024;
            long size = b / 8;
            return size;
        }

        object lockDic = new object();

        Dictionary<string, StreamWriteData> streamWriters = new Dictionary<string, StreamWriteData>();

        List<StreamWriteData> tempList = new List<StreamWriteData>();

        long id = 0;

        StreamWriteData GetStreamWriter(string filePath,string lockKey, string tex, byte[] bytes,object serializObj, bool append, bool errLogWrite = true)
        {
            StreamWriteData writeData = null;
            lock (FileLock.GetStringLock(lockKey))
            {
                //
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->37-->GetStreamWriter-->{0}", lockKey));
                }
                //
                lock (lockDic)
                {
                    //
                    ErrLockData errLockDataDic = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockDataDic = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->40-->GetStreamWriter Dic-->{0}", lockKey));
                    }
                    //
                    try
                    {
                        long timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();
                        //超时关闭 超过20分钟为操作强制关闭
                        Dictionary<string, StreamWriteData>.Enumerator enumerator = streamWriters.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            StreamWriteData data = enumerator.Current.Value;
                            if ((timeStamp-data.TimeStamp)>1200)
                            {
                                tempList.Add(data);
                            }
                        }
                        if (tempList.Count>0)
                        {
                            for (int i=0;i< tempList.Count;++i)
                            {
                                StreamWriteData data = tempList[i];
                                CloseStreamWriter(data.FilePath, data.LockKey, data.Id, data);
                            }
                            tempList.Clear();
                        }
                        int closeCount = streamWriters.Count-64;
                        if (closeCount>0)
                        {
                            enumerator = streamWriters.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                StreamWriteData data = enumerator.Current.Value;
                                tempList.Add(data);
                            }
                            tempList.Sort((A,B) => {
                                return (timeStamp - A.TimeStamp).CompareTo((timeStamp - B.TimeStamp));
                            });

                            for (int i=0;i<64;++i)
                            {
                                tempList.RemoveAt(0);
                            }
                            if (tempList.Count > 0)
                            {
                                for (int i = 0; i < tempList.Count; ++i)
                                {
                                    StreamWriteData data = tempList[i];
                                    CloseStreamWriter(data.FilePath, data.LockKey, data.Id, data);
                                }
                                tempList.Clear();
                            }
                        }
                        //
                        WriteType writeType;
                        if (serializObj != null)
                        {
                            writeType = WriteType.Serializ;
                        }
                        else if (bytes != null)
                        {
                            writeType = WriteType.Bytes;
                        }
                        else
                        {
                            writeType = WriteType.Text;
                        }
                        if (!streamWriters.TryGetValue(lockKey, out writeData))
                        {
                            writeData = streamWriteDataPool.Spawn();
                            writeData.FilePath = filePath;
                            InitWriteData(writeType,writeData, filePath, append);
                            writeData.Id = id;
                            writeData.TimeStamp = timeStamp;
                            id++;
                            streamWriters.Add(lockKey, writeData);
                        }
                        else
                        {
                            if (writeData.Append != append || writeType!= writeData.WriteType)
                            {
                                CloseStreamWriter(filePath, lockKey, writeData.Id, writeData);
                                writeData = streamWriteDataPool.Spawn();
                                writeData.FilePath = filePath;
                                InitWriteData(writeType,writeData, filePath,  append);
                                writeData.Id = id;
                                writeData.TimeStamp = timeStamp;
                                id++;
                                streamWriters.Add(lockKey, writeData);
                            }
                            else
                            {
                                InitWriteData(writeType, writeData, filePath, append);
                                writeData.TimeStamp = timeStamp;
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                        //
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                            ErrLock.LockEnd(errLockDataDic);
                        }
                        //
                        return null;
                    }
                    //
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockDataDic);
                    }
                    //
                }
                //
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                //
            }
            return writeData;
        }

        enum WriteType
        {
            Text,
            Bytes,
            Serializ,
        }

        void InitWriteData(WriteType writeType,StreamWriteData writeData, string filePath , bool append)
        {
            if (writeType== WriteType.Text && writeData.FirstStreamWriter == null)
            {
                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(filePath, append, FileHelper.WriteEncoding);
                streamWriter.AutoFlush = true;
                writeData.FirstStreamWriter = streamWriter;
            }
            if (writeType == WriteType.Bytes && writeData.FirstBinaryWriter == null)
            {
                FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                System.IO.BinaryWriter binaryWriter = new System.IO.BinaryWriter(fileStream, FileHelper.WriteEncoding);
                writeData.FirstBinaryWriter = binaryWriter;
            }
            if (writeType == WriteType.Serializ && writeData.FirstBinaryFormatterFileStream == null)
            {
                FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
                writeData.FirstBinaryFormatterFileStream = fileStream;
            }
            writeData.WriteType = writeType;
            writeData.Append = append;
        }

        void CloseStreamWriter(string filePath, string lockKey, long writeDataId, StreamWriteData writeData, bool errLogWrite = true)
        {
            lock (FileLock.GetStringLock(lockKey))
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->154-->CloseStreamWriter-->{0}", lockKey));
                }

                lock (lockDic)
                {
                    if (!string.IsNullOrEmpty(lockKey))
                    {
                        StreamWriteData findData;
                        if (streamWriters.TryGetValue(lockKey,out findData))
                        {
                            if (writeDataId == writeData.Id)
                            {
                                if (findData== writeData)
                                {
                                    writeData.Close(errLogWrite);
                                    streamWriteDataPool.Recycle(writeData);
                                }
                                else
                                {
                                    findData.Close(errLogWrite);
                                    streamWriteDataPool.Recycle(findData);
                                    writeData.Close(errLogWrite);
                                    streamWriteDataPool.Recycle(writeData);
                                }
                                streamWriters.Remove(lockKey);
                            }
                        }
                    }
                }

                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Close(string filePath)
        {
            string lockKey = filePath.PathToLower();
            lock (FileLock.GetStringLock(lockKey))
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->177-->Close-->{0}", lockKey));
                }
                lock (lockDic)
                {
                    ErrLockData errLockDataDic = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockDataDic = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->180-->Close-->{0}", lockDic));
                    }
                    StreamWriteData writeData = null;
                    if (streamWriters.TryGetValue(lockKey, out writeData))
                    {
                        if (writeData != null)
                        {
                            CloseStreamWriter(filePath, lockKey, writeData.Id, writeData, true);
                        }
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockDataDic);
                    }
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        /// <param name="filePath"></param>
        public void Close(string filePath, string lockKey)
        {
            lock (FileLock.GetStringLock(lockKey))
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->203-->Close-->{0}", lockKey));
                }
                lock (lockDic)
                {
                    ErrLockData errLockDataDic = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockDataDic = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->206-->Close-->{0}", lockDic));
                    }
                    StreamWriteData writeData = null;
                    if (streamWriters.TryGetValue(lockKey, out writeData))
                    {
                        if (writeData != null)
                        {
                            CloseStreamWriter(filePath, lockKey, writeData.Id, writeData, true);
                        }
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockDataDic);
                    }
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="txt">写出内容</param>
        /// <param name="append"></param>
        /// <param name="useThread">使用多线程写出</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void Write(string filePath, string txt, bool append,  bool createDirectory = true , bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteText(filePath, filePath.PathToLower(), txt, append,null,null, false, createDirectory, true, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="filePath">写出路径</param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="txt">写出内容</param>
        /// <param name="append"></param>
        /// <param name="useThread">使用多线程写出</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void Write(string filePath, string lockKey, string txt, bool append, bool createDirectory = true , bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteText(filePath, lockKey, txt, append,null,null, false, createDirectory, true, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出 异步
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="txt">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void WriteAsyn(string filePath, string txt, bool append, System.Action<string, object> oneFileCallBack, object parObj, 
            bool createDirectory = true, bool flush = true, bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteText(filePath, filePath.PathToLower(), txt, append, oneFileCallBack, parObj, true, createDirectory, flush, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出 异步
        /// </summary>
        /// <param name="filePath">写出路径</param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="txt">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void WriteAsyn(string filePath, string lockKey, string txt, bool append, System.Action<string, object> oneFileCallBack,object parObj, 
            bool createDirectory = true, bool flush = true, bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteText(filePath, lockKey, txt, append, oneFileCallBack, parObj, true, createDirectory, flush, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出 异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool </param>
        /// <param name="txt">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径</param>
        /// <param name="finishCallBack">多线程写完所有文件的回调</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void WriteAsyn(List<string> filePaths, List<string> txts, bool append, System.Action<string> oneFileCallBack,System.Action finishCallBack, 
            bool createDirectory = true, bool flush = true, bool finishClose = false, string encryptKey = null)
        {
            if (filePaths==null || filePaths.Count==0)
            {
                finishCallBack();
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            List<string> tempTextList = ListPool.Instance.GetOneStringList();
            for (int i=0;i< filePaths.Count;++i)
            {
                string filePath = filePaths[i];
                string tx = txts[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath) && !string.IsNullOrEmpty(tx))
                {
                    tempList.Add(filePath);
                    tempTextList.Add(tx);
                }
            }
            WriteThreadCountData writeThreadCountData = writeThreadCountDataPool.Spawn();
            writeThreadCountData.Count = tempList.Count;
            writeThreadCountData.finishCallBack = finishCallBack;
            writeThreadCountData.oneFileCallBack = oneFileCallBack;
            for (int i=0;i< tempList.Count;++i)
            {
                string lockKey = tempList[i].PathToLower();
                WriteAsyn(tempList[i], lockKey, tempTextList[i], append, (p,obj) => {
                    WriteThreadCountData countData = (WriteThreadCountData)obj;
                    System.Action<string> fileCallBack = countData.oneFileCallBack;
                    countData.CountSub();
                    try
                    {
                        fileCallBack.Invoke(p);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"写多个文件回调异常:path={p} ex={ex.Message}");
                        VLog.Exception(ex);
                    }
                    if (countData.Count==0)
                    {
                        System.Action finishCallBack = countData.finishCallBack;
                        writeThreadCountDataPool.Recycle(countData);
                        try
                        {
                            finishCallBack.Invoke();
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"写多个文件完成后回调异常:ex={ex.Message}");
                            VLog.Exception(ex);
                        }
                    }
                }, writeThreadCountData, createDirectory, flush, finishClose, encryptKey);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
        }

        /// <summary>
        /// 写Log  异步
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="txt"></param>
        /// <param name="sizeLimit">大于0的时候，会分成多个文件写出</param>
        public void WriteLog(string filePath, string lockKey, string txt, int sizeLimit)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteText(filePath, lockKey, txt, true, null, null, false, true, true, false, sizeLimit, false);
        }

        /// <summary>
        /// 写Log  异步
        /// </summary>
        /// <param name="path"></param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="txt"></param>
        /// <param name="sizeLimit">大于0的时候，会分成多个文件写出</param>
        public void WriteLogAsyn(string filePath,string lockKey, string txt, int sizeLimit, System.Action<string, object> oneFileCallBack)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteText(filePath, lockKey,txt,true, oneFileCallBack,null, true, true, true, false, sizeLimit, false);
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="txt">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="useThread">使用多线程写出</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="sizeLimit">大于0的时候，会分成多个文件写出</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        void WriteText(string filePath, string lockKey, string txt, bool append, System.Action<string, object> oneFileCallBack, object parObj, 
            bool useThread = true,bool createDirectory = true, bool flush = true, bool finishClose = false, int sizeLimit = 0, bool errLogWrite = true,bool force=false,string encryptKey = null)
        {
            if (IsRelease && !force) return;
            if (string.IsNullOrEmpty(filePath) || txt == null)
            {
                VLog.Error("Write Err ! Path Is Null Or Tex Is Null !");
                return;
            }
            if (useThread)
            {
                WriteThreadData writeThreadData = writeThreadDataPool.Spawn();
                writeThreadData.parObj = parObj;
                writeThreadData.filePath = filePath;
                writeThreadData.oneFileCallBack = oneFileCallBack;
                writeThreadData.lockKey = lockKey;
                writeThreadData.txt = txt;
                writeThreadData.append = append;
                writeThreadData.createDirectory = createDirectory;
                writeThreadData.flush = flush;
                writeThreadData.finishClose = finishClose;
                writeThreadData.sizeLimit = sizeLimit;
                writeThreadData.errLogWrite = errLogWrite;
                writeThreadData.encryptKey = encryptKey;
                AddWriteThread(writeThreadData);
            }
            else
            {
                lock (FileLock.GetStringLock(lockKey))
                {
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->425-->WriteText-->{0}", lockKey));
                    }
                    AddCurThreadWriteCount();
                    if (createDirectory)
                    {
                        string dir = Path.GetDirectoryName(filePath);
                        string dirLockKey = dir.PathToLower();
                        lock (FileLock.GetStringLock(dirLockKey))
                        {
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }
                    }
                    StreamWriteData writeData = GetStreamWriter(filePath, lockKey, txt, null,null, append, errLogWrite);
                    if (writeData != null)
                    {
                        writeData.WriteLine(txt, sizeLimit, flush, errLogWrite, encryptKey);
                        if (finishClose)
                        {
                            CloseStreamWriter(filePath, lockKey, writeData.Id, writeData, errLogWrite);
                        }
                    }
                    if (oneFileCallBack != null)
                    {
                        try
                        {
                            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
                            {
                                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                                    oneFileCallBack(filePath, parObj);
                                }, null);
                            }
                            else
                            {
                                oneFileCallBack(filePath, parObj);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"写完文件回调异常: path={filePath} ex={ex.Message}");
                            VLog.Exception(ex);
                        }
                    }
                    SubCurThreadWriteCount();
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="bytes">写出内容</param>
        /// <param name="append"></param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void Write(string filePath, byte[] bytes, bool append,  bool createDirectory = true, bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteBytes(filePath, filePath.PathToLower(), bytes, append,null,null, false, createDirectory, true, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="bytes">写出内容</param>
        /// <param name="append"></param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void Write(string filePath, string lockKey, byte[] bytes, bool append, bool createDirectory = true, bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteBytes(filePath, lockKey, bytes, append,null,null, false, createDirectory, true, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出  异步
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="bytes">写出内容</param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void WriteAsyn(string filePath, byte[] bytes, bool append, System.Action<string, object> oneFileCallBack, object parObj,
            bool createDirectory = true, bool flush = true, bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteBytes(filePath, filePath.PathToLower(), bytes, append, oneFileCallBack, parObj, true, createDirectory, flush, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出  异步
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="bytes">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void WriteAsyn(string filePath, string lockKey, byte[] bytes, bool append, System.Action<string, object> oneFileCallBack, object parObj, 
            bool createDirectory = true, bool flush = true, bool finishClose = false, string encryptKey = null)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteBytes(filePath, lockKey, bytes, append, oneFileCallBack, parObj, true, createDirectory, flush, finishClose, 0,true,false, encryptKey);
        }

        /// <summary>
        /// 文件写出  异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="bytes">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径</param>
        /// <param name="finishCallBack">多线程写完所有文件的回调</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        public void WriteAsyn(List<string> filePaths, List<byte[]> bytesList, bool append, System.Action<string> oneFileCallBack, System.Action finishCallBack, 
            bool createDirectory = true, bool flush = true, bool finishClose = false, string encryptKey = null)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishCallBack();
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            List<byte[]> tempBytesList = ListPool.Instance.GetOneBytesList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                byte[] bytes = bytesList[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath) && bytes!=null)
                {
                    tempList.Add(filePath);
                    tempBytesList.Add(bytes);
                }
            }
            WriteThreadCountData writeThreadCountData = writeThreadCountDataPool.Spawn();
            writeThreadCountData.Count = tempList.Count;
            writeThreadCountData.finishCallBack = finishCallBack;
            writeThreadCountData.oneFileCallBack = oneFileCallBack;

            for (int i = 0; i < tempList.Count; ++i)
            {
                string lockKey = tempList[i].PathToLower();
                WriteAsyn(tempList[i], lockKey, tempBytesList[i], append, (p, obj) => {
                    WriteThreadCountData countData = (WriteThreadCountData)obj;
                    System.Action<string> fileCallBack = countData.oneFileCallBack;
                    countData.CountSub();
                    try
                    {
                        fileCallBack.Invoke(p);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"写多个文件回调异常:path={p} ex={ex.Message}");
                        VLog.Exception(ex);
                    }
                    if (countData.Count == 0)
                    {
                        System.Action finishCallBack = countData.finishCallBack;
                        writeThreadCountDataPool.Recycle(countData);
                        try
                        {
                            finishCallBack.Invoke();
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"写多个文件完成后回调异常:ex={ex.Message}");
                            VLog.Exception(ex);
                        }
                    }
                }, writeThreadCountData, createDirectory, flush, finishClose, encryptKey);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
            ListPool.Instance.PutBackOneBytesList(tempBytesList);
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="bytes">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param
        /// <param name="parObj">传入的参数</param>
        /// <param name="useThread">使用多线程写出</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="sizeLimit">大于0的时候，会分成多个文件写出</param>
        /// <param name="encryptKey">加密密钥 8位 如："%o-!_d5@"</param>
        void WriteBytes(string filePath, string lockKey, byte[] bytes, bool append, System.Action<string, object> oneFileCallBack, object parObj, 
            bool useThread = true, bool createDirectory = true, bool flush = true, bool finishClose = false, int sizeLimit = 0, bool errLogWrite = true, bool force = false, string encryptKey = null)
        {
            if (IsRelease && !force) return;
            if (string.IsNullOrEmpty(filePath) || bytes == null)
            {
                VLog.Error("Write Err ! Path Is Null Or Bytes Is Null !");
                return;
            }
            if (useThread)
            {
                WriteThreadData writeThreadData = writeThreadDataPool.Spawn();
                writeThreadData.parObj = parObj;
                writeThreadData.oneFileCallBack= oneFileCallBack;
                writeThreadData.filePath = filePath;
                writeThreadData.lockKey = lockKey;
                writeThreadData.bytes = bytes;
                writeThreadData.append = append;
                writeThreadData.createDirectory = createDirectory;
                writeThreadData.flush = flush;
                writeThreadData.finishClose = finishClose;
                writeThreadData.errLogWrite = errLogWrite;
                writeThreadData.encryptKey = encryptKey;
                AddWriteThread(writeThreadData);
            }
            else
            {
                lock (FileLock.GetStringLock(lockKey))
                {
                    ErrLockData errLockData =null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->650-->WriteBytes-->{0}", lockKey));
                    }
                    AddCurThreadWriteCount();
                    if (createDirectory)
                    {
                        string dir = Path.GetDirectoryName(filePath);
                        string dirLockKey= dir.PathToLower();
                        lock (FileLock.GetStringLock(dirLockKey))
                        {
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }
                    }
                    StreamWriteData writeData = GetStreamWriter(filePath, lockKey, null, bytes,null, append, errLogWrite);
                    if (writeData != null)
                    {
                        writeData.WriteBytes(bytes, sizeLimit, flush, errLogWrite, encryptKey);
                        if (finishClose)
                        {
                            CloseStreamWriter(filePath, lockKey, writeData.Id, writeData, errLogWrite);
                        }
                    }
                    if (oneFileCallBack != null)
                    {
                        try
                        {
                            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
                            {
                                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                                    oneFileCallBack(filePath, parObj);
                                }, null);
                            }
                            else
                            {
                                oneFileCallBack(filePath, parObj);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"写完文件回调异常: path={filePath} ex={ex.Message}");
                            VLog.Exception(ex);
                        }
                    }
                    SubCurThreadWriteCount();
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="filePath">写出路径</param>
        /// <param name="serializObjs">写出内容</param>
        /// <param name="append"></param>
        /// <param name="useThread">使用多线程写出</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        public void WriteSerializ(string filePath, object serializObj, bool append, bool createDirectory = true , bool finishClose = false)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteSerializObject(filePath, filePath.PathToLower(), serializObj, append,null,null, true, createDirectory, true, finishClose);
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="filePath">写出路径</param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="serializObjs">写出内容</param>
        /// <param name="append"></param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        public void WriteSerializ(string filePath, string lockKey, object serializObj, bool append,  bool createDirectory = true, bool finishClose = false)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteSerializObject(filePath, lockKey, serializObj, append,null,null, createDirectory, true, finishClose);
        }

        /// <summary>
        /// 文件写出  异步
        /// </summary>
        /// <param name="filePath">写出路径</param>
        /// <param name="serializObjs">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        public void WriteSerializAsyn(string filePath, object serializObj, bool append, System.Action<string, object> oneFileCallBack, object parObj, bool createDirectory = true, bool flush = true, bool finishClose = false)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteSerializObject(filePath, filePath.PathToLower(), serializObj, append, oneFileCallBack, parObj, true, createDirectory, flush, finishClose);
        }

        /// <summary>
        /// 文件写出  异步
        /// </summary>
        /// <param name="filePath">写出路径</param>
        /// <param name="lockKey"> filePath.PathToLower() </param>
        /// <param name="serializObjs">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="finishClose">写出后关闭流</param>
        public void WriteSerializAsyn(string filePath, string lockKey, object serializObj, bool append, System.Action<string, object> oneFileCallBack, object parObj, bool createDirectory = true, bool flush = true, bool finishClose = false)
        {
            if (FrameSystemConfig.EndlessLoop(100))
            {
                return;
            }
            WriteSerializObject(filePath, lockKey, serializObj, append, oneFileCallBack, parObj, true, createDirectory, flush, finishClose);
        }

        /// <summary>
        /// 文件写出  异步
        /// </summary>
        /// <param name="filePaths">写出路径,使用完成需要手动回收List<string>  StringListPool</param>
        /// <param name="serializObjs">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径</param>
        /// <param name="finishCallBack">多线程写完所有文件的回调</param>
        /// <param name="parObj">传入的参数</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        public void WriteSerializAsyn(List<string> filePaths, List<object> serializObjs, bool append, System.Action<string> oneFileCallBack, System.Action finishCallBack, bool createDirectory = true, bool flush = true, bool finishClose = false)
        {
            if (filePaths == null || filePaths.Count == 0)
            {
                finishCallBack();
                return;
            }
            List<string> tempList = ListPool.Instance.GetOneStringList();
            List<object> tempSerializObjList = ListPool.Instance.GetOneObjectList();
            for (int i = 0; i < filePaths.Count; ++i)
            {
                string filePath = filePaths[i];
                object serializObj = serializObjs[i];
                if (!string.IsNullOrEmpty(filePath) && !tempList.Contains(filePath) && serializObj!=null)
                {
                    tempList.Add(filePath);
                    tempSerializObjList.Add(serializObj);
                }
            }
            if (tempList.Count == 0)
            {
                ListPool.Instance.PutBackOneStringList(tempList);
                ListPool.Instance.PutBackOneObjectList(tempSerializObjList);
                finishCallBack();
                return;
            }
            WriteThreadCountData writeThreadCountData = writeThreadCountDataPool.Spawn();
            writeThreadCountData.Count = tempList.Count;
            writeThreadCountData.finishCallBack = finishCallBack;
            writeThreadCountData.oneFileCallBack = oneFileCallBack;
            for (int i = 0; i < tempList.Count; ++i)
            {
                string lockKey = tempList[i].PathToLower();
                WriteSerializAsyn(tempList[i], lockKey, tempSerializObjList[i], append, (p, obj) => {
                    WriteThreadCountData countData = (WriteThreadCountData)obj;
                    System.Action<string> fileCallBack = countData.oneFileCallBack;
                    countData.CountSub();
                    try
                    {
                        fileCallBack.Invoke(p);
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Error($"写多个文件回调异常:path={p} ex={ex.Message}");
                        VLog.Exception(ex);
                    }
                    if (countData.Count == 0)
                    {
                        System.Action finishCallBack = countData.finishCallBack;
                        writeThreadCountDataPool.Recycle(countData);
                        try
                        {
                            finishCallBack.Invoke();
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"写多个文件完成后回调异常:ex={ex.Message}");
                            VLog.Exception(ex);
                        }
                    }
                }, writeThreadCountData, createDirectory, flush, finishClose);
            }
            ListPool.Instance.PutBackOneStringList(tempList);
            ListPool.Instance.PutBackOneObjectList(tempSerializObjList);
        }

        /// <summary>
        /// 文件写出
        /// </summary>
        /// <param name="path">写出路径</param>
        /// <param name="bytes">写出内容</param>
        /// <param name="append"></param>
        /// <param name="oneFileCallBack">多线程写完一个文件的回调 参数为文件路径和传入的参数parObj</param
        /// <param name="parObj">传入的参数</param>
        /// <param name="useThread">使用多线程写出</param>
        /// <param name="createDirectory">自动创建目录</param>
        /// <param name="flush">立刻写出缓存</param>
        /// <param name="finishClose">写出后关闭流</param>
        /// <param name="sizeLimit">大于0的时候，会分成多个文件写出</param>
        void WriteSerializObject(string filePath, string lockKey, object serializObj, bool append, System.Action<string,object> oneFileCallBack, object parObj, 
            bool useThread = true, bool createDirectory = true, bool flush = true, bool finishClose = false, int sizeLimit = 0, bool errLogWrite = true, bool force = false)
        {
            if (IsRelease && !force) return;
            if (string.IsNullOrEmpty(filePath) || serializObj == null)
            {
                VLog.Error("Write Err ! Path Is Null Or SerializObj Is Null !");
                return;
            }
            if (useThread)
            {
                WriteThreadData writeThreadData = writeThreadDataPool.Spawn();
                writeThreadData.filePath = filePath;
                writeThreadData.lockKey = lockKey;
                writeThreadData.parObj = parObj;
                writeThreadData.oneFileCallBack = oneFileCallBack;
                writeThreadData.serializObj = serializObj;
                writeThreadData.append = append;
                writeThreadData.createDirectory = createDirectory;
                writeThreadData.flush = flush;
                writeThreadData.finishClose = finishClose;
                writeThreadData.errLogWrite = errLogWrite;
                AddWriteThread(writeThreadData);
            }
            else
            {
                lock (FileLock.GetStringLock(lockKey))
                {
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->884-->WriteSerializObject-->{0}", lockKey));
                    }
                    AddCurThreadWriteCount();
                    if (createDirectory)
                    {
                        string dir = Path.GetDirectoryName(filePath);
                        string dirLockKey = dir.PathToLower();
                        lock (FileLock.GetStringLock(dirLockKey))
                        {
                            if (!Directory.Exists(dir))
                            {
                                Directory.CreateDirectory(dir);
                            }
                        }
                    }
                    StreamWriteData writeData = GetStreamWriter(filePath, lockKey, null, null, serializObj, append, errLogWrite);
                    if (writeData != null)
                    {
                        writeData.WriteSerializObject(serializObj, sizeLimit, flush, errLogWrite);
                        if (finishClose)
                        {
                            CloseStreamWriter(filePath, lockKey, writeData.Id, writeData, errLogWrite);
                        }
                    }
                    if (oneFileCallBack != null)
                    {
                        try
                        {
                            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
                            {
                                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                                    oneFileCallBack(filePath, parObj);
                                }, null);
                            }
                            else
                            {
                                oneFileCallBack(filePath, parObj);
                            }
                  
                        }
                        catch (System.Exception ex)
                        {
                            VLog.Error($"写完文件回调异常: path={filePath} ex={ex.Message}");
                            VLog.Exception(ex);
                        }
                    }
                    SubCurThreadWriteCount();
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        /// <summary>
        /// 同时写入数量限制
        /// </summary>
        int maxThreadWriteCount = 64;

        int curThreadWriteCount = 0;

        object curThreadWriteCountLock = new object();

        void AddCurThreadWriteCount()
        {
            lock (curThreadWriteCountLock)
            {
                curThreadWriteCount = curThreadWriteCount + 1;
            }
        }

        void SubCurThreadWriteCount()
        {
            lock (curThreadWriteCountLock)
            {
                curThreadWriteCount = curThreadWriteCount - 1;
                RunWriteThreadList();
            }
        }

        int GetCurThreadWriteCount()
        {
            lock (curThreadWriteCountLock)
            {
                return curThreadWriteCount;
            }
        }

        Queue<WriteThreadData> writeAddThreads = new Queue<WriteThreadData>();

        SimpleListPool<List<WriteThreadData>, WriteThreadData> writeOutThreadPool = new SimpleListPool<List<WriteThreadData>, WriteThreadData>();

        void AddWriteThread(WriteThreadData writeThreadData)
        {

            lock (writeAddThreads)
            {
                if (isDispose || IsRelease)
                {
                    //程序即将关闭，直接写出
                    if (writeThreadData.txt != null)
                    {
                        WriteText(writeThreadData.filePath, writeThreadData.lockKey, writeThreadData.txt, writeThreadData.append, writeThreadData.oneFileCallBack, writeThreadData.parObj, false,
                            writeThreadData.createDirectory, writeThreadData.flush, writeThreadData.finishClose, writeThreadData.sizeLimit, writeThreadData.errLogWrite, true, writeThreadData.encryptKey);
                    }
                    if (writeThreadData.bytes != null)
                    {
                        WriteBytes(writeThreadData.filePath, writeThreadData.lockKey, writeThreadData.bytes, writeThreadData.append, writeThreadData.oneFileCallBack, writeThreadData.parObj, false,
                            writeThreadData.createDirectory, writeThreadData.flush, writeThreadData.finishClose, writeThreadData.sizeLimit, writeThreadData.errLogWrite, true, writeThreadData.encryptKey);
                    }
                    if (writeThreadData.serializObj != null)
                    {
                        WriteSerializObject(writeThreadData.filePath, writeThreadData.lockKey, writeThreadData.serializObj, writeThreadData.append, writeThreadData.oneFileCallBack, writeThreadData.parObj, false,
                            writeThreadData.createDirectory, writeThreadData.flush, writeThreadData.finishClose, writeThreadData.sizeLimit, writeThreadData.errLogWrite, true);
                    }
                    return;
                }
                writeAddThreads.Enqueue(writeThreadData);
            }

            RunWriteThreadList();
        }

        void RunWriteThreadList()
        {
            int count = maxThreadWriteCount - GetCurThreadWriteCount();
            if (count>0)
            {
                Task task = Task.Factory.StartNew((obj) => {
                    int addCount = (int)obj;
                    List<WriteThreadData> writeOutThreads = writeOutThreadPool.Spawn();
                    lock (writeAddThreads)
                    {
                        int num = 0;
                        while (writeAddThreads.Count > 0)
                        {
                            WriteThreadData data = writeAddThreads.Dequeue();
                            writeOutThreads.Add(data);
                            num++;
                            if (num>= addCount)
                            {
                                break;
                            }
                        }
                    }
                    for (int i = 0; i < writeOutThreads.Count; ++i)
                    {
                        WriteThreadData outData = writeOutThreads[i];
                        if (outData.txt != null)
                        {
                            WriteText(outData.filePath, outData.lockKey, outData.txt, outData.append, outData.oneFileCallBack, outData.parObj, false, outData.createDirectory, outData.flush, outData.finishClose, outData.sizeLimit, outData.errLogWrite);
                        }
                        if (outData.bytes != null)
                        {
                            WriteBytes(outData.filePath, outData.lockKey, outData.bytes, outData.append, outData.oneFileCallBack, outData.parObj, false, outData.createDirectory, outData.flush, outData.finishClose, outData.sizeLimit, outData.errLogWrite);
                        }
                        if (outData.serializObj != null)
                        {
                            WriteSerializObject(outData.filePath, outData.lockKey, outData.serializObj, outData.append, outData.oneFileCallBack, outData.parObj, false, outData.createDirectory, outData.flush, outData.finishClose, outData.sizeLimit, outData.errLogWrite);
                        }
                        writeThreadDataPool.Recycle(outData);
                    }
                    writeOutThreadPool.Recycle(writeOutThreads);
                }, count);
            }
        }

        SimplePool<StreamWriteData> streamWriteDataPool = new SimplePool<StreamWriteData>();

        class StreamWriteData : ISimplePoolData
        {

            public long Id;

            public long Size;

            public string FilePath;

            public string LockKey;

            public bool Append = true;

            public WriteType WriteType;

            public long TimeStamp;

            public System.IO.StreamWriter FirstStreamWriter;

            public System.IO.BinaryWriter FirstBinaryWriter;

            public FileStream FirstBinaryFormatterFileStream;

            static SimpleListPool<List<System.IO.StreamWriter>, System.IO.StreamWriter> streamWriterList = new SimpleListPool<List<StreamWriter>, StreamWriter>();

            static SimpleListPool<List<System.IO.BinaryWriter>, System.IO.BinaryWriter> binaryWriterList = new SimpleListPool<List<BinaryWriter>, BinaryWriter>();

            static SimpleListPool<List<FileStream>, FileStream> binaryFormatterList = new SimpleListPool<List<FileStream>, FileStream>();

            List<System.IO.StreamWriter> streamWriters;

            List<System.IO.BinaryWriter> binaryWriters;

            List<FileStream> binaryFormatters;

            string GetSplitPath(int index)
            {
                string suffix = FilePath.GetSuffix() + String.Intern("SizeSplit");
                string pathNotSuffix = FilePath.DeleteSuffix();
                string indexPath = string.Format("{0}_{1}.{2}", pathNotSuffix, index + 1, suffix);
                return indexPath;
            }

            System.IO.StreamWriter FindStreamWriter(int index, bool errLogWrite = true)
            {
                if (index <= 0)
                {
                    return FirstStreamWriter;
                }
                if (streamWriters == null)
                {
                    streamWriters = streamWriterList.Spawn();
                }
                index = index - 1;
                try
                {
                    if (index < streamWriters.Count)
                    {
                        if (streamWriters[index] != null)
                        {
                            return streamWriters[index];
                        }
                        else
                        {
                            string indexPath = GetSplitPath(index);
                            lock (FileLock.GetStringLock(indexPath.PathToLower()) )
                            {
                                System.IO.StreamWriter indexStreamWriter = new System.IO.StreamWriter(indexPath, Append, FileHelper.WriteEncoding);
                                streamWriters[index] = indexStreamWriter;
                            }
                            return streamWriters[index];
                        }
                    }
                    for (int i = 0; i <= index; ++i)
                    {
                        if (i == streamWriters.Count)
                        {
                            if (i == index)
                            {
                                string indexPath = GetSplitPath(index);
                                lock (FileLock.GetStringLock(indexPath.PathToLower()))
                                {
                                    System.IO.StreamWriter indexStreamWriter = new System.IO.StreamWriter(indexPath, Append, FileHelper.WriteEncoding);
                                    streamWriters.Add(indexStreamWriter);
                                }
                            }
                            else
                            {
                                streamWriters.Add(null);
                            }
                        }
                    }
                    return streamWriters[index];
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                    return null;
                }
            }

            System.IO.BinaryWriter FindBinaryWriter(int index, bool errLogWrite = true)
            {
                if (index <= 0)
                {
                    return FirstBinaryWriter;
                }
                if (binaryWriters == null)
                {
                    binaryWriters = binaryWriterList.Spawn();
                }
                index = index - 1;
                try
                {
                    if (index < binaryWriters.Count)
                    {
                        if (binaryWriters[index] != null)
                        {
                            return binaryWriters[index];
                        }
                        else
                        {
                            string indexPath = GetSplitPath(index);
                            lock (FileLock.GetStringLock(indexPath.PathToLower()))
                            {
                                FileStream fileStream = new FileStream(indexPath, FileMode.Open, FileAccess.Write);
                                System.IO.BinaryWriter indexBinaryWriter = new System.IO.BinaryWriter(fileStream, FileHelper.WriteEncoding);
                                binaryWriters[index] = indexBinaryWriter;
                            }
                            return binaryWriters[index];
                        }
                    }
                    for (int i = 0; i <= index; ++i)
                    {
                        if (i == binaryWriters.Count)
                        {
                            if (i == index)
                            {
                                string indexPath = GetSplitPath(index);
                                lock (FileLock.GetStringLock(indexPath.PathToLower()))
                                {
                                    FileStream fileStream = new FileStream(indexPath, FileMode.Open, FileAccess.Write);
                                    System.IO.BinaryWriter indexBinaryWriter = new System.IO.BinaryWriter(fileStream, FileHelper.WriteEncoding);
                                    binaryWriters.Add(indexBinaryWriter);
                                }
                            }
                            else
                            {
                                binaryWriters.Add(null);
                            }
                        }
                    }
                    return binaryWriters[index];
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                    return null;
                }
            }

            FileStream FindBinaryFormatter(int index, bool errLogWrite = true)
            {
                if (index <= 0)
                {
                    return FirstBinaryFormatterFileStream;
                }
                if (binaryFormatters == null)
                {
                    binaryFormatters = binaryFormatterList.Spawn();
                }
                index = index - 1;
                try
                {
                    if (index < binaryFormatters.Count)
                    {
                        if (binaryFormatters[index] != null)
                        {
                            return binaryFormatters[index];
                        }
                        else
                        {
                            string indexPath = GetSplitPath(index);
                            lock (FileLock.GetStringLock(indexPath.PathToLower()))
                            {
                                FileStream indexBinaryFormatter = new FileStream(indexPath, FileMode.Open, FileAccess.Write);
                                binaryFormatters[index] = indexBinaryFormatter;
                            }
                            return binaryFormatters[index];
                        }
                    }
                    for (int i = 0; i <= index; ++i)
                    {
                        if (i == binaryFormatters.Count)
                        {
                            if (i == index)
                            {
                                string indexPath = GetSplitPath(index);
                                lock (FileLock.GetStringLock(indexPath.PathToLower()))
                                {
                                    FileStream indexBinaryFormatter = new FileStream(indexPath, FileMode.Open, FileAccess.Write);
                                    binaryFormatters.Add(indexBinaryFormatter);
                                }
                            }
                            else
                            {
                                binaryFormatters.Add(null);
                            }
                        }
                    }
                    return binaryFormatters[index];
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                    return null;
                }
            }

            public void WriteLine(string txt, long sizeLimit, bool flush, bool errLogWrite = true, string encryptKey=null)
            {
                int index = 0;
                if (sizeLimit > 0)
                {
                    index = (int)(Size / sizeLimit);
                }
                System.IO.StreamWriter targetStreamWriter = FindStreamWriter(index, errLogWrite);
                if (targetStreamWriter != null)
                {
                    try
                    {
                        if (!Append)
                        {
                            targetStreamWriter.BaseStream.Seek(0, SeekOrigin.Begin);
                            targetStreamWriter.BaseStream.SetLength(0);
                        }
                        if (encryptKey != null)
                        {
                            txt = EncryptHelper.EncryptDES(txt, string.Intern(encryptKey));
                        }
                        targetStreamWriter.WriteLine(txt);
                        if (flush)
                        {
                            targetStreamWriter.Flush();
                        }
                        if (Append)
                        {
                            Size = Size + txt.Length;
                        }
                        else
                        {
                            Size = txt.Length;
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                }
            }

            public void WriteBytes(byte[] bytes, long sizeLimit, bool flush, bool errLogWrite = true,string encryptKey=null)
            {
                System.IO.BinaryWriter targetBinaryWriter = FirstBinaryWriter;
                if (targetBinaryWriter != null)
                {
                    try
                    {
                        if (encryptKey!=null)
                        {
                            bytes = EncryptHelper.EncryptDES_B(bytes, string.Intern(encryptKey));
                        }
                        targetBinaryWriter.Write(bytes);
                        if (flush)
                        {
                            targetBinaryWriter.Flush();
                        }
                        Size = bytes.Length;
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                }
            }

            public void WriteSerializObject(object serializObject, long sizeLimit, bool flush, bool errLogWrite = true)
            {
                FileStream targetBinaryFormatter = FirstBinaryFormatterFileStream;
                if (targetBinaryFormatter != null)
                {
                    try
                    {
                        FileHelper.BinaryFormatter.Serialize(targetBinaryFormatter, serializObject);
                        if (flush)
                        {
                            targetBinaryFormatter.Flush();
                        }
                        Size = 0;
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                }
            }

            public void Close(bool errLogWrite = true)
            {
                if (FirstStreamWriter != null)
                {
                    try
                    {
                        FirstStreamWriter.Close();
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                    FirstStreamWriter = null;
                }
                if (streamWriters != null)
                {
                    try
                    {
                        for (int i = 0; i < streamWriters.Count; ++i)
                        {
                            if (streamWriters[i] != null)
                            {
                                streamWriters[i].Close();
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                    streamWriterList.Recycle(streamWriters);
                    streamWriters = null;
                }
                if (FirstBinaryWriter != null)
                {
                    try
                    {
                        FirstBinaryWriter.Close();
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                    FirstBinaryWriter = null;
                }
                if (binaryWriters != null)
                {
                    try
                    {
                        for (int i = 0; i < binaryWriters.Count; ++i)
                        {
                            if (binaryWriters[i] != null)
                            {
                                binaryWriters[i].Close();
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                }
                if (FirstBinaryFormatterFileStream != null)
                {
                    try
                    {
                        FirstBinaryFormatterFileStream.Close();
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                    FirstBinaryFormatterFileStream = null;
                }
                if (binaryFormatters != null)
                {
                    try
                    {
                        for (int i = 0; i < binaryFormatters.Count; ++i)
                        {
                            if (binaryFormatters[i] != null)
                            {
                                binaryFormatters[i].Close();
                            }
                        }
                    }
                    catch (System.Exception e)
                    {
                        VLog.Exception(e);
                    }
                }
            }

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
                Close();
                Id = -1;
                Size = 0;
                FilePath = null;
                LockKey = null;
                isUsed = false;
            }

            public void PutOut()
            {
                Size = 0;
                isUsed = true;
            }

        }

        SimplePool<WriteThreadData> writeThreadDataPool = new SimplePool<WriteThreadData>();

        class WriteThreadData : ISimplePoolData
        {
            public string filePath;
            public string lockKey;
            public string txt;
            public byte[] bytes;
            public object serializObj;
            public bool append;
            public bool createDirectory;
            public bool flush;
            public bool finishClose;
            public int sizeLimit;
            public bool errLogWrite;
            public object parObj;
            public string encryptKey;
            public System.Action<string,object> oneFileCallBack;

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
                encryptKey = null;
                parObj = null;
                oneFileCallBack = null;
                lockKey  = null;
                filePath = null;
                txt = null;
                bytes = null;
                serializObj = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }

        }

        SimplePool<WriteThreadCountData> writeThreadCountDataPool = new SimplePool<WriteThreadCountData>();

        class WriteThreadCountData : ISimplePoolData
        {

            public System.Action finishCallBack;

            public System.Action<string> oneFileCallBack;

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
                    count = count-1;
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
                oneFileCallBack = null;
                finishCallBack = null;
                Count = 0;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

        bool isDispose = false;

        public void Dispose()
        {
            lock (writeAddThreads)
            {
                isDispose = true;
                writeAddThreads.Clear();
            }
            lock (lockDic)
            {
                //强制写出剩余
                List<WriteThreadData> writeOutThreads = writeOutThreadPool.Spawn();
                lock (writeAddThreads)
                {
                    int num = 0;
                    while (writeAddThreads.Count > 0)
                    {
                        WriteThreadData data = writeAddThreads.Dequeue();
                        writeOutThreads.Add(data);
                        num++;
                    }
                }
                for (int i = 0; i < writeOutThreads.Count; ++i)
                {
                    WriteThreadData outData = writeOutThreads[i];
                    if (outData.txt != null)
                    {
                        WriteText(outData.filePath, outData.lockKey, outData.txt, outData.append, outData.oneFileCallBack, outData.parObj, false,
                            outData.createDirectory, outData.flush, outData.finishClose, outData.sizeLimit, outData.errLogWrite, true);
                    }
                    if (outData.bytes != null)
                    {
                        WriteBytes(outData.filePath, outData.lockKey, outData.bytes, outData.append, outData.oneFileCallBack, outData.parObj, false,
                            outData.createDirectory, outData.flush, outData.finishClose, outData.sizeLimit, outData.errLogWrite, true);
                    }
                    if (outData.serializObj != null)
                    {
                        WriteSerializObject(outData.filePath, outData.lockKey, outData.serializObj, outData.append, outData.oneFileCallBack, outData.parObj, false,
                            outData.createDirectory, outData.flush, outData.finishClose, outData.sizeLimit, outData.errLogWrite, true);
                    }
                }
                //
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("FileWriteHelper.cs-->930-->ReleaseFun-->{0}", lockDic));
                }
                try
                {
                    Dictionary<string, StreamWriteData>.Enumerator enumerator = streamWriters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        StreamWriteData writeData = enumerator.Current.Value;
                        if (writeData != null)
                        {
                            writeData.Close(false);
                        }
                    }
                    streamWriters.Clear();
                }
                catch (System.Exception e)
                {
                    VLog.Exception(e);
                    throw (e);
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
                writeOutThreadPool.Clear();
                streamWriteDataPool.Clear();
                writeThreadDataPool.Clear();
                writeThreadCountDataPool.Clear();
            }
        }

    }

}

