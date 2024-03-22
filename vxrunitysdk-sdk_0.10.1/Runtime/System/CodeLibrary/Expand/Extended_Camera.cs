
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    public static class Extended_Camera
    {

        /// <summary>
        /// 获得距离摄像机前方distance处的摄像机视口位置
        /// 0：左上角 1：右上角 2：左下角 3：右下角
        /// CameraCornersData使用完之后需要进行数据回收 CameraCornersData.PutBackOne()
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="cam"></param>
        /// 用法:Camera cam.GetCorners(0);
        /// <returns></returns>
        public static CameraCornersData GetCorners(this Camera cam, float distance)
        {
            CameraCornersData res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetCornersRun(cam, distance);
                }, null);
            }
            else
            {
                res = GetCornersRun(cam, distance);
            }
            return res;
        }

        static CameraCornersData GetCornersRun(Camera cam, float distance)
        {
            CameraCornersData res = CameraCornersData.GetOne();
            float height;
            float width;
            if (cam.orthographic)
            {
                float orSize = cam.orthographicSize;
                float ratio = (float)Screen.width / (float)Screen.height;
                height = orSize * 2f;
                width = ratio * height;
                height = height / 2f;
                width = width / 2f;
            }
            else
            {
                float halfFOV = (cam.fieldOfView * 0.5f) * Mathf.Deg2Rad;
                float aspect = cam.aspect;
                height = distance * Mathf.Tan(halfFOV);
                width = height * aspect;
            }
            // UpperLeft
            Vector3 upperLeft = cam.transform.position - (cam.transform.right * width);
            upperLeft += cam.transform.up * height;
            upperLeft += cam.transform.forward * distance;
            res.Resoult.Add(upperLeft);

            // UpperRight
            Vector3 upperRight = cam.transform.position + (cam.transform.right * width);
            upperRight += cam.transform.up * height;
            upperRight += cam.transform.forward * distance;
            res.Resoult.Add(upperRight);

            // LowerLeft
            Vector3 lowerLeft = cam.transform.position - (cam.transform.right * width);
            lowerLeft -= cam.transform.up * height;
            lowerLeft += cam.transform.forward * distance;
            res.Resoult.Add(lowerLeft);

            // LowerRight
            Vector3 lowerRight = cam.transform.position + (cam.transform.right * width);
            lowerRight -= cam.transform.up * height;
            lowerRight += cam.transform.forward * distance;
            res.Resoult.Add(lowerRight);

            return res;
        }

        /// <summary>
        /// 直接赋给UI的transform.position (3D转换UI坐标)
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="posWorld"></param>
        /// 用法:Camera cam.WorldToUI(pos);
        /// <returns></returns>
        public static Vector3 WorldToUI(this Camera camera, Vector3 posWorld)
        {
            Vector3 res = Vector3.one;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = WorldToUIRun(camera, posWorld);
                }, null);
            }
            else
            {
                res = WorldToUIRun(camera, posWorld);
            }
            return res;
        }

        static Vector3 WorldToUIRun( Camera camera, Vector3 posWorld)
        {
            return camera.WorldToScreenPoint(posWorld);
        }

        /// <summary>
        /// 判断是否在摄像机视口
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="worldPos"></param>
        /// 用法:Camera cam.IsInCameraView(worldPos);
        /// <returns></returns>
        public static bool IsInCameraView(this Camera camera, Vector3 worldPos)
        {
            bool res = false;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = IsInCameraViewRun(camera, worldPos);
                }, null);
            }
            else
            {
                res = IsInCameraViewRun(camera, worldPos);
            }
            return res;
        }

        static bool IsInCameraViewRun(this Camera camera, Vector3 worldPos)
        {
            Transform camTransform = camera.transform;
            Vector2 viewPos = camera.WorldToViewportPoint(worldPos);
            Vector3 dir = (worldPos - camTransform.position).normalized;
            float dot = Vector3.Dot(camTransform.forward, dir);     //判断物体是否在相机前面
            if (dot > 0 && viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 调节游戏屏幕大小
        /// width:屏幕宽度 height:屏幕高度
        /// fullscreen:是否满屏显示 refreshRate:刷新率
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="width">要设置成的宽度</param>
        /// <param name="height">要设置成的高度</param>
        /// <param name="setAllCam">是否设置全部</param>
        public static void SetScreenSize(this Camera camera, int width, int height, bool setAllCam)
        {
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    SetScreenSizeRun(camera, width, height, setAllCam);
                }, null);
            }
            else
            {
                SetScreenSizeRun(camera, width, height, setAllCam);
            }
        }

        static void SetScreenSizeRun( Camera camera, int width, int height, bool setAllCam)
        {
            if (setAllCam)
            {
                Camera[] cams = Camera.allCameras;
                if (cams != null)
                {
                    for (int i = 0; i < cams.Length; i++)
                    {
                        cams[i].aspect = (float)width / height;
                    }
                }
            }
            else
            {
                camera.aspect = (float)width / height;
            }
        }
    }

    /// <summary>
    /// 摄像机视口数据 使用完之后需要进行数据回收 CameraCornersData.PutBackOne()
    /// </summary>
    public class CameraCornersData
    {
        public bool IsPutBack = false;

        /// <summary>
        /// 存放整数每个位的数 0：左上角 1：右上角 2：左下角 3：右下角
        /// </summary>
        public List<Vector3> Resoult = new List<Vector3>();

        #region//池子

        static Queue<CameraCornersData> pool = new Queue<CameraCornersData>();

        /// <summary>
        /// 取出数据
        /// </summary>
        /// <returns></returns>
        public static CameraCornersData GetOne()
        {
            if (pool.Count > 0)
            {
                CameraCornersData findData = pool.Dequeue();
                while (findData == null && pool.Count > 0)
                {
                    findData = pool.Dequeue();
                }
                if (findData != null)
                {
                    findData.IsPutBack = false;
                    return findData;
                }
            }
            CameraCornersData newData = new CameraCornersData();
            newData.IsPutBack = false;
            return newData;
        }

        /// <summary>
        /// 回收数据
        /// </summary>
        /// <param name="data"></param>
        public static void PutBackOne(CameraCornersData data)
        {
            if (data.IsPutBack) return;
            data.IsPutBack = true;
            data.Resoult.Clear();
            pool.Enqueue(data);
        }

        #endregion
    }
}


