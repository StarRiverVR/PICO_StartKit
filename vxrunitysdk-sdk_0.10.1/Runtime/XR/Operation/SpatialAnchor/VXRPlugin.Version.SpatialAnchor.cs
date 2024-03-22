using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_5_0
        {
           
            [DllImport(pluginName, EntryPoint = "vxr_CreateSpatialAnchor")]
            internal static extern Result vxr_CreateSpatialAnchor(Posef pose, out ulong handle, string uuid);
            
            [DllImport(pluginName, EntryPoint = "vxr_DestroySpatialAnchor")]
            internal static extern Result vxr_DestroySpatialAnchor(IntPtrList uHandles, out IntPtrList results);
            
            [DllImport(pluginName, EntryPoint = "vxr_GetSpatialAnchorPose")]
            internal static extern Result vxr_GetSpatialAnchorPose(ulong handle, out Posef pose);
            
            [DllImport(pluginName, EntryPoint = "vxr_SaveSpatialAnchor")]
            internal static extern Result vxr_SaveSpatialAnchor(IntPtrList uHandles, out IntPtrList results);
            
            [DllImport(pluginName, EntryPoint = "vxr_UnsaveSpatialAnchor")]
            internal static extern Result vxr_UnsaveSpatialAnchor(IntPtrList uHandles, out IntPtrList results);
            
            [DllImport(pluginName, EntryPoint = "vxr_LoadSpatialAnchorByUuid")]
            internal static extern Result vxr_LoadSpatialAnchorByUuid(IntPtrList uuids, out IntPtrList anchors);

            [DllImport(pluginName, EntryPoint = "vxr_UnsaveAllSpatialAnchor")]
            internal static extern Result vxr_UnsaveAllSpatialAnchor();
            
            [DllImport(pluginName, EntryPoint = "vxr_GetSpatialAnchorUuid")]
            internal static extern Result vxr_GetSpatialAnchorUuid(ulong handle, out string uuid);
        }
    }
}
