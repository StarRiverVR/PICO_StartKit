using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public static bool StartEyeTracking()
        {
            Result ret = VXRVersion_0_0_1.vxr_StartEyeTracking();
            return ret == Result.Success;
        }

        /// <summary>
        /// <see cref="VXRVersion_0_0_1.vxr_GetEyeTrackingData(out EyeTrackingData)"/>
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool GetEyeTrackingData(out Vector3 origin, out Vector3 target)
        {
            Result ret = VXRVersion_0_0_1.vxr_GetEyeTrackingData(out EyeTrackingData data);
            if (ret == Result.Success)
            {
                if (data.Flags == 0)
                {
                    origin = Vector3.zero;
                    target = Vector3.zero;
                }
                else
                {
                    //origin = Camera.main.transform.localToWorldMatrix * data.GazeOriginCombined.ToVector3();
                    origin = Camera.main.transform.position;
                    target = data.GazeDirectionCombined.ToVector3();
                    target.z = 10;
                    target = Camera.main.transform.localToWorldMatrix * target;
                }
            }
            else
            {
                origin = Vector3.zero;
                target = Vector3.zero;
            }

            return ret == Result.Success;
        }

        public static bool GetEyeTrackingData(out EyeTrackingData data)
        {
            Result ret = VXRVersion_0_0_1.vxr_GetEyeTrackingData(out data);
            return ret == Result.Success;
        }

        public static bool StopEyeTracking()
        {
            Result ret = VXRVersion_0_0_1.vxr_StopEyeTracking();
            return ret == Result.Success;
        }
    }
}
