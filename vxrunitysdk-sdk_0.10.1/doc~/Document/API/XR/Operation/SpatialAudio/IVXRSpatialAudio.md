## 命名空间
>com.vivo.openxr

## 声明
```CSharp
public class IVXRSpatialAudio
```

空间音频抽象类

---------------------


## 成员方法
方法名 | static | 描述
------ | ------ | ------ 
  [CreateSpatialAudio](#CreateSpatialAudio)| false | 创建空间音频实现类实例
  [DestorySpatialAudio](#DestorySpatialAudio)| false | 销毁空间音频实现类实例
  [CreateSource](#CreateSource)| false | 创建音源对象实例
  [DestroySource](#DestroySource)| false | 销毁音源对象实例
  [SetSourceVolume](#SetSourceVolume)| false | 设置音源的音量
  [SetSourceAttenuation](#SetSourceAttenuation)| false | 设置音源衰减信息
  [SetSourcePosition](#SetSourcePosition)| false | 设置音源的坐标
  [SetSourceRotation](#SetSourceRotation)| false | 设置音源的方向
  [SetSourceBuffer](#SetSourceBuffer)| false | 设置音源缓冲区
  [SetListenerPosition](#SetListenerPosition)| false | 设置听筒的坐标
  [SetListenerRotation](#SetListenerRotation)| false | 设置听筒的方向
  [GetListenerBuffer](#GetListenerBuffer)| false | 获取听筒缓冲区


<span id="CreateSpatialAudio"></span>
**CreateSpatialAudio**

```CSharp
public abstract bool CreateSpatialAudio(int channels,int framesBuffer, int sampleRate);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> channels | int | 输入音源通道数
> framesBuffer | int | 每帧处理的点数(每帧的缓存大小)
> sampleRate | int | 输入音源的采样率(Hz)
>
>*返回值*
>* 是否创建成功

<span id="DestorySpatialAudio"></span>
**DestorySpatialAudio**

```CSharp
public abstract void DestorySpatialAudio();
```

>*参数列表*
>* 无
>
>*返回值*
>* 无

<span id="CreateSource"></span>
**CreateSource**

```CSharp
public abstract int CreateSource(AudioRenderMode mode);
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
public abstract void DestroySource(int sourceId);
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
public abstract void SetSourceVolume(int sourceId, float vol);
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
public abstract void SetSourceAttenuation(int sourceId, AudioAttenuation attenuation);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> sourceId | int | 创建的声源对象ID
> attenuation | [AudioAttenuation](AudioAttenuation.md) | 衰减配置
>
>*返回值*
>* 无

<span id="SetSourcePosition"></span>
**SetSourcePosition**

```CSharp
public abstract void SetSourcePosition(int sourceId, Vector3 pos);
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
public abstract void SetSourceRotation(int sourceId, Quaternion rotation);
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
public abstract void SetSourceBuffer(int sourceId, float[] buffer, int channels, int frames);
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

<span id="SetListenerPosition"></span>
**SetListenerPosition**

```CSharp
public abstract void SetListenerPosition(Vector3 pos);
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
public abstract void SetListenerRotation(Quaternion rotation);
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
public abstract bool GetListenerBuffer(int channels, int frames, float[] buffer);
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> channels | int | 通道数
> channels | int | 每个通道的帧数
> frames | float[]  | 缓冲区数组指针
>
>*返回值*
>* 渲染成功返回true,渲染失败返回false