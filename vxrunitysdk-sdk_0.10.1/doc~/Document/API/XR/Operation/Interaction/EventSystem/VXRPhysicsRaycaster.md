## 命名空间
>com.vivo.openxr

## 类名
```CSharp
public class VXRPhysicsRaycaster : BaseRaycaster
```

EventSystem物理射线投射

---------------------

## 父类
* [BaseRaycaster](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.BaseRaycaster.html)

## 成员变量
变量名 | Editor | Static | 类型 | 描述
------ | ------ | ------ | ------ | ------ 
 eventMask | true | false | [LayerMask](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/LayerMask.html) | 投射交互层 
 sortOrder | true | false | int | 投射器优先级
 
## 成员方法
方法名 | Static | 描述
------ | ------ | ------
 [Raycast](#Raycast) | false | 射线投射检测
 [Spherecast](#Spherecast) | false | 球体投射检测
 [GetScreenPos](#GetScreenPos) | false | 获取屏幕位置

<span id="Raycast"></span>
**Raycast**

>```CSharp
>public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
>```
>
>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> eventData | [PointerEventData](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html) | 触摸事件
> resultAppendList | List\<[RaycastResult](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.RaycastResult.html)\> | 投射结果
>
>*返回值*
>* 无

<span id="Spherecast"></span>
**Spherecast**


```CSharp
public void Spherecast(PointerEventData eventData, List<RaycastResult> resultAppendList, float radius)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> eventData | [PointerEventData](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html) | 触摸事件
> resultAppendList | List\<[RaycastResult](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.RaycastResult.html)> | 投射结果
> radius | float | 球体半径
>
>*返回值*
>* 无

<span id="GetScreenPos"></span>
**GetScreenPos**

```CSharp
public Vector2 GetScreenPos(Vector3 worldPosition)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> worldPosition | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 世界坐标
>
>*返回值*
>* 屏幕坐标