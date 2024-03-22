## 命名空间
>com.vivo.openxr

## 类名
```CSharp
public class VXRRaycaster : GraphicRaycaster, IPointerEnterHandler
```

基于[Canvas](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Canvas.html)的眼动射线检测

---------------------

## 父类
* [GraphicRaycaster](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.GraphicRaycaster.html)
* [IPointerEnterHandler](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.IPointerEnterHandler.html)


## 成员变量
名称 | Editor | Editor | 类型 | 描述
------ | ------ | ------ | ------ | ------
 sortOrder | true | false | int | 投射器优先级 

## 成员方法
名称 | Static | 描述
------ | ------ | ------
 [Raycast](#Raycast) | false | 获取眼动射线UI交互结果
 [GetScreenPosition](#GetScreenPosition) | false | 获取屏幕坐标
 [IsFocussed](#IsFocussed) | false | 是否为当前聚焦的Raycaster

<span id="Raycast"></span>
**Raycast**

```CSharp
private void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList, Ray ray, bool checkForBlocking)
```
>
>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> eventData | [PointerEventData](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.PointerEventData.html) | 触摸事件
> resultAppendList | List\<[RaycastResult](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.RaycastResult.html)> | 射线交互结果
> ray | [Ray](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Ray.html) | 射线
> checkForBlocking | bool | 是否检测画布阻塞(阻塞类型[BlockingObjects](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/UI.GraphicRaycaster.BlockingObjects.html))
>
>*返回值*
>* 无

<span id="GetScreenPosition"></span>
**GetScreenPosition**

```CSharp
public Vector2 GetScreenPosition(RaycastResult raycastResult)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> raycastResult | [RaycastResult](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/EventSystems.RaycastResult.html) | 射线交互结果
>
>*返回值*
>* 屏幕坐标

<span id="IsFocussed"></span>
**IsFocussed**

>```CSharp
>public bool IsFocussed()
>```
>
>*返回值*
>* 是否为当前聚焦的Raycaster
