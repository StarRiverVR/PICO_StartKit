using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{
    public class VXRSpatialAudioRoom : MonoBehaviour
    {
        [Header("房间尺寸")]
        [Tooltip("房间.长")]
        [Range(1.0f, 200.0f)]
        [SerializeField]
        float _length = 1f;
        [Tooltip("房间.宽")]
        [Range(1.0f, 200.0f)]
        [SerializeField]
        float _width = 1f;
        [Tooltip("房间.高")]
        [Range(1.0f, 200.0f)]
        [SerializeField]
        float _height = 1f;

        [Header("房间材质")]
        [Tooltip("房间.顶材质")]
        [SerializeField]
        VXRSpatialAudio.ReflectionMaterial _up = VXRSpatialAudio.ReflectionMaterial.Brick;
        [Tooltip("房间.底材质")]
        [SerializeField]
        VXRSpatialAudio.ReflectionMaterial _down = VXRSpatialAudio.ReflectionMaterial.Brick;
        [Tooltip("房间.前材质")]
        [SerializeField]
        VXRSpatialAudio.ReflectionMaterial _front = VXRSpatialAudio.ReflectionMaterial.Brick;
        [Tooltip("房间.后材质")]
        [SerializeField]
        VXRSpatialAudio.ReflectionMaterial _back = VXRSpatialAudio.ReflectionMaterial.Brick;
        [Tooltip("房间.左材质")]
        [SerializeField]
        VXRSpatialAudio.ReflectionMaterial _left = VXRSpatialAudio.ReflectionMaterial.Brick;
        [Tooltip("房间.右材质")]
        [SerializeField]
        VXRSpatialAudio.ReflectionMaterial _right = VXRSpatialAudio.ReflectionMaterial.Brick;

        [Header("房间反射")]
        [Tooltip("反射系数比例因子")]
        [SerializeField]
        float _roomReflectionScalar = 1f;

        [Header("房间混响")]
        [Tooltip("混响增益")]
        [SerializeField]
        float _roomReverbGain = 1f;
        [Tooltip("调整所有频带上的混响时间，设置为1.0f时没有效果")]
        [SerializeField]
        float _roomReverbTime = 1.3f;
        [Tooltip("混响明亮度")]
        [SerializeField]
        float _roomReverbBrightness = 1f;

        Transform _attachTransform;
        public Transform AttachTransform
        {
            get
            {
                if (_attachTransform == null) 
                {
                    _attachTransform = Camera.main.transform;
                }
                if(_attachTransform == null)
                {
                    return transform;
                }
                return _attachTransform;
            }
        }
        
        void OnEnable()
        {
            VLog.Info("VXRSpatialAudioRoom.OnEnable");
            if(VXRSpatialAudio.Initialized)
            {
                VXRSpatialAudio.SetRoomEnable(true);
            }
        }
        
        void Start()
        {
            VXRSpatialAudio.InitRoom(_length, _width, _height, new[]{_left, _right, _down, _up,  _front, _back, });
            // 启用
            VXRSpatialAudio.SetRoomEnable(true);
            // 反射
            VXRSpatialAudio.SetRoomReflection(_roomReflectionScalar);
            // 混响
            VXRSpatialAudio.SetRoomReverb(_roomReverbGain, _roomReverbTime, _roomReverbBrightness);
        }

        void OnDisable()
        {
            // 禁用
            VXRSpatialAudio.SetRoomEnable(false);
        }

        void Update()
        {
            VXRSpatialAudio.SetRoomPose(AttachTransform.position, AttachTransform.rotation);
        }

        private static VXRSpatialAudioRoom RoomReflectionGizmoAS = null;
        
        void OnDrawGizmos()
        {
            // Are we the first one created? make sure to set our static ONSPAudioSource
            // for drawing out room parameters once
            if (RoomReflectionGizmoAS == null)
            {
                RoomReflectionGizmoAS = this;
            }

            Color c;
            const float colorSolidAlpha = 0.1f;
            // Draw room parameters ONCE only, provided reflection engine is on
            if (RoomReflectionGizmoAS == this)
            {
                // Set color of cube (cyan is early reflections only, white is with reverb on)
                c = Color.cyan;

                Gizmos.color = c;
                Gizmos.DrawWireCube(AttachTransform.position, new Vector3(_width, _height, _length));
                c.a = colorSolidAlpha;
                Gizmos.color = c;
                Gizmos.DrawCube(AttachTransform.position, new Vector3(_width, _height, _length));
            }
        }
    }
}
