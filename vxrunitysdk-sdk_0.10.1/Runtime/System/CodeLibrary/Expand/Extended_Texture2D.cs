
using UnityEngine;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace com.vivo.codelibrary
{

    public static class Extended_Texture2D
    {

        /// <summary>
        /// 保存Texture2D 到本地
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="savePath"></param>
        /// <param name="textureType"></param>
        public static void SaveTexture2D(this Texture2D texture, string savePath, TextureHelper.TextureType textureType = TextureHelper.TextureType.PNG,bool destroyTexture = false)
        {
            TextureHelper.SaveTexture( texture,  savePath, textureType);
            if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    if (destroyTexture)
                    {
                        if (Application.isPlaying)
                        {
                            Texture2D.Destroy(texture);
                        }
                        else
                        {
                            Texture2D.DestroyImmediate(texture);
                        }
                    }
                },null);
            }
            else
            {
                if (destroyTexture)
                {
                    if (Application.isPlaying)
                    {
                        Texture2D.Destroy(texture);
                    }
                    else
                    {
                        Texture2D.DestroyImmediate(texture);
                    }
                }
            }
        }

        /// <summary>
        /// 异步 保存Texture2D 到本地
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="savePath"></param>
        /// <param name="callBack"></param>
        /// <param name="textureType"></param>
        /// <param name="destroyTexture"></param>
        public static void SaveTexture2DAsyn(this Texture2D texture, string savePath, System.Action callBack, 
            TextureHelper.TextureType textureType = TextureHelper.TextureType.PNG, bool destroyTexture = false)
        {
            TextureHelper.SaveTextureAsyn(texture, savePath,()=> {
                callBack();
                if (ThreadHelper.UnitySynchronizationContext != SynchronizationContext.Current)
                {
                    ThreadHelper.UnitySynchronizationContext.Send((o) => {
                        if (destroyTexture)
                        {
                            if (Application.isPlaying)
                            {
                                Texture2D.Destroy(texture);
                            }
                            else
                            {
                                Texture2D.DestroyImmediate(texture);
                            }
                        }
                    }, null);
                }
                else
                {
                    if (destroyTexture)
                    {
                        if (Application.isPlaying)
                        {
                            Texture2D.Destroy(texture);
                        }
                        else
                        {
                            Texture2D.DestroyImmediate(texture);
                        }
                    }
                }
            }, textureType);
        }


        /// <summary>
        /// 可读写开启
        /// </summary>
        /// <param name="texture"></param>
        public static void ReadableOn(this Texture2D texture)
        {
            TextureHelper.TextureReadableOn(texture, (b,c,w,h,b2) => { }, false);
        }

        /// <summary>
        /// 可读写关闭
        /// </summary>
        /// <param name="texture"></param>
        public static void ReadableOff(this Texture2D texture)
        {
            TextureHelper.TextureReadableOff(texture);
        }

        /// <summary>
        /// 读取像素
        /// </summary>
        /// <param name="texture"></param>
        /// <returns></returns>
        public static Color[] GetColors(this Texture2D texture)
        {
            return TextureHelper.GetColors(texture);
        }

        static bool IsPureColorConfirmation(Color[] colors, int width, int height)
        {
            bool isPure = true;
            Color targetColor = Color.green;
            bool initTargetColor = false;
            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    Color findColor = colors[j * width + i];
                    if (!initTargetColor)
                    {
                        targetColor = findColor;
                    }
                    else
                    {
                        if (targetColor != findColor)
                        {
                            isPure = false;
                            break;
                        }
                    }
                }
                if (!isPure)
                {
                    break;
                }
            }
            return isPure;
        }

    }

}
