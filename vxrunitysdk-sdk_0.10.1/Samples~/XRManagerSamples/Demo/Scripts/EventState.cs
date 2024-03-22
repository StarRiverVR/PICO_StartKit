using System;
using System.Collections;
using System.Collections.Generic;
using com.vivo.codelibrary;
using UnityEngine;

public class EventState
{
    private static EventState _instance;
    public static EventState Instance
    {
        get
        {
            if (_instance==null)
            {
                _instance = new EventState();
                _instance._logs = new List<string>();
            }

            return _instance;
        }
    }

    public bool OpenXRState { get; set; }

    public bool OpenXRFocused { get; set; }


    private List<string> _logs;

    public List<string> Logs
    {
        get
        {
            return _logs;
        }
    }

    public Action<List<string>> OnLogsChanged;
    

    private int _logCount = 10;
    public void AddLog(string log)
    {
        _logs.Add(log);
        while (_logs.Count>_logCount)
        {
            _logs.RemoveAt(0);
        }
        OnLogsChanged?.Invoke(_logs);
      
    }
}
