## 命名空间
>com.vivo.openxr.VXRSpatialAnchor

## 声明
```CSharp
public struct LoadAsynResult
```

加载空间锚点结果信息

---------------------

## 成员变量
变量名 | 类型| 描述
------ | ------ | ------
 taskId  | ulong | 任务ID
 result  | bool | 是否成功
 handles | ulong[] | 空间锚点句柄列表
 uuids | Guid[] | 空间锚点唯一标志符列表
