## 命名空间
>com.vivo.openxr

## 类名
```CSharp
abstract public class VXRCursor : MonoBehaviour
```

眼动UI射线

-------------------


## 父类
* [MonoBehaviour](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/MonoBehaviour.html)


## 成员方法
方法 | Static | 描述
------ | ------ | ------
[SetCursorRay](#StartTrack) | false | 设置UI射线
[SetCursorStartDest](#SetCursorStartDest) | false | 射线交互位置信息

<span id="SetCursorRay"></span>
**SetCursorRay**

```CSharp
public abstract void SetCursorRay(Transform ray);
```

>*参数列表*
>
>参数 | 类型 | 描述
>------ | ------ | ------
> ray | [Transform](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Transform.html) | UI射线节点
>
>*返回值*
>* 无


<span id="SetCursorStartDest"></span>
**SetCursorStartDest**

```CSharp
public abstract void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal);
```

>*参数列表*
>参数 | 类型 | 描述
>------ | ------ | ------
> start | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 射线起点坐标
> dest | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 碰撞点坐标
> normal | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 碰撞点法线
>
>*返回值*
>* 无