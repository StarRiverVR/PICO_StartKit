using System;
using Unity;
using UnityEngine;

public static class Extensions
{
    public static Vector3 Rotate(this Vector3 vector, float angle)
    {
        Vector3 axis = Vector3.up; // 绕y轴旋转

        // 创建一个表示旋转的四元数
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);

        return rotation * vector;
    }
}

