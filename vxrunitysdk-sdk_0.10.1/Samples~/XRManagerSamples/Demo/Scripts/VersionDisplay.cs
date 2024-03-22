
using com.vivo.codelibrary;
using UnityEngine;
using UnityEngine.UI;
using com.vivo.openxr;
public class VersionDisplay : MonoBehaviour
{

    [SerializeField]private Text _uiTextVersionInfo;
    // Start is called before the first frame update
    void Start()
    { 
        _uiTextVersionInfo.text = $" SDK Version: {VXRSystem.GetSDKVersion()}";
        VLog.Info( $"SDK Version: {VXRSystem.GetSDKVersion()}");
    }
    
}
