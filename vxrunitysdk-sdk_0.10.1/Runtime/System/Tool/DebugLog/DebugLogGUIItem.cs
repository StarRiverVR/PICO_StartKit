using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace com.vivo.codelibrary
{
    public class DebugLogGUIItem : MonoBehaviour
    {

        Button btn;

        InputField inputField;

        string text;

        Color textColor;

        System.Action<string, Color> callBack;

        public void Show(string str,System.Action<string, Color> callBack,Color textColor)
        {
            Init();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(BtnClick);
            this.callBack = callBack;
            this.textColor = textColor;
            this.text = str;
            int index = str.IndexOf("\n");
            if (index>0)
            {
                inputField.text = text.Substring(0, index);
            }
            else
            {
                inputField.text = text;
            }
            inputField.textComponent.color = textColor;
        }

        bool isInit = false;

        void Init()
        {
            if (isInit) return;
            isInit = true;
            btn = transform.GetComponent<Button>();
            inputField = transform.Find("InputField").GetComponent<InputField>();
        }

        void BtnClick()
        {
            if (callBack!=null)
            {
                callBack(text, textColor);
            }
        }

    }
}

