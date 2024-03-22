using com.vivo.codelibrary;
using System;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static bool CreateSpatialAnchor(Posef pose, out ulong handle, out Guid guid)
        {
            guid = Guid.NewGuid();
#if VXR_UNSUPPORTED_PLATFORM
            handle = 0;
            return false;
#else
            return VXRVersion_0_5_0.vxr_CreateSpatialAnchor(pose, out handle, guid.ToString()) == Result.Success;
#endif
        }

        public static bool DestroySpatialAnchor(ulong[] uhandles, out ulong[] outHandles)
        {
#if VXR_UNSUPPORTED_PLATFORM
            outHandles = new ulong[0];
            return false;
#else
            IntPtrList handles = VXRPluginUtil.ToIntPtrList(uhandles);
            Result ret = VXRVersion_0_5_0.vxr_DestroySpatialAnchor(handles, out IntPtrList results);
            handles.Free();
            results.ToArray(out outHandles);
            if (ret != Result.Success)
            {
                return false;
            }

            return true;
#endif
        }

        public static bool GetSpatiaAnchorPose(ulong handle, out Posef pose)
        {
#if VXR_UNSUPPORTED_PLATFORM
            pose = default(XrPosef);
            return false;
#else
            Result ret = VXRVersion_0_5_0.vxr_GetSpatialAnchorPose(handle, out pose);
            if (ret != Result.Success)
            {
                return false;
            }

            return true;
#endif
        }

        public static bool SaveSpatialAnchor(ulong[] uhandles, out ulong[] outHandles)
        {
#if VXR_UNSUPPORTED_PLATFORM
            outHandles = new ulong[0];
            return false;
#else
            IntPtrList handles = VXRPluginUtil.ToIntPtrList(uhandles);
            Result ret = VXRVersion_0_5_0.vxr_SaveSpatialAnchor(handles, out var results);
            handles.Free();
            results.ToArray(out outHandles);
            if (ret != Result.Success)
            {
                return false;
            }

            return true;
#endif
        }

        public static bool UnsaveSpatialAnchor(ulong[] uhandles, out ulong[] outHandles)
        {
#if VXR_UNSUPPORTED_PLATFORM
            outHandles = new ulong[0];
            return false;
#else
            IntPtrList handles = VXRPluginUtil.ToIntPtrList(uhandles);
            Result ret = VXRVersion_0_5_0.vxr_UnsaveSpatialAnchor(handles, out var results);
            handles.Free();
            results.ToArray(out outHandles);
            if (ret != Result.Success)
            {
                return false;
            }

            return true;
#endif
        }

        public static bool UnsaveAllSpatialAnchor()
        {
#if VXR_UNSUPPORTED_PLATFORM
            return false;
#else
            Result ret = VXRVersion_0_5_0.vxr_UnsaveAllSpatialAnchor();
            if (ret != Result.Success)
            {
                return false;
            }

            return true;
#endif
        }

        public static bool LoadSpatialAnchorByUuid(Guid[] uuids, out SpatialAnchorData[] anchors)
        {
#if VXR_UNSUPPORTED_PLATFORM
            anchors = new SpatialAnchor[0];
            return false;
#else
            IntPtrList list = VXRPluginUtil.ToIntPtrList(uuids);
            Result ret = VXRVersion_0_5_0.vxr_LoadSpatialAnchorByUuid(list, out var results);
            VLog.Info($"[SpatialAnchor] 空间锚点 加载 结果[{ret}] 数量[{results.Count}]");
            list.Free();
            results.ToStructureArray(out anchors);

            if (ret != Result.Success)
            {
                return false;
            }

            return true;
#endif
        }

        public static bool GetSpatialAnchorUuid(ulong handle, out Guid uuid)
        {
            uuid = Guid.Empty;
#if VXR_UNSUPPORTED_PLATFORM
            return false;
#else
            Result ret = VXRVersion_0_5_0.vxr_GetSpatialAnchorUuid(handle, out string name);
            VLog.Info($"[SpatialAnchor] 获取空间锚点UUID 结果[{ret}] Handle[{handle}]");
            if (ret != Result.Success)
            {
                VLog.Warning($"[SpatialAnchor] 获取空间锚点UUID 结果[{ret}] Handle[{handle}]");
                return false;
            }

            VLog.Info($"[SpatialAnchor] 获取空间锚点UUID 成功 Handle[{handle}] UUID[{name}]");
            uuid = Guid.Parse(name);
            return true;
#endif
        }
    }
}
