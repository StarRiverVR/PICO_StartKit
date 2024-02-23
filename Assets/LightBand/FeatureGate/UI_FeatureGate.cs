using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Reflection;

public class UI_FeatureGate : MonoBehaviour
{
    public GameObject ToggleTemplate;
    public GameObject SliderTemplate;
    public GameObject ButtonTemplate;
    public FeatureGateCenter fg;
    public GameObject FGContainer;

    public Toggle UpDownToggle;

    public float CurrentHeight = 0;

    private void Awake()
    {
        this.InitTemplate();
        this.InitFeatureGate();
    }

    private void Start()
    {
        // StartCoroutine(this.AutoRefresh());
    }
    private IEnumerator AutoRefresh()
    {
        while (true)
        {
            yield return new WaitForSeconds(10);
            this.Refresh();
        }
    }
    public void SelfTest()
    {
        this.fg.AddFeatureGate("今天上大分1", true, (x) => { Debug.Log("上打分成功1： " + x); });
        this.fg.AddFeatureGate("今天上大分2", true, (x) => { Debug.Log("上打分成功2： " + x); });
        this.fg.AddFeatureGate("今天上大分3", 10, 100, (x) => { Debug.Log("上打分成功3： " + x); });
        this.fg.AddFeatureGate("今天上大分4", 1f, 2f, (x) => { Debug.Log("上打分成功4： " + x); });

        this.fg.AddFeatureGate("调整日光颜色", 1f, 255f, (x) =>
        {
            Debug.Log(x);
            Debug.Log((int)x);
            Debug.Log((byte)x);
            var gameObj = GameObject.Find("Directional Light");
            var light = gameObj.GetComponent<Light>();
            light.color = new Color(light.color.r, light.color.g, (byte)x, light.color.a);
        });
    }

    public void Refresh()
    {
        //Debug.Log(this.name);
        for (int i = 0; i < this.FGContainer.transform.childCount; i++)
        {
            var child = this.FGContainer.transform.GetChild(i);
            if (child.tag == "AutoGenerate")
                Destroy(child.gameObject);

        }

        this.CurrentHeight = 0;
        this.fg.ReTriggerAll();
    }

    public void InitTemplate()
    {
        this.SliderTemplate.SetActive(false);
        this.ToggleTemplate.SetActive(false);
        this.ButtonTemplate.SetActive(false);

        // this.FGContainer = GameObject.Find("FGContainer");

        this.UpDownToggle.onValueChanged.AddListener((x) =>
        {
            this.FGContainer.SetActive(x);
        });

    }
    public void InitFeatureGate()
    {
        this.fg = FeatureGateCenter.Instance;

        this.fg.Register<bool>(this.AddNewToggle);
        this.fg.Register<float>(this.AddNewSlider);
        this.fg.Register<int>(this.AddNewSlider);
    }

    private void OnDestroy() {
        this.fg.UnRegister<bool>(this.AddNewToggle);
        this.fg.UnRegister<float>(this.AddNewSlider);
        this.fg.UnRegister<int>(this.AddNewSlider);
    }

    void AddNewSlider(Feature<int> feature)
    {
        GameObject sliderContainer = Instantiate(this.SliderTemplate, this.SliderTemplate.transform);
        sliderContainer.SetActive(true);
        sliderContainer.transform.SetParent(this.FGContainer.transform);
        sliderContainer.tag = "AutoGenerate";

        var rectTransform = sliderContainer.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, this.CurrentHeight);

        this.CurrentHeight -= rectTransform.sizeDelta.y;

        var nameLabel = sliderContainer.GetComponentsInChildren<Text>()[0];
        var valueLabel = sliderContainer.GetComponentsInChildren<Text>()[1];
        nameLabel.text = feature.Name + ":0-" + feature.MaxValue;
        valueLabel.text = feature.GetValue().ToString();

        var slider = sliderContainer.GetComponentInChildren<Slider>();

        slider.maxValue = feature.MaxValue;
        slider.value = feature.GetValue();
        slider.wholeNumbers = true;
        slider.onValueChanged.AddListener((float value) =>
        {
            feature.SetValue((int)value);
            sliderContainer.GetComponentsInChildren<Text>()[1].text = value.ToString();
            Debug.Log(feature.Name + ": " + value);
        });
    }

    void AddNewSlider(Feature<float> feature)
    {
        GameObject sliderContainer = Instantiate(this.SliderTemplate, this.SliderTemplate.transform);
        sliderContainer.SetActive(true);
        sliderContainer.transform.SetParent(this.FGContainer.transform);
        sliderContainer.tag = "AutoGenerate";

        var rectTransform = sliderContainer.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, this.CurrentHeight);

        this.CurrentHeight -= rectTransform.sizeDelta.y;

        var nameLabel = sliderContainer.GetComponentsInChildren<Text>()[0];
        var valueLabel = sliderContainer.GetComponentsInChildren<Text>()[1];

        nameLabel.text = feature.Name + ":0-" + feature.MaxValue;
        valueLabel.text = feature.GetValue().ToString();

        var slider = sliderContainer.GetComponentInChildren<Slider>();

        slider.maxValue = feature.MaxValue;
        slider.value = feature.GetValue();

        slider.onValueChanged.AddListener((float value) =>
        {
            feature.SetValue(value);
            sliderContainer.GetComponentsInChildren<Text>()[1].text = value.ToString();
            Debug.Log(feature.Name + ": " + value);
        });
    }

    void AddNewToggle(Feature<bool> feature)
    {
        GameObject toggleContainer = Instantiate(ToggleTemplate, this.ToggleTemplate.transform);
        toggleContainer.SetActive(true);
        toggleContainer.transform.SetParent(this.FGContainer.transform);
        toggleContainer.tag = "AutoGenerate";

        var rectTransform = toggleContainer.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, this.CurrentHeight);

        this.CurrentHeight -= rectTransform.sizeDelta.y;

        toggleContainer.GetComponentInChildren<Text>().text = feature.Name;

        Toggle toggle = toggleContainer.GetComponent<Toggle>();
        toggle.isOn = feature.GetValue();
        toggle.onValueChanged.AddListener((bool value) =>
        {
            feature.SetValue(value);
            Debug.Log(feature.Name + ": " + value);
        });
    }

    void AddNewButton(FeatureMethod feature)
    {
        var method = feature.MethodInfo;
        var attribute = feature.GMCommandAttribute;

        // Instantiate your button prefab
        var buttonContainer = Instantiate(ButtonTemplate, this.ButtonTemplate.transform);
        buttonContainer.SetActive(true);
        buttonContainer.transform.SetParent(this.FGContainer.transform);
        buttonContainer.tag = "AutoGenerate";

        var rectTransform = buttonContainer.transform.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, this.CurrentHeight);

        this.CurrentHeight -= rectTransform.sizeDelta.y;

        // Set the button text to the command name
        var buttonText = buttonContainer.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = attribute.Command;
        }

        // Add a click listener to the button that executes the method
        var button = buttonContainer.GetComponent<Button>();
        button.onClick.AddListener(() => method.Invoke(null, null));
    }
}
