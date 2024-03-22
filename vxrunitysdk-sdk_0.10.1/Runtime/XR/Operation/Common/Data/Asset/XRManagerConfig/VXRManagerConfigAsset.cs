
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace com.vivo.openxr {

    [System.Serializable]
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class VXRManagerConfigAsset : VXRAssetBase<VXRManagerConfigAsset>, ISerializationCallbackReceiver
    {

        static string assetDir = "XRManagerSet/";

        public static VXRManagerConfigAsset Data
        {
            get
            {
                return GetData(ref assetDir, true);
            }
        }

        protected override void OnCreateInstance()
        {
            VXRManagerConfigAsset data = Data;
            if (projectConfig==null)
            {
                projectConfig = new VXRProjectConfig();
            }
            projectConfig.Init();
        }

        public VXRProjectConfig projectConfig;

        public void EditorApplyData()
        {
            projectConfig.EditorApplyData();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {           

        }
    }

    [System.Serializable]
    public class VXRProjectConfig
    {
        public void Init()
        {
            insightPassthroughSupport = FeatureSupport.None;
            MsaaSampleCount = VXRCommon.MsaaSample;
            RenderViewportScale = 1f;
            OpenFadeInOutOnStart = false;
            FadeInOutStartDelay = 0f;
            FadeInOutAnimLenght = 5f;
            FadeInOutStartColor = new Color(0, 0, 0, 1);
            FadeInOutEndColor = new Color(0, 0, 0, 0);
            LateLatching = false;
        }

        public FeatureSupport insightPassthroughSupport
        {
            get => _insightPassthroughSupport;
            set => _insightPassthroughSupport = value;
        }

        [SerializeField]
        internal FeatureSupport _insightPassthroughSupport = FeatureSupport.None;

        public UnityEngine.Rendering.MSAASamples MsaaSampleCount = UnityEngine.Rendering.MSAASamples.None;

        [Range(0,1)]
        public float RenderViewportScale = 1f;

        public enum FeatureSupport
        {
            None = 0,
            Supported = 1,
            Required = 2
        }

        public Texture2D SystemSplashScreen;

        #region//Fade In Out

        public bool OpenFadeInOutOnStart = false;

        public float FadeInOutStartDelay = 0f;

        public float FadeInOutAnimLenght = 5f;

        public Color FadeInOutStartColor = new Color(0,0,0,1);

        public Color FadeInOutEndColor = new Color(0, 0, 0, 0);

        public Texture2D FadeInOutTexture = null;

        #endregion

        public bool LateLatching = false;

        /// <summary>
        /// 数据应用
        /// </summary>
        public void EditorApplyData()
        {
            VXRCommon.MsaaSample = MsaaSampleCount;
#if UNITY_ANDROID && !UNITY_EDITOR
            VXRCommon.RenderViewportScale = RenderViewportScale;
#endif
            VXRManagerConfigAsset.Data.SaveOnEditor();
        }
    }

    static class VXRProjectConfigExtensions
    {
        public static string ToRequiredAttributeValue(this VXRProjectConfig.FeatureSupport value)
            => value == VXRProjectConfig.FeatureSupport.Required ? "true" : "false";
    }


}



