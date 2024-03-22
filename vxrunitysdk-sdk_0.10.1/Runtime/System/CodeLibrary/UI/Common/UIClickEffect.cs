using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using DG.Tweening;

public class UIClickEffect : MonoBehaviour,IPointerDownHandler,IPointerExitHandler
{
    [Header("缩放点击动画")]
    public Transform ScaleTransform;

    [Header("缩放点击动画")]
    public bool ScaleAnim = false;

    [Header("高亮点击动画")]
    public Image HightImage;

    void Awake()
    {
        //HightImage.DOFade(0f, 0.15f);
    }

    public System.Action<RectTransform> PointerDownCallBack;

    RectTransform _rectTransform;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        //if (ScaleAnim)
        //{
        //    ScaleTransform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.15f);
        //}
        //HightImage.DOFade(1f,0.1f);
        //if (PointerDownCallBack!=null)
        //{
        //    if (_rectTransform==null)
        //    {
        //        _rectTransform = gameObject.GetComponent<RectTransform>();
        //    }
        //    PointerDownCallBack(_rectTransform);
        //}
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        //if (ScaleAnim)
        //{
        //    ScaleTransform.DOScale(Vector3.one, 0.15f);
        //}
        //HightImage.DOFade(0f, 0.1f);
    }
}
