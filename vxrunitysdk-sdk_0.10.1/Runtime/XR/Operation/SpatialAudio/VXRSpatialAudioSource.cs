using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{

    [RequireComponent(typeof(AudioSource))]
    public class VXRSpatialAudioSource : MonoBehaviour
    {
        [Tooltip("渲染模式")]
        [SerializeField]
        private VXRSpatialAudio.AudioRenderMode _renderMode = VXRSpatialAudio.AudioRenderMode.HRTFDisable;
        [Tooltip("最小衰减距离")]
        [SerializeField]
        private float _minAttenuationDistance = 1.0f;
        [Tooltip("最大衰减距离")]
        [SerializeField]
        private float _maxAttenuationDistance = 100.0f;
        [Tooltip("衰减距离")]
        [SerializeField]
        [Range(0.0f, 1.0f)] 
        private float _distanceAttenuation = 0.0f;

        public bool IsActive { get; private set; }
        public int SourceId { get; private set; } = -1;

        private AudioSource _audioSource;

        void Start()
        {
            InitAudio();
        }

        void InitAudio()
        {
            SourceId = VXRSpatialAudio.CreateSource(_renderMode);
            if (SourceId < 0)
            {
                IsActive = false;
                VLog.Info($"VXRSpatialAudioSource Create Sound Fail. sourceId[{SourceId}]");
                return;
            }
            IsActive = true;
            _audioSource = GetComponent<AudioSource>();

            VXRSpatialAudio.AudioAttenuation info = new VXRSpatialAudio.AudioAttenuation();
            info.MinDistance = _minAttenuationDistance;
            info.MaxDistance = _maxAttenuationDistance;
            info.Mode = (int)_audioSource.rolloffMode;
            info.CustomParam = _distanceAttenuation;
            VXRSpatialAudio.SetSourceAttenuation(SourceId, info);

            VXRSpatialAudio.SetSourceVolume(SourceId, _audioSource.volume);
        }

        void Update()
        {
            if (IsActive && SourceId >= 0)
            {
                if (transform.hasChanged)
                {
                    VXRSpatialAudio.SetSourcePose(SourceId,transform.position, transform.rotation);
                    transform.hasChanged = false;
                }
            
            }
        }
    
        void OnDestroy()
        {
            IsActive = false;
            VXRSpatialAudio.DestroySource(SourceId);
            SourceId = -1;
        }

        void OnDrawGizmos()
        {
            Color c;
            const float colorSolidAlpha = 0.1f;
        
            //  min
            c.r = 1.0f;
            c.g = 0.35f;
            c.b = 0.0f;
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, _minAttenuationDistance);
            c.a = colorSolidAlpha;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, _minAttenuationDistance);

            //  max
            c.r = 0.0f;
            c.g = 1.0f;
            c.b = 1.0f;
            c.a = 1.0f;
            Gizmos.color = c;
            Gizmos.DrawWireSphere(transform.position, _maxAttenuationDistance);
            c.a = colorSolidAlpha;
            Gizmos.color = c;
            Gizmos.DrawSphere(transform.position, _maxAttenuationDistance);
        
        }

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (!IsActive || SourceId < 0)
            {
                for (int i = 0; i < data.Length; ++i)
                    data[i] = 0.0f;
                return;
            }
        
            int numFrames = data.Length / channels;
            VXRSpatialAudio.SetSourceBuffer(SourceId, data, channels, numFrames);

            for (int i = 0; i < data.Length; ++i)
                data[i] = 0.0f;
        }
    }
}