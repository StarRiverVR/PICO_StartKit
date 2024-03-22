using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class QuitApp : MonoBehaviour
{
    [Flags]
    public enum Hand
    {
        Left  = 0x0001,
        Right = 0x0010,
    }

    [SerializeField]
    public enum InputFeature
    {
        primaryButton,
        secondaryButton,
        gripButton,
        triggerButton,
        menuButton
    }

    private Dictionary<InputFeature, InputFeatureUsage<bool>> inputFeatureUsageMap = new Dictionary<InputFeature, InputFeatureUsage<bool>>() {
            { InputFeature.primaryButton, CommonUsages.primaryButton },
            { InputFeature.secondaryButton, CommonUsages.secondaryButton },
            { InputFeature.gripButton, CommonUsages.gripButton },
            { InputFeature.menuButton, CommonUsages.menuButton },
            { InputFeature.triggerButton, CommonUsages.triggerButton },
        };

    [SerializeField]
    InputFeature quitButton = InputFeature.menuButton;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isKeyClick(quitButton))
        {
            Application.Quit();
        }
    }

    [SerializeField]
    public Hand handFlag = Hand.Left;
    private bool isKeyClick(InputFeature feature)
    {
        if (handFlag.HasFlag(Hand.Left) && isHandKeyClick(XRNode.LeftHand, feature))
        {
            return true;
        }

        if(handFlag.HasFlag(Hand.Right) && isHandKeyClick(XRNode.RightHand, feature))
        {
            return true;
        }

        return false;

    }
    Dictionary<string, bool> buttonStatus = new Dictionary<string, bool>();
    private bool isHandKeyClick(XRNode hand, InputFeature feature)
    {
        string name = $"{hand}-{feature}";
        if (!buttonStatus.TryGetValue(name, out bool lastState))
        {
            lastState = false;
        }
        InputDevice handDevice = InputDevices.GetDeviceAtXRNode(hand);
        if (handDevice.TryGetFeatureValue(inputFeatureUsageMap[feature], out bool press))
        {
            buttonStatus[name] = press;

            if (press != lastState && lastState)
            {
                return true;
            }
        }

        return false;
    }
}
