using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    public class Control<T> : CSingleton<Control<T>>, IControl, IDisposable
    {
        ControlBase<Control<T>> control;

        public ControlBase<Control<T>> ThisControl
        {
            get
            {
                return control;
            }
        }

        public Control()
        {
            control = new ControlBase<Control<T>>();
        }

        public virtual void Dispose()
        {
            control.Clear();
        }
    }

    public class MVCControl : Control<MVCControl>
    {

    }

    public delegate void ControllerMsgDelegate(params object[] objs);

    public delegate void ControllerActionMsgDelegate(System.Action<object> action, params object[] objs);

    public delegate void ControllerAction2MsgDelegate(System.Action<object> actionYes, System.Action<object> actionNo, params object[] objs);

    public class ControlBase<T> where T : CSingleton<T>, IDisposable, new()
    {
        public ControlBase()
        {

        }

        public void ModelDataChange<EnumT>(int id, bool runInUnityMainThread) where EnumT : Enum
        {
            Send<EnumT>(id, runInUnityMainThread);
        }

        private Dictionary<Type, Dictionary<int, ControllerMsgDelegate>> msgFuns = new Dictionary<Type, Dictionary<int, ControllerMsgDelegate>>();

        private Dictionary<Type, Dictionary<int, ControllerActionMsgDelegate>> msgActionFuns = new Dictionary<Type, Dictionary<int, ControllerActionMsgDelegate>>();

        private Dictionary<Type, Dictionary<int, ControllerAction2MsgDelegate>> msgAction2Funs = new Dictionary<Type, Dictionary<int, ControllerAction2MsgDelegate>>();

        public void AddListen<EnumT>(int id, ControllerMsgDelegate fun) where EnumT : Enum
        {
            lock (msgFuns)
            {
                Type t = typeof(EnumT);
                Dictionary<int, ControllerMsgDelegate> dic;
                if (!msgFuns.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<int, ControllerMsgDelegate>();
                    msgFuns.Add(t, dic);
                }

                ControllerMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    saveFun += fun;
                    dic[id] = saveFun;
                }
                else
                {
                    saveFun += fun;
                    dic.Add(id, saveFun);
                }
            }
        }

        public void AddListenAction<EnumT>(int id, ControllerActionMsgDelegate fun) where EnumT : Enum
        {
            lock (msgActionFuns)
            {
                Type t = typeof(EnumT);
                Dictionary<int, ControllerActionMsgDelegate> dic;
                if (!msgActionFuns.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<int, ControllerActionMsgDelegate>();
                    msgActionFuns.Add(t, dic);
                }

                ControllerActionMsgDelegate saveFun;
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

        public void AddListenActionT<EnumT>(int id, ControllerAction2MsgDelegate fun) where EnumT : Enum
        {
            lock (msgAction2Funs)
            {
                Type t = typeof(EnumT);
                Dictionary<int, ControllerAction2MsgDelegate> dic;
                if (!msgAction2Funs.TryGetValue(t, out dic))
                {
                    dic = new Dictionary<int, ControllerAction2MsgDelegate>();
                    msgAction2Funs.Add(t, dic);
                }

                ControllerAction2MsgDelegate saveFun;
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

        public void RemoveListen<EnumT>(int id, ControllerMsgDelegate fun) where EnumT : Enum
        {
            lock (msgFuns)
            {
                Type t = typeof(EnumT);
                Dictionary<int, ControllerMsgDelegate> dic;
                msgFuns.TryGetValue(t, out dic);
                if (dic == null) return;

                ControllerMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    dic[id] = saveFun;
                }
            }
        }

        public void RemoveListenAction<EnumT>(int id, ControllerActionMsgDelegate fun) where EnumT : Enum
        {
            lock (msgActionFuns)
            {
                Type t = typeof(EnumT);
                Dictionary<int, ControllerActionMsgDelegate> dic;
                msgActionFuns.TryGetValue(t, out dic);
                if (dic == null) return;

                ControllerActionMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    dic[id] = saveFun;
                }
            }
        }

        public void RemoveListenActionT<EnumT>(int id, ControllerAction2MsgDelegate fun) where EnumT : Enum
        {
            lock (msgAction2Funs)
            {
                Type t = typeof(EnumT);
                Dictionary<int, ControllerAction2MsgDelegate> dic;
                msgAction2Funs.TryGetValue(t, out dic);
                if (dic == null) return;

                ControllerAction2MsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    saveFun -= fun;
                    dic[id] = saveFun;
                }
            }
        }

        public void Send<EnumT>(int id, bool runInUnityMainThread, params object[] objs) where EnumT : Enum
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (msgFuns)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("Control.cs-->178-->Send-->{0}", typeof(T)));
                }
                Type t = typeof(EnumT);
                Dictionary<int, ControllerMsgDelegate> dic;
                msgFuns.TryGetValue(t, out dic);
                if (dic == null)
                {
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return;
                }

                ControllerMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                    {
                        MsgFunData data = msgFunDataPool.Spawn();
                        data.objs = objs;
                        data.saveFun = saveFun;
                        ThreadHelper.UnitySynchronizationContext.Post((p) => {
                            MsgFunData msgFunData = (MsgFunData)p;
                            try
                            {
                                msgFunData.saveFun.Invoke(msgFunData.objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                            msgFunDataPool.Recycle(msgFunData);
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

        public void SendAction<EnumT>(int id, bool runInUnityMainThread, System.Action<object> action, params object[] objs) where EnumT : Enum
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (msgActionFuns)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("Control.cs-->229-->SendAction-->{0}", typeof(T)));
                }
                Type t = typeof(EnumT);
                Dictionary<int, ControllerActionMsgDelegate> dic;
                msgActionFuns.TryGetValue(t, out dic);
                if (dic == null)
                {
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return;
                }

                ControllerActionMsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                    {
                        MsgActionFunData data = msgActionFunDataPool.Spawn();
                        data.objs = objs;
                        data.action = action;
                        data.saveFun = saveFun;
                        ThreadHelper.UnitySynchronizationContext.Post((p) => {
                            MsgActionFunData msgActionFunData = (MsgActionFunData)p;
                            try
                            {
                                msgActionFunData.saveFun.Invoke(msgActionFunData.action, msgActionFunData.objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                            msgActionFunDataPool.Recycle(msgActionFunData);
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

        public void SendActionT<EnumT>(int id, bool runInUnityMainThread, System.Action<object> actionYes, System.Action<object> actionNo, params object[] objs) where EnumT : Enum
        {
            if (FrameSystemConfig.EndlessLoop())
            {
                return;
            }
            lock (msgAction2Funs)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart(String.Format("Control.cs-->282-->SendActionT-->{0}", typeof(T)));
                }
                Type t = typeof(EnumT);
                Dictionary<int, ControllerAction2MsgDelegate> dic;
                msgAction2Funs.TryGetValue(t, out dic);
                if (dic == null)
                {
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockData);
                    }
                    return;
                }

                ControllerAction2MsgDelegate saveFun;
                if (dic.TryGetValue(id, out saveFun))
                {
                    if (runInUnityMainThread && System.Threading.Thread.CurrentThread.ManagedThreadId != ThreadHelper.UnityThreadId)
                    {
                        MsgAction2FunData data = msgAction2FunDataPool.Spawn();
                        data.objs = objs;
                        data.actionYes = actionYes;
                        data.actionNo = actionNo;
                        data.saveFun = saveFun;
                        ThreadHelper.UnitySynchronizationContext.Post((p) => {
                            MsgAction2FunData msgAction2FunData = (MsgAction2FunData)p;
                            try
                            {
                                msgAction2FunData.saveFun.Invoke(msgAction2FunData.actionYes, msgAction2FunData.actionNo, msgAction2FunData.objs);
                            }
                            catch (System.Exception e)
                            {
                                VLog.Error(e.Message);
                            }
                            msgAction2FunDataPool.Recycle(msgAction2FunData);
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

        static SimplePool<MsgFunData> msgFunDataPool = new SimplePool<MsgFunData>();

        class MsgFunData : FunData
        {
            public object[] objs;

            public ControllerMsgDelegate saveFun;

            public override void PutIn()
            {
                objs = null;
                saveFun = null;
                base.PutIn();
            }
        }

        static SimplePool<MsgActionFunData> msgActionFunDataPool = new SimplePool<MsgActionFunData>();

        class MsgActionFunData : FunData
        {
            public object[] objs;

            public System.Action<object> action;

            public ControllerActionMsgDelegate saveFun;

            public override void PutIn()
            {
                action = null;
                objs = null;
                saveFun = null;
                base.PutIn();
            }
        }

        static SimplePool<MsgAction2FunData> msgAction2FunDataPool = new SimplePool<MsgAction2FunData>();

        class MsgAction2FunData : FunData
        {
            public object[] objs;

            public System.Action<object> actionYes;

            public System.Action<object> actionNo;

            public ControllerAction2MsgDelegate saveFun;

            public override void PutIn()
            {
                actionYes = null;
                actionNo = null;
                objs = null;
                saveFun = null;
                base.PutIn();
            }
        }

        public void Clear()
        {
            msgFuns.Clear();
            msgActionFuns.Clear();
            msgAction2Funs.Clear();
            msgFunDataPool.Clear();
            msgActionFunDataPool.Clear();
            msgAction2FunDataPool.Clear();
        }
    }
}


