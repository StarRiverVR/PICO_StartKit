using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayDemoLayerClickEvent : MonoBehaviour
{
    public void OnRayPrimaryButtonClick(Transform tran)
    {
        Ray ray = new Ray(tran.position, tran.forward);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            OverlayDemoDepthDisplay interactor = hits[i].collider.GetComponent<OverlayDemoDepthDisplay>();
            if (interactor)
            {
                interactor.AddLayerDepth();
                break;
            }
        }
    }

    public void OnRaysecondaryButtonClick(Transform tran)
    {
        Ray ray = new Ray(tran.position, tran.forward);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            OverlayDemoDepthDisplay interactor = hits[i].collider.GetComponent<OverlayDemoDepthDisplay>();
            if (interactor)
            {
                interactor.LessLayerDepth();
                break;
            }
        }
    }

    public void OnRayGribButtonClick(Transform tran)
    {
        Ray ray = new Ray(tran.position, tran.forward);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray);
        for (int i = 0; i < hits.Length; i++)
        {
            OverlayDemoDepthDisplay interactor = hits[i].collider.GetComponent<OverlayDemoDepthDisplay>();
            if (interactor)
            {
                interactor.ChangeOverlayType();
                break;
            }
        }
    }
}
