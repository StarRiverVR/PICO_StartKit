using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// Log记录重新排序
    /// </summary>
    public class DebugLogSaveFileSort
    {

#if UNITY_EDITOR

        //[MenuItem("Tools/DebugLog/Log记录排序/按文件目录")]
        static void EditorReSortAndSaveLogSelectDir()
        {
            ReSortAndSaveLogSelectDir();
        }

        //[MenuItem("Tools/DebugLog/Log记录排序/按文件")]
        static void EditorReSortAndSaveLogSelectFile()
        {
            ReSortAndSaveLogSelectFile();
        }
#endif

        /// <summary>
        /// 选择Log文件目录排序
        /// </summary>
        public static void ReSortAndSaveLogSelectDir()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            WindowsHelper.SelectFolder((logDir) => {
                if (!string.IsNullOrEmpty(logDir))
                {
                    ReSortAndSaveLogDir(logDir);
                }
            }, "请选择Log记录文件夹");
#endif
        }

        /// <summary>
        /// 目录中的Log记录重新排序
        /// </summary>
        /// <param name="logDir"></param>
        public static void ReSortAndSaveLogDir(string logDir)
        {
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((obj) => {
                    string dir = (string)obj;
                    ReSortAndSaveLogDirRun(dir);
                }, logDir);
            }
            else
            {
                ReSortAndSaveLogDirRun(logDir);
            }
        }

        static void ReSortAndSaveLogDirRun(string logDir)
        {
            string dirLockKey = logDir.PathToLower();
            string[] files = null;
            lock (FileLock.GetStringLock(dirLockKey))
            {
                FileWriteHelper.Instance.Close(logDir, dirLockKey);
                files = Directory.GetFiles(logDir, "*.txt", SearchOption.AllDirectories);
            }
            if (files==null || files.Length==0)
            {
                VLog.Info($"Log记录排序，目录为空:{logDir}");
                return;
            }
            for (int i = 0; i < files.Length; ++i)
            {
                string filePath = files[i];
                ReSortAndSaveLogFilePath(filePath);
            }
        }

        /// <summary>
        /// 选择Log文件排序
        /// </summary>
        public static void ReSortAndSaveLogSelectFile()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            WindowsHelper.SelectFileMutiSelect((list) => {
                if (list!=null)
                {
                    for (int i=0;i< list.Count;++i)
                    {
                        ReSortAndSaveLogFilePath(list[i]);
                    }
                }
            }, "Log记录(*.txt)\0*.txt");
#endif
        }

        /// <summary>
        /// Log记录重新排序
        /// </summary>
        /// <param name="logFilePath"></param>
        public static void ReSortAndSaveLogFilePath(string logFilePath)
        {
            FileReadHelper.Instance.ReadTextAsyn(logFilePath, (path, txt, parObj) => {
                if (!string.IsNullOrEmpty(txt))
                {
                    ReSortAndSaveLog(path,txt);
                }
                else
                {
                    VLog.Error($"Log记录排序失败:{logFilePath}");
                }
            }, (bl, path, parObj) => { 
            }, null, System.Text.Encoding.UTF8);
        }

        static void ReSortAndSaveLog(string filePath, string txt)
        {
            ReSortAndSaveLogData data = new ReSortAndSaveLogData();
            data.Txt = txt;
            data.FilePath = filePath;
            ThreadHelper.StartTask<ReSortAndSaveLogData>(ReSortAndSaveLogRun, data, (ret, parObj, ex) => {
                ReSortAndSaveLogData reSortAndSaveLogData = (ReSortAndSaveLogData)parObj;
                FileWriteHelper.Instance.WriteAsyn(reSortAndSaveLogData.FilePath, reSortAndSaveLogData.Txt, false, (p,par) => { }, null, true, true, true);
            });
        }

        static object ReSortAndSaveLogRun(object obj)
        {
            ReSortAndSaveLogData data = (ReSortAndSaveLogData)obj;
            List<string> tempList = new List<string>();
            string[] strs = data.Txt.Split('\n');
            //按照条目拆分
            List<string> head = new List<string>();
            bool findHead = false;
            StringBuilder lineSB = null;
            List<StringBuilder> lines = new List<StringBuilder>();
            for (int i = 0; i < strs.Length; ++i)
            {
                string line = strs[i];
                bool isFirst = false;
                if (line.StartsWith("<"))
                {
                    lineSB = new StringBuilder();
                    lines.Add(lineSB);
                    isFirst = true;
                    findHead = true;
                }
                if (!findHead)
                {
                    head.Add(line);
                }
                if (lineSB!=null)
                {
                    if (isFirst)
                    {
                        lineSB.Append(String.Format("{0}", line));
                    }
                    else{
                        lineSB.Append(String.Format("\n{0}", line));
                    }
                }
            }
            //
            List<ReSortAndSaveLogElementData> elementList = new List<ReSortAndSaveLogElementData>();
            for (int i=0;i< lines.Count;++i)
            {
                ReSortAndSaveLogElementData elementData = new ReSortAndSaveLogElementData();
                elementData.Set(lines[i].ToString());
                elementList.Add(elementData);
            }
            elementList.Sort((A,B) => {
                return ReSortAndSaveLogElementData.SortTime(A,B);
            });
            Dictionary<string, List<ReSortAndSaveLogElementData>> sortDic = new Dictionary<string, List<ReSortAndSaveLogElementData>>();
            for (int i=0;i< elementList.Count;++i)
            {
                ReSortAndSaveLogElementData elementData = elementList[i];
                List<ReSortAndSaveLogElementData> list;
                if (!sortDic.TryGetValue(elementData.Key,out list))
                {
                    list = new List<ReSortAndSaveLogElementData>();
                    sortDic.Add(elementData.Key, list);
                }
                list.Add(elementData);
            }
            //
            elementList.Clear();
            Dictionary<string, List<ReSortAndSaveLogElementData>>.Enumerator enumerator = sortDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                List<ReSortAndSaveLogElementData> list = enumerator.Current.Value;
                list.Sort((A, B) => {
                    return ReSortAndSaveLogElementData.SortIndex(A, B);
                });
                for (int i=0;i< list.Count;++i)
                {
                    elementList.Add(list[i]);
                }
            }
            //
            StringBuilder sb = new StringBuilder();
            for (int i=0;i< head.Count;++i)
            {
                sb.Append(string.Format("{0}\n", head[i]));
            }
            for (int i=0;i< elementList.Count;++i)
            {
                ReSortAndSaveLogElementData elementData = elementList[i];
                sb.Append(string.Format("{0}\n", elementData.Str));
            }
            data.Txt = sb.ToString();
            VLog.Info($"Log记录排序完成:{data.FilePath}");
            return data;
        }

        class ReSortAndSaveLogData: IThreadHelperPar
        {
            public string Txt;

            public string FilePath;

            public SynchronizationContext Context { get; set; }

        }

        class ReSortAndSaveLogElementData
        {
            public string Str;

            public string Key;

            public int Index;

            public int Year;

            public int Month;

            public int Day;

            public int Hour;

            public int Minute;

            public int Second;

            public int Microsecond;

            public bool Set(string txt)
            {
                Str = txt;
                Key = "";
                string tempStr = Str;
                if (tempStr.StartsWith("<"))
                {
                    try
                    {
                        tempStr = tempStr.Substring(1, tempStr.Length - 1);

                        //index
                        int index2 = tempStr.IndexOf('>');
                        if (index2 < 0) return false;
                        string indexStr = tempStr.Substring(0, index2);
                        Index = int.Parse(indexStr);

                        //
                        index2 = tempStr.IndexOf('^');
                        if (index2 < 0) return false;
                        tempStr = tempStr.Substring(index2 + 1, tempStr.Length - index2 - 1);
                        index2 = tempStr.IndexOf('^');
                        if (index2 < 0) return false;
                        tempStr = tempStr.Substring(0, index2);
                        string[] yStrs = tempStr.Split(' ');
                        string yStr1 = yStrs[0];
                        string yStr2 = yStrs[1];

                        //Year Month Day
                        string[] tempStrs = yStr1.Split('-');
                        Year = int.Parse(tempStrs[0]);
                        Month = int.Parse(tempStrs[1]);
                        Day = int.Parse(tempStrs[2]);

                        //Hour Minute Second Microsecond
                        tempStrs = yStr2.Split(':');
                        Hour = int.Parse(tempStrs[0]);
                        Minute = int.Parse(tempStrs[1]);
                        Second = int.Parse(tempStrs[2]);
                        Microsecond = int.Parse(tempStrs[3]);

                        Key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", Year, Month, Day, Hour, Minute, Second, Microsecond);
                        return true;
                    }
                    catch (System.Exception ex)
                    {
                        VLog.Exception(ex);
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            public static int SortTime(ReSortAndSaveLogElementData A , ReSortAndSaveLogElementData B)
            {
                if (A.Year < B.Year)
                {
                    return -1;
                }
                if (A.Month < B.Month)
                {
                    return -1;
                }
                if (A.Day < B.Day)
                {
                    return -1;
                }
                if (A.Hour < B.Hour)
                {
                    return -1;
                }
                if (A.Minute < B.Minute)
                {
                    return -1;
                }
                if (A.Second < B.Second)
                {
                    return -1;
                }
                if (A.Microsecond < B.Microsecond)
                {
                    return -1;
                }
                return 1;
            }

            public static int SortIndex(ReSortAndSaveLogElementData A, ReSortAndSaveLogElementData B)
            {
                if (A.Index < B.Index)
                {
                    return -1;
                }
                return 1;
            }

        }

    }
}


