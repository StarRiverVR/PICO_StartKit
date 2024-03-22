
using UnityEngine;
using System;


/// <summary>
/// UGui 缩放动画控制脚本
/// </summary>
public class UGUI_Tween_Scale : MonoBehaviour {

    public Vector3 startScale_s = Vector3.zero;
    public Vector3 endScale_s = Vector3.zero;
    public float startTime_s = 0f;
    public float durTime_s = 0f;
    public Transform transform_s = null;
    UGUI_Anim_Type Anim_Type_s;
    Action endFun_s;//节点出发函数
    //
    float oldT;
    private bool Start()
    {

        transform_s.localScale = startScale_s;
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
                    AddAnim_s(transform_s, startScale_s, endScale_s, startTime_s, oldT, Anim_Type_s);
                    break;
                case UGUI_Anim_Type.ping:
                    AddAnim_s(transform_s, endScale_s, startScale_s, startTime_s, oldT, Anim_Type_s);
                    break;
            }
        }
        durTime_s = durTime_s - 0.01f;
        transform_s.localScale = startScale_s + (endScale_s - startScale_s) * (1 - durTime_s / oldT);
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
    public bool AddAnim_s(Transform transform_t, Vector3 startScale, Vector3 endScale, float startTime, float durTime, UGUI_Anim_Type Anim_Type = UGUI_Anim_Type.once, Action endFun = null)
    {
        startScale_s = startScale;
        endScale_s = endScale;
        startTime_s = startTime;
        durTime_s = durTime;
        transform_s = transform_t;
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
    public static bool AddAnim(GameObject obj, Vector3 startScale, Vector3 endScale, float startTime, float durTime, UGUI_Anim_Type Anim_Type = UGUI_Anim_Type.once, Action endFun = null)
    {
       Transform transform_t = obj.GetComponent<Transform>();
        if (transform_t == null)
        {
            return false;
        }
        UGUI_Tween_Scale Anim = obj.gameObject.AddComponent<UGUI_Tween_Scale>();
        return Anim.AddAnim_s(transform_t, startScale, endScale, startTime, durTime, Anim_Type, endFun);
    }
}
