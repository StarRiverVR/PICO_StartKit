
using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static void Imeinit()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
#else
            VXRVersion_0_7_0.Imeinit();
#endif
        }

        public static int[] ImeGetSize()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
           return new int[] {0,0};
#else
            return VXRVersion_0_7_0.ImeGetSize();
#endif
        }

        public static bool ImeIsInited()
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported VirtualKeyboard");
#else
            return VXRVersion_0_7_0.ImeIsInited();
#endif
        }

        public static bool ImeIsNeedUpdate()
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported VirtualKeyboard");
            return false;
#else
            return VXRVersion_0_7_0.ImeIsNeedUpdate();
#endif
        }

        public static byte[] ImeGetTextureData()
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported VirtualKeyboard");
            return new byte[] { };
#else
            return VXRVersion_0_7_0.ImeGetTextureData();
#endif
        }

        public static void ImeShow(VXRPlugin.ImeInputType typeInput, VXRPlugin.ImeTextType typeText)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported VirtualKeyboard");
#else
            VXRVersion_0_7_0.ImeShow(typeInput, typeText);
#endif
        }

        public static void ImeHide()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
#else
            VXRVersion_0_7_0.ImeHide();
#endif
        }

        public static void ImeOnTouch(float x, float y, VXRPlugin.ImeMotionEventType type)
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
#else
            VXRVersion_0_7_0.ImeOnTouch(x, y, type);
#endif
        }

        public static bool ImeIsRecording()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
           return false;
#else
            return VXRVersion_0_7_0.ImeIsRecording();
#endif
        }

        public static int ImeGetCommitCode()
        {

#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
           return -1000;
#else
            return VXRVersion_0_7_0.ImeGetCommitCode();
#endif
        }

        public static string ImeGetCommitString()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
           return null;
#else
            return VXRVersion_0_7_0.ImeGetCommitString();
#endif
        }

        public static bool ImeIsShow()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
           return false;
#else
            return VXRVersion_0_7_0.ImeIsShow();
#endif
        }

        public static int ImeGetScene()
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
           return -1;
#else
            return VXRVersion_0_7_0.ImeGetScene();
#endif
        }

        public static void ImeSetScene(int type)
        {
#if VXR_UNSUPPORTED_PLATFORM
           VLog.Warning($"Not Supported VirtualKeyboard");
#else
            VXRVersion_0_9_0.ImeSetLocation(type);
#endif
        }

        public static void ImeRegisterUnityImeListener(ImeUnityListener listener)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported VirtualKeyboard");
#else
            VXRVersion_0_9_0.ImeRegisterUnityImeListener(listener);
#endif

        }

    }
}
