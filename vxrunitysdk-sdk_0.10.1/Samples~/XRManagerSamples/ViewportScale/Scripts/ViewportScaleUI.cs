using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.vivo.openxr;

public class ViewportScaleUI : MonoBehaviour
{

    public ViewportScale ViewportScale;

    public Toggle Toggle_1;

    public Text Toggle_1Text;

    public Toggle Toggle_0_8;

    public Text Toggle_0_8Text;

    private void Update()
    {
        if (ViewportScale.index==1)
        {
            Toggle_1.onValueChanged.RemoveAllListeners();
            Toggle_1.isOn = true;
            Toggle_1.onValueChanged.AddListener(Toggle_1Change);
            Toggle_1Text.text = VXRCommon.RenderViewportScale + "";

            Toggle_0_8.onValueChanged.RemoveAllListeners();
            Toggle_0_8.isOn = false;
            Toggle_0_8.onValueChanged.AddListener(Toggle_0_8Change);
        }
        else
        {
            Toggle_1.onValueChanged.RemoveAllListeners();
            Toggle_1.isOn = false;
            Toggle_1.onValueChanged.AddListener(Toggle_1Change);

            Toggle_0_8.onValueChanged.RemoveAllListeners();
            Toggle_0_8.isOn = true;
            Toggle_0_8.onValueChanged.AddListener(Toggle_0_8Change);
            Toggle_0_8Text.text = VXRCommon.RenderViewportScale + "";
        }
    }

    void Toggle_1Change(bool bl)
    {
        ViewportScale.SetIndex(0);
    }

    void Toggle_0_8Change(bool bl)
    {
        ViewportScale.SetIndex(1);
    }

}
