using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.openxr
{
    public class VXRPath
    {

        public static string VivoOpenXRUnityDir = "Assets/VivoUnityOpenXRPlugin/Runtime/Data/";

        public static string VivoOpenXRFileDir
        {
            get
            {
                return Application.dataPath+ "/VivoUnityOpenXRPlugin/Runtime/Data/OpenXRLoader/";
            }
        }
    }
}


