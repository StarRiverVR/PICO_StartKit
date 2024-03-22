
using UnityEngine;
using System;


/// <summary>
/// UGui Alpha动画控制脚本
/// </summary>
public class UGUI_Tween_Alpha : MonoBehaviour {
    public float startAlpha_s = 0f;
    public float endAlpha_s = 0f;
    public float startTime_s = 0f;
    public float durTime_s = 0f;
    public CanvasGroup canvasGroup_s = null;
    UGUI_Anim_Type Anim_Type_s;
    Action endFun_s;//节点出发函数
    //
    float oldT;
    private bool Start()
    {
        if (canvasGroup_s==null)
        {
            return false;
        }
        canvasGroup_s.alpha = startAlpha_s;
        oldT = durTime_s;
        InvokeRepeating("StartRun", startTime_s,0.01f);
        return true;
    }

    void StartRun()
    {
        if (durTime_s<=0f)
        {
            CancelInvoke("StartRun");
            if (endFun_s!=null)
            {
                endFun_s();
            }
            switch (Anim_Type_s)
            {
                case UGUI_Anim_Type.once:
                    Destroy(this);
                    break;
                case UGUI_Anim_Type.loop:
                    AddAnim_s(canvasGroup_s, startAlpha_s, endAlpha_s, startTime_s, oldT, Anim_Type_s);
                    break;
                case UGUI_Anim_Type.ping:
                    AddAnim_s(canvasGroup_s, endAlpha_s, startAlpha_s, startTime_s, oldT, Anim_Type_s);
                    break;
            }
        }
        durTime_s = durTime_s - 0.01f;
        canvasGroup_s.alpha = startAlpha_s +(endAlpha_s- startAlpha_s)* (1-durTime_s/ oldT);
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
    public bool AddAnim_s(CanvasGroup canvasGroup,float startAlpha,float endAlpha,float startTime,float durTime, UGUI_Anim_Type Anim_Type= UGUI_Anim_Type.once, Action endFun=null)
    {
        startAlpha_s = startAlpha;
        endAlpha_s = endAlpha;
        startTime_s = startTime;
        durTime_s = durTime;
        canvasGroup_s = canvasGroup;
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
    public static  bool AddAnim(GameObject obj,float startAlpha, float endAlpha, float startTime, float durTime, UGUI_Anim_Type Anim_Type = UGUI_Anim_Type.once, Action endFun = null)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup==null)
        {
            canvasGroup=obj.AddComponent<CanvasGroup>();
        }
        UGUI_Tween_Alpha Anim = canvasGroup.gameObject.AddComponent<UGUI_Tween_Alpha>();
        return Anim.AddAnim_s( canvasGroup,  startAlpha,  endAlpha,  startTime,  durTime,  Anim_Type, endFun);
    }
}
