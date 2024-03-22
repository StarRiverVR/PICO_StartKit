 ## 命名空间
>com.vivo.openxr

 ## 类名
```CSharp
public class VXRGazeInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
```

实时更新眼动位置，为[VXRInputModule](../Interaction/EventSystem/VXRInputModule.md)提供眼动射线数据

--------------------------

 ## 父类
 * [MonoBehaviour](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/MonoBehaviour.html)
 * [IPointerEnterHandler](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IPointerEnterHandler.html)
 * [IPointerExitHandler](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IPointerExitHandler.html)
 * [IPointerClickHandler](https://docs.unity3d.com/Packages/com.unity.ugui@1.0/api/UnityEngine.EventSystems.IPointerClickHandler.html)

 ## 内部类

<span id="EyeTrackingEvent"></span>
```CSharp
public class EyeTrackingEvent : UnityEvent { }
```

 ## 成员变量

变量名 | Editor | Static | 类型 | 描述
------ | ------ | ------ | ------ | ------ 
 maxGazeTime | true | false | float | 触发凝视时间 
 onHoverEntered | true | false | [EyeTrackingEvent](#EyeTrackingEvent) | 注视开始事件 
 onClick | true | false | [EyeTrackingEvent](#EyeTrackingEvent) | 点击事件 
 onHoverExited | true | false | [EyeTrackingEvent](#EyeTrackingEvent) | 注视结束事件 
 onGaze | true | false | [EyeTrackingEvent](#EyeTrackingEvent) | 凝视事件 
