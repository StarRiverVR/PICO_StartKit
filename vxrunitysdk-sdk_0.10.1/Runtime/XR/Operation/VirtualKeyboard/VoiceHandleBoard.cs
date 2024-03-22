using System;
using System.Collections;
using com.vivo.codelibrary;
using com.vivo.openxr;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;
using Pose = UnityEngine.Pose;

public class VoiceHandleBoard : MonoBehaviour
{
    private VXRVirtualKeyboard _virtualKeyboard;
    private ImeDelegateBase _imeDelegate;
    private Transform _mTransform;
    private GameObject _boardTarget;
    private GameObject[] _bgImages;
    private Image _touchImage;
    private Image _moveImage;
    private Image _pressImage;
    private RectTransform _handle;
    private bool _mShow = false;
    private InputDeviceRole _controllerRole;
    private const float k_convertNearBeside = 0.5f;
    private Pose _controllerPose;
    private int _controllerFlag = 0;

    /// <summary>
    /// 区分左/右手柄上的语音快捷键提示板
    /// </summary>
    [Tooltip("区分左/右手柄上的语音快捷键提示板")] 
    [SerializeField]
    private XRNode _controllerNode = XRNode.LeftHand; 
    
    /// <summary>
    /// 按下时，语音快捷键提示板的UI颜色
    /// </summary>
    [Tooltip("按下时，语音快捷键提示板的UI颜色")]
    [SerializeField] 
    private Color _pressColor;
    
    /// <summary>
    /// 默认（抬起）状态下，语音快捷键提示板的UI颜色
    /// </summary>
    [Tooltip("默认（抬起）状态下，语音快捷键提示板的UI颜色")]
    [SerializeField] 
    private Color _normalColor;
    
    /// <summary>
    /// 绑定摇杆触摸的输入事件
    /// </summary>
    [Space(5)]
    [Header("Controller Joystick Touch")] 
    [Tooltip("绑定摇杆拨动方向的输入事件")]
    [SerializeField]
    private InputAction _joystick2DAxisTouchAction;
    
    /// <summary>
    /// 绑定摇杆按下的输入事件
    /// </summary>
    [Space(5)]
    [Header("Controller Joystick Click")] 
    [Tooltip("绑定摇杆快捷键按下的输入事件")]
    [SerializeField]
    private InputAction _joystick2DAxisClickAction;
    
    /// <summary>
    /// 绑定摇杆拨动方向的输入事件
    /// </summary>
    [Space(5)]
    [Header("Controller Joystick Move")] 
    [Tooltip("绑定摇杆拨动方向的输入事件")]
    [SerializeField]
    private InputAction _joystick2DAxisAction;
    
    private void Awake()
    {
        _virtualKeyboard = GetComponentInParent<VXRVirtualKeyboard>();
        _boardTarget = transform.GetChild(0).gameObject;
        _bgImages = new GameObject[2];
        _bgImages[0] = _boardTarget.transform.Find("Vector2Control/HandleArea/BackImageDefault").gameObject;
        _bgImages[1] = _boardTarget.transform.Find("Vector2Control/HandleArea/BackImageEditing").gameObject;
        _handle = _boardTarget.transform.Find("Vector2Control/HandleArea/Handle").GetComponent<RectTransform>();
        _touchImage = _boardTarget.transform.Find("Vector2Control/HandleArea/Handle/TouchedImage").GetComponent<Image>();
        _moveImage = _boardTarget.transform.Find("Vector2Control/HandleArea/Handle/MovedImage").GetComponent<Image>();
        _pressImage = _boardTarget.transform.Find("Vector2Control/HandleArea/Handle/PressedImage").GetComponent<Image>();
        _bgImages[0].SetActive(false);
        _bgImages[1].SetActive(false);
        _controllerPose = Pose.identity;
        if (_moveImage != null)
        {
            _moveImage.enabled = false;
            _moveImage.color = _normalColor;
        }

        if (_pressImage == null) return;
        _pressImage.enabled = false;
        _pressImage.color = _normalColor;
        
        _controllerRole = _controllerNode == XRNode.LeftHand ? InputDeviceRole.LeftHanded : InputDeviceRole.RightHanded;
        _controllerFlag = _controllerNode == XRNode.LeftHand ? 0 : 1;
    }
    
    private void OnEnable()
    {
        if (_boardTarget != null)
        {
            _boardTarget.SetActive(true);
            StartCoroutine(UpdateVisibility());
        }
        _joystick2DAxisTouchAction.Enable();
        _joystick2DAxisAction.Enable();
        _joystick2DAxisClickAction.Enable();

        _joystick2DAxisTouchAction.started += OnTouchStarted;
        _joystick2DAxisTouchAction.canceled += OnTouchCanceled;
        _joystick2DAxisAction.performed += OnMovePerformed;
        _joystick2DAxisAction.canceled += OnMoveCanceled;
        _joystick2DAxisClickAction.performed += OnPressPerformed;
        _joystick2DAxisClickAction.canceled += OnPressCanceled;
    }

    private void OnDisable()
    {
        _joystick2DAxisTouchAction.Disable();
        _joystick2DAxisAction.Disable();
        _joystick2DAxisClickAction.Disable();
        _joystick2DAxisTouchAction.started -= OnTouchStarted;
        _joystick2DAxisTouchAction.canceled -= OnTouchCanceled;
        _joystick2DAxisAction.performed -= OnMovePerformed;
        _joystick2DAxisAction.canceled -= OnMoveCanceled;
        _joystick2DAxisClickAction.performed -= OnPressPerformed;
        _joystick2DAxisClickAction.canceled -= OnPressCanceled;
    }

    private IEnumerator UpdateVisibility()
    {
        while (isActiveAndEnabled)
        {
            if (_joystick2DAxisAction is { controls: { Count: > 0 } }
                && _joystick2DAxisAction.controls[0].device != null
                && OpenXRInput.TryGetInputSourceName(_joystick2DAxisAction, 0, out var actionName,
                    OpenXRInput.InputSourceNameFlags.Component, _joystick2DAxisAction.controls[0].device))
            {
                VLog.Info("VoiceHandleBoard", "UpdateVisibility  true");
                _boardTarget.SetActive(true);
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void Start()
    {
        _mTransform = transform;
        _imeDelegate = _virtualKeyboard.GetComponentInChildren<ImeDelegateBase>();
        
        if (_touchImage)
        {
            _touchImage.color = _normalColor;
        }
        
        if (_pressImage)
        {
            _pressImage.color = _normalColor;
        }
        
        if (_moveImage)
        {
            _moveImage.color = _normalColor;
        }

    }
    
    void Update()
    {
        if (!DefaultImeDeviceInfo.ExistDeviceByRole(_controllerRole))
        {
            if (_mShow)
            {
                VLog.Info("VoiceHandleBoard","Update  ExistDeviceByRole Hide");
                Hide();
            }
        }
        if (TryGetControllerPose())
        {
            _mTransform.position = _controllerPose.position;
            _mTransform.rotation = _controllerPose.rotation;
        }
    }

    private bool TryGetControllerPose()
    {
        if (_controllerFlag == 0)
        {
            if (_virtualKeyboard.LeftZAxisGameObject)
            {
                _controllerPose.position = _virtualKeyboard.LeftZAxisGameObject.transform.position;
                _controllerPose.rotation = _virtualKeyboard.LeftZAxisGameObject.transform.rotation;
            }
            else
            {
                try
                {
                    _controllerPose.position = _virtualKeyboard._leftRayPositionUpdateAction.ReadValue<Vector3>();
                    _controllerPose.rotation = _virtualKeyboard._leftRayRotationUpdateAction.ReadValue<Quaternion>();
                }
                catch (Exception e)
                {
                    VLog.Warning("VirtualKeyboard not get left controller pointer position/rotation");
                    return false;
                }
            }
        }
        else
        {
            if (_virtualKeyboard.RightZAxisGameObject)
            {
                _controllerPose.position = _virtualKeyboard.RightZAxisGameObject.transform.position;
                _controllerPose.rotation = _virtualKeyboard.RightZAxisGameObject.transform.rotation;
            }
            else
            {
                try
                {
                    _controllerPose.position = _virtualKeyboard._rightRayUpdatePositionAction.ReadValue<Vector3>();
                    _controllerPose.rotation = _virtualKeyboard._rightRayUpdateRotationAction.ReadValue<Quaternion>();
                }
                catch (Exception e)
                {
                    VLog.Warning("VirtualKeyboard not get right controller pointer position/rotation");
                    return false;
                }
            }
        }

        return true;
    }
  
    #region SubDevices event

    public void Show(VXRPlugin.HandlerPhase handlerPhase)
    {
        VLog.Info("VoiceHandleBoard","Show");
        if (handlerPhase >= VXRPlugin.HandlerPhase.HANDLER_PHASE_BUTT ||
            !DefaultImeDeviceInfo.ExistDeviceByRole(_controllerRole))
        {
            return;
        }
        
        for (int i = 0; i < _bgImages.Length; i++)
        {
            if (i==(int)handlerPhase)
            {
                _bgImages[i].SetActive(true);
            }
            else
            {
                _bgImages[i].SetActive(false);
            }
           
        }
        if (_touchImage)
        {
            _touchImage.enabled = true;
        }

        if (_pressImage)
        {
            _pressImage.enabled = true;
        }

        if (_moveImage)
        {
            _moveImage.enabled = true;
        }
        _mShow = true;
    }
    
    public void Hide()
    {
        VLog.Info("VoiceHandleBoard","Hide");
        foreach (var bgImage in _bgImages)
        {
            bgImage.SetActive(false);
        }

        _mShow = false;
        if (_touchImage)
        {
            _touchImage.enabled = false;
        }
        
        if (_pressImage)
        {
            _pressImage.enabled = false;
        }
        
        if (_moveImage)
        {
            _moveImage.enabled = false;
        }

    }
    #endregion

    #region input event
    private void OnTouchStarted(InputAction.CallbackContext info)
    {
        _touchImage.color = _pressColor;
    }

    private void OnTouchCanceled(InputAction.CallbackContext info)
    {
        _touchImage.color = _normalColor;
    }

    private void OnMovePerformed(InputAction.CallbackContext info)
    {
        if (_imeDelegate == null)
        {
            return;
        }
        VXRPlugin.ImeDeviceCommandType commandType = ValueToCommand(info);
        _imeDelegate.OnCommand(commandType);
        if (_moveImage != null && commandType != VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT)
        {
            _moveImage.color = _pressColor;
        }
        UpdateHandle(info);
    }

    private void OnMoveCanceled(InputAction.CallbackContext info)
    {
        _moveImage.color = _normalColor;
        UpdateHandle(info);
        if (_imeDelegate == null) return;
        _imeDelegate.OnCommand(VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_RELEASE);;
    }

    private void OnPressPerformed(InputAction.CallbackContext info)
    {
        if (_imeDelegate == null)
        {
            return;
        }
        VXRPlugin.ImeDeviceCommandType commandType = ValueToCommand(info);
        _imeDelegate.OnCommand(commandType);
        if (_pressImage != null && commandType != VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT)
        {
            _pressImage.color = _pressColor;
        }
    }

    private void OnPressCanceled(InputAction.CallbackContext info)
    {
        _pressImage.color = _normalColor;
        if (_imeDelegate == null) return;
        _imeDelegate.OnCommand(VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_RELEASE);;
    }
    #endregion
    
    private VXRPlugin.ImeDeviceCommandType ValueToCommand(InputAction.CallbackContext ctx)
    {
        try
        {
            var v = ctx.ReadValue<float>();
            return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_PRESS;
        }
        catch (System.Exception e)
        {
            VLog.Info("Input not is Joystick Press");
        }

        try
        {
            var v2 = ctx.ReadValue<Vector2>();
            return Vector2ToImeDeviceCommandType(ConvertToNearBeside(v2[0]), ConvertToNearBeside(v2[1]));
        }
        catch (System.Exception e)
        {
            VLog.Info("Input not is Joystick Move");
        }
        return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT;
    }

    private int ConvertToNearBeside(float v)
    {
        if (v > 0)
        {
            if (v > k_convertNearBeside)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (v < -k_convertNearBeside)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    private VXRPlugin.ImeDeviceCommandType Vector2ToImeDeviceCommandType(int v1, int v2)
    {
        if (v1 == 0 && v2 == 1)
            return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_UP;
        if (v1 == 0 && v2 == -1)
            return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_DOWN;
        if (v1 == -1 && v2 == 0)
            return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_LEFT;
        if (v1 == 1 && v2 == 0)
            return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_RIGHT;
        return VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT;
    }

    private void UpdateHandle(InputAction.CallbackContext ctx)
    {
        _handle.anchorMin = _handle.anchorMax = (ctx.ReadValue<Vector2>()*new Vector2(0.5f,1.0f) + Vector2.one) * 0.5f;
    }
}