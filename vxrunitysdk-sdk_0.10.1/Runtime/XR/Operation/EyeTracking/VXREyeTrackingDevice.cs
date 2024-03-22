using UnityEngine;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.XR.OpenXR.Features.Interactions;
using com.vivo.openxr;
using UnityEngine.XR;

public class VXREyeTrackingDevice : MonoBehaviour
{
    UnityEngine.InputSystem.XR.PoseState _poseState;
    UnityEngine.InputSystem.InputDevice _eyeGazeDevice;
    UnityEngine.InputSystem.InputControl _poseControl;

    protected void Awake()
    {
        VXREyeTracking.StartEyeTracking();

        _poseState = new UnityEngine.InputSystem.XR.PoseState
        {
            isTracked = default,
            trackingState = default,
            position = default,
            rotation = Quaternion.identity,
            velocity = default,
            angularVelocity = default,
        };

        // Register eye gaze layout
        UnityEngine.InputSystem.InputSystem.RegisterLayout(
            typeof(EyeGazeInteraction.EyeGazeDevice),
            "EyeGaze",
            matches: new InputDeviceMatcher()
                .WithInterface(UnityEngine.InputSystem.XR.XRUtilities.InterfaceMatchAnyVersion)
                .WithProduct("Vivo Eye Gaze Adapter")
        );
    }
    protected void OnEnable()
    {
        AddEyeTrackingDevice();
    }
    protected void Update()
    {
        ProcessPoseInput();

        if (_eyeGazeDevice != null && _poseControl != null)
            UnityEngine.InputSystem.InputSystem.QueueDeltaStateEvent(_poseControl, _poseState);
    }
    protected void OnDisable()
    {
        RemoveEyeTrackingDevice();
    }
    protected void OnDestroy()
    {
        VXREyeTracking.StopEyeTracking();
    }

    void AddEyeTrackingDevice()
    {
        if (_eyeGazeDevice != null && _eyeGazeDevice.added)
            return;

        _eyeGazeDevice = UnityEngine.InputSystem.InputSystem.AddDevice<EyeGazeInteraction.EyeGazeDevice>();
        if (_eyeGazeDevice == null)
        {
            Debug.LogError("Failed to create Eye Gaze device.", this);
            _poseControl = null;
            return;
        }

        _poseControl = _eyeGazeDevice["pose"];
    }
    void ProcessPoseInput()
    {
        if(VXREyeTracking.GetEyeGazeData(out var data))
        {
            _poseState.trackingState = InputTrackingState.Position | InputTrackingState.Rotation;
        }
        else
        {
            _poseState.trackingState = InputTrackingState.None;
        }

        _poseState.isTracked = data.IsTracked;
        _poseState.position = data.Position;
        _poseState.rotation = data.Rotation;
    }
    void RemoveEyeTrackingDevice()
    {
        if (_eyeGazeDevice != null && _eyeGazeDevice.added)
        {
            UnityEngine.InputSystem.InputSystem.RemoveDevice(_eyeGazeDevice);
            _poseControl = null;
        }
    }
}
