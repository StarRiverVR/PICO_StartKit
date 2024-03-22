using UnityEngine;

namespace com.vivo.openxr {
    public class ImeUnityListener : AndroidJavaProxy
    {
        private ImeBase _ime;
        public ImeUnityListener() : base("com.vivo.vxrime.ImeUnityListener")
        {
        }
        
        public void playBoom(int boomType)
        {            
            if (_ime != null)
            {
                _ime.PlayBoom(boomType);
            }
        }

        public void SetIme(ImeBase ime)
        {
            _ime = ime;
        }
    }

}
