using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 飘字管理界面
    /// </summary>
    public class FloatUIPanel : UIPanelBase
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
        /// 显示飘字
        /// </summary>
        /// <param name="_str">飘字内容</param>
        /// <param name="_pos">飘字出生位置 世界坐标</param>
        public void ShowTip(string _str, Vector3 _pos)
        {
            FloatUIPanelItem item = GetOneItem(_str);
            item.Play(_pos);
        }

        /// <summary>
        /// 显示飘字
        /// </summary>
        /// <param name="_str">飘字内容</param>
        /// <param name="_posMod">飘字出生屏幕位置 1：上 2：下 3：左 4：右 5:中</param>
        public void ShowTip(string _str, int _posMod)
        {
            Vector3 pos = Vector3.zero;
            switch (_posMod)
            {
                case 1:
                    {
                        pos = showPointUp.position;
                    }
                    break;
                case 2:
                    {
                        pos = showPointDown.position;
                    }
                    break;
                case 3:
                    {
                        pos = showPointLeft.position;
                    }
                    break;
                case 4:
                    {
                        pos = showPointRight.position;
                    }
                    break;
                case 5:
                    {
                        pos = showPointMid.position;
                    }
                    break;
            }
            ShowTip(_str, pos);
        }

        /// <summary>
        /// 显示飘字
        /// </summary>
        /// <param name="_str">飘字内容</param>
        /// <param name="_worldTarget">飘字跟随的世界物体目标</param>
        public void ShowTip(string _str, Transform _worldTarget)
        {
            FloatUIPanelItem item = GetOneItem(_str);
            item.Play(_worldTarget);
        }

        public Transform LeftPoint;

        public Transform RightPoint;

        GameObject item;

        Transform itemRoot;

        Transform showPointUp;

        Transform showPointDown;

        Transform showPointLeft;

        Transform showPointRight;

        Transform showPointMid;

        Transform _transform;

        bool isInit = false;

        void Init()
        {
            if (isInit) return;
            isInit = true;
            _transform = transform;
            itemRoot = _transform;
            item = _transform.Find("Item").gameObject;
            item.SetActive(false);
            showPointUp = _transform.Find("ShowPointUp");
            showPointDown = _transform.Find("ShowPointDown");
            showPointLeft = _transform.Find("ShowPointLeft");
            showPointRight = _transform.Find("ShowPointRight");
            showPointMid = _transform.Find("ShowPointMid");
            LeftPoint = _transform.Find("LeftPoint");
            RightPoint = _transform.Find("RightPoint");
        }

        #region//池子 FloatUIPanelItem

        Queue<FloatUIPanelItem> pool = new Queue<FloatUIPanelItem>();

        FloatUIPanelItem GetOneItem(string _str)
        {
            if (pool.Count > 0)
            {
                FloatUIPanelItem findData = pool.Dequeue();
                while (findData == null && pool.Count > 0)
                {
                    findData = pool.Dequeue();
                }
                if (findData != null)
                {
                    findData.ShowString = _str;
                    findData.IsPutBack = false;
                    findData.ResetUI();
                    findData.gameObject.SetActive(true);
                    return findData;
                }
            }
            GameObject obj = Instantiate(item);
            obj.transform.SetParent(itemRoot, false);
            FloatUIPanelItem newData = obj.GetComponent<FloatUIPanelItem>();
            newData.FloatUIPanel = this;
            newData.ShowString = _str;
            newData.IsPutBack = false;
            newData.ResetUI();
            obj.SetActive(true);
            return newData;
        }

        public void PutBackOneItem(FloatUIPanelItem _item)
        {
            if (_item.IsPutBack) return;
            _item.IsPutBack = true;
            _item.gameObject.SetActive(false);
            pool.Enqueue(_item);
        }

        #endregion
    }
}


