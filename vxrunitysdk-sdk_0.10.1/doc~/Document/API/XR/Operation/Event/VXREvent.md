## 命名空间
>com.vivo.openxr

## 声明
```CSharp
public class VXREvent
```

OpenXR事件入口，用于注册事件监听并派发事件

---------------------
### 成员变量

变量名 | Editor | Static | 类型 | 描述
------ | ------ | ------ | ------ | ------ 
 CROWN_ROTATE_MAX_ANGLE  | false | true | int | 表冠归一化旋转旋转最大角度
 CROWN_ROTATE_MIN_ANGLE  | false | true | int | 表冠归一化旋转旋转最小角度



## 成员方法
方法名 | static | 描述
------ | ------ | ------ 
 [AddOpenXREnteringListener](#AddOpenXREnteringListener)| true | 添加 OpenXR Session 进入事件监听
 [RemoveOpenXREnteringListener](#RemoveOpenXREnteringListener)| true | 移除 OpenXR Session 进入事件监听
 [AddOpenXRExitingListener](#AddOpenXRExitingListener)| true | 添加 OpenXR Session 退出事件监听
 [RemoveOpenXRExitingListener](#RemoveOpenXRExitingListener)| true | 移除 OpenXR Session 退出事件监听
 [AddOpenXRFocusChangedListener](#AddOpenXRFocusChangedListener)| true | 添加 OpenXR Session 焦点变化事件监听
 [RemoveOpenXRFocusChangedListener](#RemoveOpenXRFocusChangedListener)| true | 移除 OpenXR Session 焦点变化事件监听
 [AddCrownRotateListener](#AddCrownRotateListener)| true | 添加表冠单次旋转事件
 [RemoveCrownRotateListener](#RemoveCrownRotateListener)| true | 移除注册表冠单次旋转事件
 [AddCrownRotate360Listener](#AddCrownRotate360Listener)| true | 添加表冠归一化旋转事件<br>旋转角范围为 -360度 ~ +360度
 [RemoveCrownRotate360Listener](#RemoveCrownRotate360Listener)| true | 移除注册表冠归一化旋转事件

 
<span id="AddOpenXREnteringListener"></span>  
**AddOpenXREnteringListener**

```CSharp
public static void AddOpenXREnteringListener(Action listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action | OpenXR Session 进入事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="RemoveOpenXREnteringListener"></span>  
**RemoveOpenXREnteringListener**

```CSharp
public static void RemoveOpenXREnteringListener(Action listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action | OpenXR Session 进入事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="AddOpenXRExitingListener"></span>  
**AddOpenXRExitingListener**

```CSharp
 public static void AddOpenXRExitingListener(Action listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action | OpenXR Session 退出事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="RemoveOpenXRExitingListener"></span>  
**RemoveOpenXRExitingListener**

```CSharp
 public static void RemoveOpenXRExitingListener(Action listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action | OpenXR Session 退出事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="AddOpenXRFocusChangedListener"></span>  
**AddOpenXRFocusChangedListener**

```CSharp
 public static void AddOpenXRFocusChangedListener(Action<bool> listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action\<bool> | OpenXR Session 焦点变化事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="RemoveOpenXRFocusChangedListener"></span>  
**RemoveOpenXRFocusChangedListener**

```CSharp
 public static void RemoveOpenXRFocusChangedListener(Action<bool> listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action\<bool> | OpenXR Session 焦点变化事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="AddCrownRotateListener"></span>  
**AddCrownRotateListener**

```CSharp
 public static void AddCrownRotateListener(Action<CrownRotateData> listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action\<[CrownRotateData](CrownRotateData.md)>| 表冠旋转事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="RemoveCrownRotateListener"></span>  
**RemoveCrownRotateListener**

```CSharp
 public static void RemoveCrownRotateListener(Action<CrownRotateData> listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action<[CrownRotateData](CrownRotateData.md)>| 表冠旋转事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="AddCrownRotate360Listener"></span>  
**AddCrownRotate360Listener**

```CSharp
 public static void AddCrownRotate360Listener(Action<CrownRotate360Data> listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action\<[CrownRotate360Data](CrownRotate360Data.md)>| OpenXR Session 表冠旋转事件回调函数
>
> *返回值*
> * 无

---------------------

<span id="RemoveCrownRotateListener"></span>  
**RemoveCrownRotateListener**

```CSharp
 public static void RemoveCrownRotateListener(Action<CrownRotate360Data> listener)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> listener| Action<[CrownRotate360Data](CrownRotate360Data.md)>| OpenXR Session 表冠旋转事件回调函数
>
> *返回值*
> * 无

---------------------
