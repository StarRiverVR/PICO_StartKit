using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.vivo.openxr
{
    public class VirtualKeyboardDemo : MonoBehaviour
    {

        public VXRVirtualKeyboard keyBoard;

        public Button BtnShowKeyboard;
        public Button BtnHideKeyboard;
        public Text TextCommit;
        public Button BtnNear;
        public Button BtnFar;

        void Start()
        {
            BtnShowKeyboard.onClick.AddListener(() =>
            {                
                keyBoard.ShowKeyBoard();

            });

            BtnHideKeyboard.onClick.AddListener(() => {                
                keyBoard.HideKeyBoard();
            });

            BtnNear.onClick.AddListener(()=> {                
                keyBoard.UseRecommendLocation(VXRVirtualKeyboard.KeyboardLocationType.Near);
            });

            BtnFar.onClick.AddListener(()=> {                
                keyBoard.UseRecommendLocation(VXRVirtualKeyboard.KeyboardLocationType.Far);
            });
        }

        private void OnEnable()
        {
            keyBoard.AddCommitTextListener(KeyBoardCommitText);
            keyBoard.AddBackSpaceListener(KeyboardBackSpace);
            keyBoard.AddEnterListener(KeyboardEnter);
        }

        private void OnDisable()
        {
            keyBoard.RemoveCommitTextListener(KeyBoardCommitText);
            keyBoard.RemoveBackSpaceListener(KeyboardBackSpace);
            keyBoard.RemoveEnterListener(KeyboardEnter);
        }

        private void KeyBoardCommitText(string text)
        {
            TextCommit.text = TextCommit.text + text;
        }

        private void KeyboardBackSpace()
        {
            string str = TextCommit.text;
            str = str.Remove(str.Length - 1);
            TextCommit.text = str;
        }

        private void KeyboardEnter()
        {
            string str = TextCommit.text;
            str += "\n";
            TextCommit.text = str;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
