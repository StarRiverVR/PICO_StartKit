## 命名空间
>com.vivo.openxr

## 声明
```CSharp
public class VXRSpatialAudio
```

空间音频管理类

---------------------


## 成员变量
变量名 | static |Editor | 类型| 描述
------ | ------ | ------ | ------ |  ------ 
 UseImplType | true | false  | [ImplType](ImplType.md) | 当前空间音频实现类型
 InstanceImpl | true | false  | [IVXRSpatialAudio](IVXRSpatialAudio.md) | 当前空间音频实现类实例
 Initialized | true | false  | bool | 是否初始化空间音频

## 成员方法
方法名 | static | 描述
------ | ------ | ------ 
  [CreateSpatialAudio](#CreateSpatialAudio)| true | 创建空间音频实现类实例
  [DestorySpatialAudio](#DestorySpatialAudio)| true | 销毁空间音频实现类实例
  [CreateSource](#CreateSource)| true | 创建音源对象实例
  [DestroySource](#DestroySource)| true | 销毁音源对象实例
  [SetSourceVolume](#SetSourceVolume)| true | 设置音源的音量
  [SetSourceAttenuation](#SetSourceAttenuation)| true | 设置音源衰减信息
  [SetSourcePose](#SetSourcePose)| true | 设置音源的位置
  [SetSourcePosition](#SetSourcePosition)| true | 设置音源的坐标
  [SetSourceRotation](#SetSourceRotation)| true | 设置音源的方向
  [SetSourceBuffer](#SetSourceBuffer)| true | 设置音源缓冲区
  [SetListenerPose](#SetListenerPose)| true | 设置听筒的位置
  [SetListenerPosition](#SetListenerPosition)| true | 设置听筒的坐标
  [SetListenerRotation](#SetListenerRotation)| true | 设置听筒的方向
  [GetListenerBuffer](#GetListenerBuffer)| true | 获取听筒缓冲区
  [InitRoom](#InitRoom)| true | 初始化房间配置
  [SetRoomEnable](#SetRoomEnable)| true | 设置房间启用/禁用状态
  [SetRoomReflection](#SetRoomReflection)| true | 设置房间反射因子
  [SetRoomReverb](#SetRoomReverb)| true | 设置房间混响参数
  [SetRoomPose](#SetRoomPose)| true | 设置房间位置



<span id="CreateSpatialAudio"></span>
**CreateSpatialAudio**

```CSharp
public bool CreateSpatialAudio(ImplType type, int channels,int framesBuffer, int sampleRate);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> type | [ImplType](ImplType.md) | 空间音频实现类型
> channels | int | 输入音源通道数
> framesBuffer | int | 每帧处理的点数(每帧的缓存大小)
> sampleRate | int | 输入音源的采样率(Hz)
>
>*返回值*
>* 是否创建成功


<span id="DestorySpatialAudio"></span>
**DestorySpatialAudio**

```CSharp
public void DestorySpatialAudio();
```

>*参数列表*
>* 无
>
>*返回值*
>* 无

<span id="CreateSource"></span>
**CreateSource**

```CSharp
public int CreateSource(AudioRenderMode mode);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> mode | [AudioRenderMode](AudioRenderMode.md) | 设置渲染模式
>
>*返回值*
>* 返回声源对象ID

<span id="DestroySource"></span>
**DestroySource**

```CSharp
public void DestroySource(int sourceId);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
>
>*返回值*
>* 无

<span id="SetSourceVolume"></span>
**SetSourceVolume**

```CSharp
public void SetSourceVolume(int sourceId, float vol);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> vol | float | 数字增益大小
>
>*返回值*
>* 无

<span id="SetSourceAttenuation"></span>
**SetSourceAttenuation**

```CSharp
public void SetSourceAttenuation(int sourceId, AudioAttenuation attenuation);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> attenuation | [AudioAttenuation](AudioAttenuation.md) | 衰减配置
>
>*返回值*
>* 无

<span id="SetSourcePose"></span>
**SetSourcePose**

```CSharp
public void SetSourcePose(int sourceId, Vector3 pos, Quaternion rot);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> pos | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 音源的三维位置坐标
> rotation | [Quaternion](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Quaternion.html) | 音源旋转四元素
>
>*返回值*
>* 无

<span id="SetSourcePosition"></span>
**SetSourcePosition**

```CSharp
public void SetSourcePosition(int sourceId, Vector3 pos);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> pos | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 音源的三维位置坐标
>
>*返回值*
>* 无

<span id="SetSourceRotation"></span>
**SetSourceRotation**

```CSharp
public void SetSourceRotation(int sourceId, Quaternion rotation);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> rotation | [Quaternion](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Quaternion.html) | 音源旋转四元素
>
>*返回值*
>* 无

<span id="SetSourceBuffer"></span>
**SetSourceBuffer**

```CSharp
public void SetSourceBuffer(int sourceId, float[] buffer, int channels, int frames);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> buffer | float[] | 缓冲区数组指针
> channels | int | 通道数
> frames | int | 每个通道的帧数
>
>*返回值*
>* 无

<span id="SetListenerPose"></span>
**SetListenerPose**

```CSharp
public void SetListenerPose(Vector3 pos, Quaternion rot);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> pos | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 声音接收者的三维位置坐标
> rot | [Quaternion](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Quaternion.html) | 声音接收者旋转的四元数
>
>*返回值*
>* 无

<span id="SetListenerPosition"></span>
**SetListenerPosition**

```CSharp
public void SetListenerPosition(Vector3 pos);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> pos | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 声音接收者的三维位置坐标
>
>*返回值*
>* 无

<span id="SetListenerRotation"></span>
**SetListenerRotation**

```CSharp
public void SetListenerRotation(Quaternion rotation);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> rotation | [Quaternion](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Quaternion.html) | 声音接收者旋转的四元数
>
>*返回值*
>* 无

<span id="GetListenerBuffer"></span>
**GetListenerBuffer**

```CSharp
public bool GetListenerBuffer(float[] buffer, int channels, int frames);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> buffer | float[]  | 缓冲区数组指针
> channels | int | 通道数
> frames | int | 每个通道的帧数
>
>*返回值*
>* 渲染成功返回true,渲染失败返回false

<span id="InitRoom"></span>
**InitRoom**

```CSharp
 public static void InitRoom(float length, float width, float height, ReflectionMaterial[] materials)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> length | float | 房间长度
> width | float | 房间宽度
> height | float | 房间高度
> length | [ReflectionMaterial](ReflectionMaterial.md) | 房间六面墙材质预设类型，数组长度为6的数组
>
>*返回值*
>* 无

<span id="SetRoomEnable"></span>
**SetRoomEnable**

```CSharp
public static void SetRoomEnable(bool enable)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> enable | bool | 启用/禁用状态
>
>*返回值*
>* 无

<span id="SetRoomReflection"></span>
**SetRoomReflection**

```CSharp
public static void SetRoomReflection(float scalar)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> scalar | float | 反射系数比例因子
>
>*返回值*
>* 无

<span id="SetRoomReverb"></span>
**SetRoomReverb**

```CSharp
public static void SetRoomReverb(float gain, float time, float brightness)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> gain | float | 混响增益
> time | float | 调整所有频带上的混响时间，RT60值乘以该系数，设置为1.0f时没有效果
> brightness | float | 混响明亮度
>
>*返回值*
>* 无

<span id="SetRoomPose"></span>
**SetRoomPose**

```CSharp
public static void SetRoomPose(Vector3 pos, Quaternion rot)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> pos | [Vector3](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Vector3.html) | 房间中心点坐标
> pos | [Quaternion](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Quaternion.html)  | 房间中心点方向
>
>*返回值*
>* 无