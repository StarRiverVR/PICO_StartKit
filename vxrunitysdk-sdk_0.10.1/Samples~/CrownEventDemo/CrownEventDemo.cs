using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace com.vivo.openxr 
{
    public class CrownEventDemo : MonoBehaviour
    {
        public GameObject RotateObj;
        public Button BtnVST;
        public Text TextBtnVST;
        public Transform SKYBarHead;
        public Material MatSkyBox;
        private bool _isOpenVST = false;
        private float prossMax = 283;
        private float skyAlpha = 0;

        int totalAngle = 0;
        public Text textAngle;
        void Start()
        {
            TextBtnVST.text =  "开启VST";
            BtnVST.onClick.AddListener(onClickVST);
            textAngle.text = $"累计旋转角度：{totalAngle}°\r\n单次旋转角度：0°";

        }
        private void OnEnable()
        {
            VXREvent.AddCrownRotate360Listener(onCrownRotate360Event);
            VXREvent.AddCrownRotateListener(onCrownRotateEvent);
        }
        private void OnDisable()
        {
            VXREvent.RemoveCrownRotate360Listener(onCrownRotate360Event);
            VXREvent.RemoveCrownRotateListener(onCrownRotateEvent);
        }

        private void OnDestroy()
        {
            VXRPassthrough.SetPassthroughEnable(false);
        }

        public void onCrownRotate360Event(VXREvent.CrownRotate360Data rotateData)
        {
            Debug.Log("表冠旋转返回旋转值：" + rotateData.RotateAngle  + "  最大值：" + rotateData.MaxAngle);
            // 天空盒透明度
            setSkyAlpha(Math.Abs((float)rotateData.RotateAngle / rotateData.MaxAngle));
            // 进度条
            float x = (float)rotateData.RotateAngle / rotateData.MaxAngle * prossMax;
            SKYBarHead.localPosition = new Vector3(x, SKYBarHead.localPosition.y, SKYBarHead.localPosition.z);
        }

        void onCrownRotateEvent(VXREvent.CrownRotateData data)
        {
            // 累计旋转角度
            totalAngle += data.RotateDelta;
            textAngle.text = $"累计旋转角度：{totalAngle}°\r\n单次旋转角度：{data.RotateDelta}°";
            // 旋转旋钮
            RotateObj.transform.Rotate(Vector3.forward * -data.RotateDelta);
        }

        public void onClickVST()
        {
            _isOpenVST = !_isOpenVST;
            TextBtnVST.text = _isOpenVST ? "关闭VST" : "开启VST";
            VXRPassthrough.SetPassthroughEnable(_isOpenVST);
            //同步天空盒透明度
            setSkyAlpha(skyAlpha);
        }
        void setSkyAlpha(float alpha)
        {
            skyAlpha = alpha;
            if (_isOpenVST)
            {
                MatSkyBox.SetFloat("_Alpha", alpha);
            }
            else
            {
                MatSkyBox.SetFloat("_Alpha", 1);
            }
        }

    }

}
