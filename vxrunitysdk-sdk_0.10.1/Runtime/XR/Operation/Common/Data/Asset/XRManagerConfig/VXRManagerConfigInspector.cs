#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.IO;
using System;


namespace com.vivo.openxr
{
    public class VXRManagerConfigInspector
    {
        static SerializedObject serializedObject;
        static SerializedProperty projectConfigObject;
        //
        const string kMsaaSampleCount = "MsaaSampleCount";
        static GUIContent s_MsaaSampleCountLabel = EditorGUIUtility.TrTextContent("MSAA Level");
        static SerializedProperty m_MsaaSampleCount;

        const string kRenderViewportScale = "RenderViewportScale";
        static GUIContent s_RenderViewportScaleLabel = EditorGUIUtility.TrTextContent("RenderViewportScale");
        static SerializedProperty m_RenderViewportScale;

        const string kSystemSplashScreen = "SystemSplashScreen";
        static GUIContent s_SystemSplashScreenLabel = EditorGUIUtility.TrTextContent("System Splash Screen");
        static SerializedProperty m_SystemSplashScreen;

        const string kInsightPassthroughSupport = "_insightPassthroughSupport";
        static GUIContent s_InsightPassthroughSupportLabel = EditorGUIUtility.TrTextContent("Insight Passthrough Support");
        static SerializedProperty m_InsightPassthroughSupport;

        #region//Fade In Out

        const string kOpenFadeInOutOnStart = "OpenFadeInOutOnStart";
        static GUIContent s_OpenFadeInOutOnStartLabel = EditorGUIUtility.TrTextContent("Open FadeInOut On Start");
        static SerializedProperty m_OpenFadeInOutOnStart;

        const string kFadeInOutStartDelay = "FadeInOutStartDelay";
        static GUIContent s_FadeInOutStartDelayLabel = EditorGUIUtility.TrTextContent("FadeInOut Start Delay");
        static SerializedProperty m_FadeInOutStartDelay;

        const string kFadeInOutAnimLenght = "FadeInOutAnimLenght";
        static GUIContent s_FadeInOutAnimLenghtLabel = EditorGUIUtility.TrTextContent("FadeInOut Anim Lenght");
        static SerializedProperty m_FadeInOutAnimLenght;

        const string kFadeInOutStartColor = "FadeInOutStartColor";
        static GUIContent s_FadeInOutStartColorLabel = EditorGUIUtility.TrTextContent("FadeInOut Start Color");
        static SerializedProperty m_FadeInOutStartColor;

        const string kFadeInOutEndColor = "FadeInOutEndColor";
        static GUIContent s_FadeInOutEndColorLabel = EditorGUIUtility.TrTextContent("FadeInOut End Color");
        static SerializedProperty m_FadeInOutEndColor;

        const string kFadeInOutTexture = "FadeInOutTexture";
        static GUIContent s_FadeInOutTextureLabel = EditorGUIUtility.TrTextContent("FadeInOut Texture");
        static SerializedProperty m_FadeInOutTexture;

        #endregion

        const string kLateLatching = "LateLatching";
        static GUIContent s_LateLatchingLabel = EditorGUIUtility.TrTextContent("Late Latching");
        static SerializedProperty m_LateLatching;

        #region//EyeTracking
        const string kEyeTracking = "eyeTracking";
        static GUIContent s_EyeTracking = EditorGUIUtility.TrTextContent("Eye Tracking");
        static SerializedProperty m_EyeTracking;
        #endregion

        #region//FFR
        const string kFoveationLevel = "foveationLevel";
        static GUIContent s_FoveationLevel = EditorGUIUtility.TrTextContent("Foveation Level");
        static SerializedProperty m_FoveationLevel;

        const string kUseFoveationDynamic = "useFoveationDynamic";
        static GUIContent s_UseFoveationDynamic = EditorGUIUtility.TrTextContent("Use Foveation Dynamic");
        static SerializedProperty m_UseFoveationDynamic;
        #endregion

        //
        public static void InspectorGUI(VXRManager target, VXRManagerConfigAsset data,SerializedObject vxrmanagerObject)
        {
            if (serializedObject==null)
            {
                serializedObject = new SerializedObject(data);
            }
            if (serializedObject == null || serializedObject.targetObject == null)
                return;
            if (projectConfigObject==null)
            {
                projectConfigObject = serializedObject.FindProperty("projectConfig");
            }

            if (VXRManagerConfigAsset.Data.projectConfig.MsaaSampleCount!= VXRCommon.MsaaSample)
            {
                VXRManagerConfigAsset.Data.projectConfig.MsaaSampleCount = VXRCommon.MsaaSample;
            }

//#if UNITY_ANDROID && !UNITY_EDITOR
//            if (VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale != VXRCommon.RenderViewportScale)
//            {
//                VXRManagerConfigAsset.Data.projectConfig.RenderViewportScale = VXRCommon.RenderViewportScale;
//            }
//#endif

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            //
            if (m_MsaaSampleCount == null) m_MsaaSampleCount = projectConfigObject.FindPropertyRelative(kMsaaSampleCount);
            EditorGUILayout.PropertyField(m_MsaaSampleCount, s_MsaaSampleCountLabel);

            if (m_RenderViewportScale == null) m_RenderViewportScale = projectConfigObject.FindPropertyRelative(kRenderViewportScale);
            EditorGUILayout.PropertyField(m_RenderViewportScale, s_RenderViewportScaleLabel);
            //if (m_RenderViewportScale.floatValue==0)
            //{
            //    m_RenderViewportScale.floatValue = 0.5f;
            //}

            if (m_SystemSplashScreen == null) m_SystemSplashScreen = projectConfigObject.FindPropertyRelative(kSystemSplashScreen);
            EditorGUILayout.PropertyField(m_SystemSplashScreen, s_SystemSplashScreenLabel);

            #region//Fade In Out

            if (m_OpenFadeInOutOnStart == null) m_OpenFadeInOutOnStart = projectConfigObject.FindPropertyRelative(kOpenFadeInOutOnStart);
            EditorGUILayout.PropertyField(m_OpenFadeInOutOnStart, s_OpenFadeInOutOnStartLabel);

            if (m_OpenFadeInOutOnStart.boolValue)
            {
                EditorGUI.indentLevel = EditorGUI.indentLevel + 2;
                
                if (m_FadeInOutStartDelay == null) m_FadeInOutStartDelay = projectConfigObject.FindPropertyRelative(kFadeInOutStartDelay);
                EditorGUILayout.PropertyField(m_FadeInOutStartDelay, s_FadeInOutStartDelayLabel);

                if (m_FadeInOutAnimLenght == null) m_FadeInOutAnimLenght = projectConfigObject.FindPropertyRelative(kFadeInOutAnimLenght);
                EditorGUILayout.PropertyField(m_FadeInOutAnimLenght, s_FadeInOutAnimLenghtLabel);

                try
                {
                    if (m_FadeInOutStartColor == null) m_FadeInOutStartColor = projectConfigObject.FindPropertyRelative(kFadeInOutStartColor);
                    EditorGUILayout.PropertyField(m_FadeInOutStartColor, s_FadeInOutStartColorLabel);

                    if (m_FadeInOutEndColor == null) m_FadeInOutEndColor = projectConfigObject.FindPropertyRelative(kFadeInOutEndColor);
                    EditorGUILayout.PropertyField(m_FadeInOutEndColor, s_FadeInOutEndColorLabel);
                }catch
                {

                }

                if (m_FadeInOutTexture == null) m_FadeInOutTexture = projectConfigObject.FindPropertyRelative(kFadeInOutTexture);
                EditorGUILayout.PropertyField(m_FadeInOutTexture, s_FadeInOutTextureLabel);

                EditorGUI.indentLevel = EditorGUI.indentLevel - 2;
            }

            #endregion

            if (m_LateLatching == null) m_LateLatching = projectConfigObject.FindPropertyRelative(kLateLatching);
            EditorGUILayout.PropertyField(m_LateLatching, s_LateLatchingLabel);

            VXRLateLatching vxrLateLatching = target.MainCamera.gameObject.GetComponent<VXRLateLatching>();
            if (m_LateLatching.boolValue)
            {
                if (vxrLateLatching == null)
                {
                    vxrLateLatching = target.MainCamera.gameObject.AddComponent<VXRLateLatching>();
                    Selection.activeObject = target.MainCamera.gameObject;
                }
            }
            else
            {
                if (vxrLateLatching != null)
                {
                    if (Application.isPlaying)
                    {
                        GameObject.Destroy(vxrLateLatching);
                    }
                    else
                    {
                        GameObject.DestroyImmediate(vxrLateLatching);
                    }
                    Selection.activeObject = target.MainCamera.gameObject;
                }
            }

            InspectorGUI_VXRManager(target, vxrmanagerObject);

            if (EditorGUI.EndChangeCheck())
            {
            
                serializedObject.ApplyModifiedProperties();

                data.EditorApplyData();
            }

            EditorGUILayout.EndVertical();
        }


        static void InspectorGUI_VXRManager(VXRManager target, SerializedObject vxrmanagerObject)
        {
            vxrmanagerObject.Update();
#if VXREYETRACKING
            #region//EyeTracking
            m_EyeTracking = vxrmanagerObject.FindProperty(kEyeTracking);
            EditorGUILayout.PropertyField(m_EyeTracking, s_EyeTracking);
            #endregion
#endif

#if VXRFOVEATIONRENDERING
            #region//FFR
            m_UseFoveationDynamic = vxrmanagerObject.FindProperty(kUseFoveationDynamic);
            EditorGUILayout.PropertyField(m_UseFoveationDynamic, s_UseFoveationDynamic);

            m_FoveationLevel = vxrmanagerObject.FindProperty(kFoveationLevel);
            EditorGUILayout.PropertyField(m_FoveationLevel, s_FoveationLevel);
            #endregion
#endif

            vxrmanagerObject.ApplyModifiedProperties();
            vxrmanagerObject.SetIsDifferentCacheDirty();
        }
    }
}

#endif
