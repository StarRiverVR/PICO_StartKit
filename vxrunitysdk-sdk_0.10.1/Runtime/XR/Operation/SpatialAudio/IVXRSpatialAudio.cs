using com.vivo.codelibrary;
using System;
using System.Linq;
using UnityEngine;

namespace com.vivo.openxr
{
    public abstract class IVXRSpatialAudio
    {
        // Context
        /// <summary>
        ///     创建空间音频Context
        /// </summary>
        /// <param name="channels">输入音源通道数</param>
        /// <param name="framesBuffer">每帧处理的点数(每帧的缓存大小)</param>
        /// <param name="sampleRate">输入音源的采样率(Hz)</param>
        public abstract bool CreateSpatialAudio(int channels,int framesBuffer, int sampleRate);
        
        /// <summary>
        ///     销毁空间音频Context
        /// </summary>
        public abstract void DestorySpatialAudio();
        
        // Source
        
        /// <summary>
        ///     创建声音对象源实例
        /// </summary>
        /// <param name="mode">设置渲染模式</param>
        /// <returns>返回声源对象ID(sourceId)</returns>
        public abstract int CreateSource(VXRSpatialAudio.AudioRenderMode mode);
        
        /// <summary>
        ///     销毁声音对象源实例
        /// </summary>
        /// <param name="sourceId">创建的声源对象ID</param>
        public abstract void DestroySource(int sourceId);
        
        /// <summary>
        ///     设置给定源的音量
        /// </summary>
        /// <param name="sourceId">创建的声源对象ID</param>
        /// <param name="vol">数字增益大小</param>
        public abstract void SetSourceVolume(int sourceId, float vol);
        
        /// <summary>
        ///     设置音源衰减
        /// </summary>
        /// <param name="sourceId">创建的声源对象ID</param>
        /// <param name="attenuation">衰减配置</param>
        public abstract void SetSourceAttenuation(int sourceId, VXRSpatialAudio.AudioAttenuation attenuation);
       
        /// <summary>
        ///     设置给定声源的坐标
        /// </summary>
        /// <param name="sourceId">创建的声源对象ID</param>
        /// <param name="pos">音源的三维位置坐标</param>
        public abstract void SetSourcePosition(int sourceId, Vector3 pos);
        
        /// <summary>
        ///     设置给定声源的旋转
        /// </summary>
        /// <param name="sourceId">创建的声源对象ID</param>
        /// <param name="rotation">旋转四元素</param>
        public abstract void SetSourceRotation(int sourceId, Quaternion rotation);
        
        /// <summary>
        ///     设置Float格式的音源缓冲区
        /// </summary>
        /// <param name="sourceId">创建的声源对象ID</param>
        /// <param name="buffer">缓冲区数组指针</param>
        /// <param name="channels">通道数</param>
        /// <param name="frames">每个通道的帧数</param>
        public abstract void SetSourceBuffer(int sourceId, float[] buffer, int channels, int frames);

        // Listener
        
        /// <summary>
        ///     设置声音接收者坐标
        /// </summary>
        /// <param name="pos">声音接收者的三维位置坐标</param>
        public abstract void SetListenerPosition(Vector3 pos);
        
        /// <summary>
        ///     设置声音接收者旋转
        /// </summary>
        /// <param name="rotation">声音接收者旋转的四元数</param>
        public abstract void SetListenerRotation(Quaternion rotation);
        
        /// <summary>
        ///     渲染并输出Float格式音源缓冲区
        /// </summary>
        /// <param name="channels">通道数</param>
        /// <param name="frames">每个通道的帧数</param>
        /// <param name="buffer">缓冲区数组指针</param>
        /// <returns>渲染成功返回true,渲染失败返回false</returns>
        public abstract bool GetListenerBuffer(int channels, int frames, float[] buffer);
        
        /// <summary>
        /// 初始化静态房间模型
        /// </summary>
        /// <param name="length">房间对象对象尺寸-长</param>
        /// <param name="width">房间对象对象尺寸-宽</param>
        /// <param name="height">房间对象对象尺寸-高</param>
        /// <param name="materials">房间六面材质</param>
        public abstract void InitStaticRoom(float length, float width, float height, VXRSpatialAudio.ReflectionMaterial[] materials);
        
        /// <summary>
        /// 开启/关闭房间效果
        /// </summary>
        /// <param name="enable">开启/关闭</param>
        public abstract void SetStaticRoomEnable(bool enable);
        
        /// <summary>
        /// 设置房间反射参数
        /// </summary>
        /// <param name="scalar">用户定义的反射系数比例因子</param>
        public abstract void SetStaticRoomReflection(float scalar);
        
        /// <summary>
        /// 设置房间混响参数
        /// </summary>
        /// <param name="gain">用户定义的混响增益Gain</param>
        /// <param name="time">调整所有频带上的混响时间，RT60值乘以该系数，设置为1.0f时没有效果</param>
        /// <param name="brightness">混响明亮度</param>
        public abstract void SetStaticRoomReverb(float gain, float time, float brightness);
        
        /// <summary>
        /// 设置房间位置
        /// </summary>
        /// <param name="pos">房间对象三维位置坐标</param>
        /// <param name="rot">房间对象旋转四元数</param>
        public abstract void SetStaticRoomPose(Vector3 pos,Quaternion rot);
    }
    public class VXRSpatialAudioNative : IVXRSpatialAudio
    {
        IntPtr _context = IntPtr.Zero;

        #region// Context
        
        public override bool CreateSpatialAudio(int channels, int framesBuffer, int sampleRate)
        {
            VLog.Info("CreateSpatialAudio");
            if(_context==null || _context == IntPtr.Zero)
            {
                _context = VXRPlugin.CreateAudioContext(channels, framesBuffer, sampleRate);
            }
            return _context != IntPtr.Zero;
        }
        
        public override void DestorySpatialAudio()
        {
            VXRPlugin.DestroyAudioContext(_context);
            _context = IntPtr.Zero;
        }
        #endregion

        #region// Source
        
        public override int CreateSource(VXRSpatialAudio.AudioRenderMode mode)
        {
            return VXRPlugin.CreateAudioSource(_context, (VXRPlugin.AudioSourceRenderMode)mode);
        }
        
        public override void DestroySource(int sourceId)
        {
            VXRPlugin.DestroyAudioSource(_context, sourceId);
        }
        
        public override void SetSourceVolume(int sourceId, float vol)
        {
            VXRPlugin.SetAudioSourceVolume(_context, sourceId, vol);
        }
        
        public override void SetSourceAttenuation(int sourceId, VXRSpatialAudio.AudioAttenuation attenuation)
        {
            VXRPlugin.AudioSourceAttenuation cfg = new VXRPlugin.AudioSourceAttenuation();
            cfg.minDistance = attenuation.MinDistance;
            cfg.maxDistance = attenuation.MaxDistance;
            cfg.mode = attenuation.Mode;
            cfg.customParam = attenuation.CustomParam;
            VXRPlugin.SetAudioSourceAttenuation(_context, sourceId, cfg);
        }
        
        public override void SetSourcePosition(int sourceId, Vector3 pos)
        {
            VXRPlugin.SetAudioSourcePosition(_context, sourceId, pos);
        }
        
        public override void SetSourceRotation(int sourceId, Quaternion rotation)
        {
            VXRPlugin.SetAudioSourceRotation(_context, sourceId, rotation);
        }
        
        public override void SetSourceBuffer(int sourceId, float[] buffer_ptr, int channels, int frames)
        {
            VXRPlugin.SetAudioSourceBuffer(_context, sourceId, buffer_ptr, channels, frames);
        }
        #endregion

        #region// Listener
        public override void SetListenerPosition(Vector3 pos)
        {
            VXRPlugin.SetAudioListenerPosition(_context, pos);
        }
        public override void SetListenerRotation(Quaternion rotation)
        {
            VXRPlugin.SetAudioListenerRotation(_context, rotation);
        }
        public override bool GetListenerBuffer(int channels, int frames, float[] buffer)
        {
            return VXRPlugin.GetAudioListenerBuffer(_context, channels, frames, buffer);
        }
        #endregion

        #region// Room
        
        public override void InitStaticRoom(float length, float width, float height, VXRSpatialAudio.ReflectionMaterial[] materials)
        {
            VXRPlugin.ReflectionMaterial[] reflectionMaterials = materials.Select((v => (VXRPlugin.ReflectionMaterial)v)).ToArray();
            VXRPlugin.InitAudioRoom(_context,length, width, height, reflectionMaterials);
        }

        public override void SetStaticRoomEnable(bool enable)
        {
            VXRPlugin.SetAudioRoomEnable(_context, enable);
        }

        public override void SetStaticRoomReflection(float scalar)
        {
            VXRPlugin.SetAudioRoomReflection(_context, scalar);
        }

        public override void SetStaticRoomReverb(float gain, float time, float brightness)
        {
            VXRPlugin.SetAudioRoomReverb(_context, gain, time, brightness);
        }

        public override void SetStaticRoomPose(Vector3 pos, Quaternion rot)
        {
            VXRPlugin.SetAudioRoomPose(_context, pos.ToVector3f(), rot.ToQuatf());
        }
        #endregion
    }
}