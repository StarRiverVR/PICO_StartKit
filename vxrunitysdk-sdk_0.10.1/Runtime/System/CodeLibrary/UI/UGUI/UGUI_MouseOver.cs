
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// ugui鼠标悬停事件
/// </summary>
public class UGUI_MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static bool UGUIMouseOver=false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        UGUIMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UGUIMouseOver = false;
    }

    private void OnDestroy()
    {
        UGUIMouseOver = false;
    }
}
