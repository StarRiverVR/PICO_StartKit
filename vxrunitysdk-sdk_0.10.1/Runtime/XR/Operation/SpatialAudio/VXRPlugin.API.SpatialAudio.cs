using com.vivo.codelibrary;
using System;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        // Context
        public static IntPtr CreateAudioContext(int channels, int buffer, int sampleRate)
        {
            VLog.Info($"创建空间音频Context");
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported Spatial Audio");
            return IntPtr.Zero;
#else
            IntPtr intPtr = IntPtr.Zero;

            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                var ret = VXRVersion_0_7_0.vxr_CreateSpatializerContext(Spatializerlmpl.Goer, channels, buffer, sampleRate, ref intPtr);
                if (ret != Result.Success)
                {
                    VLog.Info($"创建空间音频Context失败[{ret}]");
                    return IntPtr.Zero;
                }
            }

            return intPtr;
#endif
        }
        
        internal static void DestroyAudioContext(IntPtr context)
        {
            VLog.Info($"销毁空间音频Context");
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning($"Not Supported Spatial Audio");
            return ;
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_DestroySpatializerContext(context);
            }
#endif
        }
        // Source
        
        public static int CreateAudioSource(IntPtr context, AudioSourceRenderMode mode)
        {
            VLog.Info($"CreateAudioSource");
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
            return -1;
#else
            int sourceId = -1;
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                Result ret = VXRVersion_0_7_0.vxr_CreateSpatializerSource(context, mode, out sourceId);
                if (ret != Result.Success)
                {
                    VLog.Error($"空间音频 创建音源失败[{ret}]");
                }
            }
            return sourceId;
#endif
        }
        
        public static void DestroyAudioSource(IntPtr context, int sourceId)
        {
            VLog.Info($"DestroyAudioSource");
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_DestroySpatializerSource(context, sourceId);
            }
#endif
        }
        
        public static void SetAudioSourceVolume(IntPtr context, int sourceId, float vol)
        {
            VLog.Info($"SetAudioSourceVolume");
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerSourceVolume(context, sourceId, vol);
            }
#endif
        }
        
        public static void SetAudioSourceAttenuation(IntPtr context, int sourceId, AudioSourceAttenuation attenuation)
        {
            VLog.Info($"SetSourceAtenation");

#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerSourceAttenuation(context, sourceId, attenuation);
            }
#endif
        }
        
        public static void SetAudioSourcePosition(IntPtr context, int sourceId, Vector3 pos)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerSourcePosition(context, sourceId, pos.ToVector3f());
            }
#endif
        }
        
        public static void SetAudioSourceRotation(IntPtr context, int sourceId, Quaternion rotation)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerSourceOrientation(context, sourceId, rotation.ToQuatf());
            }
#endif
        }
        
        public static void SetAudioSourceBuffer(IntPtr context, int sourceId, float[] buffer, int channels, int frames)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerSourceBuffer(context, sourceId, buffer, channels, frames);
            }
#endif
        }

        // Listener
        
        public static void SetAudioListenerPosition(IntPtr context, Vector3 pos)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerListenerPosition(context, pos.ToVector3f());
            }
#endif
        }
        
        public static void SetAudioListenerRotation(IntPtr context, Quaternion rot)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_7_0.vxr_SetSpatializerListenerOrientation(context, rot.ToQuatf());
            }
#endif
        }
        
        public static bool GetAudioListenerBuffer(IntPtr context, int channels, int frames, float[] buffer)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
            return false;
#else
            if (VXRVersion_0_7_0.version <= s_supportPluginVersion)
            {
                return VXRVersion_0_7_0.vxr_GetSpatializerListenerBuffer(context, channels, frames, buffer) == Result.Success;
            }
            return false;
#endif
        }

        // Room
        
        public static void InitAudioRoom(IntPtr context, float length, float width, float height, ReflectionMaterial[] materials)
        {

#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else
            if (VXRVersion_0_8_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_8_0.vxr_InitSpatializerStaticRoom(context, new SpatialAudioStaticRoomInfo()
                {
                    length = length,
                    width = width,
                    height = height,

                    left = materials[0],
                    right = materials[1],
                    down = materials[2],
                    up = materials[3],
                    front = materials[4],
                    back = materials[5],
                });
            }
#endif
        }
        
        public static void SetAudioRoomEnable(IntPtr context, bool enable)
        {

#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else

            if (VXRVersion_0_8_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_8_0.vxr_SetSpatializerStaticRoomEnable(context, enable);
            }
#endif
        }
        
        public static void SetAudioRoomReflection(IntPtr context, float scalar)
        {

#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else

            if (VXRVersion_0_8_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_8_0.vxr_SetSpatializerStaticRoomReflection(context, scalar);
            }
#endif
        }
        
        public static void SetAudioRoomReverb(IntPtr context, float gain, float time, float brightness)
        {

#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else

            if (VXRVersion_0_8_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_8_0.vxr_SetSpatializerStaticRoomReverb(context, gain, time, brightness);
            }
#endif
        }
        
        public static void SetAudioRoomPose(IntPtr context, Vector3f pos, Quatf rot)
        {
#if VXR_UNSUPPORTED_PLATFORM
            VLog.Warning("Not Supported Spatial Audio");
#else

            if (VXRVersion_0_8_0.version <= s_supportPluginVersion)
            {
                VXRVersion_0_8_0.vxr_SetSpatializerStaticRoomPose(context, pos, rot);
            }
#endif
        }
    }
}
