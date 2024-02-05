using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightBand;
using TMPro;

public class MessagePanel : MonoBehaviour
{
    public TextMeshProUGUI messagePanel_Txt;
    private int MessageCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        MessageCenter.Register(MessageTypes.ShowMessage, (x)=> {
            this.MessageCount++;
            this.messagePanel_Txt.text = x + "\n" + messagePanel_Txt.text;
            if (MessageCount > 20) {
                this.messagePanel_Txt.text = GetFirstFiveLines(this.messagePanel_Txt.text);
            }
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static string GetFirstFiveLines(string input)
    {
        // 将输入字符串分割为行
        string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

        // 获取最多前5行
        int numberOfLines = Math.Min(5, lines.Length);

        // 将这些行再组合成一个字符串
        string result = string.Join("\n", lines, 0, numberOfLines);

        return result;
    }
}
