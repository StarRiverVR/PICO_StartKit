
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;
using com.vivo.codelibrary;
using UnityEngine;
using com.vivo.openxr;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EventSampleDemo : MonoBehaviour
{
    
    [SerializeField]private Text _textOpenXRState;
    [SerializeField]private Text _textOpenFocusState;
    [SerializeField]private Text _textApplicationFocusState;
    [SerializeField]private Text _textRefreshState;
    [SerializeField]private Text _textLogs;
    
    
    private void OnEnable()
    {
       
        VXREvent.AddOpenXREnteringListener(OnOpenXREntering);
        VXREvent.AddOpenXRExitingListener(OnOpenXRExiting);
        VXREvent.AddOpenXRFocusChangedListener(OnOpenXRFocusChanged);      
    }

    private void OnDisable()
    {
        VXREvent.RemoveOpenXREnteringListener(OnOpenXREntering);
        VXREvent.RemoveOpenXRExitingListener(OnOpenXRExiting);
        VXREvent.RemoveOpenXRFocusChangedListener(OnOpenXRFocusChanged);        
    }

    void Start()
    {
        _textOpenXRState.text = EventState.Instance.OpenXRState.ToString();
        _textOpenFocusState.text = EventState.Instance.OpenXRFocused.ToString();
        _textApplicationFocusState.text = Application.isFocused.ToString();
        
        ShowLogs(EventState.Instance.Logs);        
        VXRRefreshRate.AddChangeListener(OnRefreshRateChanged);
        
        _textRefreshState.text =  VXRRefreshRate.GetRefreshRate().ToString(CultureInfo.InvariantCulture);

        Application.focusChanged += OnFocusChanged;
        Application.quitting += OnQuiting;
        EventState.Instance.OnLogsChanged += ShowLogs;

    }


    private void OnOpenXREntering()
    {
        EventState.Instance.OpenXRState = true;
        
        EventState.Instance.AddLog("进入OpenXR");
        VLog.Info($"进入OpenXR");
        _textOpenXRState.text = "开启";
        
    }

    private void OnOpenXRExiting()
    {
        EventState.Instance.OpenXRState = false;
        EventState.Instance.AddLog("退出OpenXR");
        VLog.Info($"退出OpenXR");
        _textOpenXRState.text = "关闭";
    }

    private void OnOpenXRFocusChanged(bool state)
    {
        EventState.Instance.OpenXRFocused = state;
        EventState.Instance.AddLog($"OpenXR 焦点{state}");
        _textOpenFocusState.text = state.ToString();
        VLog.Info($"OpenXR 焦点{state}");
         
    }

    private void OnRefreshRateChanged(float fromeRate,float toRate)
    {
        _textRefreshState.text = $"上一次{fromeRate},当前{toRate}";
        EventState.Instance.AddLog($"刷新率变化： 上一次{fromeRate},当前{toRate}");
        VLog.Info($"OpenXR刷新率从{fromeRate}变化到{toRate}");
    }

    private void OnFocusChanged(bool state)
    {
        _textApplicationFocusState.text=state==true?"聚焦":"离开";
    }

    private void OnQuiting()
    {
       VLog.Info("退出应用");
    }

    private void ShowLogs(List<string> logs)
    {
        StringBuilder info = new StringBuilder();
        for (int i = 0; i < logs.Count; i++)
        {
            info.Append(logs[i]);
            info.Append("\n");
        }

        _textLogs.text = info.ToString();
    }

    private void OnDestroy()
    {
        VXRRefreshRate.RemoveChangeListener(OnRefreshRateChanged);
    }
}
