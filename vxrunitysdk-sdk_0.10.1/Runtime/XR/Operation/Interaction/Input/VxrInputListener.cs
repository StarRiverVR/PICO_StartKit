using UnityEngine;
using com.vivo.codelibrary;
using System;
using System.Collections.Generic;

namespace com.vivo.openxr
{
    public class VXRInputListener : MonoSingleton<VXRInputListener>
    {

        public static bool IsCreateInputListener = false;
        public Action<string> _listenerEvent;
        public Dictionary<Action<string>, object> _listenerEventBindInfo = new Dictionary<Action<string>, object>();

        protected override void AwakeFun()
        {
            base.AwakeFun();
            if (Application.isPlaying)
            {
                GameObject.DontDestroyOnLoad(gameObject);
            }
            gameObject.name = "VXRInputListenerNode";
            VXRInputListener.IsCreateInputListener = true;
            Debug.Log("创建监听成功");

        }

        public void Init()
        {

        }

        public void StartListener()
        {
            Debug.Log("设置监听绑定信息");
            VXRInput.SetMessageByUnity("VXRInputListenerNode", "VXRInputListenerEvent");
        }

        public void BindListenerEvent(object obj, Action<string> listenerEvent = null)
        {
            if (listenerEvent != null && !_listenerEventBindInfo.ContainsKey(listenerEvent))
            {
                _listenerEventBindInfo.Add(listenerEvent, obj);
                _listenerEvent += listenerEvent;
            }
        }

        public void UnBindListenerEvent(object obj)
        {       
            List<Action<string>> baselIListenerRecord = new List<Action<string>>();            
            foreach (var item in _listenerEventBindInfo)
            {               
                if (item.Value == obj)
                {
                    baselIListenerRecord.Add(item.Key);
                }
            }    

            for (int i = 0; i < baselIListenerRecord.Count; i++)
            {
                _listenerEventBindInfo.Remove(baselIListenerRecord[i]);     
                _listenerEvent -= baselIListenerRecord[i];
            }               
        }


        public void VXRInputListenerEvent(string content)
        {
            Debug.Log("监听回调");
            Debug.Log("================================");
            Debug.Log("Android2Unity : " + content);
            if (!VXRControllerPlugin.IsServiceConnected && content == VXRControllerPlugin.ServiceConnectedCode)
            {
                VXRControllerPlugin.IsServiceConnected = true;                 
            }

            Debug.Log("监听分发事件信息检测：开始");
            if (_listenerEvent != null)
            {
                _listenerEvent(content);
            }
            Debug.Log("监听分发事件信息检测：结束");                     
        }

        protected override void OnDestroy()
        {
            VXRInputListener.IsCreateInputListener = false;
            if (_listenerEvent != null) {
                Delegate[] baseEventDeles = _listenerEvent.GetInvocationList();
                for (int i = 0; i < baseEventDeles.Length; i++)
                {
                    _listenerEvent -= baseEventDeles[i] as Action<string>;
                }
            }            
            base.OnDestroy();
        }
    }
}
