using com.vivo.codelibrary;
using com.vivo.openxr;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FFRDemo : MonoBehaviour
{
    [SerializeField]
    private Dropdown dropdownMenu;
   
    [SerializeField]
    private Button _UseDynamicFoveationLevelSwitch;
    
    private bool _isUseDynamicFoveationLevel;

    [SerializeField]private Color activeColor;
    [SerializeField]private Color unActiveColor;
    // Start is called before the first frame update
    void Start()
    {
        var level = VXRFoveationRendering.GetFoveationLevel();
        VLog.Info($"FFR Get Level = {level}");
        dropdownMenu.options.Clear();
        dropdownMenu.options.Add(new Dropdown.OptionData(VXRFoveationRendering.Level.None.ToString()));
        dropdownMenu.options.Add(new Dropdown.OptionData(VXRFoveationRendering.Level.Low.ToString()));
        dropdownMenu.options.Add(new Dropdown.OptionData(VXRFoveationRendering.Level.Medinum.ToString()));
        dropdownMenu.options.Add(new Dropdown.OptionData(VXRFoveationRendering.Level.High.ToString()));
        bool find = false;
        for(int i = 0; i < dropdownMenu.options.Count; i++)
        {
            if (dropdownMenu.options[i].text.Equals(level.ToString()))
            {
                VLog.Info($"FFR Set Dropdown Index[{i}] = {level}");
                dropdownMenu.value = i;
                find = true;
                break;
            }
        }
        if( !find )
        {
            VLog.Info($"FFR Set Dropdown Fail = {level}");
        }
        dropdownMenu.RefreshShownValue();
        dropdownMenu.onValueChanged.AddListener(OnDropdownChanged);
        _isUseDynamicFoveationLevel = VXRFoveationRendering.GetUseDynamicFoveationLevel();
        _UseDynamicFoveationLevelSwitch.onClick.AddListener(UseDynamicFoveationLevelSwitch);
        InvokeRepeating("UpdateDynamicFoveationLevelState",0,0.3f);
    }

    void OnDropdownChanged(int value)
    {
        VLog.Info($"FFR Dropdown Change = {value}");
        string name = dropdownMenu.options[value].text;
        if (Enum.TryParse(name, out VXRFoveationRendering.Level level))
        {
            VLog.Info($"FFR Dropdown = {level}");
            VXRFoveationRendering.SetFoveationLevel(level);
        }
        else
        {
            VLog.Info($"FFR Set Fail = {name}");
        }
    }

    public void UseDynamicFoveationLevelSwitch()
    {
        _isUseDynamicFoveationLevel = VXRFoveationRendering.GetUseDynamicFoveationLevel();
        VXRFoveationRendering.SetUseDynamicFoveationLevel(!_isUseDynamicFoveationLevel);
    }

    public void UpdateDynamicFoveationLevelState()
    {
        var level = VXRFoveationRendering.GetFoveationLevel();
        _isUseDynamicFoveationLevel = VXRFoveationRendering.GetUseDynamicFoveationLevel();
        _UseDynamicFoveationLevelSwitch.GetComponentInChildren<Text>().text = _isUseDynamicFoveationLevel == true ? "已开启" : "未开启";
        _UseDynamicFoveationLevelSwitch.GetComponent<Image>().color=_isUseDynamicFoveationLevel == true ? activeColor : unActiveColor;
    }
}
