using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        /// <summary>
        /// 空间音频Native实现方案
        /// </summary>
        public enum Spatializerlmpl
        {
            Goer
        }

        /// <summary>
        /// 音源渲染模式
        /// </summary>
        public enum AudioSourceRenderMode
        {
            HRTF_Disable = 0, //禁用基于HRTF的渲染
            HRTF_Ambisonics_8 = 1, //基于HRTF的渲染，使用一阶Ambisonics， 8个扬声器的虚拟阵列
            HRTF_Ambisonics_12 = 2, //基于HRTF的渲染，二阶Ambisonics，12个扬声器的虚拟阵列
            HRTF_Ambisonics_26 = 3, //基于HRTF的渲染，三阶Ambisonics，26个扬声器的虚拟阵列
            Room = 4 //仅房间效果渲染。这将禁用基于HRTF的渲染和声音对象的直接（干）输出
        }
        
        /// <summary>
        /// 音频衰减模式
        /// </summary>
        public struct AudioSourceAttenuation
        {
            public float minDistance;//衰减最小距离
            public float maxDistance;//衰减最大距离
            public int mode;//衰减模式
            public float customParam;//自定义参数
        }

        /// <summary>
        /// 反射属性材质
        /// </summary>
        public enum ReflectionMaterial
        {
            Transparent = 0,
            AcousticTile,
            Brick,
            BrickPainted,
            Carpet,
            CarpetHeavy,
            CarpetHeavyPadded,
            CeramicTile,
            Concrete,
            ConcreteRough,
            ConcreteBlock,
            ConcreteBlockPainted,
            Curtain,
            Foliage,
            Glass,
            GlassHeavy,
            Grass,
            Gravel,
            GypsumBoard,
            PlasterOnBrick,
            PlasterOnConcreteBlock,
            Soil,
            SoundProof,
            Snow,
            Steel,
            Water,
            WoodThin,
            WoodThick,
            WoodFloor,
            WoodOnConcrete
        }

        /// <summary>
        /// 静态房间模型配置属性
        /// </summary>
        public struct SpatialAudioStaticRoomInfo
        {
            // 尺寸
            public float length;
            public float width;
            public float height;
            // 六面墙材质
            public ReflectionMaterial/*0*/ left;
            public ReflectionMaterial/*1*/ right;
            public ReflectionMaterial/*2*/ down;
            public ReflectionMaterial/*3*/ up;
            public ReflectionMaterial/*4*/ front;
            public ReflectionMaterial/*5*/ back;
        }
    }
}