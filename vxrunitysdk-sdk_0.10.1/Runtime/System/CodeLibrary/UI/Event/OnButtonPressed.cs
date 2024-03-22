﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 脚本位置：UGUI按钮组件身上
    /// 脚本功能：实现按钮长按状态的判断
    /// 创建时间：2015年11月17日
    /// </summary>

    // 继承：按下，抬起和离开的三个接口
    public class OnButtonPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public OnPointerDownDel OnPointerDownEvent;//按下事件
        public OnPointerUpDel OnPointerUpEvent;//抬起事件
        public OnPointerExitDel OnPointerExitEvent;//离开事件

        void Awake()
        {

        }

        private void OnDestroy()
        {

        }

        // 延迟时间
        private float delay = 0.2f;

        // 按钮是否是按下状态
        private bool isDown = false;

        // 按钮最后一次是被按住状态时候的时间
        private float lastIsDownTime;

        void Update()
        {
            // 如果按钮是被按下状态
            if (isDown)
            {
                // 当前时间 -  按钮最后一次被按下的时间 > 延迟时间0.2秒
                if (Time.time - lastIsDownTime > delay)
                {
                    // 触发长按方法
                    Debug.Log("长按");
                    // 记录按钮最后一次被按下的时间
                    lastIsDownTime = Time.time;

                }
            }

        }

        // 当按钮被按下后系统自动调用此方法
        public void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;
            lastIsDownTime = Time.time;
            if (OnPointerDownEvent != null)
            {
                OnPointerDownEvent();
            }
        }

        // 当按钮抬起的时候自动调用此方法
        public void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
            if (OnPointerUpEvent != null)
            {
                OnPointerUpEvent();
            }
        }

        // 当鼠标从按钮上离开的时候自动调用此方法
        public void OnPointerExit(PointerEventData eventData)
        {
            isDown = false;
            if (OnPointerExitEvent != null)
            {
                OnPointerExitEvent();
            }
        }
    }

    public delegate void OnPointerDownDel();//按下事件
    public delegate void OnPointerUpDel();//抬起事件
    public delegate void OnPointerExitDel();//离开事件
}

