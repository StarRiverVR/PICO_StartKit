
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameSystem
{
    /// <summary>
    /// UGUI事件拓展
    /// 不会取消原UGUI事件
    /// 需要 EventTrigger 脚本的支援
    /// 用法：EventTriggerHandler.onClick.AddListener()
    ///       EventTriggerHandler.onClick.RemoveListener()
    /// </summary>
    [RequireComponent(typeof(UnityEngine.EventSystems.EventTrigger))]//关联脚本
    public class EventTriggerHandler : MonoBehaviour
    {
        void Start()
        {
            EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
            //添加鼠标点击事件
            EventTrigger.Entry entryClick = new EventTrigger.Entry();
            entryClick.eventID = EventTriggerType.PointerClick;// 鼠标点击事件
            entryClick.callback = new EventTrigger.TriggerEvent();
            entryClick.callback.AddListener(OnClick);
            trigger.triggers.Add(entryClick);
            //添加鼠标进入事件
            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;// 鼠标进入事件
            entryEnter.callback = new EventTrigger.TriggerEvent();
            entryEnter.callback.AddListener(OnMouseEnter);
            trigger.triggers.Add(entryEnter);
            //鼠标滑出事件
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;// 鼠标滑出事件
            entryExit.callback = new EventTrigger.TriggerEvent();
            entryExit.callback.AddListener(OnMouseExit);
            trigger.triggers.Add(entryExit);
            //鼠标按下
            EventTrigger.Entry entryMouseDown = new EventTrigger.Entry();
            entryMouseDown.eventID = EventTriggerType.PointerDown;
            entryMouseDown.callback = new EventTrigger.TriggerEvent();
            entryMouseDown.callback.AddListener(OnMouseDown);
            trigger.triggers.Add(entryMouseDown);
            //鼠标抬起
            EventTrigger.Entry entryMouseUp = new EventTrigger.Entry();
            entryMouseUp.eventID = EventTriggerType.PointerUp;
            entryMouseUp.callback = new EventTrigger.TriggerEvent();
            entryMouseUp.callback.AddListener(OnMouseUp);
            trigger.triggers.Add(entryMouseUp);

            //开始拖拽事件 
            EventTrigger.Entry entryBeiginDrag = new EventTrigger.Entry();
            entryBeiginDrag.eventID = EventTriggerType.BeginDrag;
            entryBeiginDrag.callback = new EventTrigger.TriggerEvent();
            entryBeiginDrag.callback.AddListener(OnMouseBeiginDrag);
            trigger.triggers.Add(entryBeiginDrag);
            //正在拖拽
            EventTrigger.Entry entryDrag = new EventTrigger.Entry();
            entryDrag.eventID = EventTriggerType.Drag;
            entryDrag.callback = new EventTrigger.TriggerEvent();
            entryDrag.callback.AddListener(OnMouseDrag);
            trigger.triggers.Add(entryDrag);
            //结束拖拽
            EventTrigger.Entry entryEndDrag = new EventTrigger.Entry();
            entryEndDrag.eventID = EventTriggerType.EndDrag;
            entryEndDrag.callback = new EventTrigger.TriggerEvent();
            entryEndDrag.callback.AddListener(OnMouseEndDrag);
            trigger.triggers.Add(entryEndDrag);
        }

        public EventTriggerHandlerCallBack onLeftClick = new EventTriggerHandlerCallBack();
        public EventTriggerHandlerCallBack onLeftDoubleClick = new EventTriggerHandlerCallBack();

        public EventTriggerHandlerCallBack onRightClick = new EventTriggerHandlerCallBack();
        public EventTriggerHandlerCallBack onRightDoubleClick = new EventTriggerHandlerCallBack();
        private void OnClick(BaseEventData pointData)
        {
            PointerEventData p = (PointerEventData)pointData;
            if (p.clickCount == 2)
            {
                if (p.button == PointerEventData.InputButton.Left)
                {
                    CancelInvoke("ToLeftOneclick");
                    ToLeftDoubleclick();
                }
                else if (p.button == PointerEventData.InputButton.Right)
                {
                    CancelInvoke("ToRightOneclick");
                    ToRightDoubleclick();
                }
            }
            else if (p.clickCount == 1)
            {
                if (p.button == PointerEventData.InputButton.Left)
                {
                    Invoke("ToLeftOneclick", 0.3f);
                }
                else if (p.button == PointerEventData.InputButton.Right)
                {
                    Invoke("ToRightOneclick", 0.3f);
                }
            }
        }
        void ToLeftDoubleclick()
        {
            if (onLeftDoubleClick != null && onLeftDoubleClick.callBack != null)
            {
                onLeftDoubleClick.callBack(gameObject);
            }
        }
        void ToLeftOneclick()
        {
            if (onLeftClick != null && onLeftClick.callBack != null)
            {
                onLeftClick.callBack(gameObject);
            }
        }

        void ToRightDoubleclick()
        {
            if (onRightDoubleClick != null && onRightDoubleClick.callBack != null)
            {
                onRightDoubleClick.callBack(gameObject);
            }
        }
        void ToRightOneclick()
        {
            if (onRightClick != null && onRightClick.callBack != null)
            {
                onRightClick.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseEnter = new EventTriggerHandlerCallBack();
        private void OnMouseEnter(BaseEventData pointData)
        {
            if (onMouseEnter != null && onMouseEnter.callBack != null)
            {
                onMouseEnter.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseExit = new EventTriggerHandlerCallBack();
        private void OnMouseExit(BaseEventData pointData)
        {
            if (onMouseExit != null && onMouseExit.callBack != null)
            {
                onMouseExit.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseDown = new EventTriggerHandlerCallBack();
        private void OnMouseDown(BaseEventData pointData)
        {
            if (onMouseDown != null && onMouseDown.callBack != null)
            {
                onMouseDown.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseUp = new EventTriggerHandlerCallBack();
        private void OnMouseUp(BaseEventData pointData)
        {
            if (onMouseUp != null && onMouseUp.callBack != null)
            {
                onMouseUp.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseBeiginDrag = new EventTriggerHandlerCallBack();
        void OnMouseBeiginDrag(BaseEventData pointData)
        {
            if (onMouseBeiginDrag != null && onMouseBeiginDrag.callBack != null)
            {
                onMouseBeiginDrag.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseDrag = new EventTriggerHandlerCallBack();
        void OnMouseDrag(BaseEventData pointData)
        {
            if (onMouseDrag != null && onMouseDrag.callBack != null)
            {
                onMouseDrag.callBack(gameObject);
            }
        }

        public EventTriggerHandlerCallBack onMouseEndDrag = new EventTriggerHandlerCallBack();
        void OnMouseEndDrag(BaseEventData pointData)
        {
            if (onMouseEndDrag != null && onMouseEndDrag.callBack != null)
            {
                onMouseEndDrag.callBack(gameObject);
            }
        }

        private void OnDestroy()
        {
            onLeftClick.callBack = null;
            onMouseEnter.callBack = null;
            onMouseExit.callBack = null;
            onMouseBeiginDrag.callBack = null;
            onMouseDrag.callBack = null;
            onMouseEndDrag.callBack = null;
        }
    }

    public delegate void EventTriggerHandlerDel(GameObject obj);

    public class EventTriggerHandlerCallBack
    {
        public EventTriggerHandlerDel callBack;

        public void AddListener(EventTriggerHandlerDel fun)
        {
            callBack -= fun;
            callBack += fun;
        }

        public void RemoveListener(EventTriggerHandlerDel fun)
        {
            callBack -= fun;
        }
    }
}

