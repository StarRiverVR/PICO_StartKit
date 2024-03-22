namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_7_0
        {
            public static void Imeinit()
            {
                ImeBridge.init();
            }

            public static int[] ImeGetSize()
            {
                return ImeBridge.GetSize();
            }

            public static bool ImeIsInited()
            {
                return ImeBridge.IsInited();
            }

            public static bool ImeIsNeedUpdate()
            {
                return ImeBridge.IsNeedUpdate();

            }

            public static byte[] ImeGetTextureData()
            {
                return ImeBridge.GetTextureData();
            }

            public static void ImeShow(VXRPlugin.ImeInputType typeInput, VXRPlugin.ImeTextType typeText)
            {
                ImeBridge.Show(typeInput, typeText);
            }

            public static void ImeHide()
            {
                ImeBridge.Hide();
            }

            public static void ImeOnTouch(float x, float y, VXRPlugin.ImeMotionEventType type)
            {
                ImeBridge.OnTouch(x, y, type);
            }

            public static bool ImeIsRecording()
            {
                return ImeBridge.IsRecording();
            }

            public static int ImeGetCommitCode()
            {
                return ImeBridge.GetCommitCode();
            }

            public static string ImeGetCommitString()
            {
                return ImeBridge.GetCommitString();
            }

            public static bool ImeIsShow()
            {
                return ImeBridge.IsShow();
            }

            public static int ImeGetScene()
            {
                return ImeBridge.GetScene();
            }

        }

        private sealed partial class VXRVersion_0_9_0
        {

            public static void ImeSetLocation(int type)
            {
                ImeBridge.SetLocation(type);
            }

            public static void ImeRegisterUnityImeListener(ImeUnityListener listener)
            {
                ImeBridge.RegisterUnityImeListener(listener);
            }
        }
    }
}
