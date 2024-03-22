using System;
using System.Runtime.InteropServices;


namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public enum OverlayShape
        {
            Quad = 0,
            Cylinder = 1,
            //Cubemap = 2,
            //Equirect = 3,
        }

        public enum OverlayLayerLayout
        {
            Mono = 0,
            Stereo = 1,
        }

        public enum OverlayLayerFormat
        {
            GL_RGBA8 = 0x8058,
            GL_SRGB8_ALPHA8 = 0x8C43,
        }

        public enum OverlayType
        {
            Underlay = 0,
            Overlay = 1
        }

        public struct OverlayCreateParams
        {
            public UInt32 LayerId;
            public OverlayType LayerType;
            public OverlayLayerLayout Layout;
            public int Depth;
            public UInt64 Format;
            public UInt32 SampleCount;
            public UInt32 Width;
            public UInt32 Height;
            public UInt32 MipCount;
            public int FaceCount;
            public int IsDynamic;
            public int IsExternalTexture;
        }
        
        public struct OverlaySubmitParams
        {
            public OverlayType OverLayerType;
            public int OverlayDepth;
            public Vector3f Position;
            public Quatf Quaternion;
            public Vector3f Size;
        }
        
        public enum OverlayAndroidSurfaceEvent
        {
            Create = 0,
            Update = 1,
            Shutdown = 2,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct XrExtent2Df
        {
            public float Width;
            public float Height;
        }

        public struct XrExtent2Df_t
        {
            public static XrExtent2Df identity
            {
                get { return new XrExtent2Df() { Width = 1, Height = 1 }; }
            }

            public static XrExtent2Df zero
            {
                get { return new XrExtent2Df() { Width = 0, Height = 0 }; }
            }
        }
    }
}
