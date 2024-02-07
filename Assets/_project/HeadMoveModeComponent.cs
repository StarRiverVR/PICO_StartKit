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
  

public class HeadMoveModeComponent : MonoBehaviour
{
    public InputActionReference triggerActionReference;
    public GameObject OriginGameObject;
    private Vector3 ReferencePosition;
    private void OnEnable()
    {
    
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
        Debug.Log(GameObject.Find("ReferencePoint").transform.forward);


    }
    // Start is called before the first frame update
    void Start()
    {
        this.ReferencePosition = this.OriginGameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //this.OriginGameObject
        
    }
}
