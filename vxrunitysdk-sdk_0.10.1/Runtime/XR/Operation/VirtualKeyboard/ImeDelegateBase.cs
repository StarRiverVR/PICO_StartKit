using UnityEngine;
using System;

namespace com.vivo.openxr
{
    public abstract class ImeDelegateBase : MonoBehaviour
    {
        public abstract void OnIMEShow(Vector2 vSize);
        public abstract void OnIMEHide();
        public abstract void OnIMECommit(string strCommit);
        public abstract void OnIMEKey(VXRPlugin.ImeKey key);
        public abstract void OnIMEError(VXRPlugin.ImeError nType, string strErr);
        public abstract void OnAdjustLayout(Vector2 vSize, ref byte[] textureData);
        public abstract bool OnImeIsShow();
        public abstract void OnCommand(VXRPlugin.ImeDeviceCommandType command);
        public abstract Transform GetTransform();
        public abstract void OnIMESetScene(VXRPlugin.ImeSceneType scene);
        public abstract void SetTriggerStatus(int controllerFlag, bool isTrigger);
        public abstract void SetKeyVirtualBoard(VXRVirtualKeyboard keyboard);
        public abstract void PlayVibrator(string type);
    }

}
