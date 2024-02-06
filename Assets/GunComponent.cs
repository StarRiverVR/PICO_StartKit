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

public class GunComponent : MonoBehaviour
{
    public InputActionReference triggerActionReference;
    public GameObject Bullet;

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
        MessageCenter.SendMessage(MessageTypes.ShowMessage, "Trigger button was pressed.");
        var bullet = Instantiate(this.Bullet);
        var startPosition = this.transform.parent.Find("[Right Controller] Attach").transform.position;
      
        bullet.transform.position = startPosition;
      
        bullet.GetComponent<Rigidbody>().velocity = this.transform.forward * 6f;


     

        //bullet.transform.SetParent(this.transform.root);
        bullet.SetActive(true);


        // Trigger按钮被按下时执行的代码
        Debug.Log("Trigger button was pressed.");
        // 在这里添加你的逻辑
    }

    //public XRIDefaultInputActions _inputActions;
    // Start is called before the first frame update
    private void Awake()
    {
        //_inputActions = new XRIDefaultInputActions();
        //_inputActions.Enable();
    }

    void Start()
    {
  
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
