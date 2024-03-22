#if USING_XR_UNITYXR && USING_XR_MANAGEMENT
#define USING_XR_SDK
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_XR_SDK
using UnityEngine.XR.OpenXR;
using UnityEngine.XR;
using UnityEngine.Experimental.XR;
using UnityEngine.XR.Management;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.Management;
#endif

using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public class VXRLoader : XRLoaderHelper
    {
        public static string AssetPath = VXRPath.VivoOpenXRUnityDir+ "OpenXRLoader/" + nameof(VXRLoader)+".asset";

        private static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRDisplaySubsystem>();
            }
        }

        private static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();

        public XRInputSubsystem inputSubsystem
        {
            get
            {
                return GetLoadedSubsystem<XRInputSubsystem>();
            }
        }

        public override bool Initialize()
        {
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, VXRNameConfig.DisplaySubsystemDescriptorsId);
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, VXRNameConfig.InputSubsystemDescriptorsId);

            if (displaySubsystem == null && inputSubsystem == null)
            {
#if (UNITY_ANDROID && !UNITY_EDITOR)
                VLog.Error("Unable to start Oculus XR Plugin.");
#else

                VLog.Error("Unable to start Vivo XR Plugin.\n" +
                                 "Possible causes include a headset not being attached, or the Vivo runtime is not installed or up to date.\n" +
                                 "If you've recently installed or updated the Vivo runtime, you may need to reboot or close Unity and the Unity Hub and try again.");
#endif
            }
            else if (displaySubsystem == null)
            {
                VLog.Error("Unable to start Vivo XR Plugin. Failed to load display subsystem.");
            }
            else if (inputSubsystem == null)
            {
                VLog.Error("Unable to start Vivo XR Plugin. Failed to load input subsystem.");
            }
            else
            {
                //RegisterUpdateCallback.Initialize();
            }

            VLog.Info("VivoOpenXRLoader:Vivo XR Plugin start !");
            return displaySubsystem != null && inputSubsystem != null;
        }

        public override bool Start()
        {
            return true;

        }
    }
}


