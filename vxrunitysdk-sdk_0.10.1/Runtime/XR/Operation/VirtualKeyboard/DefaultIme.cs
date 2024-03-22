using com.vivo.codelibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using System;

namespace com.vivo.openxr
{
    public class DefaultIme : ImeBase
    {
#if UNITY_EDITOR
        private bool _useAndroid = false;
#else
    private bool _useAndroid = true;
#endif        
        private ImeDelegateBase _imeViewDelegate;
        private Vector2 _textureSize;
        private bool _isShow = false;

        public bool Create(VXRVirtualKeyboard keyboard)
        {
            //if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            //{
            //    Permission.RequestUserPermission(Permission.Microphone);
            //    VLog.Info("ime Permission.Microphone got");
            //}
            //else
            //{
            //    VLog.Info("ime Permission.Microphone get fail");
            //}
            VLog.Info("ime create");            
            _imeViewDelegate = keyboard.GetComponentInChildren<ImeDelegateBase>();
            _imeViewDelegate.SetKeyVirtualBoard(keyboard);
            JavaInit();
            Hide();

            return true;
        }

        public Vector2 GetSize()
        {            
            int[] s = VXRPlugin.ImeGetSize();
            _textureSize[0] = s[0];
            _textureSize[1] = s[1];
            VLog.Info("ime getsize func : " + _textureSize[0] + "," + _textureSize[1]);
            return _textureSize;
        }

        public void Draw()
        {
            if (IsInited() && IsNeedUpdate())
            {
                byte[] textureData = GetTextureData();
                if (textureData == null)
                {
                    VLog.Info("ime GetTextureData() fail");
                    return;
                }
                GetSize();
                VLog.Info("ime Draw() w:" + (int)_textureSize.x + ", h" + (int)_textureSize.y);
                _imeViewDelegate.OnAdjustLayout(_textureSize, ref textureData);
            }
        }

        public byte[] GetTextureData()
        {
            if (IsInited() && IsNeedUpdate())
            {
                return VXRPlugin.ImeGetTextureData();
            }
            return null;
        }

        public void Show(VXRPlugin.ImeInputType typeInput, VXRPlugin.ImeTextType typeText)
        {
            if (IsInited())
            {
                VLog.Info("ime show:" + typeInput + "," + typeText);
                VXRPlugin.ImeShow(typeInput, typeText);
            }
        }

        public void Hide()
        {
            if (IsInited())
            {
                VLog.Info("ime hide");
                VXRPlugin.ImeHide();                
            }
        }

        public void OnTouch(float x, float y, VXRPlugin.ImeMotionEventType type)
        {
            if (IsInited())
            {
                if (type != VXRPlugin.ImeMotionEventType.ACTION_MOVE)
                    VLog.Info("ime ontouch:(" + x + "," + y + "),type=" + type);
                VXRPlugin.ImeOnTouch(x, y, type);
            }
        }

        public void UpdateData()
        {
            if (!IsInited())
            {
                return;
            }
            //update hide
            bool bShow = IsShow();
            bool bSurfaceShow = _imeViewDelegate.OnImeIsShow();
            
            if (!bShow && bSurfaceShow)
            {            
                _imeViewDelegate.OnIMEHide();
            }
            else if (bShow && !bSurfaceShow)
            {
                Draw();
                _imeViewDelegate.OnIMEShow(_textureSize);
            }

            //update scene
            int nScene = GetScene();
            VLog.Info("SGIme UpdateData --- scene:" + nScene);
            if (nScene != -1)
            {
                _imeViewDelegate.OnIMESetScene((VXRPlugin.ImeSceneType)nScene);
            }
        }

        public bool IsRecording()
        {
            return VXRPlugin.ImeIsRecording();
        }

        private void JavaInit()
        {
            VLog.Info("ime start JavaInit");
            if (_useAndroid)
            {
                VXRPlugin.Imeinit();
                int[] s = VXRPlugin.ImeGetSize();
                _textureSize[0] = s[0];
                _textureSize[1] = s[1];
                VLog.Info("ime start JavaInit, getKeyBoardSize size = " + _textureSize[0] + ":" + _textureSize[1]);
            }
        }

        private bool IsInited()
        {
            return VXRPlugin.ImeIsInited();
        }

        private bool IsNeedUpdate()
        {
            bool bNeedUpdate = VXRPlugin.ImeIsNeedUpdate();
            return bNeedUpdate;
        }

        public int GetCommitCode()
        {
            return VXRPlugin.ImeGetCommitCode();
        }

        public string GetCommitString()
        {
            string strCommit = VXRPlugin.ImeGetCommitString();
            return strCommit;
        }

        public bool IsShow()
        {
            return VXRPlugin.ImeIsShow();
        }

        private int GetScene()
        {
            return VXRPlugin.ImeGetScene();
        }

        public void SetTriggerStatus(int controllerFlag, bool isTrigger)
        {
            _imeViewDelegate.SetTriggerStatus(controllerFlag, isTrigger);
        }

        public void PlayBoom(int type)
        {
            _imeViewDelegate.PlayVibrator( Convert.ToString(type));
        }

        public void SetKeyboardVisible(bool isVisible)
        {
            if (_isShow != isVisible)
            {
                _isShow = isVisible;
                if (isVisible)
                {
                    Draw();
                    _imeViewDelegate.OnIMEShow(GetSize());
                }
                else
                {
                    _imeViewDelegate.OnIMEHide();
                }
            }
        }
    }
}
