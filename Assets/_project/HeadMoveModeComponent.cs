using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using Unity.XR;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
using UnityEngine.EventSystems;

using LightBand;
using System;


public class HeadMoveModeComponent : ControlModeBaseBehaviour
{

    public SpawnManagerScriptableObject spawnManagerValues;

    private void Awake()
    {
        Debug.Log(spawnManagerValues.numberOfPrefabsToCreate);
        spawnManagerValues.numberOfPrefabsToCreate = 20;

        this.controlMode = new Mode2();
        this.ReferenceGameObject = new GameObject();
        this.ReferenceGameObject.transform.SetParent(this.transform.parent.parent);
    }
    private void OnEnable()
    {
        
        // XROrigin
        this.ReferenceGameObject.transform.position = this.transform.position;
    
        // 注册事件监听器
        triggerActionReference.action.started += OnTriggerPressed;
    }

    private void OnDisable()
    {
        // 移除事件监听器
        triggerActionReference.action.started -= OnTriggerPressed;
    }

    private void OnTriggerPressed(InputAction.CallbackContext context)
    {
        //this.ShootBullet();
        this.ResetReferencePoint();
        Debug.Log(GameObject.Find("ReferencePoint").transform.forward);


    }

    private void ResetReferencePoint()
    {
        this.ReferenceGameObject.transform.position = this.transform.position;
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        //this.OriginGameObject

        this.controlMode.Move(this.gameObject, this.ReferenceGameObject, this.Origin);
        MessageCenter.SendMessage(MessageTypes.ControlModeUpdate, this.controlMode);
        this.ShowMessage(this.controlMode.State + "  " + this.controlMode.Multiplier);
        this.ShowMessage(this.transform.up.ToString() + "+" + this.ReferenceGameObject.transform.up.ToString());
    }
}
