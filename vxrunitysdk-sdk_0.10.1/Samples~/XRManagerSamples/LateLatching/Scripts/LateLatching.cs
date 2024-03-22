using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.openxr;

public class LateLatching : MonoBehaviour
{
    void Start()
    {
        color = Mat.GetColor("_BaseColor");
        StartCoroutine(AddScript());
    }

    public Material Mat;

    VXRLateLatching vxrLateLatching;

    Color color;

    IEnumerator AddScript()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if (VXRManager.Instance!=null && VXRManager.Instance.MainCamera!=null)
            {
                vxrLateLatching = VXRManager.Instance.MainCamera.GetComponent<VXRLateLatching>();
                if (vxrLateLatching==null)
                {
                    vxrLateLatching = VXRManager.Instance.MainCamera.gameObject.AddComponent<VXRLateLatching>();
                }
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    float timeLenght = 5;
    float curTime = 0;
    public bool IsOpenLateLatching
    {
        get
        {
            if (vxrLateLatching!=null)
            {
                return vxrLateLatching.enabled;
            }
            return false;
        }
    }

    bool isSetOpenLateLatching = false;

    public void SetOpenLateLatching(bool bl)
    {
        if (vxrLateLatching != null)
        {
            vxrLateLatching.enabled = bl;
            if (vxrLateLatching.enabled)
            {
                Mat.SetColor("_BaseColor", new Color(0.3f, 0.3f, 0.3f, 1));
            }
            else
            {
                Mat.SetColor("_BaseColor", color);
            }
        }
        isSetOpenLateLatching = true;
    }

    private void Update()
    {
        if (vxrLateLatching==null)
        {
            return;
        }
        if (isSetOpenLateLatching)
        {
            return;
        }
        if (curTime< timeLenght)
        {
            curTime = curTime + Time.deltaTime;
            return;
        }
        curTime = 0;
        vxrLateLatching.enabled = !vxrLateLatching.enabled;
        if (vxrLateLatching.enabled)
        {
            Mat.SetColor("_BaseColor",new Color(0.3f, 0.3f, 0.3f, 1));
        }
        else
        {
            Mat.SetColor("_BaseColor", color);
        }
    }

}
