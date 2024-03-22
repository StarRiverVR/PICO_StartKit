using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    public class ThreadHelperSynchronizationContext : SynchronizationContext
    {
        private struct ThreadWorkRequest
        {
            /// <summary>
            /// 线程控制句柄
            /// </summary>
            private readonly ManualResetEvent m_WaitHandle;

            /// <summary>
            /// 回调方法
            /// </summary>
            private readonly SendOrPostCallback m_DelagateCallback;

            /// <summary>
            /// 回调的执行参数
            /// </summary>
            private readonly object m_DelagateState;

            public ThreadWorkRequest(SendOrPostCallback callback, object state, ManualResetEvent waitHandle = null)
            {
                m_DelagateCallback = callback;
                m_DelagateState = state;
                m_WaitHandle = waitHandle;
            }

            public void Invoke()
            {
                try
                {
                    m_DelagateCallback(m_DelagateState);
                }
                catch (Exception exception)
                {
                    VLog.Exception(exception);
                }
                if (m_WaitHandle != null)
                {
                    //线程接触阻塞
                    m_WaitHandle.Set();
                }
            }
        }

        private readonly List<ThreadWorkRequest> m_AsyncWorkQueue;

        public ThreadHelperSynchronizationContext()
        {
            if (SynchronizationContext.Current == null)
            {
                SynchronizationContext.SetSynchronizationContext(this);
                m_AsyncWorkQueue = new List<ThreadWorkRequest>(20);
            }
        }

        public void Start(System.Action<object, ThreadHelperSynchronizationContext> run, object runPar)
        {
            FrameSystemConfig.EndlessLoop();
            try
            {
                run.Invoke(runPar, this);
            }
            catch (System.Exception e)
            {
                VLog.Error($"线程执行失败:{e.Message}");
                VLog.Exception(e);
                return;
            }
            //if (SynchronizationContext.Current != this)
            //{
            //    StopLoopExecution();
            //    return;
            //}
            lock (loopExecutionObject)
            {
                if (loopExecution)
                {
                    return;
                }
            }
            StartLoopExecution();
            while (LoopExecution || m_AsyncWorkQueueCount>0)
            {
                Exec();
            }
        }

        public override void Send(SendOrPostCallback callback, object state)
        {
            if (SynchronizationContext.Current==this)
            {
                callback.Invoke(state);
                return;
            }
            using ManualResetEvent manualResetEvent = new ManualResetEvent(initialState: false);
            lock (m_AsyncWorkQueue)
            {
                m_AsyncWorkQueue.Add(new ThreadWorkRequest(callback, state, manualResetEvent));
            }
            manualResetEvent.WaitOne();
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            lock (m_AsyncWorkQueue)
            {
                m_AsyncWorkQueue.Add(new ThreadWorkRequest(callback, state));
            }
        }

        private int m_AsyncWorkQueueCount
        {
            get
            {
                lock (m_AsyncWorkQueue)
                {
                    if (m_AsyncWorkQueue==null)
                    {
                        return 0;
                    }
                    return m_AsyncWorkQueue.Count;
                }
            }
        }

        SimpleListPool<List<ThreadWorkRequest>, ThreadWorkRequest> threadWorkRequestPool = new SimpleListPool<List<ThreadWorkRequest>, ThreadWorkRequest>();

        private void Exec()
        {
            List<ThreadWorkRequest> m_CurrentFrameWork = threadWorkRequestPool.Spawn();
            lock (m_AsyncWorkQueue)
            {
                m_CurrentFrameWork.AddRange(m_AsyncWorkQueue);
                m_AsyncWorkQueue.Clear();
            }
            while (m_CurrentFrameWork.Count > 0)
            {
                ThreadWorkRequest item = m_CurrentFrameWork[0];
                m_CurrentFrameWork.Remove(item);
                item.Invoke();
            }
            threadWorkRequestPool.Recycle(m_CurrentFrameWork);
        }

        bool loopExecution = false;

        object loopExecutionObject = new object();

        private bool LoopExecution
        {
            get
            {
                lock (loopExecutionObject)
                {
                    return loopExecution;
                }
            }
        }

        /// <summary>
        /// 退出线程循环
        /// </summary>
        public void StopLoopExecution()
        {
            lock (loopExecutionObject)
            {
                loopExecution = false;
            }
        }

        private void StartLoopExecution()
        {
            lock (loopExecutionObject)
            {
                loopExecution = true;
            }
        }

    }

}

