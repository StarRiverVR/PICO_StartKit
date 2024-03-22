using com.vivo.codelibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.openxr
{
    public class ImeBridge
    {
        private static AndroidJavaClass _javaIme = null;
        private static bool _isInited = false;
        private static AndroidJavaObject _androidContext;

        public static void init()
        {
            _javaIme = new AndroidJavaClass("com.vivo.vxrime.VXRImeMain");


            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            _androidContext = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            _javaIme.CallStatic("initIme", _androidContext, 1);

            _isInited = true;
        }

        public static int[] GetSize()
        {
            int[] size = _javaIme.CallStatic<int[]>("getSize");
            VLog.Info("ime bridge call getsize func : " + size[0] + "," + size[1]);
            return size;
        }

        public static bool IsInited()
        {
            return null != _javaIme;
        }

        public static bool IsNeedUpdate()
        {
            bool bNeedUpdate = _javaIme.CallStatic<bool>("isNeedRefresh");
            return bNeedUpdate;
        }


        public static byte[] GetTextureData()
        {
            VLog.Info("ime bridge call getTextureData");
            return _javaIme.CallStatic<byte[]>("getTextureData");
        }


        public static void Show(VXRPlugin.ImeInputType typeInput, VXRPlugin.ImeTextType typeText)
        {
            VLog.Info("ime bridge call show:" + typeInput + "," + typeText);
            _javaIme.CallStatic<bool>("show", (int)typeInput, (int)typeText);
        }

        public static void Hide()
        {
            VLog.Info("ime bridge call hide");
            _javaIme.CallStatic<bool>("hide");
        }

        public static void OnTouch(float x, float y, VXRPlugin.ImeMotionEventType type)
        {
            _javaIme.CallStatic<bool>("onTouch", x, y, (int)type);
        }

        public static bool IsRecording()
        {
            return _javaIme.CallStatic<bool>("isRecording");
        }

        public static int GetCommitCode()
        {
            return _javaIme.CallStatic<int>("getCommitCode");
        }

        public static string GetCommitString()
        {
            string strCommit = _javaIme.CallStatic<string>("getCommitString");
            return strCommit;
        }

        public static bool IsShow()
        {
            return _javaIme.CallStatic<bool>("isShow");
        }

        public static int GetScene()
        {
            return _javaIme.CallStatic<int>("getLocation");
        }

        public static void SetLocation(int type)
        {

            _javaIme.CallStatic("setLocation", type);
        }

        public static void RegisterUnityImeListener(ImeUnityListener listener)
        {
            VLog.Info("vxrsdk_ime imebridge unitylistener call register");
            _javaIme.CallStatic("registerImeUnityListener", listener);
        }

    }

}
