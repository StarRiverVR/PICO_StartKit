using com.vivo.openxr;
using UnityEngine;

public class OverlayDemoDepthDisplay : MonoBehaviour
{

    private VXROverlay overlay;
    private TextMesh label;

    void Start()
    {
        overlay = GetComponent<VXROverlay>();
        label = GetComponentInChildren<TextMesh>();
        label.text = $"{overlay.CurOverlayType.ToString()} {overlay.CurOverlayDepth.ToString()}";
    }

    public void AddLayerDepth()
    {
        overlay.CurOverlayDepth++;
        label.text = $"{overlay.CurOverlayType.ToString()} {overlay.CurOverlayDepth.ToString()}";
    }

    public void LessLayerDepth()
    {
        overlay.CurOverlayDepth--;
        label.text = $"{overlay.CurOverlayType.ToString()} {overlay.CurOverlayDepth.ToString()}";
    }

    public void ChangeOverlayType()
    {
        if (overlay.CurOverlayType == VXROverlay.OverlayType.Overlay)
        {
            overlay.CurOverlayType = VXROverlay.OverlayType.Underlay;
        }
        else
        {
            overlay.CurOverlayType = VXROverlay.OverlayType.Overlay;
        }
        label.text = $"{overlay.CurOverlayType.ToString()} {overlay.CurOverlayDepth.ToString()}";
    }
}
