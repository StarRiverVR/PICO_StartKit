using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.editor
{
    /// <summary>
    /// 依赖配置
    /// </summary>
    [CreateAssetMenu(fileName = "DependentConfig", menuName = "ScriptableObject/DependentConfig", order = 0)]
    public class DependentConfig : ScriptableObject
    {
        public List<PackageInfo> dependentPackages;

        public List<SampleInfo> dependentSamples;
        
        [Serializable]
        public struct PackageInfo
        {
            public string PackageName;
            public string PackageVersion;
        }

        [Serializable]
        public struct SampleInfo
        {
            public string PackageName;
            public string SampleName;
        }
    }
}
