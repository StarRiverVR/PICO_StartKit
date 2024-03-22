using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SpatialAnchorData
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string uuid;
            public ulong handle;
        }
    }
}
