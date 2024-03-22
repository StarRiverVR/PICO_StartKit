using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{

    public class VXRSpatialAudioContext : MonoBehaviour
    {
        [SerializeField]
        protected VXRSpatialAudio.ImplType _implType = VXRSpatialAudio.ImplType.TypeDefault;

        protected AudioConfiguration _audioCon;
        protected virtual void Awake()
        {
            _audioCon = AudioSettings.GetConfiguration();
            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChangedEvent;
            VXRSpatialAudio.CreateSpatialAudio(_implType, (int)_audioCon.speakerMode, _audioCon.dspBufferSize, _audioCon.sampleRate);
        }
        protected virtual void OnAudioConfigurationChangedEvent(bool changed)
        {
            _audioCon = AudioSettings.GetConfiguration();
        }
        protected virtual void OnDestroy()
        {
            VXRSpatialAudio.DestroySpatialAudio();
        }
    }
}