using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using System.Reflection;
using System.Text;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 检查函数调用堆栈，函数死循环
    /// </summary>
    public class FrameSystemConfig
    {
        /// <summary>
        /// 函数调用堆栈控制 开启后会开启函数堆栈检测 检测函数死循环崩溃问题
        /// </summary>
        public static bool FunctionStackTrace=false;

        /// <summary>
        /// 判断函数是否陷入了死循环
        /// </summary>
        /// <param name="maxEndlessCount">检测的最大循环次数  -1：检测到一次就会返回</param>
        /// <returns></returns>
        public static bool EndlessLoop(float maxEndlessCount=-1)
        {
            if (!FunctionStackTrace)
            {
                return false;
            }
            StackTrace allStackTrace = new StackTrace();
            StackFrame[] allStackFrame = allStackTrace.GetFrames();
            if (allStackFrame==null || allStackFrame.Length<=2)
            {
                return false;
            }
            //调用者
            StackFrame callerStackFrame = allStackFrame[1];
            MethodBase callerMethodBase = callerStackFrame.GetMethod();

            int curEndlessCount = 0;
            bool needReture = false;

            for (int i=2;i< allStackFrame.Length;++i)
            {
                StackFrame sf = allStackFrame[i];
                if (callerMethodBase== sf.GetMethod())
                {
                    if (maxEndlessCount>0)
                    {
                        curEndlessCount++;
                        if (curEndlessCount>= maxEndlessCount)
                        {
                            needReture = true;
                            break;
                        }
                    }
                    else
                    {
                        needReture = true;
                        break;
                    }
                }
            }

            if (needReture)
            {
                StackTrace curSt = new StackTrace(new StackFrame(true));
                StackFrame curSf = curSt.GetFrame(0);
                VLog.Error($"检测到死循环函数：Method={callerMethodBase.Name} File={curSf.GetFileName()} Line Number={curSf.GetFileLineNumber()} Column Number={curSf.GetFileColumnNumber()}");
                StringBuilder sb = new StringBuilder();
                sb.Append("调用堆栈:");
                for (int j = 0; j < allStackFrame.Length; ++j)
                {
                    StackFrame sf = allStackFrame[j];
                    sb.Append(string.Format("Method={0}\n", sf.GetMethod().Name));
                }
                VLog.Error(sb.ToString());
                return true;
            }

            return false;
        }

        /// <summary>
        /// 调用堆栈打印
        /// </summary>
        public static void EndlessDebugLog()
        {
            if (!FunctionStackTrace) return;
            StackTrace curSt = new StackTrace(new StackFrame(true));
            StackFrame curSf = curSt.GetFrame(0);
            VLog.Error($"检测到死循环函数：Method={curSf.GetMethod().Name} File={curSf.GetFileName()} Line Number={curSf.GetFileLineNumber()} Column Number={curSf.GetFileColumnNumber()}");
            //
            StackTrace allStackTrace = new StackTrace();
            StackFrame[] allStackFrame = allStackTrace.GetFrames();
            StringBuilder sb = new StringBuilder();
            for (int i=0;i< allStackFrame.Length;++i)
            {
                sb.Append(string.Format("    Method:{0}\n", allStackFrame[i].GetMethod().Name));
            }
            VLog.Error($"函数调用堆栈:\n{sb.ToString()}");
        }
    }
}

