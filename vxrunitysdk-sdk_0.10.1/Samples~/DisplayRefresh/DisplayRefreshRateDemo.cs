using com.vivo.codelibrary;
using com.vivo.openxr;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayRefreshRateDemo : MonoBehaviour
{
    [SerializeField]
    Dropdown dd;
    void Start()
    {
        Debug.Log("刷新率 开始");
        float frequency = VXRRefreshRate.GetRefreshRate();
        float[] frequencies = VXRRefreshRate.GetRefreshRates();
     
        dd.options.Clear();
        Debug.Log($"刷新率 数组[{frequencies.Length}] = {frequencies.Length}");
        int select = -1;
        for (int i = 0; i < frequencies.Length; i++)
        {
            if (frequency == frequencies[i])
            {
                select = i;
            }
            Debug.Log($"刷新率 数组[{i}] = {frequencies[i]}");
            dd.options.Add(new Dropdown.OptionData(frequencies[i].ToString()));
        }
        if(select > 0)
        {
            dd.value = select;
        }
        dd.RefreshShownValue();
           
        dd.onValueChanged.AddListener(OnDropdownChange);

        Debug.Log($"刷新率 VXRCommon {VXRRefreshRate.GetRefreshRate()}");
        
    }
    void OnDropdownChange(int value)
    {
        VXRRefreshRate.SetRefreshRate(dd.options[value].text.ToFloat());
    }
}