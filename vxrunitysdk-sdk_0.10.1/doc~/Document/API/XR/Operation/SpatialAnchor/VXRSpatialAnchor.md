## 命名空间
>com.vivo.openxr

## 声明
```CSharp
public class VXRSpatialAnchor
```

描述

---------------------

## 成员变量
变量名 | static |Editor | 类型| 描述
------ | ------ | ------ | ------ |  ------ 
 CreatedComplete | true | false  | Action\<[CreateAsynResult](CreateAsynResult.md)\> | 创建空间锚点回调
 SaveComplete | true | false  | Action\<[SaveAsynResult](SaveAsynResult.md)\> | 空间锚点持久化回调
 UnsaveComplete | true | false  | Action\<[UnsaveAsynResult](UnsaveAsynResult.md)\> | 空间锚点去持久化回调
 UnsaveAllComplete | true | false  | Action\<[UnsaveAllAsynResult](UnsaveAllAsynResult.md)\> | 清理所有持久化空间锚点回调
 LoadComplete | true | false  | Action\<[LoadAsynResult](LoadAsynResult.md)\> | 加载空间锚点

## 成员方法
方法名 | static | 描述
------ | ------ | ------ 
 [CreateSpatialAnchor](#CreateSpatialAnchor)| true | 创建空间锚点
 [DestroySpatialAnchor](#DestroySpatialAnchor)| true | 销毁空间锚点
 [GetSpatialAnchorPose](#GetSpatialAnchorPose)| true | 获取空间锚点坐标信息
 [SaveSpatialAnchor](#SaveSpatialAnchor)| true | 空间锚点持久化事件
 [UnsaveSpatialAnchor](#UnsaveSpatialAnchor)| true | 空间锚点反持久化
 [UnsaveAllSpatialAnchor](#UnsaveAllSpatialAnchor)| true | 清理所有持久化空间锚点
 [LoadSpatialAnchorByUuid](#LoadSpatialAnchorByUuid)| true | 通过UUID加载空间锚点
 [GetSpatialAnchorUuid](#GetSpatialAnchorUuid)| true | 获取空间锚点UUID


<span id="CreateSpatialAnchor"></span>
**CreateSpatialAnchor**

```CSharp
public static bool CreateSpatialAnchor(Vector3 position, Quaternion rotatioin, ref ulong taskId)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> position| Vector3 | 空间锚点坐标
> rotatioin| Quaternion | 空间锚点方向
> taskId | ulong | 任务ID
>
> *返回值*
> * 是否开始创建空间锚点，true则taskId有效


<span id="DestroySpatialAnchor"></span>
**DestroySpatialAnchor**

```CSharp
public static ulong[] DestroySpatialAnchor(ulong[] handles)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> handles | ulong[] | 空间锚点句柄列表
>
> *返回值*
> * 删除成功的空间锚点句柄列表


<span id="GetSpatialAnchorPose"></span>
**GetSpatialAnchorPose**

```CSharp
public static bool GetSpatialAnchorPose(ulong handle, out Vector3 position, out Quaternion rotation)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> handle | ulong | 空间锚点句柄
> position | Vector3 | 空间锚点坐标
> orientation | Quaternion | 空间锚点方向
>
> *返回值*
> * 是否获取成功


<span id="SaveSpatialAnchor"></span>
**SaveSpatialAnchor**

```CSharp
public static bool SaveSpatialAnchor(ulong[] handles, out ulong taskId)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> handles | ulong[] | 空间锚点句柄列表
> taskId | ulong | 任务ID
>
> *返回值*
> * 是否开始执行持久化空间锚点，true则taskId有效


<span id="UnsaveSpatialAnchor"></span>
**UnsaveSpatialAnchor**

```CSharp
public static bool UnsaveSpatialAnchor(ulong[] handles, out ulong taskId)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> handles | ulong[] | 空间锚点句柄列表
> taskId | ulong | 任务ID
>
> *返回值*
> * 是否开始执行反持久化空间锚点，true则taskId有效


<span id="UnsaveAllSpatialAnchor"></span>
**UnsaveAllSpatialAnchor**

```CSharp
public static bool UnsaveAllSpatialAnchor(out ulong taskId)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> taskId | ulong | 任务ID
>
> *返回值*
> * 是否开始执行反持久化所有空间锚点，true则taskId有效

<span id="LoadSpatialAnchorByUuid"></span>
**LoadSpatialAnchorByUuid**

```CSharp
public static bool LoadSpatialAnchorByUuid(Guid[] uuids, out ulong taskId)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> uuids | Guid[] | 空间锚点唯一标志符列表
> taskId | ulong | 任务ID
>
> *返回值*
> * 是否加载成功

<span id="GetSpatialAnchorUuid"></span>
**GetSpatialAnchorUuid**

```CSharp
public static bool GetSpatialAnchorUuid(ulong handle, out Guid uuid)
```

> *参数列表*
> 参数 | 类型 | 描述
> ------ | ------ | ------
> handle | ulong | 空间锚点句柄
> uuid | Guid | 空间锚点唯一标志符
>
> *返回值*
> * 是否获取UUID成功

