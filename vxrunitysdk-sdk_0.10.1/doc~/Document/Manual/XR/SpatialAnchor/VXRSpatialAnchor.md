#空间锚点
空间锚点技术可以将虚拟环境中的位置与真实世界中的位置进行锚定，用于将虚拟对象锚定到现实世界的位置或物体上。锚定后，使用同一设备多次进入相同的空间后，都可以在锚点位置看到虚拟物体。


## 基本概念
| 名称 | 说明 |
| --- | --- |
| UUID | 通用唯一识别码，空间锚点的唯一标识，在创建空间锚点时分配。 |
| Handle | 空间锚点句柄，在创建空间锚点时分配 |
| taskId | 任务ID。异步调用接口时任务唯一ID。以创建空间锚点为例，调用 ***CreateSpatialAnchor*** 接口创建锚点后，会立刻返回一个 taskId。此外，异步推送的 ***CreatedComplete*** 事件中也会携带一个 taskId。若两个 taskId 相同，说明该锚点创建成功。若你调用CreateAnchorEntity 接口创建了锚点 1、2、3，但 ***CreatedComplete*** 事件可能以锚点 3、2、1 为顺序进行推送，此时需要使用 taskId 来匹配锚点。 |



## 相关事件 
``` CSharp
// 创建空间锚点
public static event Action<CreateAsynResult> CreatedComplete;
// 空间锚点持久化
public static event Action<SaveAsynResult> SaveComplete;
// 空间锚点反持久化
public static event Action<UnsaveAsynResult> UnsaveComplete;
// 清理空间锚点
public static event Action<UnsaveAllAsynResult> UnsaveAllComplete;
// 加载空间锚点
public static event Action<LoadAsynResult> LoadComplete;
```

## 创建空间锚点
调用 CreateSpatialAnchor 接口，在应用的内存中创建空间锚点。每个锚点都拥有唯一的 UUID 和 Handle。后续进行空间锚点相关操作时，可通过 Handle 指定空间锚点。
``` CSharp
public static bool VXRSpatialAnchor.CreateSpatialAnchor(Vector3 position, Quaternion rotatioin, ref int taskId)
```
监听 CreatedComplete 事件，事件信息如下
``` CSharp
public struct CreateAsynResult
{
    public ulong taskId;// 任务ID
    public bool result;// 是否成功
    public ulong handle;// 空间锚点句柄
    public Guid uuid;//空间锚点唯一标识符
}
```
示例代码
``` CSharp
private void CreateAnchor(Vector3 position, Quaternion rotatioin)
{
    VXRSpatialAnchor.CreatedComplete += OnAnchorCreatedEvent;
    VXRSpatialAnchor.CreateSpatialAnchor(position, rotation, out var taskId);
}

private void OnAnchorCreatedEvent(CreateAsynResult result)
{
    if(result.result)
    {
        // TODO 创建成功
    }  
}
```

## 销毁空间锚点
调用 DestroySpatialAnchor 接口，销毁应用内存中的空间锚点。若创建空间锚点之后立刻销毁，则无法找回该锚点；若创建后将该空间锚点持久化，销毁锚点后，可以通过加载相关接口加载该空间锚点。
``` CSharp
public static ulong[] VXRSpatialAnchor.DestroySpatialAnchor(ulong[] handles)
```
示例代码
``` CSharp
private void DestroyAnchor(ulong[] handles)
{
    ulong[] ret = VXRSpatialAnchor.DestroySpatialAnchor(handles);
    if(ret.Length>0)
    {
        // TODO 删除成功
    }
}

```

## 获取空间锚点坐标
调用 *GetSpatialAnchorPose* 接口获取空间锚点的实时位置，保证空间锚点的位置始终固定。若不获取，当用户在场景内移动时，空间锚点的位置可能会发生偏移。接口调用频率可自行决定，建议 1 秒左右调用一次，最高支持每帧调用。
``` CSharp
public static bool GetSpatialAnchorPose(ulong handle, out Vector3 position, out Quaternion rotation)
```
示例代码
``` CSharp
private void GetAnchorPose(ulong handle)
{
    bool ret = VXRSpatialAnchor.GetSpatialAnchorPose(handle, out var pos, out var rot);
    if(ret)
    {
        // TODO 获取坐标成功
    }
}

```



## 空间锚点持久化事件
调用 SaveSpatialAnchor 接口，将锚点持久化。目前仅支持将锚点存储至 PICO 设备的本地内存中。
``` CSharp
public static bool SaveSpatialAnchor(ulong[] handles, out ulong taskId)
```
监听 SaveComplete 事件，事件信息如下
``` CSharp
public struct SaveAsynResult
{
    public ulong taskId;// 任务ID
    public bool result;// 是否成功
    public ulong[] handles;// 持久化成功的空间锚点句柄列表
}
```
示例代码
``` CSharp
private void SaveAnchor(ulong[] handles)
{
    VXRSpatialAnchor.SaveComplete += OnAnchorSaveEvent;
    VXRSpatialAnchor.SaveSpatialAnchor(handles, out var taskId);
}

private void OnAnchorSaveEvent(SaveAsynResult result)
{
    if(result.result)
    {
        // TODO 持久化成功
    }  
}
```


## 空间锚点反持久化
调用 UnsaveSpatialAnchor 接口，删除设备本地内存中存储的指定空间锚点。删除后的空间锚点将无法被加载。
``` CSharp
public static bool UnsaveSpatialAnchor(ulong[] handles, out ulong taskId)
```
监听 UnsaveComplete 事件，事件信息如下
``` CSharp
public struct UnsaveAsynResult
{
    public ulong taskId;// 任务ID
    public bool result;// 是否成功
    public ulong[] handles;// 反持久化成功的空间锚点句柄列表
}
```
示例代码
``` CSharp
private void UnsaveAnchor(ulong[] handles)
{
    VXRSpatialAnchor.UnsaveComplete += OnAnchorUnsaveEvent;
    VXRSpatialAnchor.UnsaveSpatialAnchor(handles, out var taskId);
}

private void OnAnchorUnsaveEvent(UnsaveAnchorData result)
{
    if(result.result)
    {
        // TODO 反持久化成功
    }  
}
```

## 清除持久化空间锚点
调用 UnsaveAllSpatialAnchor 接口，删除设备本地内存中存储的所有空间锚点。删除后的空间锚点将无法被加载。
``` CSharp
public static bool UnsaveAllSpatialAnchor(out ulong taskId)
```
监听 UnsaveAllComplete 事件，事件信息如下
``` CSharp
public struct UnsaveAllAsynResult
{
    public ulong taskId;// 任务ID
    public bool result;// 是否成功
    public ulong[] handles;// 反持久化成功的空间锚点句柄列表
}
```
示例代码
``` CSharp
private void ClearAnchor(ulong[] handles)
{
    VXRSpatialAnchor.UnsaveAllComplete += OnAnchorUnsaveAllEvent;
    VXRSpatialAnchor.UnsaveAllSpatialAnchor(out var taskId);
}

private void OnAnchorUnsaveAllEvent(UnsaveAnchorData result)
{
    if(result.result)
    {
        // TODO 清除成功
    }  
}
```

## 加载空间锚点
调用 LoadSpatialAnchorByUuid 接口，获取锚点的 UUID。不传入任何 UUID，则加载所有锚点。
``` CSharp
public static bool LoadSpatialAnchorByUuid(Guid[] uuids, out ulong taskId)
```
监听 LoadComplete 事件，事件信息如下
``` CSharp
public struct LoadAsynResult
{
    public ulong taskId;// 任务ID
    public bool result;// 是否成功
    public ulong[] handles;//加载成功的空间锚点句柄列表
    public Guid[] uuids;// 加载成功的空间锚点唯一ID列表
}
```
示例代码
``` CSharp
private void LoadAnchor(Guid[] uuids)
{
    VXRSpatialAnchor.LoadComplete += OnAnchorLoadEvent;
    VXRSpatialAnchor.LoadSpatialAnchorByUuid(uuids, out var taskId);
}

private void OnAnchorLoadEvent(LoadAsynResult result)
{
    if(result.result)
    {
        // TODO 加载成功
    }  
}
```


## 查询空间锚点UUID
调用 GetSpatialAnchorUuid 接口，获取锚点的 UUID。
``` CSharp
public static bool GetSpatialAnchorUuid(ulong handle, out Guid uuid)
```
示例代码
``` CSharp
private bool GetAnchorUUID(ulong handle, out Guid uuid)
{
    return VXRSpatialAnchor.GetSpatialAnchorUuid(handle, out uuid);
}

```


### 注意事项
* 创建锚点需要在可视范围内
* 空间锚点创建位置不宜超过距离远点半径3米以内


<details>
<summary>
引用
</summary>

* [VXRSpaitalAnchor](../../../API/XR/Operation/SpatialAnchor/VXRSpatialAnchor.md)
</details>