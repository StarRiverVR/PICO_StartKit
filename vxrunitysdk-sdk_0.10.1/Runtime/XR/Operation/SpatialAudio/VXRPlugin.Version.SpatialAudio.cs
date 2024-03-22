using System;
using System.Runtime.InteropServices;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private const string k_SpatialAudioPluginName = "VXRSpatialAudio";
        private sealed partial class VXRVersion_0_7_0
        {
            // Context
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_CreateSpatializerContext")]
            public static extern Result vxr_CreateSpatializerContext(Spatializerlmpl type, int numChannels, int framesBuffer, int sampleRate, ref IntPtr context);
          
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_DestroySpatializerContext")]
            public static extern Result vxr_DestroySpatializerContext(IntPtr context);

            // Source
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_CreateSpatializerSource")]
            public static extern Result vxr_CreateSpatializerSource(IntPtr context, AudioSourceRenderMode mode, out int sourceId);
         
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_DestroySpatializerSource")]
            public static extern Result vxr_DestroySpatializerSource(IntPtr context, int sourceId);
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerSourceVolume")]
            public static extern Result vxr_SetSpatializerSourceVolume(IntPtr context, int sourceId, float vol);
          
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerSourceAttenuation")]
            public static extern Result vxr_SetSpatializerSourceAttenuation(IntPtr context, int sourceId, AudioSourceAttenuation attenuation);
         
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerSourcePosition")]
            public static extern Result vxr_SetSpatializerSourcePosition(IntPtr context, int sourceId, Vector3f pos);
           
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerSourceOrientation")]
            public static extern Result vxr_SetSpatializerSourceOrientation(IntPtr context, int sourceId, Quatf rot);
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerSourceBuffer")]
            public static extern Result vxr_SetSpatializerSourceBuffer(IntPtr context, int sourceId, float[] buffer, int channels, int frames);

            // Listener
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerListenerPosition")]
            public static extern Result vxr_SetSpatializerListenerPosition(IntPtr context, Vector3f pos);

            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerListenerOrientation")]
            public static extern Result vxr_SetSpatializerListenerOrientation(IntPtr context, Quatf rot);

            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_GetSpatializerListenerBuffer")]
            public static extern Result vxr_GetSpatializerListenerBuffer(IntPtr context, int channels, int frames, float[] buffer);

        }
        private sealed partial class VXRVersion_0_8_0
        {
            // Room
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_InitSpatializerStaticRoom")]
            public static extern Result vxr_InitSpatializerStaticRoom(IntPtr context, SpatialAudioStaticRoomInfo data);
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerStaticRoomEnable")]
            public static extern Result vxr_SetSpatializerStaticRoomEnable(IntPtr context, bool enable);
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerStaticRoomReflection")]
            public static extern Result vxr_SetSpatializerStaticRoomReflection(IntPtr context, float scalar);
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerStaticRoomReverb")]
            public static extern Result vxr_SetSpatializerStaticRoomReverb(IntPtr context, float gain, float time, float brightness);
            
            [DllImport(k_SpatialAudioPluginName, EntryPoint = "vxr_SetSpatializerStaticRoomPose")]
            public static extern Result vxr_SetSpatializerStaticRoomPose(IntPtr context, Vector3f pos, Quatf rot);
        }
    }
}

