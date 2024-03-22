using com.vivo.codelibrary;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class KeyClick : MonoBehaviour
{
    [Flags]
    public enum Hand
    {
        Left = 0x0001,
        Right = 0x0010,
    }
    public enum InputFeature
    {
        primaryButton,
        secondaryButton,
        gripButton,
        triggerButton,
        menuButton
    }

    [Tooltip("左/右手按键")]
    [SerializeField]
    public Hand handFlag = Hand.Right;
    [Tooltip("按钮")]
    [SerializeField]
    InputFeature button = InputFeature.triggerButton;
    [Tooltip("绑定动作")]
    [SerializeField]
    InputActionProperty action;
    // 动作状态
    ActionState actionState;

    [Header("事件")]
    [Tooltip("事件")]
    [SerializeField]
    public UnityEvent<InputFeature> clickEvent;


    struct Device
    {
        public XRNode node;
        public UnityEngine.XR.InputDevice device;
    }
    List<Device> handDevices = new List<Device> ();

    private Dictionary<InputFeature, InputFeatureUsage<bool>> inputFeatureUsageMap = new Dictionary<InputFeature, InputFeatureUsage<bool>>() {
        { InputFeature.primaryButton, UnityEngine.XR.CommonUsages.primaryButton },
        { InputFeature.secondaryButton, UnityEngine.XR.CommonUsages.secondaryButton },
        { InputFeature.gripButton, UnityEngine.XR.CommonUsages.gripButton },
        { InputFeature.triggerButton, UnityEngine.XR.CommonUsages.triggerButton },
        { InputFeature.menuButton, UnityEngine.XR.CommonUsages.menuButton },
    };
    public struct ActionState
    {
        public void SetFrameState(bool isActive)
        {
            SetFrameState(isActive, isActive ? 1f : 0f);
        }
        public void SetFrameState(bool isActive, float newValue)
        {
            value = newValue;
            activatedThisFrame = !active && isActive;
            deactivatedThisFrame = active && !isActive;
            active = isActive;
        }
        public bool deactivatedThisFrame;
        public bool activatedThisFrame;
        public bool active;
        public float value;
    }

    private void OnEnable()
    {
        handDevices.Clear ();
        if (handFlag.HasFlag(Hand.Left))
        {
            AddHandDevice(XRNode.LeftHand);
        }
        if (handFlag.HasFlag(Hand.Right))
        {
            AddHandDevice(XRNode.RightHand);
        }

        if (action.reference == null)
        {
            action.action?.Enable();
        }

    }
    private void OnDisable()
    {
        handDevices.Clear();

        if (action.reference == null)
        {
            action.action?.Disable();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(isKeyClick(button))
        {
            clickEvent?.Invoke(button);
        }
    }
    private bool isKeyClick(InputFeature feature)
    {
        if (handDevices.Count > 0)
        {
            foreach (var device in handDevices)
            {
                if (IsHandKeyClick(device, feature))
                {
                    return true;
                }
            }
        }
        else if (action.action != null)
        {
            actionState.SetFrameState(IsActionPressed(action.action), 0);
            if (actionState.activatedThisFrame)
            {
                return true;
            }
        }
        return false;
    }


    void AddHandDevice(XRNode node)
    {
        var device = UnityEngine.XR.InputDevices.GetDeviceAtXRNode(node);
        if (device != null)
        {
            VLog.Info("KEYCLICK = " + device);
            Device hand = new Device();
            hand.node = node;
            hand.device = device;
            handDevices.Add(hand);
        }
    }
    Dictionary<string, bool> buttonStatus = new Dictionary<string, bool>();
    private bool IsHandKeyClick(Device handDevice, InputFeature feature)
    {
        string name = $"{handDevice.node}-{feature}";
        if (!buttonStatus.TryGetValue(name, out bool lastState))
        {
            lastState = false;
        }
        if (handDevice.device.TryGetFeatureValue(inputFeatureUsageMap[feature], out bool press))
        {
            buttonStatus[name] = press;

            if (press != lastState && lastState)
            {
                return true;
            }
        }

        return false;
    }

    protected virtual bool IsActionPressed(InputAction action)
    {
        if (action == null)
            return false;
        return action.phase == InputActionPhase.Performed;

    }
}
