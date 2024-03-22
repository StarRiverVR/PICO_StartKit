## 命名空间
>com.vivo.openxr


## 类名
```CSharp
public class VXRInputModule : PointerInputModule
```

眼动追踪UI交互器

-----------------------
## 父类
* [PointerInputModule](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerInputModule.html)

## 成员变量
变量名 | Editor | Static | 类型 | 描述
------ | ------ | ------ | ------ | ------ 
 rayTransform | true | false | [Transform](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Transform.html) | 眼动射线向量
 m_Cursor | true | false | [VXRCursor](VXRCursor.md) | 射线信息
 hand | true | false | [XRNode](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/XR.XRNode.html) | 眼动点击按键
 performSphereCastForGazepointer | true | false | bool | 眼动交互物理射线深度校正
 useRightStickScroll | true | false | bool | 是否启用摇杆进行滑动
 rightStickDeadZone | true | false | float | 摇杆滑动限制，防止意外滚动
 useSwipeScroll | true | false | bool | 启用触摸板滑动
 swipeDragThreshold | true | false | float | 最小滑动距离
 InvertSwipeXAxis | true | false | bool |  反转触摸板上的X轴
 swipeDragScale | true | false | float | 触摸板滑动单位滑动距离
 m_SpherecastRadius | true | false | float | 球面的半径