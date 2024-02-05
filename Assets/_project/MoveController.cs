using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using Unity.XR;
using UnityEngine.XR;
using TMPro;

using LightBand;



public class MoveController : MonoBehaviour
{
    private bool IsAttaching = false;

    private Vector3 defaultPostion;

    private GameObject relativeDefaultPosition;
 
    private Vector3 direction;
    private List<InputDevice> inputDevices = new List<InputDevice>();
    private InputDevice leftHandDevice;

    private IControlMode controlMode = new Mode1();

    private GameObject Origin;

    // Start is called before the first frame update
    void Start()
    {
        this.Origin = GameObject.Find("XR Origin");
        this.transform.parent.SetParent(this.Origin.transform);

        InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, inputDevices);
        if (this.inputDevices.Count > 0) {
            this.leftHandDevice = this.inputDevices[0];
        }

        Debug.Log("superwolf start");
    }

    private void OnTriggerEnter(Collider other) {
       if (other.name == "Left Hand")
           this.CheckBound(other);
    }

    private void OnTriggerExit(Collider other)
    {
        this.Reset();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.name == "Left Hand")
            this.CheckBound(other);
    }

    private void CheckBound(Collider other)
    {
        bool triggerValue;
        if (leftHandDevice.TryGetFeatureValue(UnityEngine.XR.CommonUsages.gripButton, out triggerValue) && triggerValue)
        {
            //this.ShowMessage("Trigger Value" + triggerValue);
            Debug.Log("Trigger button is pressed.");

            if (triggerValue)
            {
                this.IsAttaching = true;

                this.transform.position = new Vector3(other.transform.position.x, this.transform.position.y, other.transform.position.z);

                this.controlMode.Direction = this.transform.localPosition;

                
            }
            else
            {
                this.IsAttaching = false;
                this.Reset();
            }
        }
        //moveController.IsAttaching
    }


    private void Reset()
    {
        
        this.direction = Vector3.zero;
        this.IsAttaching = false;
        //this.transform.parent.SetParent(this.transform.root);
        this.transform.localPosition = Vector3.zero;

    }

    private void ShowMessage(string msg)
    {
        MessageCenter.SendMessage(MessageTypes.ShowMessage, msg);
    }


    // Update is called once per frame
    void Update()
    {
        if (this.IsAttaching) {
            this.controlMode.Move(this.controlMode.Direction, this.Origin);
            //this.Origin.transform.position += this.direction.normalized * 0.003f;
        }
    }
}
