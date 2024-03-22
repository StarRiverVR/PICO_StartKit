using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.openxr;

public class FadeInOut : MonoBehaviour
{
    public float FadeInOutStartDelay=2;
    public float FadeInOutAnimLenght = 5;
    public Color FadeInOutStartColor = Color.white;
    public Color FadeInOutEndColor = new Color (0,0,0,0);
    public Texture2D FadeInOutTexture;

    private void Start()
    {
        StartFadeInOutAnim();
    }

    void StartFadeInOutAnim()
    {
        VXRCommon.StartFadeInOutAnim(FadeInOutStartDelay, FadeInOutAnimLenght,
            FadeInOutStartColor, FadeInOutEndColor,
            FadeInOutTexture);
    }
}
