using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{

    public interface IThreadHelperPar
    {
        public SynchronizationContext Context { get; set; }
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    /// <summary>
    /// 使用多线程执行函数
    /// </summary>
    public class ThreadHelper
    {
#if UNITY_EDITOR
        static ThreadHelper()
        {
            UnityEditor.EditorApplication.delayCall -= DoSomethingPrepare;
            UnityEditor.EditorApplication.delayCall += DoSomethingPrepare;
        }

        static void DoSomethingPrepare()
        {
            if (!Application.isPlaying)
            {
                lock (unitySynchronizationContextLock)
                {
                    unitySynchronizationContext = SynchronizationContext.Current;
                    unityThreadId = Thread.CurrentThread.ManagedThreadId;
                }
            }
            else
            {
                Install();
            }
        }
#endif

        static SynchronizationContext unitySynchronizationContext;

        static object unitySynchronizationContextLock = new object();

        /// <summary>
        /// Unity主线程上下文
        /// </summary>
        public static SynchronizationContext UnitySynchronizationContext
        {
            get
            {
                if (unitySynchronizationContext == null)
                {
#if UNITY_EDITOR
                    DoSomethingPrepare();
#else
                    Install();
#endif
                }
                return unitySynchronizationContext;
            }
        }

        static int unityThreadId=-1;

        /// <summary>
        ///  Unity主线程
        /// </summary>
        public static int UnityThreadId
        {
            get
            {
                if (unityThreadId==-1)
                {
#if UNITY_EDITOR
                    DoSomethingPrepare();
#else
                    Install();
#endif
                }
                return unityThreadId;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Install()
        {
            lock (unitySynchronizationContextLock)
            {
                unitySynchronizationContext = SynchronizationContext.Current;
                unityThreadId = Thread.CurrentThread.ManagedThreadId;
                maxThreadCount =Math.Max(SystemInfo.processorCount * 4,4);
            }
        }

        static int maxThreadCount = -1;

        static int MaxThreadCount
        {
            get
            {
                if (maxThreadCount==-1)
                {
#if UNITY_EDITOR
                    DoSomethingPrepare();
#else
                    Install();
#endif
                }
                return maxThreadCount;
            }
        }

        /// <summary>
        /// 多线程执行函数 object obj = Func(object parObj);  callBack(obj);
        /// </summary>
        /// <param name="func">线程执行目标函数 object obj = Func(object parObj)</param>
        /// <param name="parObj">Func执行函数的参数,其中的Context只能在函数func中使用,callBack回调传出</param>
        /// <param name="callBack">1.func返回值 2.parObj 3.异常 线程执行目标函数结束的回调,此回调会在StartTask函数所在线程执行func的返回值作为参数,如果调用线程为位置线程则会使用Unity主线程回调</param>
        public static void StartTask<T>(System.Func<object, object> func, T parObj, System.Action<object, object, System.Exception> callBack) where T: IThreadHelperPar
        {
            TaskData intputTaskData = taskDataPool.Spawn();
            intputTaskData.func = func;
            intputTaskData.parObj = parObj;
            intputTaskData.callBack = callBack;
            if (System.Threading.SynchronizationContext.Current == null)
            {
                ThreadHelperSynchronizationContext context = new ThreadHelperSynchronizationContext();
            }
            intputTaskData.context = System.Threading.SynchronizationContext.Current;
            if (intputTaskData.context==unitySynchronizationContext)
            {
                if (CurCount < MaxThreadCount)
                {
                    AddWaitList(intputTaskData);
                    if (CurCount < MaxThreadCount)
                    {
                        TaskData data = GetFirstWaitData();
                        if (data!=null)
                        {
                            StartTask(data);
                        }
                    }
                }
                else
                {
                    StartTask(intputTaskData);
                }
            }
            else
            {
                StartTask(intputTaskData);
            }
        }

        static int curCount = 0;

        static object curCountLockObj = new object();

        static int CurCount
        {
            get
            {
                lock (curCountLockObj)
                {
                    return curCount;
                }
            }
            set
            {
                lock (curCountLockObj)
                {
                    curCount = value;
                }
            }
        }

        static void CurCountAdd()
        {
            lock (curCountLockObj)
            {
                curCount = curCount+1;
            }
        }

        static void CurCountSub()
        {
            lock (curCountLockObj)
            {
                curCount = curCount - 1;
            }
        }

        static List<TaskData> waitList = new List<TaskData>();

        static void AddWaitList(TaskData data)
        {
            lock (waitList)
            {
                waitList.Add(data);
            }
        }

        static TaskData GetFirstWaitData()
        {
            lock (waitList)
            {
                if (waitList.Count>0)
                {
                    TaskData data = waitList[0];
                    waitList.RemoveAt(0);
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }

        static void StartTask(TaskData intputTaskData)
        {
            if (intputTaskData.context.GetType() == typeof(ThreadHelperSynchronizationContext))
            {
                ((IThreadHelperPar)(intputTaskData.parObj)).Context = intputTaskData.context;
                ThreadHelperSynchronizationContext context = intputTaskData.context as ThreadHelperSynchronizationContext;
                context.Start((obj, threadcontext) => {
                    TaskData data = (TaskData)obj;
                    data.context = threadcontext; 
                    StartTaskFactory(data);
                }, intputTaskData);
            }
            else if(intputTaskData.context!=null)
            {
                if (intputTaskData.context!=unitySynchronizationContext)
                {
                    VLog.Error($"发现未知SynchronizationContext : value={intputTaskData.context}");
                }
                ((IThreadHelperPar)(intputTaskData.parObj)).Context = intputTaskData.context;
                StartTaskFactory(intputTaskData);
            }
            else
            {
                VLog.Error("SynchronizationContext is null !");
            }
        }

        static void StartTaskFactory(TaskData intputTaskData)
        {
            if (intputTaskData.context==unitySynchronizationContext)
            {
                CurCountAdd();
            }
            Task task = Task.Factory.StartNew(StartTaskFactoryNew, intputTaskData);
        }

        static void StartTaskFactoryNew(object obj)
        {
            TaskData getTaskData = (TaskData)obj;
            System.Func<object, object> runFunc = getTaskData.func;
            object funcParObj = getTaskData.parObj;
            System.Threading.SynchronizationContext synchronizationContext = getTaskData.context;
            System.Action<object,object, System.Exception> runCallBack = getTaskData.callBack;
            object parObj = getTaskData.parObj;
            taskDataPool.Recycle(getTaskData);

            object res = null;
            System.Exception err = null;
            try
            {
                res = runFunc.Invoke(funcParObj);
                IThreadHelperPar iThreadHelperPar = (IThreadHelperPar)funcParObj;
                iThreadHelperPar.Context = null;
            }
            catch (System.Exception e)
            {
                if (runFunc!=null)
                {
                    VLog.Error($"Method={runFunc.Method.ToString()}");
                }
                VLog.Exception(e);
                err = e;
            }
            FactoryData data = factoryDataPool.Spawn();
            data.Res = res;
            data.parObj = parObj;
            data.Err = err;
            data.callBack = runCallBack;
            data.Context = synchronizationContext;
            if (synchronizationContext.GetType() == typeof(ThreadHelperSynchronizationContext))
            {
                ThreadHelperSynchronizationContext context = synchronizationContext as ThreadHelperSynchronizationContext;
                context.Post(PostThread, data);
            }
            else
            {
                //Unity主线程执行
                UnitySynchronizationContext.Post(PostUnity, data);
            }
            if (synchronizationContext==unitySynchronizationContext)
            {
                CurCountSub();
                TaskData firstData = GetFirstWaitData();
                if (firstData != null)
                {
                    firstData.context.Post((obj) => {
                        TaskData td = (TaskData)obj;
                        StartTask(td);
                    }, firstData);
                }
            }
        }

        static void PostThread(object obj)
        {
            FactoryData factoryData = obj as FactoryData;
            factoryData.callBack.Invoke(factoryData.Res, factoryData.parObj, factoryData.Err);
            ThreadHelperSynchronizationContext threadHelperContext = factoryData.Context as ThreadHelperSynchronizationContext;
            threadHelperContext.StopLoopExecution();
            factoryDataPool.Recycle(factoryData);
        }

        static void PostUnity(object obj)
        {

            FactoryData factoryData = obj as FactoryData;
            ErrLockData errLockDataA = null;
            if (ErrLock.ErrLockOpen)
            {
                errLockDataA = ErrLock.LockStart("ThreadHelper.cs-->PostUnity-->330");
            }

            factoryData.callBack.Invoke(factoryData.Res, factoryData.parObj, factoryData.Err);
            factoryDataPool.Recycle(factoryData);

            if (ErrLock.ErrLockOpen)
            {
                ErrLock.LockEnd(errLockDataA);
            }
        }

        static SimplePool<FactoryData> factoryDataPool = new SimplePool<FactoryData>();

        class FactoryData : ISimplePoolData
        {
            public object Res;

            public object parObj;

            public System.Exception Err;

            public System.Action<object, object, System.Exception> callBack;

            public System.Threading.SynchronizationContext Context;

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
                parObj = null;
                Context = null;
                callBack = null;
                Res = null;
                Err = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }

        }

        static SimplePool<TaskData> taskDataPool = new SimplePool<TaskData>();

        class TaskData : ISimplePoolData
        {
            public System.Func<object, object> func;

            public object parObj;

            public System.Action<object, object, System.Exception> callBack;

            public System.Threading.SynchronizationContext context;

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
                context = null;
                func = null;
                parObj = null;
                callBack = null;
                isUsed = false;
            }

            public void PutOut()
            {
                isUsed = true;
            }
        }

    }
}


