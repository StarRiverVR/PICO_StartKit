using com.vivo.codelibrary;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace com.vivo.openxr
{
    public class VXRSpatialAnchor
    {
        private static ulong s_AnchorTaskIndex_ = 1;
        
        private static ulong s_AnchorTaskIndex 
        {
            get
            {
                if (s_AnchorTaskIndex_ >= ulong.MaxValue)
                {
                    s_AnchorTaskIndex_ = 0;
                }
                return s_AnchorTaskIndex_++;
            }
        }
        
        private struct JobHandleData
        {
            public JobHandle handle;
            public IDisposable disposable;
        }
        
        private static List<JobHandleData> s_JobHandles = new List<JobHandleData>();

        private static void PushJob<T>(T job) where T : struct, IJob
        {
            JobHandleData vhandle = new JobHandleData();
            vhandle.handle = job.Schedule();
            vhandle.disposable = null;
            s_JobHandles.Add(vhandle);
        }
        
        private static void PushJob<T,H>(T job, NativeArray<H> array) where T : struct, IJob where H : struct
        {
            JobHandleData vhandle = new JobHandleData();
            vhandle.handle = job.Schedule();
            vhandle.disposable = array;
            s_JobHandles.Add(vhandle);
        }
        
        private static NativeArray<T> ConvertToNativeArray<T>(T[] list) where T : struct
        {
            NativeArray<T> array = new NativeArray<T>(list.Length,Allocator.TempJob);
            for(int i = 0; i < list.Length; i++)
            {
                array[i] = list[i];
            }
            return array;
        }

        internal interface IAsynResult
        {
            void Brocast();
        }
        
        private static Queue<IAsynResult> s_EventQueue = new Queue<IAsynResult>();

        #region 创建
        private struct CreateJob : IJob
        {
            public ulong taskId;
            public VXRPlugin.Posef pose;
            public void Execute()
            {
                var success = VXRPlugin.CreateSpatialAnchor(pose, out var handle, out Guid uuid);
                if (success)
                {
                    VLog.Info($"空间锚点 创建成功 UUID[{uuid}] Handle[{handle}]");
                }
                else
                {
                    VLog.Info("空间锚点 创建失败");
                }
                CreateAsynResult data = new CreateAsynResult();
                data.taskId = taskId;
                data.result = success;
                data.handle = handle;
                data.uuid = uuid;

                s_EventQueue.Enqueue(data);
            }
        }
        
        /// <summary>
        /// 创建空间锚点结果信息。
        /// </summary>
        public struct CreateAsynResult:IAsynResult
        {
            public ulong taskId;
            public bool result;
            public ulong handle;
            public Guid uuid;

            void IAsynResult.Brocast()
            {
                CreatedComplete?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 创建空间锚点回调。
        /// </summary>
        public static event Action<CreateAsynResult> CreatedComplete;

        /// <summary>
        /// 创建空间锚点。
        /// 异步调用接口，调用接口后立即返回taskId。通过监听回调<see cref="CreatedComplete"/>,
        /// 根据回调参数信息<see cref="CreateAsynResult"/>与taskId判断操作是否执行成功。
        /// </summary>
        /// <param name="position">锚点坐标</param>
        /// <param name="rotation">锚点方向</param>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否开始创建空间锚点，true则taskId有效</returns>
        public static bool CreateSpatialAnchor(Vector3 position, Quaternion rotation, out ulong taskId)
        {
            taskId = s_AnchorTaskIndex;

            CreateJob job = new CreateJob();
            job.pose = VXRPluginUtil.ToPose(position, rotation);
            job.taskId = taskId;
            PushJob(job);
            return true;
        }
        #endregion

        #region 销毁
        /// <summary>
        /// 销毁空间锚点。
        /// </summary>
        /// <param name="handles">需要删除的空间锚点句柄数组</param>
        /// <returns>删除成功的空间锚点句柄数组</returns>
        public static ulong[] DestroySpatialAnchor(ulong[] handles)
        {
            if (handles == null || handles.Length == 0) return new ulong[0];
            var ret = VXRPlugin.DestroySpatialAnchor(handles, out var results);
            VLog.Info($"空间锚点 销毁结果 Result[{ret}] Handle[{results.Length}/{handles.Length}]");
            return results;
        }
        #endregion

        #region 位置
        /// <summary>
        /// 获取空间锚点位置信息。
        /// </summary>
        /// <param name="handle">空间锚点句柄</param>
        /// <param name="position">空间锚点坐标</param>
        /// <param name="rotation">空间锚点方向</param>
        /// <returns>是否获取成功</returns>
        public static bool GetSpatialAnchorPose(ulong handle, out Vector3 position, out Quaternion rotation)
        {
            bool ret = VXRPlugin.GetSpatiaAnchorPose(handle, out VXRPlugin.Posef pose);
            position = pose.GetVector3();
            rotation = pose.GetQuaternion();
            VLog.Info($"空间锚点 位置结果 Result[{ret}] Handle[{handle}]");
            return ret;
        }
        #endregion

        #region 持久化
        private struct SaveJob : IJob
        {
            public ulong taskId;
            public NativeArray<ulong> handles;
            public void Execute()
            {
                var success = VXRPlugin.SaveSpatialAnchor(handles.ToArray(), out var results);
                VLog.Info($"空间锚点 持久化结果 Result[{success}] Handle[{handles.Length}/{results.Length}]");

                SaveAsynResult data = new SaveAsynResult();
                data.taskId = taskId;
                data.result = success;
                data.handles = results;
                s_EventQueue.Enqueue(data);
            }
        }

        /// <summary>
        /// 持久化空间锚点结果信息。
        /// </summary>
        public struct SaveAsynResult : IAsynResult
        {
            public ulong taskId;
            public bool result;
            public ulong[] handles;

            void IAsynResult.Brocast()
            {
                SaveComplete?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 持久化空间锚点回调。
        /// </summary>
        public static event Action<SaveAsynResult> SaveComplete;

        /// <summary>
        /// 持久化空间锚点。
        /// 异步调用接口，调用接口后立即返回taskId。通过监听回调<see cref="SaveComplete"/>,
        /// 根据回调参数信息<see cref="SaveAsynResult"/>与taskId判断操作是否执行成功。
        /// </summary>
        /// <param name="handles">空间锚点句柄数组</param>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否开始执行持久化空间锚点，true则taskId有效</returns>
        public static bool SaveSpatialAnchor(ulong[] handles, out ulong taskId)
        {
            if (handles == null || handles.Length == 0)
            {
                taskId = 0;
                return false;
            }
            NativeArray<ulong> array = ConvertToNativeArray(handles);
            taskId = s_AnchorTaskIndex;
            SaveJob job = new SaveJob();
            job.taskId = taskId;
            job.handles = array;
            PushJob(job, array);

            return true;
        }
        #endregion

        #region 去持久化
        private struct UnsaveJob : IJob
        {
            public ulong taskId;
            public NativeArray<ulong> handles;
            public void Execute()
            {
                var success = VXRPlugin.UnsaveSpatialAnchor(handles.ToArray(), out var results);
                VLog.Info($"空间锚点 去持久化结果 Result[{success}] Handle[{handles.Length}/{results.Length}]");

                UnsaveAsynResult data = new UnsaveAsynResult();
                data.taskId = taskId;
                data.result = success;
                data.handles = results;
                s_EventQueue.Enqueue(data);
            }
        }
        
        /// <summary>
        /// 反持久化空间锚点结果信息。
        /// </summary>
        public struct UnsaveAsynResult : IAsynResult
        {
            public ulong taskId;
            public bool result;
            public ulong[] handles;
            void IAsynResult.Brocast()
            {
                UnsaveComplete?.Invoke(this);
            }
        }

        /// <summary>
        /// 反持久化空间锚点回调。
        /// </summary>
        public static event Action<UnsaveAsynResult> UnsaveComplete;

        /// <summary>
        /// 反持久化空间锚点。
        /// 异步调用接口，调用接口后立即返回taskId。通过监听回调<see cref="UnsaveComplete"/>,
        /// 根据回调参数信息<see cref="UnsaveAsynResult"/>与taskId判断操作是否执行成功。
        /// </summary>
        /// <param name="handles">空间锚点句柄数组</param>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否开始执行反持久化空间锚点，true则taskId有效</returns>
        public static bool UnsaveSpatialAnchor(ulong[] handles, out ulong taskId)
        {
            if (handles == null || handles.Length == 0)
            {
                taskId = 0;
                return false;
            }
            NativeArray<ulong> array = ConvertToNativeArray(handles);
            taskId = s_AnchorTaskIndex;
            UnsaveJob job = new UnsaveJob();
            job.taskId = taskId;
            job.handles = array;
            PushJob(job, array);
            return true;
        }
        #endregion

        #region 清理持久化
        private struct UnsaveAllJob : IJob
        {
            public ulong taskId;
            public void Execute()
            {
                var success = VXRPlugin.UnsaveAllSpatialAnchor();
                VLog.Info($"空间锚点 清理持久化 Result[{success}]");

                UnsaveAllAsynResult data = new UnsaveAllAsynResult();
                data.taskId = taskId;
                data.result = success;
                s_EventQueue.Enqueue(data);
            }
        }
        
        /// <summary>
        /// 反持久化所有空间锚点结果信息。
        /// </summary>
        public struct UnsaveAllAsynResult : IAsynResult
        {
            public ulong taskId;
            public bool result;

            void IAsynResult.Brocast()
            {
                UnsaveAllComplete?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 反持久化所有空间锚点回调。
        /// </summary>
        public static event Action<UnsaveAllAsynResult> UnsaveAllComplete;

        /// <summary>
        /// 反持久化所有空间锚点。
        /// 异步调用接口，调用接口后立即返回taskId。通过监听回调<see cref="UnsaveAllComplete"/>,
        /// 根据回调参数信息<see cref="UnsaveAllAsynResult"/>与taskId判断操作是否执行成功。
        /// </summary>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否开始执行反持久化所有空间锚点，true则taskId有效</returns>
        public static bool UnsaveAllSpatialAnchor(out ulong taskId)
        {
            taskId = s_AnchorTaskIndex;
            UnsaveAllJob job = new UnsaveAllJob();
            job.taskId = taskId;
            PushJob(job);
            return true;
        }
        #endregion

        #region 加载
        private struct LoadJob : IJob
        {
            public ulong taskId;
            public NativeArray<Guid> uuids;
            public void Execute()
            {
                var success = VXRPlugin.LoadSpatialAnchorByUuid(uuids.ToArray(), out var results);
                VLog.Info($"空间锚点 加载 Result[{success}] Count[{results.Length}/{uuids.Length}]");
                for (int i = 0; i < results.Length; i++)
                {
                    VLog.Info($"空间锚点 加载[{i}/{results.Length}] UUID[{results[i].uuid}] Handle[{results[i].handle}]");
                }
                LoadAsynResult data = new LoadAsynResult();
                data.taskId = taskId;
                data.result = success;
                data.handles = Array.ConvertAll(results, (a) => a.handle);
                data.uuids = Array.ConvertAll(results, (a) => Guid.Parse(a.uuid));
                s_EventQueue.Enqueue(data);
            }
        }
        
        /// <summary>
        /// 加载空间锚点结果信息。
        /// </summary>
        public struct LoadAsynResult:IAsynResult
        {
            public ulong taskId;
            public bool result;
            public ulong[] handles;
            public Guid[] uuids;
            void IAsynResult.Brocast()
            {
                LoadComplete?.Invoke(this);
            }
        }

        /// <summary>
        /// 加载空间锚点回调。
        /// </summary>
        public static event Action<LoadAsynResult> LoadComplete;

        /// <summary>
        /// 加载空间锚点。
        /// 异步调用接口，调用接口后立即返回taskId。通过监听回调<see cref="LoadComplete"/>,
        /// 根据回调参数信息<see cref="LoadAsynResult"/>与taskId判断操作是否执行成功。
        /// </summary>
        /// <param name="uuids">加载空间锚点句柄数组，空或长度等于0时加载所有持久化的空间锚点。</param>
        /// <param name="taskId">任务ID</param>
        /// <returns>是否开始执行加载空间锚点，true则taskId有效</returns>
        public static bool LoadSpatialAnchorByUuid(Guid[] uuids, out ulong taskId)
        {
            if (uuids == null) uuids = new Guid[0];
            taskId = s_AnchorTaskIndex;

            NativeArray<Guid> array = ConvertToNativeArray(uuids);
            LoadJob job = new LoadJob();
            job.taskId = taskId;
            job.uuids = array;
            PushJob(job, array);

            return true;
        }
        #endregion

        #region 查询UUID
        /// <summary>
        /// 查询空间锚点的UUID
        /// </summary>
        /// <param name="handle">空间锚点句柄</param>
        /// <param name="uuid">空间锚点UUID</param>
        /// <returns>是否查询成功，true则UUID有效</returns>
        public static bool GetSpatialAnchorUuid(ulong handle, out Guid uuid)
        {
            bool ret = VXRPlugin.GetSpatialAnchorUuid(handle, out uuid);
            if (ret)
            {
                VLog.Info($"空间锚点 查询UUID Result[{ret}] Handle[{handle}] Guid[{uuid}]");
            }
            else
            {
                VLog.Info($"空间锚点 查询UUID Result[{ret}] Handle[{handle}]");
            }
            return ret;
        }
        #endregion

        internal static void Update()
        {
            for (int i = s_JobHandles.Count - 1; i >= 0; i--)
            {
                if (s_JobHandles[i].handle.IsCompleted)
                {
                    s_JobHandles[i].handle.Complete();
                    if (s_JobHandles[i].disposable != null)
                    {
                        s_JobHandles[i].disposable.Dispose();
                    }
                    s_JobHandles.RemoveAt(i);
                }
            }
            // 派发事件
            while(s_EventQueue.Count > 0)
            {
                var task = s_EventQueue.Dequeue();
                task.Brocast();
            }
        }
    }
}
