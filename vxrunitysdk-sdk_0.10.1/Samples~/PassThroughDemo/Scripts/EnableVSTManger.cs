using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.UI;

namespace com.vivo.openxr {
    public class EnableVSTManger:MonoBehaviour
    {
        public GameObject ChangeBlendModelBtn;                     
        bool isSelect_trigger = false;        

        // Start is called before the first frame update
        void Start()
        {
            VXRPassthrough.SetPassthroughBlendMode(isSelect_trigger ? VXRPassthrough.PassthroughBlendMode.AlphaBlend : VXRPassthrough.PassthroughBlendMode.Opaque);
            ChangeBlendModelBtn.transform.Find("Text").GetComponent<Text>().text = isSelect_trigger ? "VST Switch = On" : "VST Switch = Off";            
        }

        public void ButtonDown_Trigger()
        {         
            isSelect_trigger = !isSelect_trigger;
            ChangeBlendModelBtn.transform.Find("Text").GetComponent<Text>().text = isSelect_trigger ? "VST Switch = On" : "VST Switch = Off";
            VXRPassthrough.SetPassthroughEnable(isSelect_trigger);              
        }

        public void GetCurColorType()
        {                       
            string _javaClassName = "android.os.SystemProperties";
            AndroidJavaClass _javaClass = new AndroidJavaClass(_javaClassName);

            string contxt = _javaClass.CallStatic<string>("get", "persist.sxr.camname");
            Debug.Log("currColorType persist.sxr.camname: " + contxt);

        }

        public void QuitApplication()
        {
            VXRPassthrough.SetPassthroughEnable(false);            
        }       
}

}


