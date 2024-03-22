using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace com.vivo.codelibrary
{
    public class DialogPanel : UIPanelBase
    {
        public override void Show()
        {
            Init();
            base.Show();
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            canvasGroup.alpha = 1;
        }

        public override void Hide()
        {
            Init();
            base.Hide();
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="_str">内容</param>
        /// <param name="yesButtonName">确定按钮明</param>
        /// <param name="_yesAction">确定按钮点击回调</param>
        /// <param name="_noButtonName">取消按钮明</param>
        /// <param name="_noAction">取消按钮点击回调</param>
        public void Dialog(string _str, string _yesButtonName = "确定", System.Action _yesAction = null,
            string _noButtonName = "取消", System.Action _noAction = null)
        {
            strText.text = _str;
            yesButtonText.text = _yesButtonName;
            if (_noButtonName == null)
            {
                noButton.gameObject.SetActive(false);
            }
            else
            {
                noButtonText.text = _noButtonName;
                noButton.gameObject.SetActive(true);
            }
            yesAction = _yesAction;
            noAction = _noAction;
        }

        Text strText;

        Button yesButton;

        Text yesButtonText;

        Button noButton;

        Text noButtonText;

        Transform _transform;

        bool isInit = false;

        void Init()
        {
            if (isInit) return;
            isInit = true;
            _transform = transform;
            strText = _transform.Find("BG/Text").GetComponent<Text>();
            strText.text = "";
            yesButton = _transform.Find("BG/ButtonBG/YesButton").GetComponent<Button>();
            yesButton.onClick.AddListener(YesClick);
            yesButtonText = yesButton.transform.Find("Text").GetComponent<Text>();
            noButton = _transform.Find("BG/ButtonBG/NoButton").GetComponent<Button>();
            noButton.onClick.AddListener(NoClick);
            noButtonText = noButton.transform.Find("Text").GetComponent<Text>();
        }

        System.Action yesAction;

        void YesClick()
        {
            if (yesAction != null)
            {
                yesAction();
            }
            yesAction = null;
            Hide();
        }

        System.Action noAction;

        void NoClick()
        {
            if (noAction != null)
            {
                noAction();
            }
            noAction = null;
            Hide();
        }
    }
}


