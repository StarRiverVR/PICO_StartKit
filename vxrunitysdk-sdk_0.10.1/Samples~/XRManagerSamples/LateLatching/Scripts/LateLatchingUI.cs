using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LateLatchingUI : MonoBehaviour
{
    public Toggle OpenToggle;
    public Text OpenToggleText;

    public LateLatching LateLatching;

    void Update()
    {
        if (LateLatching.IsOpenLateLatching)
        {
            OpenToggleText.text = "On";
            OpenToggle.onValueChanged.RemoveAllListeners();
            OpenToggle.isOn = true;
            OpenToggle.onValueChanged.AddListener(OpenToggleChange);
        }
        else
        {
            OpenToggleText.text = "Off";
            OpenToggle.onValueChanged.RemoveAllListeners();
            OpenToggle.isOn = false;
            OpenToggle.onValueChanged.AddListener(OpenToggleChange);
        }
    }

    void OpenToggleChange(bool bl)
    {
        LateLatching.SetOpenLateLatching(bl);
    }

}
