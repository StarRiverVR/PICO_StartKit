using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using UnityEditor;
using Unity.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.EventSystems;

using LightBand;
using System;

public class ControlModeBaseBehaviour : MonoBehaviour
{
    public InputActionReference triggerActionReference;

    public GameObject Origin;

    [HideInInspector]
    public GameObject ReferenceGameObject;

    [HideInInspector]
    public IControlMode controlMode;

    public void ShowMessage(string msg)
    {
        MessageCenter.SendMessage(MessageTypes.ShowMessage, msg);
    }
}
