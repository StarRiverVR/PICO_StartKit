using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using Unity.XR;
using UnityEngine.XR;
using TMPro;

using LightBand;



public enum ControlState
{
    none,
    move_forward,
    move_backward,
    move_turn_left,
    move_turn_right,
    turn_left,
    turn_right
}

public interface IControlMode {
    public bool IsAttaching { get; set; }
    public Vector3 Direction { get; set; }
    public void Diff(Vector3 currentPosition, Vector3 originalPosition);
    public void Move(GameObject controller, GameObject referencePoint, GameObject gameObject);
    public ControlState State { get; set; }

    // 代表了该模式下的倍数
    public float Multiplier { get; set; }
    
}

public class ControlModelBase:IControlMode
{
    public bool IsAttaching { get; set; }
    public  Vector3 Direction { get; set; }
    public ControlState State { get; set; }

    // 代表了该模式下的值
    public float Multiplier { get; set; }

    public void Diff(Vector3 currentPosition, Vector3 originalPosition) {
        this.Direction = currentPosition - originalPosition;
    }
    public virtual void Move(GameObject controller, GameObject referencePoint, GameObject gameObject) {
    }
}


// 前后 90 度作为前后移动，左右各 90 度作为转向+移动
public class Mode1: ControlModelBase
{
    public override void Move(GameObject controller, GameObject referencePoint,GameObject gameObject) {
        var a1 = controller.transform.position - referencePoint.transform.position;
        a1.y = 0;
        var a2 = referencePoint.transform.forward;

        var direction = a1;
        var angle = Vector3.Angle(a1, a2);
        var cross = Vector3.Cross(a1, a2);

        if (direction.magnitude < 0.03) {
            this.State = ControlState.none;
            return;
        }
        else if (direction.magnitude < 0.08) this.Multiplier = 1;
        else if (direction.magnitude > 0.08) this.Multiplier = 2;

        float positionChangeRate = 0;
        float rotationChangeRate = 0;

        if (angle < 35) {
            this.State = ControlState.move_forward;
            positionChangeRate = 0.01f;

        }
        else if(angle < 145)
        {
            if (cross.y > 0)
            {
                this.State = ControlState.move_turn_left;
                rotationChangeRate = - 0.003f;
            }
            else
            {
                this.State = ControlState.move_turn_right;
                rotationChangeRate = 0.003f;
            }
            positionChangeRate = 0.005f;

        }
        else
        {
            this.State = ControlState.move_backward;
            positionChangeRate = 0.006f;

        }

        gameObject.transform.position += direction.normalized * positionChangeRate * this.Multiplier;
        gameObject.transform.Rotate(gameObject.transform.up, angle * rotationChangeRate * this.Multiplier);
    }

}

// 使用头部进行移动和转向
public class Mode2 : ControlModelBase
{
    public override void Move(GameObject controller, GameObject referencePoint, GameObject gameObject)
    {
        var direction = controller.transform.position - referencePoint.transform.position;
        direction.y = 0;

        var a1 = controller.transform.up;
        var a2 = referencePoint.transform.forward;

        var angle = Vector3.Angle(a1, a2);
        var cross = Vector3.Cross(a1, a2);

        if (direction.magnitude < 0.08)
        {
            this.State = ControlState.none;
            return;
        }
        else if (direction.magnitude < 0.12) this.Multiplier = 1;
        else if (direction.magnitude > 0.12) this.Multiplier = 2;

        float positionChangeRate = 0;
        float rotationChangeRate = 0;

        if (Vector3.Project(direction, referencePoint.transform.right).magnitude < 0.08 || angle < 10)
        {
            this.State = ControlState.move_forward;
            positionChangeRate = 0.01f;

        }
        else
        {
            if (cross.y > 0)
            {
                this.State = ControlState.move_turn_left;
                rotationChangeRate = -0.003f;
            }
            else
            {
                this.State = ControlState.move_turn_right;
                rotationChangeRate = 0.003f;
            }
            positionChangeRate = 0.005f;

        }

        gameObject.transform.position += direction.normalized * positionChangeRate * this.Multiplier;
        gameObject.transform.Rotate(gameObject.transform.up, angle * rotationChangeRate * this.Multiplier);
    }

}

public class Mode3 : ControlModelBase
{
   
}

