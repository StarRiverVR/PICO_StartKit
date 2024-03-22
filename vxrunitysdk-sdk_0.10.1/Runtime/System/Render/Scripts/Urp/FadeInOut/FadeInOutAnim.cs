using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;
using com.vivo.codelibrary;
using System;

namespace com.vivo.render
{
    public class FadeInOutAnim: CSingleton<FadeInOutAnim>, IDisposable
    {
        public void StartFadeInOutAnim(float fadeInOutStartDelay, float fadeInOutAnimLenght, 
            Color fadeInOutStartColor, Color fadeInOutEndColor, Texture2D fadeInOutTexture)
        {
            this.fadeInOutStartDelay = fadeInOutStartDelay;
            this.fadeInOutAnimLenght = fadeInOutAnimLenght;
            this.fadeInOutStartColor = fadeInOutStartColor;
            this.fadeInOutEndColor = fadeInOutEndColor;
            isPlaying = true;
            UrpRenderAssetData.Data.IsOpenFadeInOut = true;
            UrpRenderAssetData.Data.FadeInOutColor = fadeInOutStartColor;
            UrpRenderAssetData.Data.FadeInOutBaseMap = fadeInOutTexture;
            lastFadeInOutAnimLenght = fadeInOutAnimLenght;
            InformationManager.Instance.GameInformationCenter.AddListen<MonoUpdateMsg>((int)MonoUpdateMsg.Update, MUpdate);
        }

        float fadeInOutStartDelay;

        float fadeInOutAnimLenght;

        Color fadeInOutStartColor;

        Color fadeInOutEndColor;

        Texture2D fadeInOutTexture;

        bool isPlaying = false;

        float lastFadeInOutAnimLenght;

        void UpdatePlaying()
        {
            if (!isPlaying)
            {
                return;
            }
            if (fadeInOutStartDelay>0)
            {
                fadeInOutStartDelay = fadeInOutStartDelay - Time.fixedDeltaTime;
                return;
            }
            if (lastFadeInOutAnimLenght > 0)
            {
                lastFadeInOutAnimLenght = lastFadeInOutAnimLenght- Time.fixedDeltaTime;
                if (lastFadeInOutAnimLenght<0)
                {
                    lastFadeInOutAnimLenght = 0;
                }
                UrpRenderAssetData.Data.FadeInOutColor = Color.Lerp(fadeInOutEndColor, fadeInOutStartColor, lastFadeInOutAnimLenght/ fadeInOutAnimLenght);
            }
            else
            {
                Stop();
            }
        }


        void Stop()
        {

            isPlaying = false;
            UrpRenderAssetData.Data.IsOpenFadeInOut = false;
            InformationManager.Instance.GameInformationCenter.RemoveListen<MonoUpdateMsg>((int)MonoUpdateMsg.Update, MUpdate);
        }

        void MUpdate(params object[] objs)
        {
            UpdatePlaying();
        }

        public void Dispose()
        {
            Stop();
        }
    }
}


