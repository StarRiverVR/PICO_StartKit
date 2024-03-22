using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.vivo.openxr;
using UnityEngine;
using UnityEngine.UI;

public class VirtualKeyboardEventLog : MonoBehaviour
{
    
    [SerializeField]private Text _text;
    [SerializeField]private VXRVirtualKeyboard _virtualKeyboard;
    private List<string> _logList;
    private StringBuilder currentString;
    private int _logMaxCount = 15;
    // Start is called before the first frame update
    void Start()
    {
        _logList = new List<string>();
        currentString = new StringBuilder();
        _virtualKeyboard.AddShowListener(ShowKeyboard);
        _virtualKeyboard.AddHideListener(HideKeyboard);
        _virtualKeyboard.AddCommitTextListener(CommitText);
        _virtualKeyboard.AddEnterListener(OnEnter);
        _virtualKeyboard.AddBackSpaceListener(OnBackSpace);
        _virtualKeyboard.AddRecordStartListener(OnRecordStart);
        _virtualKeyboard.AddRecordEndListener(OnRecordEnd);
    }
    
    private void ShowKeyboard()
    {
        AddLog("Show VirtualKeyboard");
    }
    
    private void HideKeyboard()
    {
        AddLog("Hide VirtualKeyboard");
    }

    private void CommitText(string info)
    {
        AddLog("CommitText : "+info);
    }

    public void OnEnter()
    {
        AddLog("Press enter");
    }
    
    public void OnBackSpace()
    {
        AddLog("Press backSpace");
    }

    public void OnRecordStart()
    {
        AddLog("Record voice start");
    }
    
    public void OnRecordEnd()
    {
        AddLog("Record Voice End");
    }

    private void AddLog(string info)
    {
        _logList.Add(info);
        if (_logList.Count>_logMaxCount)
        {
            _logList.RemoveAt(0);
        }

        currentString.Clear();
        foreach (var log in _logList)
        {
            currentString.Append(log + "\n");
        }

        if (_text)
        {
            _text.text = currentString.ToString();
        }
    }
}
