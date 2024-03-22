using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace com.vivo.codelibrary
{

    /// <summary>
    /// 通用加载界面
    /// </summary>
    public class LoadingPanel : UIPanelBase
    {

        private void Awake()
        {
            Init();
        }

        public bool IsShow = false;

        bool isShowing = false;

        float showingWaitTime = 0;

        float alphaTime = 0;

        public override void Show()
        {
            if (IsShow)
            {
                if (!isHiding)
                {
                    return;
                }
            }
            if (isShowing) return;
            Init();
            base.Show();
            isShowing = true;
            isHiding = false;
            IsShow = true;
            if (canvasGroup.alpha == 0)
            {
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
                showingWaitTime = 2f;
            }
            alphaTime = 0;
            SupperShow();
        }

        public void SupperShow()
        {
            _transform.SetSiblingIndex(10000);
        }

        bool isHiding = false;

        public override void Hide()
        {
            if (isHiding) return;
            Init();
            base.Hide();
            isShowing = false;
            isHiding = true;
            alphaTime = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        private void Update()
        {
            CheckAnim();
        }

        Image rotateOutImage;

        Image rotateInImage;

        Image iconImage;

        Image loadingImage;

        Transform _transform;

        bool isInit = false;

        void Init()
        {
            if (isInit) return;
            isInit = true;
            _transform = transform;
            rotateOutImage = _transform.Find("BG/RotateOutImage").GetComponent<Image>();
            rotateInImage = _transform.Find("BG/RotateInImage").GetComponent<Image>();
            iconImage = _transform.Find("BG/IconImage").GetComponent<Image>();
            loadingImage = _transform.Find("BG/LoadingImage").GetComponent<Image>();
        }

        void CheckAnim()
        {
            if (isShowing)
            {
                if (showingWaitTime > 0)
                {
                    showingWaitTime = showingWaitTime - Time.deltaTime;
                }
                else
                {
                    alphaTime = alphaTime + Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, alphaTime);
                    if (canvasGroup.alpha > 0.01f)
                    {
                        canvasGroup.blocksRaycasts = true;
                        canvasGroup.interactable = true;
                    }
                    if (alphaTime >= 1)
                    {
                        canvasGroup.alpha = 1;
                        isShowing = false;
                    }
                }
            }
            if (isHiding)
            {
                alphaTime = alphaTime + Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, alphaTime);
                if (alphaTime >= 1)
                {
                    canvasGroup.alpha = 0;
                    isHiding = false;
                    IsShow = false;
                }
            }
            if (IsShow)
            {
                rotateOutImage.transform.localEulerAngles = rotateOutImage.transform.localEulerAngles - new Vector3(0, 0, 100 * Time.deltaTime);
                rotateInImage.transform.localEulerAngles = rotateInImage.transform.localEulerAngles + new Vector3(0, 0, 100 * Time.deltaTime);
                SetIconTween();
                SetLoadingTween();
            }
        }

        float lastSetIconTweenTime = 0;

        /// <summary>
        /// 设置ICON动画
        /// </summary>
        void SetIconTween()
        {
            if ((Time.time - lastSetIconTweenTime) < 1f)
            {
                return;
            }
            lastSetIconTweenTime = Time.time;
           // Tweener tweener = iconImage.transform.DOPunchScale(new Vector3(0.3f, 0.3f, 0.3f), 0.7f, 5, 1);
        }

        float lastSetLoadingTween = 0;

        /// <summary>
        /// 设置loading动画
        /// </summary>
        void SetLoadingTween()
        {
            if ((Time.time - lastSetLoadingTween) < 1f)
            {
                return;
            }
            lastSetLoadingTween = Time.time;
            //Tweener tweener = loadingImage.transform.DOPunchScale(new Vector3(-0.3f, -0.3f, -0.3f), 0.7f, 5, 1);
        }
    }
}

