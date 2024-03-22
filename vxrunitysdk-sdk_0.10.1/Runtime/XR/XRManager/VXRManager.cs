
using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{
    [ExecuteAlways]
    public class VXRManager : MonoBehaviour
    {
        public static VXRManager Instance;

        public Camera MainCamera;


#if UNITY_EDITOR
        public VXRManagerConfigAsset configAsset;
#endif

        private void OnEnable()
        {
#if UNITY_EDITOR
            configAsset = VXRManagerConfigAsset.Data;
#endif
            Instance = this;
            if (Application.isPlaying)
            {
                VXRCommon.MsaaSample = VXRManagerConfigAsset.Data.projectConfig.MsaaSampleCount;
                if (VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale>0)
                {
                    VXRCommon.RenderViewportScale = VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale;
                }
            }
            if (MainCamera == null || !MainCamera.CompareTag("MainCamera"))
            {
                MainCamera = Camera.main;
            }
        }

        private void Awake()
        {
          
            Debug.Log("启动VXRManager");
            if (Application.isPlaying)
            {
                Initialize();
                StartFadeInOutAnim();
            }
        }

        private void Start()
        {
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                if (VXRCommon.MsaaSample != VXRManagerConfigAsset.Data.projectConfig.MsaaSampleCount)
                {
                    VXRCommon.MsaaSample = VXRManagerConfigAsset.Data.projectConfig.MsaaSampleCount;
                }
                if (VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale>0)
                {
                    if (VXRCommon.RenderViewportScale != VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale)
                    {
                        VXRCommon.RenderViewportScale = VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale;
                    }
                }
                else
                {
                    VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale = 1;
                    VXRCommon.RenderViewportScale = 1;
                }

            }
            else
            {
#if VXRSPATIALANCHOR 
                VXRSpatialAnchor.Update();
#endif

            }
        }

        void Initialize()
        {
#if VXREYETRACKING
            InitEyeTracking();
#endif
#if VXRFOVEATIONRENDERING
            InitFoveationReadering();
#endif
            InitVxrEvent();
        }

#if VXREYETRACKING
        #region//EyeTracking

        [SerializeField]
        [HideInInspector]
        private bool eyeTracking = false;

        private void InitEyeTracking()
        {
            if (eyeTracking)
            {
                VXREyeTrackingDevice device = GameObject.FindObjectOfType<VXREyeTrackingDevice>();
                if (device == null)
                {
                   gameObject.AddComponent<VXREyeTrackingDevice>();
                }
            }
        }

        #endregion
#endif

        #region//Fade In Out

        void StartFadeInOutAnim()
        {
            if (VXRManagerConfigAsset.Data.projectConfig.OpenFadeInOutOnStart)
            {
                VXRCommon.StartFadeInOutAnim(VXRManagerConfigAsset.Data.projectConfig.FadeInOutStartDelay, VXRManagerConfigAsset.Data.projectConfig.FadeInOutAnimLenght,
                    VXRManagerConfigAsset.Data.projectConfig.FadeInOutStartColor, VXRManagerConfigAsset.Data.projectConfig.FadeInOutEndColor,
                    VXRManagerConfigAsset.Data.projectConfig.FadeInOutTexture);
            }
        }

        #endregion

#if VXRFOVEATIONRENDERING
        #region//FFR
        [SerializeField]
        [HideInInspector]
        VXRFoveationRendering.Level foveationLevel;
        [SerializeField]
        [HideInInspector]
        bool useFoveationDynamic;
        void InitFoveationReadering()
        {
            VXRFoveationRendering.SetFoveationLevel(foveationLevel);
            VXRFoveationRendering.SetUseDynamicFoveationLevel(useFoveationDynamic);
        }
        #endregion
#endif

        #region //VXREvent
        private void InitVxrEvent()
        {
            EventManager.Instance.Init();
        }
        #endregion
    }
}
