 ## 命名空间 
 >com.vivo.openxr
 
 ## 类名
 ```CSharp
 public class VXREyeTracking
 ```
 
眼动追踪类，处理眼动开启、关闭、实时获取眼动数据
 
 ---------------------
 

## 成员方法
名称 | static | 描述
------ | ------ | ------
 [StartEyeTracking](#StartEyeTracking) | true | 开启眼动追踪
 [StopEyeTracking](#StopEyeTracking) | true | 关闭眼动追踪
 [GetEyeGazeData](#GetEyeGazeData) | true | 获取眼动追踪数据


<span id="StartEyeTracking"></span>
**StartEyeTracking**

```CSharp
public static void StartEyeTracking()
```

>*参数列表*
>* 无
>
>*返回值*
>* 无

<span id="StopEyeTracking"></span>
**StopEyeTracking**

```CSharp
public static void StopEyeTracking()
```

>*参数列表*
>* 无
>
>*返回值*
>* 无

<span id="GetEyeGazeData"></span>
**GetEyeGazeData**

```CSharp
public static bool GetEyeGazeData(out EyeGazeData data)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> data | [EyeGazeData](EyeGazeData.md) | 眼动追踪位置信息
>
>*返回值*
>* 眼动追踪数据是否有效