using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LightBand;

public class HeadMoveIndicator : MonoBehaviour
{
    private GameObject CurrentIndicator;
    public Material ActiveMat;
    public Material NormalMat;
    // Start is called before the first frame update
    void Start()
    {
        this.CurrentIndicator = this.transform.Find("none").gameObject;
    }

    private void OnEnable()
    {
        MessageCenter.Register(MessageTypes.ControlModeUpdate, this.OnControlModeUpdate);
        //MessageCenter.SendMessage(MessageTypes.ShowMessage, "Trigger button was pressed.");
    }
    private void OnDisable()
    {
        MessageCenter.UnRegister(MessageTypes.ControlModeUpdate, this.OnControlModeUpdate);
        //MessageCenter.SendMessage(MessageTypes.ShowMessage, "Trigger button was pressed.");
    }

    private void OnControlModeUpdate(IControlMode controlMode) {
        var activeIndicatorName = controlMode.State.ToString();
        if (controlMode.Multiplier == 2) {
            activeIndicatorName += "_2";
        }

        this.CurrentIndicator.GetComponent<MeshRenderer>().material = this.NormalMat;


        this.CurrentIndicator = this.transform.Find(activeIndicatorName).gameObject;
        this.CurrentIndicator.GetComponent<MeshRenderer>().material = this.ActiveMat;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
