using com.vivo.codelibrary;
using UnityEngine;

namespace com.vivo.openxr
{
    [RequireComponent(typeof(AudioListener))]
    public class VXRSpatialAudioListener : MonoBehaviour
    {
        private void Start()
        {
            if (VXRSpatialAudio.Initialized)
            {
                VXRSpatialAudio.SetListenerPose(transform.position, transform.rotation);
            }
        }

        private void Update()
        {
            if (transform.hasChanged && VXRSpatialAudio.Initialized)
            {
                VXRSpatialAudio.SetListenerPose(transform.position, transform.rotation);
                transform.hasChanged = false;
            }
        }

        private void OnAudioFilterRead(float[] data, int channels)
        {
            if (VXRSpatialAudio.Initialized)
            {
                VXRSpatialAudio.GetListenerBuffer(data, channels, data.Length / channels);
            }
        }
    }
}