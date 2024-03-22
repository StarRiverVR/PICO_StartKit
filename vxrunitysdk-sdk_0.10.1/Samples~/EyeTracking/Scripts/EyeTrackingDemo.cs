
using com.vivo.openxr;
using UnityEngine;
using UnityEngine.UI;

public class EyeTrackingDemo : MonoBehaviour
{
    int clickCount = 0;
    public void OnClickUIButton(Text text)
    {
        text.text = $"点击次数：{++clickCount}";
    }

    [SerializeField]
    TextMesh textGaze;

    private void Update()
    {
        VXREyeTracking.GetEyeGazeData(out var data);
        textGaze.text = $"眼动追踪凝视信息：\n" +
            $"IsTracked:{data.IsTracked}\n" +
            $"Position:{data.Position}\n" +
            $"Rotation:{data.Rotation}";
    }
}
