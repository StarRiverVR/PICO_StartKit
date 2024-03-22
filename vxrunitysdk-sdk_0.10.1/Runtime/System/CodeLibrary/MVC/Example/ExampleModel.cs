using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 使用 ExampleModel.Instance
    /// </summary>
    public class ExampleModel : Model<ExampleModel, ExampleControl>
    {
        public override void Initialization()
        {
            base.Initialization();
            //if (_instance != null) return;
            //_instance = this;
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


