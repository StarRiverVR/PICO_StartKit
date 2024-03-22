using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.openxr;

public class MSAATest : MonoBehaviour
{

    UnityEngine.Rendering.MSAASamples oldData;

    List<UnityEngine.Rendering.MSAASamples> _msaaSamples;

    int index = 0;

    float msaaTimeLenght = 3f;

    float msaaTime = 0;

    List<UnityEngine.Rendering.MSAASamples> msaaSamples
    {
        get
        {
            if (_msaaSamples==null)
            {
                _msaaSamples = new List<UnityEngine.Rendering.MSAASamples>();
                _msaaSamples.Add(UnityEngine.Rendering.MSAASamples.None);
                _msaaSamples.Add(UnityEngine.Rendering.MSAASamples.MSAA2x);
                _msaaSamples.Add(UnityEngine.Rendering.MSAASamples.MSAA4x);
                _msaaSamples.Add(UnityEngine.Rendering.MSAASamples.MSAA8x);
            }
            return _msaaSamples;
        }
    }

    public Material Mat;

    public List<Texture2D> Textures = new List<Texture2D>();

    private void Start()
    {
        oldData = VXRCommon.MsaaSample;
    }

    bool isSetMsaaSample = false;

    public void SetMsaaSample(UnityEngine.Rendering.MSAASamples v)
    {
        isSetMsaaSample = true;
        VXRCommon.MsaaSample = v;
    }

    void Update()
    {
        if (isSetMsaaSample)
        {
            return;
        }
        if (msaaTime< msaaTimeLenght)
        {
            msaaTime = msaaTime + Time.deltaTime;
            return;
        }
        msaaTime = 0;
        Texture2D tex = Textures[index];
        Mat.mainTexture = tex;
        VXRCommon.MsaaSample = msaaSamples[index];

        index++;
        if (index>= msaaSamples.Count)
        {
            index = 0;
        }
    }

    private void OnDestroy()
    {
        VXRCommon.MsaaSample = oldData;
    }

}
