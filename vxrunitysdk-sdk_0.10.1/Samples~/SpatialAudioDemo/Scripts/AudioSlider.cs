using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    public Slider slider;
    public Text text;
    public string unit = "";
    
    public float value { get => slider.value; }
    // Start is called before the first frame update
    void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChange);
        text.text = $"{slider.value.ToString("F2")}{unit}";
    }



    private void OnSliderValueChange(float arg0)
    {
        text.text = $"{slider.value.ToString("F2")}{unit}";
        sliderEvent?.Invoke(arg0);
    }

    UnityAction<float> sliderEvent;
    public void AddListener(UnityAction<float> call)
    {
        sliderEvent = call;
        sliderEvent?.Invoke(slider.value);
    }
    public void RemoveListener(UnityAction<float> call)
    {
        sliderEvent = null;
    }
}
