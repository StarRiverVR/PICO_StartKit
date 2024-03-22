
using System;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public struct GazeCamera
        {
            public uint FrameNumber;
            public uint ExposureNs;
            public uint StartOfExposureNs; /* In ns using Android BOOTTIME clock. */
        }

        public enum GazeFlag
        {
            ORIGIN_COMBINED_VALID = 0x00000001,
            DIRECTION_COMBINED_VALID = 0x00000002,
            CONVERGENCE_DISTANCE_VALID = 0x00000004,
            FOVEATED_GAZE_VALID = 0x00000008,
            INTEROCULAR_DISTANCE_VALID = 0x00000010,
        }
        public struct GazeEye
        {
            /* Validity bitmask for the per-eye eye gaze attributes. */
            public GazeFlag Flags;
            /* Contains the origin (x, y, z) of the eye gaze vector in meters from the HMD center-eye coordinate system's origin. */
            public Vector3f GazeOrigin;
            /* Contains the unit vector of the eye gaze direction in the HMD center-eye coordinate system. */
            public Vector3f GazeDirection;
            /* Value between 0.0 and 1.0 where 1.0 means fully open and 0.0 closed. */
            public float EyeOpenness;
            /* Value in millimeter indicating the pupil dilation. */
            public float PupilDilation;
            /* 
             * Normalized (0.0-1.0) position of pupil in relation to optical axis where 0.5, 0.5 is on the optical axis.
             * This information is useful to convey if the user is wearing the headset correctly and if the user's interpupillary distance matches the lens separation. 
             */
            public Vector3f PositionGuide;
            /* Indicates whether the eye of the user is closed (1) or not closed (0). */
            public uint Blink;
        }

        public enum FoveatedGazeTrackState
        {
            TRACKING,       /* The user is being tracked and the value has been updated.*/
            EXTRAPOLATED,   /* The user is not being tracked and the value has been extrapolated from historical data.*/
            LAST_KNOWN,     /* The user is not being tracked and the value is a repeat of the last last tracked value.*/
            MAX = 0x7fffffff
        }
        
        public struct EyeTrackingData
        {
            /* Inns using Android BOOTTIME clock. */
            public long Timestamp;
            /* Type representing a bitmask used by the plugin to convey the availability of the per-eye gaze data. */
            public GazeFlag Flags;
            public GazeEye LeftEye;
            public GazeEye RightEye;
            /* Contains the origin (x, y, z) of the combined gaze vector in meters from the HMD center-eye coordinate system origin. */
            public Vector3f GazeOriginCombined;
            /* Contains the origin (x, y, z) of the combined gaze vector in meters from the HMD center-eye coordinate system's origin. */
            public Vector3f GazeDirectionCombined;
            /* Distance in meters from gazeOriginCombined where the vectors converge. */
            float GazeConvergenceDistance;
            /* Data source timing information (ex: camera exposure and timestamp). */
            public GazeCamera Camera;
            /* 
             * Contains the unit vector of the gaze direction in the HMD center-eye coordinate system.
             * Origin of this vector is the same as the origin for the HMD center-eye coordinate system.
             * This signal is optimized for foveated rendering or other use cases gazeDirectionCombined is preferred.
             */
            public Vector3f FoveatedGazeDirection;
            /* Contains the current state of the foveatedGazeDirection signal. */
            public FoveatedGazeTrackState FoveatedGazeTrackingState;  
            /* Distance in millimeters between the eyeballs. */
            public float GazeInterocularDistance;
        }
    }
}