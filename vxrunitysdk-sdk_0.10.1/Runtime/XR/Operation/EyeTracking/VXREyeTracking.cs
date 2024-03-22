using UnityEngine;

namespace com.vivo.openxr
{
    public class VXREyeTracking
    {
        /// <summary>
        /// 眼动追踪数据
        /// </summary>
        public struct EyeGazeData
        {
            public Vector3 Position;//眼动原点坐标
            public Quaternion Rotation;//眼动原点方向
            public bool IsTracked;// 眼动数据是否可用
        }
        
        /// <summary>
        /// 开始眼动追踪
        /// </summary>
        public static void StartEyeTracking()
        {
            VXRPlugin.StartEyeTracking();
        }
        
        /// <summary>
        /// 停止眼动追踪
        /// </summary>
        public static void StopEyeTracking()
        {
            VXRPlugin.StopEyeTracking();
        }
        
        /// <summary>
        /// 获取眼动追踪中心眼信息
        /// </summary>
        /// <param name="data">眼动中心眼位置信息</param>
        /// <returns>是否获取数据成功</returns>
        public static bool GetEyeGazeData(out EyeGazeData data)
        {
            data = default(EyeGazeData);

            if(VXRPlugin.GetEyeTrackingData(out VXRPlugin.EyeTrackingData centerEye))
            {
                data.IsTracked = centerEye.Flags != 0;
                data.Position = Camera.main.transform.position;
                var target = centerEye.GazeDirectionCombined.ToVector3();
                target.z = 10;
                target = Camera.main.transform.localToWorldMatrix * target;
                data.Rotation = Quaternion.LookRotation(target - data.Position);
                return true;
            }

            data.IsTracked = false;
            data.Position = Vector3.zero;
            data.Rotation = Quaternion.identity;
            return false;

        }
    }
}
