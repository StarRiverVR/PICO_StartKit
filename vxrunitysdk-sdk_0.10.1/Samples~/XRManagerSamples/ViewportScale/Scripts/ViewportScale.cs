using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.openxr;

public class ViewportScale : MonoBehaviour
{
    List<float> _viewportScales;

    public int index = 0;

    float msaaTimeLenght = 3f;

    float msaaTime = 0;

    List<float> viewportScales
    {
        get
        {
            if (_viewportScales == null)
            {
                _viewportScales = new List<float>();
                _viewportScales.Add(1f);
                _viewportScales.Add(0.8f);
            }
            return _viewportScales;
        }
    }

    public Material Mat;

    public List<Texture2D> Textures = new List<Texture2D>();

    bool isSetIndex = false;

    public void SetIndex(int _index)
    {
        Texture2D tex = Textures[_index];
        Mat.mainTexture = tex;
        VXRCommon.RenderViewportScale = viewportScales[_index];
        isSetIndex = true;

        index = _index + 1;
        if (index >= viewportScales.Count)
        {
            index = 0;
        }
    }

    void Update()
    {
        if (isSetIndex)
        {
            return;
        }
        if (msaaTime < msaaTimeLenght)
        {
            msaaTime = msaaTime + Time.deltaTime;
            return;
        }
        msaaTime = 0;
        Texture2D tex = Textures[index];
        Mat.mainTexture = tex;
        VXRCommon.RenderViewportScale = viewportScales[index];

        index++;
        if (index >= viewportScales.Count)
        {
            index = 0;
        }
    }

    private void OnDestroy()
    {
        VXRCommon.RenderViewportScale = 1;
    }

}
