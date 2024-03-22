using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;
using UnityEditor;
using System.Text;
using com.vivo.openxr;

namespace com.vivo.editor
{
    [CustomEditor(typeof(VXRManager))]
    public class VXRManagerInspector : Editor
    {

        /// <summary>
        /// 功能支持选项
        /// </summary>
        [SerializeField]
        public enum FeatureSupport
        {
            None = 0,
            Supported = 1,
            Required = 2
        }

        private void OnDisable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            try
            {
                DrawInspectorGUI();
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                throw (ex);
            }
            
            //
            base.OnInspectorGUI();
        }

        VXRManager manager;

        ConfigSelectType curConfigSelectType;

        System.Text.StringBuilder titleSB = new StringBuilder(256);

        void DrawInspectorGUI()
        {
            manager = (VXRManager)target;
            InitERRStack();
            if (errList.Count!=0)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                titleSB.Clear();
                titleSB.Append(" [Required:");
                {
                    for (int i = 0; i < VXRCommon.TargetGroups.Length; ++i)
                    {
                        titleSB.Append(VXRCommon.TargetGroups[i]);
                        if (i != VXRCommon.TargetGroups.Length - 1)
                        {
                            titleSB.Append(",");
                        }
                        else
                        {
                            titleSB.Append("]");
                        }
                    }
                    EditorGUILayout.LabelField($"Platform: {titleSB.ToString()}");
                }

                EditorGUILayout.LabelField("ERR Stack :", EditorStyles.boldLabel);
                EditorGUI.indentLevel = EditorGUI.indentLevel + 1;
                for (int i=0;i< errList.Count;++i)
                {
                    EditorGUILayout.LabelField(errList[i], EditorStyles.boldLabel);
                }
                EditorGUI.indentLevel = EditorGUI.indentLevel - 1;
                EditorGUILayout.EndVertical();
                return;
            }
            BuildTarget activeBuildTarget = VXRCommon.ActiveBuildTarget;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            titleSB.Clear();
            titleSB.Append(" [Required:");
            BuildTargetGroup[] targetGroups = VXRCommon.TargetGroups;
            for (int i=0;i< targetGroups.Length;++i)
            {
                titleSB.Append(targetGroups[i]);
                if (i!= targetGroups.Length-1)
                {
                    titleSB.Append(",");
                }
                else
                {
                    titleSB.Append("]");
                }
            }
            EditorGUILayout.LabelField($"Current platform is \"{activeBuildTarget}\". {titleSB.ToString()}");
            curConfigSelectType =(ConfigSelectType)GUILayout.SelectionGrid((int)curConfigSelectType, configSelectTypeStrs, 3, GUI.skin.button);
            switch (VXRCommon.ActiveBuildTarget)
            {
                case BuildTarget.Android:
                    {
                        DrawInspectorGUI_Android();
                    }
                    break;
                case BuildTarget.StandaloneWindows64:
                    {
                        DrawInspectorGUI_PC();
                    }
                    break;
            }
            EditorGUILayout.Space(5);
            //EditorGUILayout.LabelField("Project Set :", EditorStyles.boldLabel);
            //VXRManagerConfigInspector.InspectorGUI(VXRManagerConfigAsset.Data);

            EditorGUILayout.EndVertical();            
        }

        enum ConfigSelectType
        {
            Required, //必选配置
            Recommended, //推荐配置
            Optional, //可选配置
        }

        string[] _configSelectTypeStrs;

        string[] configSelectTypeStrs
        {
            get
            {
                if (_configSelectTypeStrs==null)
                {
                    _configSelectTypeStrs = new string[] { "Required", "Recommended", "Optional" };
                }
                return _configSelectTypeStrs;
            }
        }

        GUIContent tooltipLink = new GUIContent("[?]");

        void SetupEnumField<T>(string name, ref T member, ref bool modified,string urlLink = "",System.Action endAction=null)
        {
            modified = false;
            GUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            T value = (T)(object)EditorGUILayout.EnumPopup(new GUIContent(name), member as System.Enum);
            if (EditorGUI.EndChangeCheck())
            {
                member = value;
                modified = true;
            }

            if (endAction!=null)
            {
                endAction();
            }
            UrlLink(urlLink);
            GUILayout.EndHorizontal();
        }

        void SetupBoolField(string name, ref bool member, ref bool modified,string urlLink = "", System.Action endAction = null)
        {
            modified = false;
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            bool value = EditorGUILayout.Toggle(new GUIContent(name), member);
            if (EditorGUI.EndChangeCheck())
            {
                member = value;
                modified = true;
            }

            if (endAction != null)
            {
                endAction();
            }
            UrlLink(urlLink);
            EditorGUILayout.EndHorizontal();
        }

        void UrlLink(string urlLink)
        {
            if (!string.IsNullOrEmpty(urlLink))
            {
#if UNITY_2021_1_OR_NEWER
                if (EditorGUILayout.LinkButton(tooltipLink))
                {
                    Application.OpenURL(urlLink);
                }
#else
                if (GUILayout.Button(tooltipLink, GUILayout.ExpandWidth(false)))
                {
                    Application.OpenURL(urlLink);
                }
#endif
            }
        }

        enum XRColorSpace
        {
            Gamma = 0,
            Linear = 1
        }

        #region //Android

        class AndroidConfigData
        {
            public XRColorSpace colorSpace;

            public AndroidArchitecture targetArchitectures;

            public ScriptingImplementation scriptingBackend;

            public GraphicsDeviceType selectDeviceType = GraphicsDeviceType.OpenGLES3;

            public GraphicsDeviceType[] graphicsApis;

            public bool graphicsJobs;

            public VXRAsset.BuildReleaseMode releaseMode;

            public bool development;

            public bool exportAsGoogleAndroidProject;

            public FeatureSupport passthroughSupport = FeatureSupport.None;

        }

        AndroidConfigData androidConfigData = new AndroidConfigData ();

        void DrawInspectorGUI_Android()
        {
            bool hasModified = false;
            int colorSpace = (int)PlayerSettings.colorSpace;
            if (colorSpace<0)
            {
                colorSpace = 0;
            }
            androidConfigData.colorSpace = (XRColorSpace)colorSpace;
            androidConfigData.targetArchitectures = PlayerSettings.Android.targetArchitectures;
            androidConfigData.scriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Android);
            pcConfigData.useDefaultGraphicsAPIs = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.Android);
            androidConfigData.graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.Android);
            if (androidConfigData.graphicsApis == null)
            {
                androidConfigData.graphicsApis = new GraphicsDeviceType[0];
            }
            androidConfigData.graphicsJobs = PlayerSettings.graphicsJobs;
            androidConfigData.releaseMode = VXRCommon.ReleaseMode;
            androidConfigData.development = EditorUserBuildSettings.development;
            androidConfigData.exportAsGoogleAndroidProject = EditorUserBuildSettings.exportAsGoogleAndroidProject;
            EditorGUILayout.Space(5);

            switch (curConfigSelectType)
            {
                case ConfigSelectType.Required:
                    {
                        SetupEnumField<XRColorSpace>("ColorSpace [Require:Linear]",
                            ref androidConfigData.colorSpace, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.colorSpace = (ColorSpace)((int)androidConfigData.colorSpace);
                        }
                        SetupEnumField<AndroidArchitecture>("TargetArchitectures [Require:ARM64]",
                            ref androidConfigData.targetArchitectures, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.Android.targetArchitectures = androidConfigData.targetArchitectures;
                        }
                        SetupEnumField<ScriptingImplementation>("ScriptingBackend [Require:IL2CPP]",
                            ref androidConfigData.scriptingBackend, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, androidConfigData.scriptingBackend);
                        }
                        SetupEnumField<GraphicsDeviceType>("GraphicsDeviceType [Require:Vulkan]",
                            ref androidConfigData.selectDeviceType, ref hasModified,null,()=> {
                                if (GUILayout.Button("+"))
                                {
                                    List<GraphicsDeviceType> list = androidConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                    if (!list.Contains(androidConfigData.selectDeviceType))
                                    {
                                        list.Add(androidConfigData.selectDeviceType);
                                        androidConfigData.graphicsApis = list.ToArray();
                                        pcConfigData.useDefaultGraphicsAPIs = false;
                                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android,false);
                                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, androidConfigData.graphicsApis);
                                    }
                                }
                            });
                        if (androidConfigData.graphicsApis.Length>0)
                        {
                            EditorGUI.indentLevel = EditorGUI.indentLevel + 4;
                            for (int i = 0; i < androidConfigData.graphicsApis.Length; ++i)
                            {
                                GraphicsDeviceType targetDeviceType = androidConfigData.graphicsApis[i];
                                GUILayout.BeginHorizontal();
                                if (i==0)
                                {
                                    EditorGUILayout.LabelField(targetDeviceType.ToString() + " [Require:Vulkan]");
                                }
                                else
                                {
                                    EditorGUILayout.LabelField(targetDeviceType.ToString());
                                }
                                if (i>0)
                                {
                                    if (GUILayout.Button("↑"))
                                    {
                                        List<GraphicsDeviceType> list = androidConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                        list[i] = list[i - 1];
                                        list[i - 1] = targetDeviceType;
                                        androidConfigData.graphicsApis = list.ToArray();
                                        pcConfigData.useDefaultGraphicsAPIs = false;
                                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, androidConfigData.graphicsApis);
                                    }
                                }
                                if (i < androidConfigData.graphicsApis.Length-1)
                                {
                                    if (GUILayout.Button("↓"))
                                    {
                                        List<GraphicsDeviceType> list = androidConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                        list[i] = list[i + 1];
                                        list[i + 1] = targetDeviceType;
                                        androidConfigData.graphicsApis = list.ToArray();
                                        pcConfigData.useDefaultGraphicsAPIs = false;
                                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                                        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, androidConfigData.graphicsApis);
                                    }
                                }
                                if (GUILayout.Button("-"))
                                {
                                    List<GraphicsDeviceType> list = androidConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                    list.RemoveAt(i);
                                    androidConfigData.graphicsApis = list.ToArray();
                                    pcConfigData.useDefaultGraphicsAPIs = false;
                                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
                                    PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, androidConfigData.graphicsApis);
                                }

                                GUILayout.EndHorizontal();
                            }
                            EditorGUI.indentLevel = EditorGUI.indentLevel - 4;
                        }
                        //SetupBoolField("UseDefaultGraphicsAPIs [Require:false]", ref pcConfigData.useDefaultGraphicsAPIs, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, pcConfigData.useDefaultGraphicsAPIs);
                        }
                        SetupBoolField("GraphicsJobs [Require:false]", ref androidConfigData.graphicsJobs,ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.graphicsJobs = androidConfigData.graphicsJobs;
                        }
                    }
                    break;
                case ConfigSelectType.Recommended:
                    {

                    }
                    break;
                case ConfigSelectType.Optional:
                    {
                        SetupEnumField<VXRAsset.BuildReleaseMode>("Build Mode:", ref androidConfigData.releaseMode, ref hasModified);
                        if (hasModified)
                        {
                            if (VXRAsset.data!=null)
                            {
                                VXRAsset.data.ReleaseMode = androidConfigData.releaseMode;
                            }
                        }
                        switch (VXRCommon.ReleaseMode)
                        {
                            case VXRAsset.BuildReleaseMode.Debug:
                                {
                                    SetupBoolField("Development", ref androidConfigData.development, ref hasModified);
                                    if (hasModified)
                                    {
                                        EditorUserBuildSettings.development = androidConfigData.development;
                                    }
                                    SetupBoolField("ExportAsGoogleAndroidProject", ref androidConfigData.exportAsGoogleAndroidProject, ref hasModified);
                                    if (hasModified)
                                    {
                                        EditorUserBuildSettings.exportAsGoogleAndroidProject = androidConfigData.exportAsGoogleAndroidProject;
                                    }
                                }
                                break;
                            case VXRAsset.BuildReleaseMode.Release:
                                {
                                    EditorUserBuildSettings.development = false;
                                    EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
                                }
                                break;
                        }

                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField("Project Set :", EditorStyles.boldLabel);
                        VXRManagerConfigInspector.InspectorGUI(manager,VXRManagerConfigAsset.Data,serializedObject);
                    }
                    break;
            }
        }

#endregion

        #region //PC

        class PCConfigData
        {
            public XRColorSpace colorSpace;

            public ScriptingImplementation scriptingBackend;

            public GraphicsDeviceType selectDeviceType = GraphicsDeviceType.Direct3D11;

            public GraphicsDeviceType[] graphicsApis;

            public bool useDefaultGraphicsAPIs=false;

            public bool graphicsJobs;

            public VXRAsset.BuildReleaseMode releaseMode;

            public bool development;
        }

        PCConfigData pcConfigData = new PCConfigData();

        void DrawInspectorGUI_PC()
        {
            bool hasModified = false;
            int colorSpace = (int)PlayerSettings.colorSpace;
            if (colorSpace < 0)
            {
                colorSpace = 0;
            }
            pcConfigData.colorSpace = (XRColorSpace)colorSpace;
            pcConfigData.scriptingBackend = PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone);
            pcConfigData.graphicsApis = PlayerSettings.GetGraphicsAPIs(BuildTarget.StandaloneWindows64);
            pcConfigData.useDefaultGraphicsAPIs = PlayerSettings.GetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64);
            if (pcConfigData.graphicsApis == null)
            {
                pcConfigData.graphicsApis = new GraphicsDeviceType[0];
            }
            pcConfigData.graphicsJobs = PlayerSettings.graphicsJobs;
            pcConfigData.releaseMode = VXRCommon.ReleaseMode;
            pcConfigData.development = EditorUserBuildSettings.development;
            EditorGUILayout.Space(5);
            switch (curConfigSelectType)
            {
                case ConfigSelectType.Required:
                    {
                        SetupEnumField<XRColorSpace>("ColorSpace [Require:Linear]",
                            ref pcConfigData.colorSpace, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.colorSpace = (ColorSpace)((int)pcConfigData.colorSpace);
                        }
                        SetupEnumField<ScriptingImplementation>("ScriptingBackend [Require:IL2CPP]",
                            ref pcConfigData.scriptingBackend, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Standalone, pcConfigData.scriptingBackend);
                        }
                        SetupEnumField<GraphicsDeviceType>("GraphicsDeviceType [Require:Direct3D11]",
                            ref pcConfigData.selectDeviceType, ref hasModified,null,()=> {
                                if (GUILayout.Button("+"))
                                {
                                    List<GraphicsDeviceType> list = pcConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                    if (!list.Contains(pcConfigData.selectDeviceType))
                                    {
                                        list.Add(pcConfigData.selectDeviceType);
                                        pcConfigData.graphicsApis = list.ToArray();
                                        pcConfigData.useDefaultGraphicsAPIs = false;
                                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                                        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, pcConfigData.graphicsApis);
                                    }
                                }
                            });
                        if (pcConfigData.graphicsApis.Length > 0)
                        {
                            EditorGUI.indentLevel = EditorGUI.indentLevel + 4;
                            for (int i = 0; i < pcConfigData.graphicsApis.Length; ++i)
                            {
                                GraphicsDeviceType targetDeviceType = pcConfigData.graphicsApis[i];
                                GUILayout.BeginHorizontal();
                                if (i==0)
                                {
                                    EditorGUILayout.LabelField(targetDeviceType.ToString()+ " [Require:IL2CPP]");
                                }
                                else
                                {
                                    EditorGUILayout.LabelField(targetDeviceType.ToString());
                                }
                                if (i > 0)
                                {
                                    if (GUILayout.Button("↑"))
                                    {
                                        List<GraphicsDeviceType> list = pcConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                        list[i] = list[i - 1];
                                        list[i - 1] = targetDeviceType;
                                        pcConfigData.graphicsApis = list.ToArray();
                                        pcConfigData.useDefaultGraphicsAPIs = false;
                                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                                        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, pcConfigData.graphicsApis);
                                    }
                                }
                                if (i < pcConfigData.graphicsApis.Length - 1)
                                {
                                    if (GUILayout.Button("↓"))
                                    {
                                        List<GraphicsDeviceType> list = pcConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                        list[i] = list[i + 1];
                                        list[i + 1] = targetDeviceType;
                                        pcConfigData.graphicsApis = list.ToArray();
                                        pcConfigData.useDefaultGraphicsAPIs = false;
                                        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                                        PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, pcConfigData.graphicsApis);
                                    }
                                }
                                if (GUILayout.Button("-"))
                                {
                                    List<GraphicsDeviceType> list = pcConfigData.graphicsApis.ToList<GraphicsDeviceType>();
                                    list.RemoveAt(i);
                                    pcConfigData.graphicsApis = list.ToArray();
                                    pcConfigData.useDefaultGraphicsAPIs = false;
                                    PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, false);
                                    PlayerSettings.SetGraphicsAPIs(BuildTarget.StandaloneWindows64, pcConfigData.graphicsApis);
                                }
                                GUILayout.EndHorizontal();
                            }
                            EditorGUI.indentLevel = EditorGUI.indentLevel - 4;
                        }
                        //SetupBoolField("UseDefaultGraphicsAPIs [Require:false]",ref pcConfigData.useDefaultGraphicsAPIs, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.StandaloneWindows64, pcConfigData.useDefaultGraphicsAPIs);
                        }
                        SetupBoolField("GraphicsJobs [Require:false]", ref pcConfigData.graphicsJobs, ref hasModified);
                        if (hasModified)
                        {
                            PlayerSettings.graphicsJobs = pcConfigData.graphicsJobs;
                        }
                    }
                    break;
                case ConfigSelectType.Recommended:
                    {

                    }
                    break;
                case ConfigSelectType.Optional:
                    {
                        SetupEnumField<VXRAsset.BuildReleaseMode>("Build Mode:",
                            ref pcConfigData.releaseMode, ref hasModified);
                        if (hasModified)
                        {
                            if (VXRAsset.data != null)
                            {
                                VXRAsset.data.ReleaseMode = pcConfigData.releaseMode;
                            }
                        }
                        switch (VXRCommon.ReleaseMode)
                        {
                            case VXRAsset.BuildReleaseMode.Debug:
                                {
                                    SetupBoolField("Development", ref pcConfigData.development, ref hasModified);
                                    if (hasModified)
                                    {
                                        EditorUserBuildSettings.development = pcConfigData.development;
                                    }
                                }
                                break;
                            case VXRAsset.BuildReleaseMode.Release:
                                {
                                    EditorUserBuildSettings.development = false;
                                }
                                break;
                        }

                        EditorGUILayout.Space(5);
                        EditorGUILayout.LabelField("Project Set :", EditorStyles.boldLabel);
                        VXRManagerConfigInspector.InspectorGUI(manager,VXRManagerConfigAsset.Data,serializedObject);
                    }
                    break;
            }
        }

        #endregion




        List<string> errList = new List<string>();

        void InitERRStack()
        {
            errList.Clear();
            if (!VXRCommon.isUnityXROn)
            {
                AddERRStack("Unity OpenXR Is Not Installed");
            }
            if (!VXRCommon.isUnityXRManagementOn)
            {
                AddERRStack("Unity XR Management Is Not Installed");
            }
            if (!VXRCommon.isUnityURPOn)
            {
                AddERRStack("Unity URP Package Is Not Installed");
            }
            BuildTarget activeBuildTarget = VXRCommon.ActiveBuildTarget;
            BuildTarget[] buildTargets = VXRCommon.BuildTargets;
            bool findBuildTarget = false;
            for (int i = 0; i < buildTargets.Length; ++i)
            {
                if (activeBuildTarget == buildTargets[i])
                {
                    findBuildTarget = true;
                    break;
                }
            }
            if (!findBuildTarget)
            {
                AddERRStack("Platform Does Not Support");
            }
        }

        void AddERRStack(string err)
        {
            errList.Add(err);
        }
    }
}


