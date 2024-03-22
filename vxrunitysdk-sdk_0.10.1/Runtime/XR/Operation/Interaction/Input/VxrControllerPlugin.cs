using UnityEngine;
using com.vivo.codelibrary;
using System;
using System.Collections.Generic;

namespace com.vivo.openxr
{
    class ControllerLib
    {

        private string _javaLibClassName = "com.vivo.sticklib.XRStickController";
        private string _javaLibUnityPlayer = "com.unity3d.player.UnityPlayer";
        internal AndroidJavaClass _javaLib;
        
        public ControllerLib()
        {
            VLog.Info("开始初始化库信息");
#if !UNITY_EDITOR && UNITY_ANDROID
            _javaLib = new AndroidJavaClass(_javaLibClassName);           
            AndroidJavaObject context = new AndroidJavaClass(_javaLibUnityPlayer).GetStatic<AndroidJavaObject>("currentActivity");
            _javaLib.CallStatic("init",context);
            VLog.Info("初始化库信息完毕");
#endif
        }

        public AndroidJavaClass JaveLib { get { return _javaLib; } }        
    }

    public class HandStickStateListener : AndroidJavaProxy
    {
        Action<DeviceUnit, HandleListenerType, int> listenerCallbacks;
        public HandStickStateListener(Action<DeviceUnit, HandleListenerType, int> callBack) : base("com.vivo.sticklib.IStickStateListener")
        {
            listenerCallbacks = callBack;
        }        

        public void onStateChange(int unit, int type, int value)
        {
            DeviceUnit deviceUnit = (DeviceUnit)unit;
            HandleListenerType listenerType = (HandleListenerType)type;
            if (listenerCallbacks != null)
            {
                listenerCallbacks(deviceUnit, listenerType, value);
            }
        }
    }

    public static class VXRControllerPlugin
    {

        static ControllerLib m_ctrlLib;
        static AndroidJavaClass _javaLib;

        /// <summary>
        /// SDKService链接成功返回字符串
        /// </summary>
        public const string ServiceConnectedCode = "serviceConnected";
        /// <summary>
        /// SDKService是否链接成功标标记
        /// </summary>
        public static bool IsServiceConnected = false;

        public static List<HandStickStateListener> bindStickListenerList;

        static VXRControllerPlugin()
        {
            VXRInputListener.Instance.Init();
            VLog.Info("SDK初始化完成启用测试监听完成");
            m_ctrlLib = new ControllerLib();
            _javaLib = m_ctrlLib.JaveLib;
         
            VXRInputListener.Instance.StartListener();
            bindStickListenerList = new List<HandStickStateListener>();
        }


        [RuntimeInitializeOnLoadMethod]
        public static void PluginInit()
        {
           
        }

        public static bool IsConnected()
        {
            VLog.Info("获取是否连接成功");
            VLog.Info("测试SDK信息：" + GetHeadVersion());
#if !UNITY_EDITOR && UNITY_ANDROID
            if (VXRControllerPlugin.IsServiceConnected)
            {
                VLog.Info("SDk连接成功");
                string text1 = "getHeadVersion :  " + GetHeadVersion();
                string text2 = "getLeftHandlePower :  " + GetHandlePower(DeviceUnit.Left);
                string text3 = "getLeftHandleSN :  " + GetHandleSN(DeviceUnit.Left);
                string text4 = "getHeadVersion :  " + GetHeadVersion();
                VLog.Info("测试SDK信息1：" + text1);
                VLog.Info("测试SDK信息2：" + text2);
                VLog.Info("测试SDK信息3：" + text3);
                VLog.Info("测试SDK信息4：" + text4);
            }             
            return VXRControllerPlugin.IsServiceConnected;  
#endif
            return false;
        }

        public static string GetHeadVersion()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return _javaLib.CallStatic<string>("getHeadVersion");
#else
            return null;

#endif
        }

        /// <summary>
        /// 获取手柄版本号
        /// </summary>
        /// <param name="unit"></param 设备部件>
        /// <returns></returns>
        public static string GetHandVersion(DeviceUnit unit)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
           string methodName = unit == DeviceUnit.Left ? "getLeftHandleVersion" : "getRightHandleVersion";
            return _javaLib.CallStatic<string>(methodName);
#else
            return null;

#endif
        }

        public static string GetHandleSN(DeviceUnit unit)
        {

#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "getLeftHandleSN" : "getRightHandleSN";
            return _javaLib.CallStatic<string>(methodName);
#else
            return null;

#endif
        }

        public static int PairHandleMode(DeviceUnit unit)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "pairLeftHandleMode" : "pairRightHandleMode";
            return _javaLib.CallStatic<int>(methodName);
#else
            return -1;
#endif
        }

        public static int UnpairHandleMode(DeviceUnit unit)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "unpairLeftHandleMode" : "unpairRightHandleMode";
            return _javaLib.CallStatic<int>(methodName);
#else
            return -1;
#endif
        }

        public static int OtaHead(string file)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            return _javaLib.CallStatic<int>("otaHead",file);
#else
            return -1;
#endif
        }

        public static int OtaHandle(DeviceUnit unit, string file)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "otaLeftHandle" : "otaRightHandle";
            return _javaLib.CallStatic<int>(methodName,file);
#else
            return -1;
#endif
        }

        public static int GetHandleState(DeviceUnit unit)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "getLeftHandleState" : "getRightHandleState";
            return _javaLib.CallStatic<int>(methodName);
#else
            return -1;
#endif
        }

        public static int GetHandlePower(DeviceUnit unit)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "getLeftHandlePower" : "getRightHandlePower";
            return _javaLib.CallStatic<int>(methodName);
#else
            return -1;
#endif
        }

        public static int SetTriggerForceFeedback(TriggerForceFeedbackOptions options)
        {
            DeviceUnit unit = options.unit;
            int mode = options.mode;
            int dampingForce = options.dampingForce;
            int dampingStartPos = options.dampingStartPos;
            int vibrationForce = options.vibrationForce;
            int vibrationFrequency = options.vibrationFrequency;
            int vibrationStartPos = options.vibrationStartPos;
            int vibrationStartForce = options.vibrationStartForce;
            int breakthroughStartPos = options.breakthroughStartPos;
            int breakthroughContinuousJourney = options.breakthroughContinuousJourney;
            int breakthroughResistance = options.breakthroughResistance;


#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "setLeftTriggerForceFeedback" : "setRightTriggerForceFeedback";
            return _javaLib.CallStatic<int>(methodName,
                                                mode,
                                                dampingForce,
                                                dampingStartPos,
                                                vibrationForce,
                                                vibrationFrequency,
                                                vibrationStartPos,
                                                vibrationStartForce,
                                                breakthroughStartPos,
                                                breakthroughContinuousJourney,
                                                breakthroughResistance);
#else
            return -1;
#endif
        }


        public static int SetTriggerForceFeedbackModeOnly(DeviceUnit unit, int mode)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "setLeftTriggerForceFeedbackModeOnly" : "setRightTriggerForceFeedbackModeOnly";
            return _javaLib.CallStatic<int>(methodName,mode);
#else
            return -1;
#endif
        }


        // 设置手柄Touchpad振动反馈
        public static int SetTouchpadVibrationFeedback(TouchpadVibrationFeedbackOptions options)
        {
            DeviceUnit unit = options.unit; 
            int mode = options.mode; 
            int frequency = options.frequency; 
            int amplitude = options.amplitude; 
            int duration = options.duration; 
            int startTime = options.startTime; 
            int brakeTime = options.brakeTime;
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "setLeftTouchpadVibrationFeedback" : "setRightTouchpadVibrationFeedback";
            return _javaLib.CallStatic<int>(methodName, 
                                             mode,
                                             frequency,
                                             amplitude,
                                             duration,
                                             startTime,
                                             brakeTime);
#else
            return -1;
#endif
        }

        public static int SetTouchpadVibrationFeedbackModeOnly(DeviceUnit unit, int mode)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "setLeftTouchpadVibrationFeedbackModeOnly" : "setRightTouchpadVibrationFeedbackModeOnly";
            return _javaLib.CallStatic<int>(methodName, mode);
#else
            return -1;
#endif
        }


        // 设置手柄主马达振动反馈
        public static int SetMainMotorVibrationFeedback(MainMotorVibrationFeedbackOptions options)
        {
            DeviceUnit unit = options.unit;
            int mode = options.mode;
            int frequency = options.frequency;
            int amplitude = options.amplitude;
            int duration = options.duration;
            int startTime = options.startTime;
            int brakeTime = options.brakeTime;

#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "setLeftMainMotorVibrationFeedback" : "setRightMainMotorVibrationFeedback";
            return _javaLib.CallStatic<int>(methodName, 
                                             mode,
                                             frequency,
                                             amplitude,
                                             duration,
                                             startTime,
                                             brakeTime);
#else
            return -1;
#endif
        }

        public static int SetMainMotorVibrationFeedbackModeOnly(DeviceUnit unit, int mode)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            string methodName = unit == DeviceUnit.Left ? "setLeftMainMotorVibrationFeedbackModeOnly" : "setRightMainMotorVibrationFeedbackModeOnly";
            return _javaLib.CallStatic<int>(methodName,mode);
#else
            return -1;
#endif
        }

        // 状态监听
        public static int RegisterStickStateListener(HandleListenerType type, HandStickStateListener listener)
        {

            if (bindStickListenerList.Contains(listener))
            {
                VLog.Error("该监听已经绑定过，请勿重复绑定");
                return (int)ResultOperateType.RESULT_OPERATE_FAIL;
            }

            int listenerType = (int)type;
#if !UNITY_EDITOR && UNITY_ANDROID
            VLog.Info("注册手柄状态监听的类型：" + listenerType);
           return _javaLib.CallStatic<int>("registerStickStateListener",listenerType,listener);
#else
            return -1;
#endif
        }

        public static int UnregisterStickStateListener(HandStickStateListener listener)
        {            
#if !UNITY_EDITOR && UNITY_ANDROID
            return _javaLib.CallStatic<int>("unregisterStickStateListener",listener);
#else
            return -1;
#endif
        }

        public static void OnApplicationQuit()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            _javaLib.CallStatic("unInit");
#else

#endif
        }

        public static void SetMessageByUnity(string objname, string funName)
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            VLog.Info("监听对象的物体和方法名：" + objname + "  ===   " + funName);

            _javaLib.CallStatic("setMessageByUnity",objname,funName);
#else

#endif
        }
    }
    
}

