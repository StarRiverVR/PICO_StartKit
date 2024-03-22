using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 当某个数据发生变化，需要调用 ModelDataChange((int)Enum);通知Controller，而后由Controller通知注册过消息的View
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Model<T, CT> : CSingleton<Model<T, CT>>, IModel, IDisposable where T : Model<T, CT> where CT : Control<CT>
    {
        /// <summary>
        /// 子类实例
        /// </summary>
        public static T ModelInstance
        {
            get
            {
                return (T)Instance;
            }
        }

        public Model()
        {
            MVCInitialization obj = MVCInitialization.Instance;
            control = Control<CT>.Instance.ThisControl;
            lock (lockObj)
            {
                if (_instance == null)
                {
                    _instance = this;
                }
            }
        }

        /// <summary>
        /// 会在游戏启动的时候调用
        /// </summary>
        public virtual void Initialization()
        {

        }

        /// <summary>
        /// 控制中心
        /// </summary>
        protected ControlBase<Control<CT>> control;

        public void ModelDataChange<EnumT>(int id, bool runInUnityMainThread) where EnumT : Enum
        {
            this.control.Send<EnumT>(id, runInUnityMainThread);
        }

        public void Dispose()
        {
            control.Clear();
        }
    }

    public class MVCModel : Model<MVCModel, MVCControl>
    {
        public override void Initialization()
        {
            base.Initialization();
            control.AddListen<MSG>((int)MSG.TestMsg, TestMsg);
        }

        //当数据发生变化，需要通知控制器，由控制器通知注册过消息的View更新界面，View通过ExampleModel.Instance获得当前数据

        int testInt;

        public int TestInt
        {
            get
            {
                return testInt;
            }
            set
            {
                testInt = value;
                ModelDataChange<MSG>((int)MSG.TestInt, true);
            }
        }

        void TestMsg(params object[] objs)
        {

        }


        public enum MSG
        {
            TestInt,
            TestMsg,
        }
    }
}



