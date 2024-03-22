using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 可视化控制
    /// </summary>
    public class ExampleView : View<ExampleControl, ExampleModel>
    {

        protected override void Awake()
        {
            base.Awake();
            //注册监听，监控ExampleModel数据是否发生变动
            control.AddListen<ExampleModel.MSG>((int)ExampleModel.MSG.TestInt, DataChange_TestInt);
        }

        //Control<ExampleControl> control;

        //ExampleModel model;

        //private void Start()
        //{
        //    control = ExampleControl.Instance;
        //    model = ExampleModel.Instance;

        //}

        protected override void RemoveListens()
        {
            //需要销毁注册的监听
            control.RemoveListen<ExampleModel.MSG>((int)ExampleModel.MSG.TestInt, DataChange_TestInt);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveListens();
        }

        void DataChange_TestInt(params object[] objs)
        {
            //数据发生变化 刷新界面
            int newInt = model.TestInt;
        }

    }
}



