using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace com.vivo.openxr
{
    public static class VXRExpand
    {
#if UNITY_EDITOR
        public static BuildTarget ToBuildTarget(this BuildTargetGroup buildTargetGroup)
        {
            return buildTargetGroup switch
            {
                BuildTargetGroup.Android => BuildTarget.Android,
                BuildTargetGroup.Standalone => BuildTarget.StandaloneWindows64,
                _ => BuildTarget.NoTarget
            };
        }

#endif
    }
}


