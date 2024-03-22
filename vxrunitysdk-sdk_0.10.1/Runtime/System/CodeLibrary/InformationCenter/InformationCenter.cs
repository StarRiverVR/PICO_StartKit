using System.Collections;
using System.Collections.Generic;
using System;

namespace com.vivo.codelibrary
{
    public delegate void InformationCenterMsgDelegate(params object[] objs);

    public delegate void InformationCenterActionMsgDelegate(System.Action<object> action, params object[] objs);

    public delegate void InformationCenterActionTMsgDelegate(System.Action<object> actionYes, System.Action<object> actionNo, params object[] objs);

    /// <summary>
    /// 公共消息中心
    /// </summary>
    public class InformationCenter  : IDisposable
    {
        public InformationCenter()
        {

        }

        class FunData : ISimplePoolData
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

        #region //

        List<Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>>> intMsgFunsList;
        List<Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>>> IntMsgFunsList
        {
            get
            {
                if (intMsgFunsList == null)
                {
                    intMsgFunsList = new List<Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>>>();
                    intMsgFunsList.Add(intMsgFuns);
                    intMsgFunsList.Add(intMsgFuns_DontClear);
                }
                return intMsgFunsList;
            }
        }

        private Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>> intMsgFuns = new Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>>();
        private Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>> intMsgFuns_DontClear = new Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>>();

        List<Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>>> strMsgFunsList;
        List<Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>>> StrMsgFunsList
        {
            get
            {
                if (strMsgFunsList == null)
                {
                    strMsgFunsList = new List<Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>>>();
                    strMsgFunsList.Add(strMsgFuns);
                    strMsgFunsList.Add(strMsgFuns_DontClear);
                }
                return strMsgFunsList;
            }
        }

        private Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>> strMsgFuns = new Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>>();
        private Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>> strMsgFuns_DontClear = new Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>>();

        public void AddListen<T>(int id, InformationCenterMsgDelegate fun,bool dontClear=false)
        {
            Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>> targetFun = null;
            if (dontClear)
            {
                targetFun = intMsgFuns_DontClear;
            }
            else
            {
                targetFun = intMsgFuns;
            }
            lock (targetFun)
            {
                Type t = typeof(T);
                Dictionary<int, InformationCenterMsgDelegate> dic;
                if (!targetFun.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<int, InformationCenterMsgDelegate>();
                    targetFun.Add(t, dic);
                }

                InformationCenterMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[id] = saveFun;
                }
                else
                {
                    saveFun = fun;
                    dic.Add(id, saveFun);
                }
            }
        }

        public void AddListen<T>(string msg, InformationCenterMsgDelegate fun, bool dontClear = false)
        {
            Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>> targetFun = null;
            if (dontClear)
            {
                targetFun = strMsgFuns_DontClear;
            }
            else
            {
                targetFun = strMsgFuns;
            }
            lock (targetFun)
            {
                Type t = typeof(T);
                Dictionary<string, InformationCenterMsgDelegate> dic;
                if (!targetFun.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<string, InformationCenterMsgDelegate>();
                    targetFun.Add(t, dic);
                }

                InformationCenterMsgDelegate saveFun;
                if (dic.TryGetValue(msg, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[msg] = saveFun;
                }
                else
                {
                    saveFun = fun;
                    dic.Add(msg, saveFun);
                }
            }
        }

        public void RemoveListen<T>(int id, InformationCenterMsgDelegate fun)
        {
            lock (IntMsgFunsList)
            {
                for (int i=0;i< IntMsgFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>> targetFun = IntMsgFunsList[i];
                    Type t = typeof(T);
                    Dictionary<int, InformationCenterMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        continue;
                    }

                    InformationCenterMsgDelegate saveFun;
                    if (dic.TryGetValue(id, out saveFun))
                    {
                        saveFun -= fun;
                        if (saveFun==null)
                        {
                            dic.Remove(id);
                        }
                        else
                        {
                            dic[id] = saveFun;
                        }
                    }
                    if (dic.Count==0)
                    {
                        targetFun.Remove(t);
                    }
                }
            }
        }

        public void RemoveListen<T>(string msg, InformationCenterMsgDelegate fun)
        {
            lock (StrMsgFunsList)
            {
                for (int i=0;i< StrMsgFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>> targetFun = StrMsgFunsList[i];
                    Type t = typeof(T);
                    Dictionary<string, InformationCenterMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        continue;
                    }

                    InformationCenterMsgDelegate saveFun;
                    if (dic.TryGetValue(msg, out saveFun))
                    {
                        saveFun -= fun;
                        if (saveFun==null)
                        {
                            dic.Remove(msg);
                        }
                        else
                        {
                            dic[msg] = saveFun;
                        }
                    }
                    if (dic.Count == 0)
                    {
                        targetFun.Remove(t);
                    }
                }

            }
        }

        public void Send<T>(int id, bool runInUnityMainThread, params object[] objs)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }

            lock (IntMsgFunsList)
            {
                for (int i=0;i< IntMsgFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<int, InformationCenterMsgDelegate>> targetFun = IntMsgFunsList[i];
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("InformationCenter.cs-->165-->Send-->{0}", typeof(T)));
                    }
                    Type t = typeof(T);
                    Dictionary<int, InformationCenterMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        continue;
                    }

                    InformationCenterMsgDelegate saveFun;
                    if (dic.TryGetValue(id, out saveFun))
                    {
                        if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                        {
                            FunThreadData data = funThreadDataPool.Spawn();
                            data.objs = objs;
                            data.Fun = saveFun;
                            ThreadHelper.UnitySynchronizationContext.Post((p) => {
                                FunThreadData funThreadData = (FunThreadData)p;
                                try
                                {
                                    funThreadData.Fun.Invoke(funThreadData.objs);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Error(e.Message);
                                }
                                funThreadDataPool.Recycle(funThreadData);
                            }, data);
                        }
                        else
                        {
                            try
                            {
                                saveFun.Invoke(objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                        }
                    }

                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        public void Send<T>(string msg, bool runInUnityMainThread, params object[] objs)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (StrMsgFunsList)
            {
                for (int i=0;i< StrMsgFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<string, InformationCenterMsgDelegate>> targetFun = StrMsgFunsList[i];
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("InformationCenter.cs-->217-->Send-->{0}", typeof(T)));
                    }
                    Type t = typeof(T);
                    Dictionary<string, InformationCenterMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        continue;
                    }

                    InformationCenterMsgDelegate saveFun;
                    if (dic.TryGetValue(msg, out saveFun))
                    {
                        if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                        {
                            FunThreadData data = funThreadDataPool.Spawn();
                            data.objs = objs;
                            data.Fun = saveFun;
                            ThreadHelper.UnitySynchronizationContext.Post((p) => {
                                FunThreadData funThreadData = (FunThreadData)p;
                                try
                                {
                                    funThreadData.Fun.Invoke(funThreadData.objs);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Error(e.Message);
                                }
                                funThreadDataPool.Recycle(funThreadData);
                            }, data);
                        }
                        else
                        {
                            try
                            {
                                saveFun.Invoke(objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                        }
                    }

                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
                
            }
        }

        static SimplePool<FunThreadData> funThreadDataPool = new SimplePool<FunThreadData>();

        class FunThreadData : FunData
        {
            public object[] objs;
            public InformationCenterMsgDelegate Fun;

            public override void PutIn()
            {
                Fun = null;
                objs = null;
                base.PutIn();
            }
        }

        #endregion

        #region //

        List<Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>>> intMsgActionFunsList;
        List<Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>>> IntMsgActionFunsList
        {
            get
            {
                if (intMsgActionFunsList == null)
                {
                    intMsgActionFunsList = new List<Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>>>();
                    intMsgActionFunsList.Add(intMsgActionFuns);
                    intMsgActionFunsList.Add(intMsgActionFuns_DontClear);
                }
                return intMsgActionFunsList;
            }
        }

        private Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>> intMsgActionFuns = new Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>>();
        private Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>> intMsgActionFuns_DontClear = new Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>>();

        List<Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>>> strMsgActionFunsList;
        List<Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>>> StrMsgActionFunsList
        {
            get
            {
                if (strMsgActionFunsList == null)
                {
                    strMsgActionFunsList = new List<Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>>>();
                    strMsgActionFunsList.Add(strMsgActionFuns);
                    strMsgActionFunsList.Add(strMsgActionFuns_DontClear);
                }
                return strMsgActionFunsList;
            }
        }

        private Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>> strMsgActionFuns = new Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>>();
        private Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>> strMsgActionFuns_DontClear = new Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>>();

        public void AddListenAction<T>(int id, InformationCenterActionMsgDelegate fun, bool dontClear = false)
        {
            Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>> targetFun = null;
            if (dontClear)
            {
                targetFun = intMsgActionFuns_DontClear;
            }
            else
            {
                targetFun = intMsgActionFuns;
            }
            lock (targetFun)
            {
                Type t = typeof(T);
                Dictionary<int, InformationCenterActionMsgDelegate> dic;
                if (!targetFun.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<int, InformationCenterActionMsgDelegate>();
                    targetFun.Add(t, dic);
                }

                InformationCenterActionMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[id] = saveFun;
                }
                else
                {
                    saveFun = fun;
                    dic.Add(id, saveFun);
                }
            }
        }

        public void AddListenAction<T>(string msg, InformationCenterActionMsgDelegate fun, bool dontClear = false)
        {
            Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>> targetFun = null;
            if (dontClear)
            {
                targetFun = strMsgActionFuns_DontClear;
            }
            else
            {
                targetFun = strMsgActionFuns;
            }
            lock (targetFun)
            {
                Type t = typeof(T);
                Dictionary<string, InformationCenterActionMsgDelegate> dic;
                if (!targetFun.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<string, InformationCenterActionMsgDelegate>();
                    targetFun.Add(t, dic);
                }

                InformationCenterActionMsgDelegate saveFun;
                if (dic.TryGetValue(msg, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[msg] = saveFun;
                }
                else
                {
                    saveFun = fun;
                    dic.Add(msg, saveFun);
                }
            }
        }

        public void RemoveListenAction<T>(int id, InformationCenterActionMsgDelegate fun)
        {
            lock (IntMsgActionFunsList)
            {
                for (int i = 0; i < IntMsgActionFunsList.Count; ++i)
                {
                    Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>> targetFun = IntMsgActionFunsList[i];
                    Type t = typeof(T);
                    Dictionary<int, InformationCenterActionMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        continue;
                    }

                    InformationCenterActionMsgDelegate saveFun;
                    if (dic.TryGetValue(id, out saveFun))
                    {
                        saveFun -= fun;
                        if (saveFun==null)
                        {
                            dic.Remove(id);
                        }
                        else
                        {
                            dic[id] = saveFun;
                        }
                    }
                    if (dic.Count == 0)
                    {
                        targetFun.Remove(t);
                    }
                }
            }
        }

        public void RemoveListenAction<T>(string msg, InformationCenterActionMsgDelegate fun)
        {
            lock (StrMsgActionFunsList)
            {
                for (int i=0;i< StrMsgActionFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>> targetFun = StrMsgActionFunsList[i];
                    Type t = typeof(T);
                    Dictionary<string, InformationCenterActionMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        continue;
                    }

                    InformationCenterActionMsgDelegate saveFun;
                    if (dic.TryGetValue(msg, out saveFun))
                    {
                        saveFun -= fun;
                        if (saveFun==null)
                        {
                            dic.Remove(msg);
                        }
                        else
                        {
                            dic[msg] = saveFun;
                        }
                    }
                    if (dic.Count == 0)
                    {
                        targetFun.Remove(t);
                    }
                }
            }
        }

        public void SendAction<T>(int id, bool runInUnityMainThread, System.Action<object> action, params object[] objs)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (IntMsgActionFunsList)
            {
                for (int i=0;i< IntMsgActionFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<int, InformationCenterActionMsgDelegate>> targetFun = IntMsgActionFunsList[i];
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("InformationCenter.cs-->384-->SendAction-->{0}", typeof(T)));
                    }
                    Type t = typeof(T);
                    Dictionary<int, InformationCenterActionMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        continue;
                    }

                    InformationCenterActionMsgDelegate saveFun;
                    if (dic.TryGetValue(id, out saveFun))
                    {
                        if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                        {
                            ActionFunData data = actionFunDataPool.Spawn();
                            data.action = action;
                            data.objs = objs;
                            data.Fun = saveFun;
                            ThreadHelper.UnitySynchronizationContext.Post((p) => {
                                ActionFunData actionFunData = (ActionFunData)p;
                                try
                                {
                                    actionFunData.Fun.Invoke(actionFunData.action, actionFunData.objs);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Error(e.Message);
                                }
                                actionFunDataPool.Recycle(actionFunData);
                            }, data);
                        }
                        else
                        {
                            try
                            {
                                saveFun.Invoke(action, objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                        }
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        public void SendAction<T>(string msg, bool runInUnityMainThread, System.Action<object> action, params object[] objs)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (StrMsgActionFunsList)
            {
                for (int i=0;i< StrMsgActionFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<string, InformationCenterActionMsgDelegate>> targetFun = StrMsgActionFunsList[i];
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("InformationCenter.cs-->436-->SendAction-->{0}", typeof(T)));
                    }
                    Type t = typeof(T);
                    Dictionary<string, InformationCenterActionMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        continue;
                    }

                    InformationCenterActionMsgDelegate saveFun;
                    if (dic.TryGetValue(msg, out saveFun))
                    {
                        if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                        {
                            ActionFunData data = actionFunDataPool.Spawn();
                            data.action = action;
                            data.objs = objs;
                            data.Fun = saveFun;
                            ThreadHelper.UnitySynchronizationContext.Post((p) => {
                                ActionFunData actionFunData = (ActionFunData)p;
                                try
                                {
                                    actionFunData.Fun.Invoke(actionFunData.action, actionFunData.objs);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Error(e.Message);
                                }
                                actionFunDataPool.Recycle(actionFunData);
                            }, data);
                        }
                        else
                        {
                            try
                            {
                                saveFun.Invoke(action, objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                        }
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        static SimplePool<ActionFunData> actionFunDataPool = new SimplePool<ActionFunData>();

        class ActionFunData : FunData
        {
            public System.Action<object> action;
            public object[] objs;
            public InformationCenterActionMsgDelegate Fun;

            public override void PutIn()
            {
                Fun = null;
                action = null;
                objs = null;
                base.PutIn();
            }
        }

        #endregion

        #region//

        List<Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>>> intMsgActionTFunsList;
        List<Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>>> IntMsgActionTFunsList
        {
            get
            {
                if (intMsgActionTFunsList == null)
                {
                    intMsgActionTFunsList = new List<Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>>>();
                    intMsgActionTFunsList.Add(intMsgActionTFuns);
                    intMsgActionTFunsList.Add(intMsgActionTFuns_DontClear);
                }
                return intMsgActionTFunsList;
            }
        }

        private Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>> intMsgActionTFuns = new Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>>();
        private Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>> intMsgActionTFuns_DontClear = new Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>>();


        List<Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>>> strMsgActionTFunsList;
        List<Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>>> StrMsgActionTFunsList
        {
            get
            {
                if (strMsgActionTFunsList == null)
                {
                    strMsgActionTFunsList = new List<Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>>>();
                    strMsgActionTFunsList.Add(strMsgActionTFuns);
                    strMsgActionTFunsList.Add(strMsgActionTFuns_DontClear);
                }
                return strMsgActionTFunsList;
            }
        }

        private Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>> strMsgActionTFuns = new Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>>();
        private Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>> strMsgActionTFuns_DontClear = new Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>>();

        public void AddListenActionT<T>(int id, InformationCenterActionTMsgDelegate fun, bool dontClear = false)
        {
            Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>> targetFun = null;
            if (dontClear)
            {
                targetFun = intMsgActionTFuns_DontClear;
            }
            else
            {
                targetFun = intMsgActionTFuns;
            }
            lock (targetFun)
            {
                Type t = typeof(T);
                Dictionary<int, InformationCenterActionTMsgDelegate> dic;
                if (!targetFun.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<int, InformationCenterActionTMsgDelegate>();
                    targetFun.Add(t, dic);
                }

                InformationCenterActionTMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[id] = saveFun;
                }
                else
                {
                    saveFun = fun;
                    dic.Add(id, saveFun);
                }
            }
        }

        public void AddListenActionT<T>(string msg, InformationCenterActionTMsgDelegate fun, bool dontClear = false)
        {
            Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>> targetFun = null;
            if (dontClear)
            {
                targetFun = strMsgActionTFuns_DontClear;
            }
            else
            {
                targetFun = strMsgActionTFuns;
            }
            lock (targetFun)
            {
                Type t = typeof(T);
                Dictionary<string, InformationCenterActionTMsgDelegate> dic;
                if (!targetFun.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<string, InformationCenterActionTMsgDelegate>();
                    targetFun.Add(t, dic);
                }

                InformationCenterActionTMsgDelegate saveFun;
                if (dic.TryGetValue(msg, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[msg] = saveFun;
                }
                else
                {
                    saveFun = fun;
                    dic.Add(msg, saveFun);
                }
            }
        }

        public void RemoveListenActionT<T>(int id, InformationCenterActionTMsgDelegate fun)
        {
            lock (IntMsgActionTFunsList)
            {
                for (int i=0;i< IntMsgActionTFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>> targetFun = IntMsgActionTFunsList[i];
                    Type t = typeof(T);
                    Dictionary<int, InformationCenterActionTMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        continue;
                    }

                    InformationCenterActionTMsgDelegate saveFun;
                    if (dic.TryGetValue(id, out saveFun))
                    {
                        saveFun -= fun;
                        if (saveFun==null)
                        {
                            dic.Remove(id);
                        }
                        else
                        {
                            dic[id] = saveFun;
                        }
                    }
                    if (dic.Count == 0)
                    {
                        targetFun.Remove(t);
                    }
                }
            }
        }

        public void RemoveListenActionT<T>(string msg, InformationCenterActionTMsgDelegate fun)
        {
            lock (StrMsgActionTFunsList)
            {
                for (int i=0;i< StrMsgActionTFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>> targetFun = StrMsgActionTFunsList[i];
                    Type t = typeof(T);
                    Dictionary<string, InformationCenterActionTMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        continue;
                    }

                    InformationCenterActionTMsgDelegate saveFun;
                    if (dic.TryGetValue(msg, out saveFun))
                    {
                        saveFun -= fun;
                        if (saveFun==null)
                        {
                            dic.Remove(msg);
                        }
                        else
                        {
                            dic[msg] = saveFun;
                        }
                    }
                    if (dic.Count == 0)
                    {
                        targetFun.Remove(t);
                    }
                }
            }
        }

        public void SendActionT<T>(int id,bool runInUnityMainThread, System.Action<object> actionYes, System.Action<object> actionNo, params object[] objs)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (IntMsgActionTFunsList)
            {
                for (int i=0;i< IntMsgActionTFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<int, InformationCenterActionTMsgDelegate>> targetFun = IntMsgActionTFunsList[i];
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("InformationCenter.cs-->603-->SendActionT-->{0}", typeof(T)));
                    }
                    Type t = typeof(T);
                    Dictionary<int, InformationCenterActionTMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        continue;
                    }

                    InformationCenterActionTMsgDelegate saveFun;
                    if (dic.TryGetValue(id, out saveFun))
                    {
                        if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                        {
                            ActionTFunData data = actionTFunDataPool.Spawn();
                            data.actionYes = actionYes;
                            data.actionNo = actionNo;
                            data.objs = objs;
                            data.Fun = saveFun;
                            ThreadHelper.UnitySynchronizationContext.Post((p) => {
                                ActionTFunData actionTFunData = (ActionTFunData)p;
                                try
                                {
                                    actionTFunData.Fun.Invoke(actionTFunData.actionYes, actionTFunData.actionNo, actionTFunData.objs);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Error(e.Message);
                                }
                                actionTFunDataPool.Recycle(actionTFunData);
                            }, data);
                        }
                        else
                        {
                            try
                            {
                                saveFun.Invoke(actionYes, actionNo, objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                        }
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        public void SendActionT<T>(string msg, bool runInUnityMainThread, System.Action<object> actionYes, System.Action<object> actionNo, params object[] objs)
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (StrMsgActionTFunsList)
            {
                for (int i=0;i< StrMsgActionTFunsList.Count;++i)
                {
                    Dictionary<Type, Dictionary<string, InformationCenterActionTMsgDelegate>> targetFun = StrMsgActionTFunsList[i];
                    ErrLockData errLockData = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockData = ErrLock.LockStart(String.Format("InformationCenter.cs-->656-->SendActionT-->{0}", typeof(T)));
                    }
                    Type t = typeof(T);
                    Dictionary<string, InformationCenterActionTMsgDelegate> dic;
                    targetFun.TryGetValue(t, out dic);
                    if (dic == null)
                    {
                        if (ErrLock.ErrLockOpen)
                        {
                            ErrLock.LockEnd(errLockData);
                        }
                        continue;
                    }

                    InformationCenterActionTMsgDelegate saveFun;
                    if (dic.TryGetValue(msg, out saveFun))
                    {
                        if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                        {
                            ActionTFunData data = actionTFunDataPool.Spawn();
                            data.actionYes = actionYes;
                            data.actionNo = actionNo;
                            data.objs = objs;
                            data.Fun = saveFun;
                            ThreadHelper.UnitySynchronizationContext.Post((p) => {
                                ActionTFunData actionTFunData = (ActionTFunData)p;
                                try
                                {
                                    actionTFunData.Fun.Invoke(actionTFunData.actionYes, actionTFunData.actionNo, actionTFunData.objs);
                                }
                                catch (System.Exception e)
                                {
                                    VLog.Error(e.Message);
                                }
                                actionTFunDataPool.Recycle(actionTFunData);
                            }, data);
                        }
                        else
                        {
                            try
                            {
                                saveFun.Invoke(actionYes, actionNo, objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                        }
                    }
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                }
            }
        }

        static SimplePool<ActionTFunData> actionTFunDataPool = new SimplePool<ActionTFunData>();

        class ActionTFunData: FunData
        {
            public System.Action<object> actionYes;
            public System.Action<object> actionNo;
            public object[] objs;
            public InformationCenterActionTMsgDelegate Fun;

            public override void PutIn()
            {
                Fun = null;
                actionYes = null;
                actionNo = null;
                objs = null;
                base.PutIn();
            }
        }

        #endregion

        public void Clear()
        {
            lock (intMsgFuns)
            {
                intMsgFuns.Clear();
            }
            lock (strMsgFuns)
            {
                strMsgFuns.Clear();
            }
            lock (intMsgActionFuns)
            {
                intMsgActionFuns.Clear();
            }
            lock (strMsgActionFuns)
            {
                strMsgActionFuns.Clear();
            }
            lock (intMsgActionTFuns)
            {
                intMsgActionTFuns.Clear();
            }
            lock (strMsgActionTFuns)
            {
                strMsgActionTFuns.Clear();
            }
        }

        public void Dispose()
        {
            Clear();
            lock (intMsgFuns)
            {
                intMsgFuns_DontClear.Clear();
            }
            lock (strMsgFuns)
            {
                strMsgFuns_DontClear.Clear();
            }
            lock (intMsgActionFuns)
            {
                intMsgActionFuns_DontClear.Clear();
            }
            lock (strMsgActionFuns)
            {
                strMsgActionFuns_DontClear.Clear();
            }
            lock (intMsgActionTFuns)
            {
                intMsgActionTFuns_DontClear.Clear();
            }
            lock (strMsgActionTFuns)
            {
                strMsgActionTFuns_DontClear.Clear();
            }
            actionTFunDataPool.Clear();
            actionFunDataPool.Clear();
            funThreadDataPool.Clear();
        }
    }
}


