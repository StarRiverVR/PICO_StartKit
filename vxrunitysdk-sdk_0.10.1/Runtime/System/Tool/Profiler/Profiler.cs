using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Profiling;
using System.Text;
using System.Linq;


namespace com.vivo.codelibrary
{
    public class Profiler
    {

        /// <summary>
        /// 获得内存快照 ".snap", Memory Profiler中导入
        /// </summary>
        /// <param name="saveDir">内存快照存储目录 "D:/Work/"</param>
        /// <param name="finishCallBack"></param>
        /// <param name="captureFlags"></param>
        public static void CreateMemorySnapshotCapture(string saveDir, System.Action<bool> finishCallBack,
            Unity.Profiling.Memory.CaptureFlags captureFlags = Unity.Profiling.Memory.CaptureFlags.ManagedObjects | Unity.Profiling.Memory.CaptureFlags.NativeObjects)
        {
            try
            {
                Unity.Profiling.Memory.MemoryProfiler.TakeTempSnapshot((str, bl) => {
                    if (bl)
                    {
                        string temporarySnapCachePath = str;
                        if (File.Exists(temporarySnapCachePath))
                        {
                            string saveFilePath = temporarySnapCachePath.Replace(Application.temporaryCachePath + "/", saveDir);
                            try
                            {
                                string dir = Path.GetDirectoryName(saveFilePath);
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                                File.Copy(temporarySnapCachePath, saveFilePath, true);
                                UnityEngine.Debug.Log(string.Format("获取内存快照:{0}", saveFilePath));
                                finishCallBack(true);
                            }
                            catch (System.Exception ex)
                            {
                                UnityEngine.Debug.LogError(string.Format("内存快照获取失败,缓存={0} 拷贝异常={1}", temporarySnapCachePath, ex));
                                finishCallBack(false);
                            }
                        }
                        else
                        {
                            UnityEngine.Debug.LogError(string.Format("内存快照获取失败,缓存未找到:{0}", temporarySnapCachePath));
                            finishCallBack(false);
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("内存快照获取失败!");
                        finishCallBack(false);
                    }
                });
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError("内存快照获取失败!");
                UnityEngine.Debug.LogException(ex);
                finishCallBack(false);
            }
        }

        /// <summary>
        /// 函数运行耗时 毫秒
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public static double ActionMilliseconds(System.Action act)
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            //
            act.Invoke();
            //
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            return ts.TotalMilliseconds;
        }

        static Dictionary<string, CustomSamplerData> customSamplers = new Dictionary<string, CustomSamplerData>();

        static CustomSamplerData GetCustomSampler(string samplerTag)
        {
            CustomSamplerData res;
            if (!customSamplers.TryGetValue(samplerTag, out res))
            {
                res = new CustomSamplerData();
                res.CustomSampler = UnityEngine.Profiling.CustomSampler.Create(samplerTag);
                res.Recorder = UnityEngine.Profiling.Recorder.Get(samplerTag);
                res.Recorder.enabled = true;
                customSamplers.Add(samplerTag, res);
            }
            return res;
        }

        /// <summary>
        /// 执行Unity Update性能采样  Profiler界面查看
        /// </summary>
        /// <param name="samplerTag"></param>
        /// <param name="act">在Update中每帧执行的逻辑</param>
        public static void UpdateSampler(string samplerTag, System.Action act)
        {
            if (string.IsNullOrEmpty(samplerTag))
            {
                act.Invoke();
            }
            else
            {
                CustomSamplerData customSampler = GetCustomSampler(samplerTag);
                customSampler.CustomSampler.Begin();
                act.Invoke();
                customSampler.CustomSampler.End();
            }
        }

        /// <summary>
        /// 每帧逻辑采样的时间
        /// </summary>
        /// <param name="samplerTag"></param>
        /// <param name="secondType"></param>
        /// <param name="secondCpuGpu">Cpu/Gpu时间</param>
        /// <returns></returns>
        public static double UpdateSamplerSeconds(string samplerTag, SecondType secondType = SecondType.Millisecond, SecondCpuGpu secondCpuGpu = SecondCpuGpu.Cpu)
        {
            if (!string.IsNullOrEmpty(samplerTag))
            {
                CustomSamplerData customSampler = GetCustomSampler(samplerTag);
                if (customSampler.Recorder.isValid)
                {
                    long res = customSampler.Recorder.elapsedNanoseconds;
                    switch (secondCpuGpu)
                    {
                        case SecondCpuGpu.Cpu:
                            {
                                res = customSampler.Recorder.elapsedNanoseconds;
                            }
                            break;
                        case SecondCpuGpu.Gpu:
                            {
                                res = customSampler.Recorder.gpuElapsedNanoseconds;
                            }
                            break;
                    }
                    switch (secondType)
                    {
                        case SecondType.Second:
                            {
                                return res / 1000000000d;
                            }
                        case SecondType.Millisecond:
                            {
                                return res / 1000000d;
                            }
                        case SecondType.Microsecond:
                            {
                                return res / 1000d;
                            }
                        case SecondType.Nanosecond:
                            {
                                return res;
                            }
                    }
                    return 0;
                }
                else
                {
                    Debug.Log(string.Format("Recorder IsValid : {0}", samplerTag));
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 每帧逻辑采样的时间
        /// </summary>
        /// <param name="samplerTag"></param>
        /// <param name="secondType"></param>
        /// <param name="secondCpuGpu"></param>
        /// <returns></returns>
        public static string GetUpdateSamplerSecondsString(string samplerTag, SecondType secondType = SecondType.Millisecond, SecondCpuGpu secondCpuGpu = SecondCpuGpu.Cpu)
        {
            double res = UpdateSamplerSeconds(samplerTag, secondType, secondCpuGpu);
            string strSec = null;
            switch (secondType)
            {
                case SecondType.Second:
                    {
                        strSec = string.Intern("秒");
                    }
                    break;
                case SecondType.Millisecond:
                    {
                        strSec = string.Intern("毫秒");
                    }
                    break;
                case SecondType.Microsecond:
                    {
                        strSec = string.Intern("微秒");
                    }
                    break;
                case SecondType.Nanosecond:
                    {
                        strSec = string.Intern("纳秒");
                    }
                    break;
            }
            string strCpuGpu = null;
            switch (secondCpuGpu)
            {
                case SecondCpuGpu.Cpu:
                    {
                        strCpuGpu = string.Intern("Cpu");
                    }
                    break;
                case SecondCpuGpu.Gpu:
                    {
                        strCpuGpu = string.Intern("Gpu");
                    }
                    break;
            }
            return string.Format("{0}{1} [2]", res, strSec, strCpuGpu);
        }

        public enum SecondType
        {
            /// <summary>
            /// 秒
            /// </summary>
            Second,
            /// <summary>
            /// 毫秒
            /// </summary>
            Millisecond,
            /// <summary>
            /// 微秒
            /// </summary>
            Microsecond,
            /// <summary>
            /// 纳秒
            /// </summary>
            Nanosecond,
        }

        public enum SecondCpuGpu
        {
            /// <summary>
            /// Cpu时间
            /// </summary>
            Cpu,
            /// <summary>
            /// Gpu时间
            /// </summary>
            Gpu,
        }

        class CustomSamplerData
        {
            public UnityEngine.Profiling.CustomSampler CustomSampler;

            public UnityEngine.Profiling.Recorder Recorder;
        }

        /// <summary>
        /// 获得Mono内存占用
        /// </summary>
        /// <returns></returns>
        public static MonoUsedData GetMonoUsed()
        {
            MonoUsedData monoUsedData = new MonoUsedData();
            System.GC.Collect();
            if (UnityEngine.Scripting.GarbageCollector.isIncremental)
            {
                monoUsedData.MonoUsedSize = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
                monoUsedData.GCCollectIsFinished = false;
            }
            else
            {
                System.GC.WaitForPendingFinalizers();
                monoUsedData.MonoUsedSize = UnityEngine.Profiling.Profiler.GetMonoUsedSizeLong();
                monoUsedData.GCCollectIsFinished = true;
            }
            return monoUsedData;
        }

        /// <summary>
        /// 获得Mono托管空间大小 Bytes
        /// </summary>
        /// <returns></returns>
        public static long GetMonoHeadUsed()
        {
            return UnityEngine.Profiling.Profiler.GetMonoHeapSizeLong();
        }

        /// <summary>
        /// 获得对象占用内存 Bytes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static long GetObjectRuntimeMemorySize(UnityEngine.Object obj)
        {
            return UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(obj);
        }

        /// <summary>
        /// 获得某种UnityEngine.Object类型内存 Bytes
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static long GetTypesRuntimeMemorySize(Type t)
        {
            UnityEngine.Object[] objs = Resources.FindObjectsOfTypeAll(t);
            if (objs != null && objs.Length > 0)
            {
                long res = 0;
                for (int i = 0; i < objs.Length; ++i)
                {
                    res = res + UnityEngine.Profiling.Profiler.GetRuntimeMemorySizeLong(objs[i]);
                }
                return res;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 获得Unity分配内存 Bytes
        /// </summary>
        /// <returns></returns>
        public static long GetUnityMemory()
        {
            return UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong();
        }

        /// <summary>
        /// 获得Unity预留内存  Bytes
        /// </summary>
        /// <returns></returns>
        public static long GetTotalReservedMemoryLong()
        {
            return UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
        }

        /// <summary>
        /// 获得Unity预留但未使用  Bytes
        /// </summary>
        /// <returns></returns>
        public static long GetTotalUnusedReservedMemoryLong()
        {
            return UnityEngine.Profiling.Profiler.GetTotalUnusedReservedMemoryLong();
        }

        /// <summary>
        /// 内存碎片信息打印
        /// </summary>
        public static void GetTotalFragmentationInfo()
        {
            int kFreeBlockPow2Buckets = 24;
            Unity.Collections.NativeArray<int> freeStats = new Unity.Collections.NativeArray<int>(kFreeBlockPow2Buckets, Unity.Collections.Allocator.Temp);
            long freeBlocks = UnityEngine.Profiling.Profiler.GetTotalFragmentationInfo(freeStats);
            Debug.Log(string.Format("Total Free Blocks: {0}", freeBlocks));
            for (int i = 0; i < kFreeBlockPow2Buckets; i++)
            {
                Debug.Log(string.Format(" size[2^{0}] = {1} blocks", i, freeStats[i]));
            }
        }

        /// <summary>
        /// Mono内存占用
        /// </summary>
        public class MonoUsedData
        {
            /// <summary>
            /// Mono占用内存 Bytes   1mb=1024kb=1048576 Bytes
            /// </summary>
            public long MonoUsedSize;

            /// <summary>
            /// 是否已经GC清理
            /// </summary>
            public bool GCCollectIsFinished = false;

            public override string ToString()
            {
                return string.Format("{0} GCCollectIsFinished={1}", MonoUsedSize / 1048576d, GCCollectIsFinished);
            }
        }

        #region//性能简报

        enum UpdateType
        {
            FPS,
            Cpu,
        }

        static List<UpdateType> updateTypes = new List<UpdateType>();

        static void UpdateAddListen(UpdateType updateType)
        {
            if (updateTypes.Contains(updateType))
            {
                return;
            }
            updateTypes.Add(updateType);
            InformationManager.Instance.GameInformationCenter.AddListen<MonoUpdateMsg>((int)MonoUpdateMsg.Update, Update);
        }

        static void RemoveListen(UpdateType updateType)
        {
            updateTypes.Remove(updateType);
            if (updateTypes.Count==0)
            {
                InformationManager.Instance.GameInformationCenter.RemoveListen<MonoUpdateMsg>((int)MonoUpdateMsg.Update, Update);
            }
        }

        static void Update(params object[] objs)
        {
            if (isStartFPS)
            {
                fps.UpdateFrameCount();
            }
            if (isCpuUsage)
            {
                cpuData.UpdateCpuUsage();
            }
        }

        #region//CPU

        static CPUData cpuData = new CPUData();

        public static float CpuUsage
        {
            get
            {
                StartCpuUsage();
                return cpuData.cpuUsage;
            }
        }

        public static string CpuUsageString
        {
            get
            {
                StartCpuUsage();
                return cpuData.ToString();
            }
        }

        public static int CpuProcessorCount
        {
            get
            {
                StartCpuUsage();
                return cpuData.processorCount;
            }
        }

        static bool isCpuUsage = false;

        static void StartCpuUsage()
        {
            if (isCpuUsage) return;
            isCpuUsage = true; 
            cpuData.Init();
            UpdateAddListen(UpdateType.Cpu);
        }

        public static void StopCpuUsage()
        {
            if (!isCpuUsage) return;
            isCpuUsage = false;
            RemoveListen(UpdateType.Cpu);
        }

        public class CPUData
        {
            public float cpuUsage;

            float lastUsage;

            public int processorCount;

            public bool isReady = false;

            public void Init()
            {
                isReady = false;
                processorCount = SystemInfo.processorCount / 2;
                //
                lastTime = Time.time;
                TimeSpan cpuTime = new TimeSpan(0);
                System.Diagnostics.Process[] AllProcesses = System.Diagnostics.Process.GetProcesses();
                cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);
                lastCpuTime = cpuTime;
            }

            float lastTime;

            TimeSpan lastCpuTime;

            public void UpdateCpuUsage()
            {
                float timeInterval = Time.time - lastTime;
                if (timeInterval >= 1f)
                {
                    lastTime = Time.time;
                    TimeSpan cpuTime = new TimeSpan(0);
                    System.Diagnostics.Process[] AllProcesses = System.Diagnostics.Process.GetProcesses();
                    cpuTime = AllProcesses.Aggregate(cpuTime, (current, process) => current + process.TotalProcessorTime);
                    TimeSpan newCPUTime = cpuTime - lastCpuTime;
                    lastCpuTime = cpuTime;
                    cpuUsage = 100f * (float)newCPUTime.TotalSeconds / timeInterval / processorCount;
                }
            }

            public override string ToString()
            {
                return string.Format("{0}% CPU", cpuUsage.ToString("F1"));
            }
        }

        #endregion

        #region//FPS

        static FPSData _fps=new FPSData ();

        public static FPSData fps
        {
            get
            {
                StartFPS();
                return _fps;
            }
        }

        static bool isStartFPS = false;

        static void StartFPS()
        {
            if (isStartFPS) return;
            isStartFPS = true;
            UpdateAddListen(UpdateType.FPS);
        }

        public static void StopFPS()
        {
            if (!isStartFPS) return;
            isStartFPS = false;
            RemoveListen(UpdateType.FPS);
        }

        public class FPSData
        {
            public FPSData()
            {
                frameCount = 0;
                allUnscaledTime = 0;
                lastFPSH = 0;
            }

            public float FPS;

            public float LastFPS;

            public float AverageFPS;

            int frameCount;

            float allUnscaledTime;

            List<float> lastFPSList = new List<float>();

            float lastFPSH = 0;

            public void UpdateFrameCount()
            {
                frameCount = frameCount + 1;
                allUnscaledTime = allUnscaledTime + Time.unscaledDeltaTime;
                FPS = GetFPS();
                LastFPS = GetLastFPS();
                AverageFPS = GetAverageFPS();
            }

            float GetFPS()
            {
                float floatFPS = 1.0f / Time.unscaledDeltaTime;
                if (lastFPSList.Count > 30)
                {
                    lastFPSH = lastFPSH - lastFPSList[0];
                    lastFPSList.RemoveAt(0);
                }
                lastFPSList.Add(floatFPS);
                lastFPSH = lastFPSH + floatFPS;
                return floatFPS;
                //int fps = Mathf.RoundToInt(floatFPS);
                //return fps;
            }

            float GetLastFPS()
            {
                //return Mathf.RoundToInt(lastFPSH / lastFPSList.Count);
                return lastFPSH / lastFPSList.Count;
            }

            float GetAverageFPS()
            {
                //return Mathf.RoundToInt(frameCount / allUnscaledTime);
                return frameCount / allUnscaledTime;
            }

            public override string ToString()
            {
                return string.Format("FPS:{0} LastFPS:{1} AverageFPS:{2}", FPS, LastFPS, AverageFPS);
            }
        }

        #endregion

        /// <summary>
        /// 调试简报
        /// </summary>
        public static string ProfilerSamplingStr
        {
            get
            {
                StringBuilder sb = StringBuilderPool.Instance.GetOneStringBuilder(1024);
                sb.AppendLine("------Internal------");
                sb.AppendLine(ProfilerSamplingStr_Internal);
                sb.AppendLine("------Render------");
                sb.AppendLine(ProfilerSamplingStr_Render);
                sb.AppendLine("------Memory------");
                sb.AppendLine(ProfilerSamplingStr_Memory);
                //
                string str = sb.ToString();
                StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
                return str;
            }
        }

        public static string ProfilerSamplingStr_Internal
        {
            get
            {
                StringBuilder sb = StringBuilderPool.Instance.GetOneStringBuilder(1024);
                SamplingData samplingData = InternalProfilerAllSamplingData.FinSamplingData(string.Intern("GPU Frame Time"));
                sb.AppendLine(samplingData.ToString());
                samplingData = InternalProfilerAllSamplingData.FinSamplingData(string.Intern("BehaviourUpdate"));
                sb.AppendLine(samplingData.ToString());
#if UNITY_2022_1_OR_NEWER
                samplingData = InternalProfilerAllSamplingData.FinSamplingData(string.Intern("CPU Main Thread Frame Time"));
                sb.AppendLine(samplingData.ToString());
                samplingData = InternalProfilerAllSamplingData.FinSamplingData(string.Intern("CPU Render Thread Frame Time"));
                sb.AppendLine(samplingData.ToString());
                samplingData = InternalProfilerAllSamplingData.FinSamplingData(string.Intern("CPU Total Frame Time"));
                sb.AppendLine(samplingData.ToString());
#endif
                //
                string str = sb.ToString();
                StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
                return str;
            }
        }

        public static string ProfilerSamplingStr_Memory
        {
            get
            {
                StringBuilder sb = StringBuilderPool.Instance.GetOneStringBuilder(1024);
                //
                SamplingData samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("System Used Memory"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Gfx Used Memory"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Texture Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Texture Memory"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Mesh Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Material Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Material Memory"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("AnimationClip Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("AnimationClip Memory"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Asset Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("GameObject Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Scene Object Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = MemoryProfilerAllSamplingData.FinSamplingData(string.Intern("Object Count"));
                sb.AppendLine(samplingData.ToString());
                //
                string str = sb.ToString();
                StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
                return str;
            }
        }

        public static string ProfilerSamplingStr_Render
        {
            get
            {
                StringBuilder sb = StringBuilderPool.Instance.GetOneStringBuilder(1024);
                //
                SamplingData samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Draw Calls Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("SetPass Calls Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Total Batches Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Vertices Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Triangles Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Used Buffers Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Used Buffers Bytes"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Used Textures Count"));
                sb.AppendLine(samplingData.ToString());
                samplingData = RenderProfilerAllSamplingData.FinSamplingData(string.Intern("Used Textures Bytes"));
                sb.AppendLine(samplingData.ToString());
                //
                string str = sb.ToString();
                StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
                return str;
            }
        }

#region//内存

        static ProfilerAllSamplingData _memoryProfilerAllSamplingData;

        /// <summary>
        /// 内存简报
        /// </summary>
        public static ProfilerAllSamplingData MemoryProfilerAllSamplingData
        {
            get
            {
                if (_memoryProfilerAllSamplingData==null)
                {
                    _memoryProfilerAllSamplingData = new ProfilerAllSamplingData();
                    //Memory
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "System Used Memory", "性能分析器中的值与操作系统任务管理器中显示的值不同，因为 Memory Profiler 不会跟踪系统中的所有内存使用情况。这包括某些驱动程序和插件使用的内存，以及用于可执行代码的内存。"));
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "GC Allocation In Frame Count", "托管分配的数量及其总大小（以字节为单位）。");
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "GC Allocated In Frame", "托管分配的数量及其总大小（以字节为单位）。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Total Used Memory", "Unity 使用和跟踪的总内存数量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Total Reserved Memory", "Unity 用于跟踪目的和池分配的总保留内存。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "GC Used Memory", "托管代码使用的已用堆大小和总堆大小。此内存量是垃圾收集量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "GC Reserved Memory", "托管代码使用的已用堆大小和总堆大小。此内存量是垃圾收集量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Gfx Used Memory", "驱动程序对纹理、渲染目标、着色器和网格数据使用的估计内存量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Gfx Reserved Memory", "驱动程序对纹理、渲染目标、着色器和网格数据使用的估计内存量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Audio Used Memory", "音频系统的估计内存使用量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Audio Reserved Memory", "音频系统的估计内存使用量。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Video Used Memory", "视频系统的估计内存使用量"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Video Reserved Memory", "视频系统的估计内存使用量"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Profiler Used Memory", "性能分析器功能使用并从系统中保留的内存。"));
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Profiler Reserved Memory", "性能分析器功能使用并从系统中保留的内存。"));
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "Texture Count", "已加载的纹理总数及其使用的内存。");
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Texture Memory", "已加载的纹理总数及其使用的内存。"));
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "Mesh Count", "已加载的网格总数及其使用的内存。");
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Mesh Memory", "已加载的网格总数及其使用的内存。"));
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "Material Count", "已加载的材质总数及其使用的内存。");
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "Material Memory", "已加载的材质总数及其使用的内存。"));
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "AnimationClip Count", "已加载的动画剪辑总数及其使用的内存。");
                    _memoryProfilerAllSamplingData.Add(new SamplingDataMemory(ProfilerCategory.Memory, "AnimationClip Memory", "已加载的动画剪辑总数及其使用的内存。"));
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "Asset Count", "已加载的资源总数。");
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "GameObject Count", "场景中游戏对象的总数。");
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "Scene Object Count", "动态 UnityEngine.Object 的总数。此数值包括 GameObject Count，加上组件的总数，以及场景中所有非资源内容的数量。");
                    _memoryProfilerAllSamplingData.Add(ProfilerCategory.Memory, "Object Count", "本机 UnityEngine.Object 总数是您的应用程序创建或加载的数量。此数值包括 Scene Object Count，加上所有资源的总数。如果此数值随时间推移而上升，表示应用程序创建了一些永不销毁或上载的游戏对象或其他资源。");
                }
                return _memoryProfilerAllSamplingData;
            }
        }

        /// <summary>
        /// 内存简报
        /// </summary>
        public static string MemoryProfilerStr
        {
            get
            {
                return MemoryProfilerAllSamplingData.ToString();
            }
        }

        /// <summary>
        /// 内存简报
        /// </summary>
        /// <param name="profilerStr"></param>
        /// <returns></returns>
        public static string MemoryProfilerStrTarget(string profilerStr)
        {
            SamplingData samplingData = MemoryProfilerAllSamplingData.FinSamplingData(profilerStr);
            if (samplingData!=null)
            {
                return samplingData.ToString();
            }
            else
            {
                return null;
            }
        }

#endregion

#region//渲染

        static ProfilerAllSamplingData _renderProfilerAllSamplingData;

        /// <summary>
        /// 渲染简报
        /// </summary>
        public static ProfilerAllSamplingData RenderProfilerAllSamplingData
        {
            get
            {
                if (_renderProfilerAllSamplingData==null)
                {
                    _renderProfilerAllSamplingData = new ProfilerAllSamplingData();
                    //Render
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Draw Calls Count", "DrawCalls");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Batches Count", "BatchesCount");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "SetPass Calls Count", "Unity 在一帧中切换用于渲染游戏对象的着色器通道的次数。一个着色器可能包含多个着色器通道，每个通道以不同的方式渲染场景中的游戏对象。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Total Batches Count", "Unity 在一帧内处理的批次总数。这个数字包括静态和动态批次。");


                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Vertices Count", "VerticesCount");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Triangles Count", "TrianglesCount");
                    //Dynamic Batching
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Dynamic Batched Draw Calls Count", "Unity 合并为动态批次的绘制调用数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Dynamic Batches Count", "Unity 在帧期间处理的动态批次数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Dynamic Batched Triangles Count", "动态批次中包含的游戏对象中的三角形数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Dynamic Batched Vertices Count", "动态批次中包含的游戏对象中的顶点数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Dynamic Batching Time", "Unity 创建动态批处理结构所花费的时间。");
                    //Static Batching
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Static Batched Draw Calls Count", "Unity 合并为静态批次的绘制调用数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Static Batches Count", "Unity 在一帧内处理的静态批次数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Static Batched Triangles Count", "静态批次中包含的游戏对象中的三角形数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Static Batched Vertices Count", "静态批次中包含的游戏对象中的顶点数。");
                    //Instancing
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Instanced Batched Draw Calls Count", "Unity 合并为实例化批次的绘制调用数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Instanced Batches Count", "Unity 在一帧内渲染实例化游戏对象的处理批次数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Instanced Batched Triangles Count", "实例化游戏对象中的三角形数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Instanced Batched Vertices Count", "实例化游戏对象中的顶点数。");
                    //
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Used Textures Count", "Unity 在帧期间使用的纹理数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Used Textures Bytes", "Unity 在帧期间使用的纹理使用的内存量。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Render Textures Changes Count", "Unity 在帧期间将一个或多个 RenderTextures 设置为渲染目标的次数。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Used Buffers Count", "所使用的 GPU 缓冲区和内存的总数。这包括顶点、索引和计算缓冲区以及渲染所需的所有内部缓冲区。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Used Buffers Bytes", "所使用的 GPU 缓冲区和内存的总数。这包括顶点、索引和计算缓冲区以及渲染所需的所有内部缓冲区。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Vertex Buffer Upload In Frame Count", "CPU 在帧中上传到 GPU 的几何体数量。这代表顶点/法线/ texcoord 数据。GPU 上可能已经有一些几何体。此统计信息仅包括 Unity 在帧中传输的几何体。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Vertex Buffer Upload In Frame Bytes", "CPU 在帧中上传到 GPU 的几何体数量。这代表顶点/法线/ texcoord 数据。GPU 上可能已经有一些几何体。此统计信息仅包括 Unity 在帧中传输的几何体。");

                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Index Buffer Upload In Frame Count", "CPU 在帧中上传到 GPU 的几何体数量。这表示三角形索引数据。GPU 上可能已经有一些几何体。此统计信息仅包括 Unity 在帧中传输的几何体。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Index Buffer Upload In Frame Bytes", "CPU 在帧中上传到 GPU 的几何体数量。这表示三角形索引数据。GPU 上可能已经有一些几何体。此统计信息仅包括 Unity 在帧中传输的几何体。");
                    _renderProfilerAllSamplingData.Add(ProfilerCategory.Render, "Shadow Casters Count", "在一帧中投射阴影的游戏对象的数量。如果一个游戏对象投射多个阴影（因为多个光源照亮它），该对象投射的每个阴影都有一个条目。");
                }
                return _renderProfilerAllSamplingData;
            }
        }

        /// <summary>
        /// 渲染简报
        /// </summary>
        public static string RenderProfilerStr
        {
            get
            {
                return RenderProfilerAllSamplingData.ToString();
            }
        }

        /// <summary>
        /// 渲染简报
        /// </summary>
        /// <param name="profilerStr"></param>
        /// <returns></returns>
        public static string RenderProfilerStrTarget(string profilerStr)
        {
            SamplingData samplingData = RenderProfilerAllSamplingData.FinSamplingData(profilerStr);
            if (samplingData != null)
            {
                return samplingData.ToString();
            }
            else
            {
                return null;
            }
        }

#endregion

#region//Internal

        static ProfilerAllSamplingData _internalProfilerAllSamplingData;

        /// <summary>
        /// Internal简报
        /// </summary>
        public static ProfilerAllSamplingData InternalProfilerAllSamplingData
        {
            get
            {
                if (_internalProfilerAllSamplingData==null)
                {
                    _internalProfilerAllSamplingData = new ProfilerAllSamplingData();
                    _internalProfilerAllSamplingData.Add(new SamplingDataMainThreadTime(ProfilerCategory.Internal, "Main Thread", "主线程平均帧耗时", 15));
#if UNITY_2022_1_OR_NEWER
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "CPU Total Frame Time", "CPU Total Frame [ms]");
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "CPU Main Thread Frame Time", "CPU Main Thread [ms]");
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "CPU Render Thread Frame Time", "CPU Render Thread [ms]");
#endif
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "FixedUpdate.PhysicsFixedUpdate", "PhysicsFixedUpdate [ms]");
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "BehaviourUpdate", "BehaviourUpdate [ms]");
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "LateBehaviourUpdate", "LateUpdate [ms]");
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "FixedBehaviourUpdate", "FixedUpdate [ms]");
                    _internalProfilerAllSamplingData.Add(ProfilerCategory.Internal, "GPU Frame Time", "GPU [ms]");
                }
                return _internalProfilerAllSamplingData;
            }
        }

        /// <summary>
        /// Internal简报
        /// </summary>
        public static string InternalProfilerStr
        {
            get
            {
                return InternalProfilerAllSamplingData.ToString();
            }
        }

        /// <summary>
        /// Internal简报
        /// </summary>
        /// <param name="profilerStr"></param>
        /// <returns></returns>
        public static string InternalProfilerStrTarget(string profilerStr)
        {
            SamplingData samplingData = InternalProfilerAllSamplingData.FinSamplingData(profilerStr);
            if (samplingData != null)
            {
                return samplingData.ToString();
            }
            else
            {
                return null;
            }
        }

#endregion

        /// <summary>
        /// 性能简报
        /// </summary>
        public class ProfilerAllSamplingData : System.IDisposable
        {
            public ProfilerAllSamplingData()
            {
                sb = StringBuilderPool.Instance.GetOneStringBuilder(2048);
            }

            Dictionary<string, SamplingData> datas = new Dictionary<string, SamplingData>();

            public void Add(SamplingData samplingData)
            {
                datas.Add(samplingData.GetKey(), samplingData);
            }

            public void Add(ProfilerCategory profilerCategory, string profilerStr, string info, int capacity = 1)
            {
                SamplingData samplingData = new SamplingData(profilerCategory, profilerStr, info, capacity);
                datas.Add(profilerStr, samplingData);
            }

            public SamplingData FinSamplingData(string profilerStr)
            {
                SamplingData data = null;
                datas.TryGetValue(profilerStr,out data);
                return data;
            }

            StringBuilder sb;

            public string ToString(ProfilerCategory profilerCategory)
            {
                sb.Clear();
                Dictionary<string, SamplingData>.Enumerator enumerator = datas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.IsSame(profilerCategory))
                    {
                        sb.AppendLine(enumerator.Current.Value.ToString());
                    }
                }
                return sb.ToString();
            }

            public string ToString(ProfilerCategory profilerCategory, string profilerStr)
            {
                sb.Clear();
                Dictionary<string, SamplingData>.Enumerator enumerator = datas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.IsSame(profilerCategory, profilerStr))
                    {
                        sb.AppendLine(enumerator.Current.Value.ToString());
                    }
                }
                return sb.ToString();
            }

            public override string ToString()
            {
                sb.Clear();
                Dictionary<string, SamplingData>.Enumerator enumerator = datas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    sb.AppendLine(enumerator.Current.Value.ToString());
                }
                return sb.ToString();
            }

            public void Dispose()
            {
                Dictionary<string, SamplingData>.Enumerator enumerator = datas.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SamplingData samplingData = enumerator.Current.Value;
                    samplingData.Dispose();
                }
                datas.Clear();
                StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
                sb = null;
            }
        }

        public class SamplingData : System.IDisposable
        {
            public SamplingData(ProfilerCategory profilerCategory, string profilerStr, string info, int capacity = 1)
            {
                this.profilerStr = profilerStr;
                this.info = info;
                this.profilerCategory = profilerCategory;
                sb = StringBuilderPool.Instance.GetOneStringBuilder(256);
                data = ProfilerRecorder.StartNew(profilerCategory, profilerStr, capacity);
            }

            protected string info;

            protected string profilerStr;

            protected ProfilerRecorder data;

            protected ProfilerCategory profilerCategory;

            protected StringBuilder sb;

            public bool IsSame(ProfilerCategory profilerCategory, string profilerStr)
            {
                if (profilerStr.CompareTo(this.profilerStr) == 0 && IsSame(profilerCategory))
                {
                    return true;
                }
                return false;
            }

            public bool IsSame(ProfilerCategory profilerCategory)
            {
                if (profilerCategory == this.profilerCategory ||
                    (profilerCategory.Name != null && this.profilerCategory.Name != null && profilerCategory.Name.CompareTo(this.profilerCategory.Name) == 0))
                {
                    return true;
                }
                return false;
            }

            public bool IsSame(string key)
            {
                if (key != null && this.profilerCategory.Name != null && this.profilerCategory.Name.CompareTo(key) == 0)
                {
                    return true;
                }
                return false;
            }

            public string GetInfo()
            {
                return info;
            }

            public string GetKey()
            {
                return profilerStr;
            }

            public override string ToString()
            {
                sb.Clear();
                sb.AppendLine($"{profilerStr}: {data.LastValue}  Info:{info}");
                return sb.ToString();
            }

            public void Dispose()
            {
                info = null;
                profilerStr = null;
                data.Dispose();
                StringBuilderPool.Instance.PutBackOneStringBuilder(sb);
                sb = null;
            }

            protected static double GetRecorderFrameAverage(ProfilerRecorder recorder)
            {
                int samplesCount = recorder.Capacity;
                if (samplesCount == 0)
                    return 0;

                double r = 0;
                unsafe
                {
                    ProfilerRecorderSample* samples = stackalloc ProfilerRecorderSample[samplesCount];
                    recorder.CopyTo(samples, samplesCount);
                    for (var i = 0; i < samplesCount; ++i)
                        r += samples[i].Value;
                    r /= samplesCount;
                }

                return r;
            }
        }

        public class SamplingDataMainThreadTime : SamplingData
        {
            public SamplingDataMainThreadTime(ProfilerCategory profilerCategory, string profilerStr, string info, int capacity = 1) : base(profilerCategory, profilerStr, info, capacity)
            {

            }

            public override string ToString()
            {
                sb.Clear();
                sb.AppendLine($"Frame Time: {GetRecorderFrameAverage(data) * (1e-6f):F1} ms   Info:{info}");
                return sb.ToString();
            }
        }

        public class SamplingDataMemory : SamplingData
        {
            public SamplingDataMemory(ProfilerCategory profilerCategory, string profilerStr, string info, int capacity = 1) : base(profilerCategory, profilerStr, info, capacity)
            {

            }

            public override string ToString()
            {
                sb.Clear();
                sb.AppendLine($"{profilerStr}: {data.LastValue / (1024 * 1024)} MB   Info:{info}");
                return sb.ToString();
            }
        }

#endregion

    }
}


