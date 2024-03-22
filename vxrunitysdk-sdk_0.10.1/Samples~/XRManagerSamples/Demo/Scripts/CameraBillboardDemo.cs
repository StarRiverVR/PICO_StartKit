using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.codelibrary;

[ExecuteAlways]
public class CameraBillboardDemo : MonoBehaviour
{

    [Range(-1,1)]
    public float OffsetV = -0.35f;
    [Range(-1, 1)]
    public float OffsetVUrp = -0.167f;

    [Range(-1, 1)]
    public float OffsetH = 0.2f;
    [Range(-1, 1)]
    public float OffsetHUrp = 0.65f;

    public float OffsetZDis = 0;

    public Vector3 EulerAnglesOffset;

    public CanvasGroup canvasGroup;

    private void Start()
    {
        Fresh();
    }

    private void OnEnable()
    {
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.alpha = 0;
        }
    }

    private void Update()
    {
        if (canvasGroup!=null && canvasGroup.alpha<1)
        {
            if (Application.isPlaying)
            {
                canvasGroup.alpha = canvasGroup.alpha + Time.deltaTime;
            }
            else
            {
                canvasGroup.alpha = 1f;
            }
        }
        Fresh();
    }

    void Fresh()
    {
        if (Camera.main != null)
        {
            Vector3 v1 = Vector3.zero;
            Vector3 v2 = Vector3.zero;
            Vector3 v3 = Vector3.zero;
            Vector3 v4 = Vector3.zero;
            GetCornersRun(Camera.main, 3f, ref v1, ref v2, ref v3, ref v4);
            //

            float offsetVDis = OffsetV * Vector3.Distance(v1, v2);
            if (com.vivo.openxr.VXRCommon.IsURP)
            {
                offsetVDis = OffsetVUrp * Vector3.Distance(v1, v2);
            }
            float offsetHDis = OffsetH * Vector3.Distance(v1, v3);
            if (com.vivo.openxr.VXRCommon.IsURP)
            {
                offsetHDis = OffsetHUrp * Vector3.Distance(v1, v3);
            }

            transform.position = v1 + offsetVDis * Camera.main.transform.up + offsetHDis * Camera.main.transform.right + OffsetZDis * Camera.main.transform.forward;
            //

            transform.eulerAngles = Camera.main.transform.eulerAngles;
            transform.localEulerAngles = transform.localEulerAngles + EulerAnglesOffset;
        }
    }

    static void GetCornersRun(Camera cam, float distance,ref Vector3 v1,ref Vector3 v2,ref Vector3 v3, ref Vector3 v4)
    {

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
            //camera.projectionMatrix;
            //float fieldOfView = Mathf.Rad2Deg * Mathf.Atan(1.0f / cam.projectionMatrix.m11) * 2.0f;
            //float halfFOV = (fieldOfView * 0.5f) * Mathf.Deg2Rad;

            //Rect pixelRect = cam.pixelRect;
            //float cameraAspect = (float)pixelRect.width / (float)pixelRect.height;
            float cameraAspect = cam.aspect;

            float halfFOV = (cam.fieldOfView * 0.5f) * Mathf.Deg2Rad;
            float aspect = cameraAspect;
            height = distance * Mathf.Tan(halfFOV);
            width = height * aspect;
        }
        // UpperLeft
        Vector3 upperLeft = cam.transform.position - (cam.transform.right * width);
        upperLeft += cam.transform.up * height;
        upperLeft += cam.transform.forward * distance;
        v1 = upperLeft;

        // UpperRight
        Vector3 upperRight = cam.transform.position + (cam.transform.right * width);
        upperRight += cam.transform.up * height;
        upperRight += cam.transform.forward * distance;
        v2 = upperRight;

        // LowerLeft
        Vector3 lowerLeft = cam.transform.position - (cam.transform.right * width);
        lowerLeft -= cam.transform.up * height;
        lowerLeft += cam.transform.forward * distance;
        v3 = lowerLeft;

        // LowerRight
        Vector3 lowerRight = cam.transform.position + (cam.transform.right * width);
        lowerRight -= cam.transform.up * height;
        lowerRight += cam.transform.forward * distance;
        v4 = lowerRight;

    }
}
