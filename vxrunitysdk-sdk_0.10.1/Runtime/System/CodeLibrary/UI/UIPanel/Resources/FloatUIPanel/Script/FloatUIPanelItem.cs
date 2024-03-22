using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace com.vivo.codelibrary
{
    public class FloatUIPanelItem : MonoBehaviour
    {
        public Camera mainCamera
        {
            get
            {
                return Camera.main;
            }
        }

        bool isPutBack = false;

        public bool IsPutBack
        {
            get
            {
                return isPutBack;
            }
            set
            {
                isPutBack = value;
                if (isPutBack)
                {
                    isStartAnim = false;
                    worldTarget = null;
                }
            }
        }

        public FloatUIPanel FloatUIPanel;

        public string ShowString;

        Transform worldTarget;

        /// <summary>
        /// 移动动画长度
        /// </summary>
        float animMoveTimeLenght = 2f;

        /// <summary>
        /// 透明度变化动画长度
        /// </summary>
        float alphaAnimTimelenght = 0.5f;

        public void Play(Vector3 _pos)
        {
            Init();
            worldTarget = null;
            _transform.position = _pos;
            FreshAlpha();
            StartAnim();
        }

        public void Play(Transform _worldTarget)
        {
            Init();
            worldTarget = _worldTarget;
            FollowTarget();
            StartAnim();
        }

        public void ResetUI()
        {
            Init();
            textItemBG.localPosition = Vector3.zero;
            textItemBG.localScale = Vector3.one;
            textItemBG.localEulerAngles = Vector3.zero;
            textItemBGCanvasGroup.alpha = 0;
            canvasGroup.alpha = 1;
        }

        private void Update()
        {
            if (IsPutBack || !isStartAnim) return;
            AnimCheck();
        }

        Transform textItemBG;

        Transform endPosition;

        Text strText;

        Transform _transform;

        CanvasGroup canvasGroup;

        CanvasGroup textItemBGCanvasGroup;

        bool isInit = false;

        void Init()
        {
            if (isInit) return;
            isInit = true;
            _transform = transform;
            textItemBG = _transform.Find("TextItemBG");
            textItemBGCanvasGroup = textItemBG.GetComponent<CanvasGroup>();
            if (textItemBGCanvasGroup == null)
            {
                textItemBGCanvasGroup = textItemBG.gameObject.AddComponent<CanvasGroup>();
            }
            textItemBGCanvasGroup.alpha = 1;
            endPosition = _transform.Find("EndPosition");
            strText = _transform.Find("TextItemBG/Text").GetComponent<Text>();
            strText.text = "";
            canvasGroup = _transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        bool isStartAnim = false;

        float startAnimDelayCheckWait = 0;

        void StartAnim()
        {
            //strText.text = ShowString;
            //textItemBGCanvasGroup.DOFade(1, alphaAnimTimelenght);
            //textItemBG.DOPunchScale(new Vector3(0.2f, 0.1f, 0f), 0.7f, 10, 1);
            //textItemBG.DOMoveY(endPosition.position.y, animMoveTimeLenght, true);
            //Tweener tweener = textItemBGCanvasGroup.DOFade(0, alphaAnimTimelenght);
            //tweener.SetDelay(animMoveTimeLenght - alphaAnimTimelenght);
            //startAnimDelayCheckWait = animMoveTimeLenght;
            //isStartAnim = true;
        }

        void AnimCheck()
        {
            if (startAnimDelayCheckWait > 0)
            {
                startAnimDelayCheckWait = startAnimDelayCheckWait - Time.deltaTime;
            }
            else
            {
                if (textItemBGCanvasGroup.alpha == 0)
                {
                    FloatUIPanel.PutBackOneItem(this);
                }
            }
            FollowTarget();
        }

        void FollowTarget()
        {
            if (worldTarget != null)
            {
                _transform.position = mainCamera.WorldToScreenPoint(worldTarget.position);
                if (_transform.position.z >= 0)
                {
                    canvasGroup.alpha = 1;
                    FreshAlpha();
                }
                else
                {
                    canvasGroup.alpha = 0;
                }
            }
        }

        /// <summary>
        /// 刷新可见度 位于面板范围之外不进行显示
        /// </summary>
        void FreshAlpha()
        {

        }
    }
}


