using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public class ImeManager
    {
        private ImeBase _ime;
        private bool _isPaused = false;
        private VXRPlugin.ImeType _imeType;


        private static ImeManager _instance;
        public static ImeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ImeManager();
                }
                return _instance;
            }
        }

        public void CreateIme(VXRPlugin.ImeType imeType, VXRVirtualKeyboard keyboard)
        {

            _imeType = imeType;  
            switch (imeType)
            {     
                case VXRPlugin.ImeType.Default:
                    VLog.Info("ime create imeInstance");
                    _ime = new DefaultIme();
                    break;
                default:
                    break;
            }
            _ime.Create(keyboard);
     
            ImeUnityListener lister = new ImeUnityListener();
            VXRPlugin.ImeRegisterUnityImeListener(lister);
            lister.SetIme(_ime);
     
        }

        public void UpdateData()
        {

            if (null == _ime)
            {
                return;
            }

            if (!_isPaused)
            {
                _ime.UpdateData();
            }

        }


        //export
        public void Show(VXRPlugin.VirtualKeyBoardLayout keyBoardLayout)
        {
            if (_imeType == VXRPlugin.ImeType.Default)
            {
                VXRPlugin.ImeInputType inputType = VXRPlugin.ImeInputType.TYPE_CLASS_TEXT;
                VXRPlugin.ImeTextType textType = VXRPlugin.ImeTextType.TYPE_TEXT_VARIATION_NORMAL;
                switch (keyBoardLayout)
                {
                    case VXRPlugin.VirtualKeyBoardLayout.Text:
                        inputType = VXRPlugin.ImeInputType.TYPE_CLASS_TEXT;
                        textType = VXRPlugin.ImeTextType.TYPE_TEXT_VARIATION_NORMAL;
                        break;
                    case VXRPlugin.VirtualKeyBoardLayout.Number:
                        inputType = VXRPlugin.ImeInputType.TYPE_CLASS_NUMBER;
                        textType = VXRPlugin.ImeTextType.TYPE_TEXT_VARIATION_NORMAL;
                        break;
                    default:
                        break;
                }
                VLog.Info("ime ImeManager::Show() typeInput=" + inputType + ", typeText=" + inputType);
                _ime.Show(inputType, textType);
            }
        }

        public void Hide()
        {
            VLog.Info("ime ImeManager::Hide");
            _ime.Hide();
        }

        public void Draw()
        {
            _ime.Draw();
        }

        public void OnTouch(float x, float y, VXRPlugin.ImeMotionEventType type)
        {
            _ime.OnTouch(x, y, type);
        }

        public byte[] GetTextureData()
        {
            return _ime.GetTextureData();
        }

        public bool IsRecording()
        {
            return _ime.IsRecording();
        }

        public string GetCommitString()
        {
            return _ime.GetCommitString();
        }

        public int GetCommitCode()
        {
            return _ime.GetCommitCode();
        }

        public bool IsShow()
        {

            return _ime.IsShow();
        }

        public void SetTriggerStatus(int controllerFlag, bool isTrigger)
        {
            _ime.SetTriggerStatus(controllerFlag, isTrigger);
        }

        public void SetKeyboardVisible(bool isVisible)
        {
            _ime.SetKeyboardVisible(isVisible);
        }

        void OnApplicationFocus(bool hasFocus)
        {
            VLog.Info("ime ImeManager::OnApplicationFocus hasFocus=" + hasFocus);
            if (!hasFocus)
            {
                Hide();
            }
        }

        void OnApplicationPause(bool pauseStatus)
        {
            VLog.Info("ime ImeManager::OnApplicationPause pauseStatus=" + pauseStatus);
            if (pauseStatus)
            {
                Hide();
            }
        }
    }
}
