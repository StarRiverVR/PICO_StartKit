using UnityEngine;

namespace com.vivo.openxr
{
    public interface ImeBase
    {
        bool Create(VXRVirtualKeyboard keyBoard);
        Vector2 GetSize();
        void Draw();
        void OnTouch(float x, float y, VXRPlugin.ImeMotionEventType type);
        void UpdateData();
        byte[] GetTextureData();
        void Show(VXRPlugin.ImeInputType typeInput, VXRPlugin.ImeTextType typeText);
        void Hide();
        bool IsRecording();
        string GetCommitString();
        int GetCommitCode();
        void SetTriggerStatus(int controllerFlag, bool isTrigger);
        void PlayBoom(int type);
        bool IsShow();
        void SetKeyboardVisible(bool isVisible);
    }

}
