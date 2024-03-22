using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using com.vivo.openxr;

public class MSAATestUI : MonoBehaviour
{
    public MSAATest MSAATest;

    public Toggle Toggle_1;
    public Toggle Toggle_2;
    public Toggle Toggle_4;
    public Toggle Toggle_8;

    void Update()
    {
        switch (VXRCommon.MsaaSample)
        {
            case UnityEngine.Rendering.MSAASamples.None:
                {
                    Toggle_1.onValueChanged.RemoveAllListeners();
                    Toggle_2.onValueChanged.RemoveAllListeners();
                    Toggle_4.onValueChanged.RemoveAllListeners();
                    Toggle_8.onValueChanged.RemoveAllListeners();

                    Toggle_1.isOn = true;
                    Toggle_2.isOn = false;
                    Toggle_4.isOn = false;
                    Toggle_8.isOn = false;

                    Toggle_1.onValueChanged.AddListener(Toggle_1Change);
                    Toggle_2.onValueChanged.AddListener(Toggle_2Change);
                    Toggle_4.onValueChanged.AddListener(Toggle_4Change);
                    Toggle_8.onValueChanged.AddListener(Toggle_8Change);
                }
                break;
            case UnityEngine.Rendering.MSAASamples.MSAA2x:
                {
                    Toggle_1.onValueChanged.RemoveAllListeners();
                    Toggle_2.onValueChanged.RemoveAllListeners();
                    Toggle_4.onValueChanged.RemoveAllListeners();
                    Toggle_8.onValueChanged.RemoveAllListeners();

                    Toggle_1.isOn = false;
                    Toggle_2.isOn = true;
                    Toggle_4.isOn = false;
                    Toggle_8.isOn = false;

                    Toggle_1.onValueChanged.AddListener(Toggle_1Change);
                    Toggle_2.onValueChanged.AddListener(Toggle_2Change);
                    Toggle_4.onValueChanged.AddListener(Toggle_4Change);
                    Toggle_8.onValueChanged.AddListener(Toggle_8Change);
                }
                break;
            case UnityEngine.Rendering.MSAASamples.MSAA4x:
                {
                    Toggle_1.onValueChanged.RemoveAllListeners();
                    Toggle_2.onValueChanged.RemoveAllListeners();
                    Toggle_4.onValueChanged.RemoveAllListeners();
                    Toggle_8.onValueChanged.RemoveAllListeners();

                    Toggle_1.isOn = false;
                    Toggle_2.isOn = false;
                    Toggle_4.isOn = true;
                    Toggle_8.isOn = false;

                    Toggle_1.onValueChanged.AddListener(Toggle_1Change);
                    Toggle_2.onValueChanged.AddListener(Toggle_2Change);
                    Toggle_4.onValueChanged.AddListener(Toggle_4Change);
                    Toggle_8.onValueChanged.AddListener(Toggle_8Change);
                }
                break;
            case UnityEngine.Rendering.MSAASamples.MSAA8x:
                {
                    Toggle_1.onValueChanged.RemoveAllListeners();
                    Toggle_2.onValueChanged.RemoveAllListeners();
                    Toggle_4.onValueChanged.RemoveAllListeners();
                    Toggle_8.onValueChanged.RemoveAllListeners();

                    Toggle_1.isOn = false;
                    Toggle_2.isOn = false;
                    Toggle_4.isOn = false;
                    Toggle_8.isOn = true;

                    Toggle_1.onValueChanged.AddListener(Toggle_1Change);
                    Toggle_2.onValueChanged.AddListener(Toggle_2Change);
                    Toggle_4.onValueChanged.AddListener(Toggle_4Change);
                    Toggle_8.onValueChanged.AddListener(Toggle_8Change);
                }
                break;
        }
    }

    void Toggle_1Change(bool bl)
    {
        MSAATest.SetMsaaSample(UnityEngine.Rendering.MSAASamples.None);
    }

    void Toggle_2Change(bool bl)
    {
        MSAATest.SetMsaaSample(UnityEngine.Rendering.MSAASamples.MSAA2x);
    }

    void Toggle_4Change(bool bl)
    {
        MSAATest.SetMsaaSample(UnityEngine.Rendering.MSAASamples.MSAA4x);
    }

    void Toggle_8Change(bool bl)
    {
        MSAATest.SetMsaaSample(UnityEngine.Rendering.MSAASamples.MSAA8x);
    }

}
