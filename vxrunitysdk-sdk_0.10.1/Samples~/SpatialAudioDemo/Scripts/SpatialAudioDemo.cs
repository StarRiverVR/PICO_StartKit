using com.vivo.codelibrary;
using com.vivo.openxr;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using static com.vivo.openxr.VXRPlugin;
using static System.Net.Mime.MediaTypeNames;

public class SpatialAudioDemo : MonoBehaviour
{
    // 环绕
    [Header("环绕")]
    [SerializeField]
    Transform aroundAudio;
    [SerializeField]
    AudioSlider sliderDistance;
    [SerializeField]
    bool isAround = false;

    // 衰减
    [Header("衰减")]
    [SerializeField]
    VXRSpatialAudioSource source;
    [Tooltip("衰减模式")]
    [SerializeField]
    Dropdown dropdownAttenuationMode;
    [Tooltip("衰减最大距离")]
    [SerializeField]
    AudioSlider sliderMaxAttenuationDistance;

    //房间
    [Header("房间")]
    [SerializeField]
    AudioSlider sliderReflectionScalar;
    [SerializeField]
    AudioSlider sliderReverbGain;
    [SerializeField]
    AudioSlider sliderReverbTime;
    [SerializeField]
    AudioSlider sliderReverbBrightness;
    [SerializeField]
    Button buttonRoom;
    [SerializeField]
    UnityEngine.UI.Text textRoom;
    [SerializeField]
    bool isOpenRoom = false;

    AudioSource _audioSource;
    AudioSource audioSource
    {
        get
        {
            if(_audioSource == null)
            {
                _audioSource = FindObjectOfType<AudioSource>();
            }
            return _audioSource;
        }
    }
    Ray ray;
    void Awake()
    {
        VLog.Info("SpatialAudioDemo AWake");
        UnityEngine.Application.focusChanged += onChangeFocus;

        VXREvent.AddOpenXRFocusChangedListener(onXRFocusEvent1);

        ray = new Ray(Camera.main.transform.position, aroundAudio.position - Camera.main.transform.position);
        aroundAudio.position = ray.GetPoint(sliderDistance.value);
    }

    private void onXRFocusEvent1(bool obj)
    {
        VLog.Info($"SpatialAudioDemo onXRFocusEvent {obj}");
    }

    private void onChangeFocus(bool obj)
    {
        VLog.Info($"SpatialAudioDemo focusChanged {obj}");
        if(obj)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Pause();
        }
    }

    void OnEnable()
    {
        VLog.Info("SpatialAudioDemo OnEnable");
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        VLog.Info("SpatialAudioDemo Start");
        // 环绕
        sliderDistance.AddListener(OnSliderValueChange);
        // 衰减
        sliderMaxAttenuationDistance.AddListener(OnSliderMaxAttenuationDistance);
        dropdownAttenuationMode.onValueChanged.AddListener(OnDropdownValueChange);
        // 环绕
        sliderReflectionScalar.AddListener(OnSliderReflectionScalar);
        sliderReverbGain.AddListener(OnSliderReverbGain);
        sliderReverbTime.AddListener(OnSliderReverbTime);
        sliderReverbBrightness.AddListener(OnSliderReverbBrightness);

        SetRoomEnable(isOpenRoom);
    }

    // Update is called once per frame
    void Update()
    {
        if (isAround)
        {
            aroundAudio.RotateAround(Camera.main.transform.position, Vector3.up, 1f);
        }
    }

    void OnDisable()
    {
        VLog.Info("SpatialAudioDemo OnDisable");
        audioSource.Pause();
    }
    #region// 环绕
    public void OnClickAround(UnityEngine.UI.Text txt)
    {
        isAround = !isAround;
        string on = isAround ? "开" : "关";
        txt.text = $"当前状态：{on}";
    }
    public void OnClickSetPosition(Transform tran)
    {
        if (isAround) return;
        ray.origin = tran.position;
        ray.direction = tran.forward;
        aroundAudio.position = ray.GetPoint(sliderDistance.value);
    }
    private void OnSliderValueChange(float arg0)
    {
        ray.origin = Camera.main.transform.position;
        ray.direction = aroundAudio.position - Camera.main.transform.position;
        aroundAudio.position = ray.GetPoint(arg0);
    }
    #endregion

    #region// 衰减
    private void OnDropdownValueChange(int index)
    {
        RefreshSourceAttenuation();
    }
    private void OnSliderMaxAttenuationDistance(float arg0)
    {
        RefreshSourceAttenuation();
    }
    private void RefreshSourceAttenuation()
    {
        VXRSpatialAudio.AudioAttenuation info = new VXRSpatialAudio.AudioAttenuation();
        info.MinDistance = 1;
        info.MaxDistance = sliderMaxAttenuationDistance.value;
        info.Mode = dropdownAttenuationMode.value;
        info.CustomParam = 0;
        VXRSpatialAudio.SetSourceAttenuation(source.SourceId, info);
    }

    #endregion

    #region// 房间
    public void OnClickRoom(UnityEngine.UI.Text txt)
    {
        VLog.Info($"SpatialAudioDemo.OnClickRoom = {!isOpenRoom}");
        SetRoomEnable(!isOpenRoom);
    }

    VXRSpatialAudioRoom _room;
    VXRSpatialAudioRoom room
    {
        get
        {
            if(_room == null)
            {
                _room = FindObjectOfType<VXRSpatialAudioRoom>(true);
                if (_room == null)
                {
                    VLog.Warning("Can not find VXRSpatialAudioRoom");
                }
            }
            return _room;
        }
    }
    private void SetRoomEnable(bool enable)
    {
        isOpenRoom = enable;
        string on = isOpenRoom ? "开" : "关";
        textRoom.text = $"当前状态：{on}";
        VLog.Info("房间"+ textRoom.text);
        room?.gameObject.SetActive(isOpenRoom);
        VXRSpatialAudio.SetRoomEnable(isOpenRoom);
    }
    private void OnSliderReflectionScalar(float arg0)
    {
        VXRSpatialAudio.SetRoomReflection(arg0);
    }
    private void OnSliderReverbGain(float arg0)
    {
        VXRSpatialAudio.SetRoomReverb(
            sliderReverbGain.value,
            sliderReverbTime.value,
            sliderReverbBrightness.value
        );
    }
    private void OnSliderReverbTime(float arg0)
    {
        VXRSpatialAudio.SetRoomReverb(
            sliderReverbGain.value,
            sliderReverbTime.value,
            sliderReverbBrightness.value
        );
    }
    private void OnSliderReverbBrightness(float arg0)
    {
        VXRSpatialAudio.SetRoomReverb(
            sliderReverbGain.value,
            sliderReverbTime.value,
            sliderReverbBrightness.value
        );
    }

    #endregion
}
