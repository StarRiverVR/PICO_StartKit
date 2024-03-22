using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{
    public static class VXRSpatialAudio
    {
        /// <summary>
        /// 空间音频实现类型
        /// </summary>
        public enum ImplType
        {
            TypeDefault
        }

        /// <summary>
        /// 反射属性材质。
        /// </summary>
        public enum ReflectionMaterial
        {
            Transparent = VXRPlugin.ReflectionMaterial.Transparent,
            AcousticTile = VXRPlugin.ReflectionMaterial.AcousticTile,
            Brick = VXRPlugin.ReflectionMaterial.Brick,
            BrickPainted = VXRPlugin.ReflectionMaterial.BrickPainted,
            Carpet = VXRPlugin.ReflectionMaterial.Carpet,
            CarpetHeavy = VXRPlugin.ReflectionMaterial.CarpetHeavy,
            CarpetHeavyPadded = VXRPlugin.ReflectionMaterial.CarpetHeavyPadded,
            CeramicTile = VXRPlugin.ReflectionMaterial.CeramicTile,
            Concrete = VXRPlugin.ReflectionMaterial.Concrete,
            ConcreteRough = VXRPlugin.ReflectionMaterial.ConcreteRough,
            ConcreteBlock = VXRPlugin.ReflectionMaterial.ConcreteBlock,
            ConcreteBlockPainted = VXRPlugin.ReflectionMaterial.ConcreteBlockPainted,
            Curtain = VXRPlugin.ReflectionMaterial.Curtain,
            Foliage = VXRPlugin.ReflectionMaterial.Foliage,
            Glass = VXRPlugin.ReflectionMaterial.Glass,
            GlassHeavy = VXRPlugin.ReflectionMaterial.GlassHeavy,
            Grass = VXRPlugin.ReflectionMaterial.Grass,
            Gravel = VXRPlugin.ReflectionMaterial.Gravel,
            GypsumBoard = VXRPlugin.ReflectionMaterial.GypsumBoard,
            PlasterOnBrick = VXRPlugin.ReflectionMaterial.PlasterOnBrick,
            PlasterOnConcreteBlock = VXRPlugin.ReflectionMaterial.PlasterOnConcreteBlock,
            Soil = VXRPlugin.ReflectionMaterial.Soil,
            SoundProof = VXRPlugin.ReflectionMaterial.SoundProof,
            Snow = VXRPlugin.ReflectionMaterial.Snow,
            Steel = VXRPlugin.ReflectionMaterial.Steel,
            Water = VXRPlugin.ReflectionMaterial.Water,
            WoodThin = VXRPlugin.ReflectionMaterial.WoodThin,
            WoodThick = VXRPlugin.ReflectionMaterial.WoodThick,
            WoodFloor = VXRPlugin.ReflectionMaterial.WoodFloor,
            WoodOnConcrete = VXRPlugin.ReflectionMaterial.WoodOnConcrete
        }

        /// <summary>
        /// 音频渲染模式。
        /// </summary>
        public enum AudioRenderMode
        {
            HRTFDisable = VXRPlugin.AudioSourceRenderMode.HRTF_Disable, //禁用基于HRTF的渲染
            HRTFAmbisonics8 = VXRPlugin.AudioSourceRenderMode.HRTF_Ambisonics_8, //基于HRTF的渲染，使用一阶Ambisonics， 8个扬声器的虚拟阵列
            HRTFAmbisonics12 = VXRPlugin.AudioSourceRenderMode.HRTF_Ambisonics_12, //基于HRTF的渲染，二阶Ambisonics，12个扬声器的虚拟阵列
            HRTFAmbisonics26 = VXRPlugin.AudioSourceRenderMode.HRTF_Ambisonics_26, //基于HRTF的渲染，三阶Ambisonics，26个扬声器的虚拟阵列
            Room = VXRPlugin.AudioSourceRenderMode.Room //仅房间效果渲染。这将禁用基于HRTF的渲染和声音对象的直接（干）输出
        }

        /// <summary>
        /// 音源衰减配置。
        /// </summary>
        public struct AudioAttenuation
        {
            public float MinDistance;//衰减最小距离
            public float MaxDistance;//衰减最大距离
            public int Mode;//衰减模式
            public float CustomParam;//自定义参数
        }

        private static ImplType _useImplType = ImplType.TypeDefault;
        
        /// <summary>
        /// 当前空间音频实现类型。
        /// </summary>
        public static ImplType UseImplType
        {
            set
            {
                if (_instanceImpl != null)
                {
                    VLog.Error("Can not set ClientType. Spatial Audio Interface was initialization");
                    return;
                }
                _useImplType = value;
            }
            get => _useImplType;
        }

        private static IVXRSpatialAudio _instanceImpl = null;
        /// <summary>
        /// 当前空间音频实现类实例。
        /// </summary>
        public static IVXRSpatialAudio InstanceImpl
        {
            get
            {
                if (_instanceImpl == null)
                {
                    if (_useImplType == ImplType.TypeDefault)
                    {
                        _instanceImpl = new VXRSpatialAudioNative();
                    }
                    else
                    {
                        VLog.Warning($"Not Supported Spatial Audio Type[{_useImplType}]");
                    }

                    if (_instanceImpl == null)
                    {
                        _useImplType = ImplType.TypeDefault;
                        _instanceImpl = new VXRSpatialAudioNative();
                    }
                }
                return _instanceImpl;
            }
        }

        /// <summary>
        /// 是否初始化空间音频。
        /// </summary>
        public static bool Initialized { private set; get; } = false;
        /* Context */

        /// <summary>
        /// 创建空间音频实现类实例。
        /// </summary>
        /// <param name="type">空间音频实现类型</param>
        /// <param name="channels">输入音源通道数</param>
        /// <param name="framesBuffer">每帧处理的点数(每帧的缓存大小)</param>
        /// <param name="sampleRate">输入音源的采样率(Hz)</param>
        /// <returns>是否创建成功</returns>
        public static bool CreateSpatialAudio(ImplType type, int channels, int framesBuffer, int sampleRate)
        {
            UseImplType = type;
            Initialized = InstanceImpl.CreateSpatialAudio(channels, framesBuffer, sampleRate);
            return Initialized;
        }

        /// <summary>
        /// 销毁空间音频实现类实例。
        /// </summary>
        public static void DestroySpatialAudio()
        {
            VLog.Info("DestroySpatialAudio");
            InstanceImpl.DestorySpatialAudio();
        }

        /* Source */

        /// <summary>
        /// 创建音源对象实例。
        /// </summary>
        /// <param name="mode">音源渲染模式</param>
        /// <returns>音源ID</returns>
        public static int CreateSource(VXRSpatialAudio.AudioRenderMode mode)
        {
            return InstanceImpl.CreateSource(mode);
        }

        /// <summary>
        /// 销毁音源对象实例。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        public static void DestroySource(int sourceId)
        {
            InstanceImpl.DestroySource(sourceId);
        }

        /// <summary>
        /// 设置音源的音量。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        /// <param name="vol">音源音量，vol取值范围：[0, 1] 增益衰减，[1, inf)增益提升</param>
        public static void SetSourceVolume(int sourceId, float vol)
        {
            InstanceImpl.SetSourceVolume(sourceId, vol);
        }

        /// <summary>
        /// 设置音源衰减信息。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        /// <param name="attenuation">衰减信息。参考<see cref="VXRSpatialAudio.AudioAttenuation"/></param>
        public static void SetSourceAttenuation(int sourceId, VXRSpatialAudio.AudioAttenuation attenuation)
        {
            InstanceImpl.SetSourceAttenuation(sourceId, attenuation);
        }

        /// <summary>
        /// 设置音源的位置。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        /// <param name="pos">音源的坐标</param>
        /// <param name="rot">音源的方向</param>
        public static void SetSourcePose(int sourceId, Vector3 pos, Quaternion rot)
        {
            InstanceImpl.SetSourcePosition(sourceId, pos);
            InstanceImpl.SetSourceRotation(sourceId, rot);
        }

        /// <summary>
        /// 设置音源的坐标。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        /// <param name="pos">音源的坐标</param>
        public static void SetSourcePosition(int sourceId, Vector3 pos)
        {
            InstanceImpl.SetSourcePosition(sourceId, pos);
        }

        /// <summary>
        /// 设置音源的方向。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        /// <param name="rot">音源的方向</param>
        public static void SetSourceRotation(int sourceId, Quaternion rot)
        {
            InstanceImpl.SetSourceRotation(sourceId, rot);
        }

        /// <summary>
        /// 设置音源缓冲区。
        /// </summary>
        /// <param name="sourceId">音源ID</param>
        /// <param name="buffer">缓冲区数组</param>
        /// <param name="channels">通道数</param>
        /// <param name="frames">每个通道的帧数</param>
        public static void SetSourceBuffer(int sourceId, float[] buffer, int channels, int frames)
        {
            InstanceImpl.SetSourceBuffer(sourceId, buffer, channels, frames);
        }

        /* Listener */

        /// <summary>
        /// 设置听筒的位置。
        /// </summary>
        /// <param name="pos">听筒的坐标</param>
        /// <param name="rot">听筒的方向</param>
        public static void SetListenerPose(Vector3 pos, Quaternion rot)
        {
            InstanceImpl.SetListenerPosition(pos);
            InstanceImpl.SetListenerRotation(rot);
        }

        /// <summary>
        /// 设置听筒的坐标
        /// </summary>
        /// <param name="pos">听筒的坐标</param>
        public static void SetListenerPosition(Vector3 pos)
        {
            InstanceImpl.SetListenerPosition(pos);
        }

        /// <summary>
        /// 设置听筒的方向
        /// </summary>
        /// <param name="rot">听筒的方向</param>
        public static void SetListenerRotation(Quaternion rot)
        {
            InstanceImpl.SetListenerRotation(rot);
        }

        /// <summary>
        /// 获取听筒缓冲区。
        /// </summary>
        /// <param name="buffer">缓冲区数组</param>
        /// <param name="channels">通道数</param>
        /// <param name="frames">每个通道的帧数</param>
        /// <returns>渲染成功则返回Ture，否则返回False</returns>
        public static bool GetListenerBuffer(float[] buffer, int channels, int frames)
        {
            return InstanceImpl.GetListenerBuffer(channels, frames, buffer);
        }

        /* Room */

        /// <summary>
        /// 初始化房间配置。
        /// </summary>
        /// <param name="length">房间长度</param>
        /// <param name="width">房间宽度</param>
        /// <param name="height">房间高度</param>
        /// <param name="materials">房间六面墙材质预设类型，数组长度为6的数组</param>
        public static void InitRoom(float length, float width, float height, VXRSpatialAudio.ReflectionMaterial[] materials)
        {
            InstanceImpl.InitStaticRoom(length, width, height, materials);
        }

        /// <summary>
        /// 设置房间启用/禁用状态。
        /// </summary>
        /// <param name="enable">启用/禁用状态</param>
        public static void SetRoomEnable(bool enable)
        {
            InstanceImpl.SetStaticRoomEnable(enable);
        }

        /// <summary>
        /// 设置房间反射因子。
        /// </summary>
        /// <param name="scalar">反射系数比例因子</param>
        public static void SetRoomReflection(float scalar)
        {
            InstanceImpl.SetStaticRoomReflection(scalar);
        }

        /// <summary>
        /// 设置房间混响参数。
        /// </summary>
        /// <param name="gain">混响增益</param>
        /// <param name="time">调整所有频带上的混响时间，RT60值乘以该系数，设置为1.0f时没有效果</param>
        /// <param name="brightness">混响明亮度</param>
        public static void SetRoomReverb(float gain, float time, float brightness)
        {
            InstanceImpl.SetStaticRoomReverb(gain,time,brightness);
        }
        
        /// <summary>
        /// 设置房间位置。
        /// 注意：房间坐标是指长方体正中心点坐标。
        /// </summary>
        /// <param name="pos">房间中心点坐标</param>
        /// <param name="rot">房间中心点方向</param>
        public static void SetRoomPose(Vector3 pos, Quaternion rot)
        {
            InstanceImpl.SetStaticRoomPose(pos,rot);
        }
    }
}
