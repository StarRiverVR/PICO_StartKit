using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using Unity.Profiling;


namespace com.vivo.codelibrary
{
    public class ProfilerMono : MonoBehaviour
    {
        public float DebugSecond = 3;

        public List<ProfilerSamplingType> SetList = new List<ProfilerSamplingType>();

        void Start()
        {

        }

        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        void FPS()
        {
            VLog.Info($"FPS={Profiler.fps.FPS} , LastFPS = {Profiler.fps.LastFPS} , AverageFPS = {Profiler.fps.AverageFPS} ");
        }

        float debugSecondTime = 0;

        void Update()
        {
            bool findProfilerInternal = false;
            bool findProfilerMemory = false;
            bool findProfilerRender = false;
            bool findFrameCount = false;
            for (int i = 0; i < SetList.Count; ++i)
            {
                switch (SetList[i])
                {
                    case ProfilerSamplingType.FrameCount:
                        {
                            findFrameCount = true;
                        }
                        break;
                    case ProfilerSamplingType.Profiler_Internal:
                        {
                            findProfilerInternal = true;
                        }
                        break;
                    case ProfilerSamplingType.Profiler_Memory:
                        {
                            findProfilerMemory = true;
                        }
                        break;
                    case ProfilerSamplingType.Profiler_Render:
                        {
                            findProfilerRender = true;
                        }
                        break;
                }
            }
            debugSecondTime = debugSecondTime + Time.deltaTime;
            if (debugSecondTime >= DebugSecond)
            {
                debugSecondTime = 0;
                SetList.TrimExcess();
                if (findProfilerInternal)
                {
                    VLog.Info(Profiler.ProfilerSamplingStr_Internal);
                }
                if (findProfilerMemory)
                {
                    VLog.Info(Profiler.ProfilerSamplingStr_Memory);
                }
                if (findProfilerRender)
                {
                    VLog.Info(Profiler.ProfilerSamplingStr_Render);
                }
            }
            if (findFrameCount)
            {
                FPS();
            }
        }

        public enum ProfilerSamplingType
        {
            Profiler_Internal,
            Profiler_Memory,
            Profiler_Render,
            FrameCount,
        }

    }
}


