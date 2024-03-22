using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace com.vivo.openxr
{
    [ExecuteAlways]
    [Serializable]
    public class VXRLateLatching : MonoBehaviour
    {
#if UNITY_2020_3_OR_NEWER

        UnityEngine.XR.XRDisplaySubsystem xrDisplaySubsystem;

        Camera camera;

        Transform cameraTransform;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (gameObject.GetComponent<Camera>()==null)
                {
                    DestroyImmediate(this);
                    UnityEngine.Debug.Log("camera component not find , Destroy VXRLateLatching .");
                }
            }
#endif
            if (Application.isPlaying)
            {
                xrDisplaySubsystem = VXRCommon.GetDisplaySubsystem() as UnityEngine.XR.XRDisplaySubsystem;
                camera = gameObject.GetComponent<Camera>();
                if (VXRCommon.IsURP)
                {
                    //
                    RenderPipelineManager.endCameraRendering -= EndCameraRendering;
                    RenderPipelineManager.endCameraRendering += EndCameraRendering;

                    RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
                    RenderPipelineManager.beginCameraRendering += BeginCameraRendering;
                }
            }
            else
            {
                xrDisplaySubsystem = null;
                camera = null;
            }
            if (camera!=null)
            {
                cameraTransform = camera.transform;
            }
            else
            {
                cameraTransform = null;
            }
        }

        private void OnDisable()
        {
            if (VXRCommon.IsURP)
            {
                RenderPipelineManager.endCameraRendering -= EndCameraRendering;
                RenderPipelineManager.beginCameraRendering -= BeginCameraRendering;
            }
        }

        void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (xrDisplaySubsystem != null && cameraTransform != null)
            {
                xrDisplaySubsystem.MarkTransformLateLatched(cameraTransform, UnityEngine.XR.XRDisplaySubsystem.LateLatchNode.Head);
            }
        }

        private void OnPreRender()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (xrDisplaySubsystem != null && camera != null)
            {
                xrDisplaySubsystem.BeginRecordingIfLateLatched(camera);
            }
        }

        private void OnPostRender()
        {
            if (!Application.isPlaying)
            {
                return;
            }
            if (xrDisplaySubsystem != null && camera != null)
            {
                xrDisplaySubsystem.EndRecordingIfLateLatched(camera);
            }
        }

        private void BeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            OnPreRender();
        }

        private void EndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            OnPostRender();
        }

#endif

    }
}


