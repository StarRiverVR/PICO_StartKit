﻿using System.Collections;
using System.Collections.Generic;
using Unity;
using UnityEngine;
using Unity.XR;
using UnityEngine.XR;
using TMPro;

using LightBand;

public interface IControlMode {
    public bool IsAttaching { get; set; }
    public Vector3 Direction { get; set; }
    public void Diff(Vector3 currentPosition, Vector3 originalPosition);
    public void Move(Vector3 direction, GameObject gameObject);
}

public class ControlModelBase:IControlMode
{
    public bool IsAttaching { get; set; }
    public  Vector3 Direction { get; set; }
    public void Diff(Vector3 currentPosition, Vector3 originalPosition) {
        this.Direction = currentPosition - originalPosition;
    }
    public virtual void Move(Vector3 direction, GameObject gameObject) { }
}


// 前后 90 度作为前后移动，左右各 90 度作为转向+移动
public class Mode1: ControlModelBase
{
    public override void Move(Vector3 direction, GameObject gameObject) {
        var a1 = direction;
        a1.y = 0;
        var a2 = gameObject.transform.forward;

        var angle = Vector3.Angle(a1, a2);




        if (angle < 35 || angle > 145) {
            gameObject.transform.position += direction.normalized * 0.01f;
        }
        else { 
            var cross =  Vector3.Cross(a1, a2);
            if (cross.y > 0) {
                gameObject.transform.Rotate(gameObject.transform.up, - angle * 0.003f);

            }
            else
            {
                gameObject.transform.Rotate(gameObject.transform.up,   angle * 0.003f);
            }

            gameObject.transform.position += direction.normalized * 0.007f;
        }
    }

}

// 使用头部进行移动和转向
public class Mode2 : ControlModelBase
{
    public override void Move(Vector3 direction, GameObject gameObject)
    {
        gameObject.transform.position = direction.normalized * 0.003f;
    }

}

public class Mode3 : ControlModelBase
{
    public override void Move(Vector3 direction, GameObject gameObject)
    {
    }

}

