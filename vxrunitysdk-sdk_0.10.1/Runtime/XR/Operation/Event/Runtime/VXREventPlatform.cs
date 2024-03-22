using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace com.vivo.openxr
{
    public static class VXREventPlatfrom
    {

#if VXR_USE_CROWN_TOUCH
        public class CrownTouchListener : AndroidJavaProxy
        {
            public CrownTouchListener() : base("com.vivo.vxrevent.crown.CrownTouchListener")
            {

            }
            public void onRespondTouch(int touchType)
            {
                VXRPlugin.CrownTouchData crownData = new VXRPlugin.CrownTouchData()
                {
                    TouchType = (VXRPlugin.CrownTouchType)touchType,
                };
                byte[] bytyArr = VXRDeserialize.StructureToByteArray(crownData);
                VXRPlugin.EventDataBuffer dataBuffer = new VXRPlugin.EventDataBuffer()
                {
                    EventType = VXRPlugin.EventType.CrownTouch,
                    EventData = bytyArr,
                };
                AddEventBufferFromAndroid(dataBuffer);
            }
        }
#endif
    

        public class CrownRotateListener : AndroidJavaProxy
        {
            public CrownRotateListener() : base("com.vivo.vxrevent.crown.CrownRotateListener")
            {

            }
            // Native事件 累计旋转角度
            public void onRespondRotateTotal(bool direction, int angle)
            {

        #if VXR_USE_CROWN_ROTATE_TOTAL
                VXRPlugin.CrownRotateData crownData = new VXRPlugin.CrownRotateData()
                {
                    Direction = direction,
                    RotateAngle = angle,
                };
                pushCrownRotateEvent(crownData, VXRPlugin.EventType.CrownRotateTotal);
        #endif
            }
            // Native事件 归一化旋转事件 -360~360度
            public void onRespondRotate360(bool direction, int angle)
            {
                VXRPlugin.CrownRotateData crownData = new VXRPlugin.CrownRotateData()
                {
                    Direction = direction,
                    RotateAngle = angle,
                };
                pushCrownRotateEvent(crownData, VXRPlugin.EventType.CrownRotate360);
            }
            // Native事件 单次旋转角度
            public void onRespondRotateDelta(bool direction, int delta)
            {

                VXRPlugin.CrownRotateData crownData = new VXRPlugin.CrownRotateData()
                {
                    Direction = direction,
                    RotateAngle = delta,
                };
                pushCrownRotateEvent(crownData, VXRPlugin.EventType.CrownRotateDelta);
            }
            // 将表冠旋转数据转换为buffer推入事件队列
            private void pushCrownRotateEvent<T>(T crownData, VXRPlugin.EventType eventType) where T : struct
            {
                // 将表冠数据转换为buffer
                byte[] bytyArr = VXRDeserialize.StructureToByteArray(crownData);
                VXRPlugin.EventDataBuffer dataBuffer = new VXRPlugin.EventDataBuffer()
                {
                    EventType = eventType,
                    EventData = bytyArr,
                };
                // 存入事件缓冲区
                AddEventBufferFromAndroid(dataBuffer);
            }
        }


        private static AndroidJavaObject _androidContext;
        public static AndroidJavaObject AndroidContext { get => _androidContext; }

        private static AndroidJavaClass _androidXrEvent;
        public static AndroidJavaClass AndroidXrEvent { get => _androidXrEvent; }

        private static List<VXRPlugin.EventDataBuffer> _androidDataBufferList = new List<VXRPlugin.EventDataBuffer>();

        static VXREventPlatfrom()
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _androidContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _androidXrEvent = new AndroidJavaClass("com.vivo.vxrevent.XrEvent");
            _androidXrEvent.CallStatic("initXrEvent", _androidContext);

#if VXR_USE_CROWN_TOUCH
            RegisterCrownTouchEvent();
#endif
            RegisterCrownRotateEvent();
        }
#if VXR_USE_CROWN_TOUCH
        public static void RegisterCrownTouchEvent()
        {
            CrownTouchListener listener = new CrownTouchListener();
            _androidXrEvent.CallStatic("registerCrownTouchEvent", listener);
        }

        public static void UnRegisterCrownTouchEvent()
        {
            _androidXrEvent.CallStatic("unRegisterCrownTouchEvent");
        }
#endif
       
        public static void RegisterCrownRotateEvent()
        {
            CrownRotateListener listener = new CrownRotateListener();
            _androidXrEvent.CallStatic("registerCrownRotateEvent", listener);
        }

        public static void UnRegisterCrownRotateEvent()
        {
            _androidXrEvent.CallStatic("unRegisterRotateEvent");
        }

        public static void ReleaseAndroidVXREvent()
        {
#if VXR_USE_CROWN_TOUCH
            UnRegisterCrownTouchEvent();
#endif
            UnRegisterCrownRotateEvent();
            _androidXrEvent.CallStatic("release");
        }

        public static void AddEventBufferFromAndroid(VXRPlugin.EventDataBuffer buffer)
        {
            _androidDataBufferList.Add(buffer);
        }

        public static void PollEvent(ref List<VXRPlugin.EventDataBuffer> buffList)
        {
            if (_androidDataBufferList.Count < 1)
            {
                return;
            }
            buffList.AddRange(_androidDataBufferList);
            _androidDataBufferList.Clear();
        }
    }

}
