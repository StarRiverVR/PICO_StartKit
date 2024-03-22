
using UnityEngine;
using System;


/// <summary>
/// UGui 位置动画控制脚本
/// </summary>
public class UGUI_Tween_Pos : MonoBehaviour {

    public Vector3 startPos_s= Vector3.zero;
    public Vector3 endPos_s = Vector3.zero;
    public float startTime_s = 0f;
    public float durTime_s = 0f;
    public RectTransform rectTransform_s = null;
    UGUI_Anim_Type Anim_Type_s;
    Action endFun_s;//节点出发函数
    //
    float oldT;
    private bool Start()
    {

        rectTransform_s.anchoredPosition3D = startPos_s;
         oldT = durTime_s;
        InvokeRepeating("StartRun", startTime_s, 0.01f);
        return true;
    }

    void StartRun()
    {
        if (durTime_s <= 0f)
        {
            CancelInvoke("StartRun");
            if (endFun_s != null)
            {
                endFun_s();
            }
            switch (Anim_Type_s)
            {
                case UGUI_Anim_Type.once:
                    Destroy(this);
                    break;
                case UGUI_Anim_Type.loop:
                    AddAnim_s(rectTransform_s, startPos_s, endPos_s, startTime_s, oldT, Anim_Type_s);
                    break;
                case UGUI_Anim_Type.ping:
                    AddAnim_s(rectTransform_s, endPos_s, startPos_s, startTime_s, oldT, Anim_Type_s);
                    break;
            }
        }
        durTime_s = durTime_s - 0.01f;
        rectTransform_s.anchoredPosition3D = startPos_s + (endPos_s - startPos_s) * (1 - durTime_s / oldT);
    }

    /// <summary>
    /// 添加alpha动画
    /// startAlpha:开始Alpha值
    /// endAlpha：结束Alpha值
    /// startTime：开始动画的时间 秒
    /// durTime：动画持续时间 秒
    /// </summary>
    /// <param name="startAlpha"></param>
    /// <param name="endAlpha"></param>
    /// <param name="startTime"></param>
    /// <param name="durTime"></param>
    public bool AddAnim_s(RectTransform rectTransform, Vector3 startPos, Vector3 endPos, float startTime, float durTime, UGUI_Anim_Type Anim_Type = UGUI_Anim_Type.once, Action endFun = null)
    {
        startPos_s = startPos;
        endPos_s = endPos;
        startTime_s = startTime;
        durTime_s = durTime;
        rectTransform_s = rectTransform;
        Anim_Type_s = Anim_Type;
        endFun_s = endFun;
        Start();
        return true;
    }

    /// <summary>
    /// 添加alpha动画
    /// startAlpha:开始Alpha值
    /// endAlpha：结束Alpha值
    /// startTime：开始动画的时间 秒
    /// durTime：动画持续时间 秒
    /// endFun:结束触发函数
    /// </summary>
    /// <param name="startAlpha"></param>
    /// <param name="endAlpha"></param>
    /// <param name="startTime"></param>
    /// <param name="durTime"></param>
    public static bool AddAnim(GameObject obj, Vector3 startPos, Vector3 endPos, float startTime, float durTime, UGUI_Anim_Type Anim_Type = UGUI_Anim_Type.once, Action endFun = null)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform==null)
        {
            return false;
        }
        UGUI_Tween_Pos Anim = obj.gameObject.AddComponent<UGUI_Tween_Pos>();
        return Anim.AddAnim_s(rectTransform, startPos, endPos, startTime, durTime, Anim_Type, endFun);
    }
}

public enum UGUI_Anim_Type
{
    once,//播放一次
    loop,//循环播放
    ping,//来回播放
}
