using com.vivo.codelibrary;
using System.Collections;
using UnityEngine;
using static com.vivo.openxr.VXRPlugin;

namespace com.vivo.openxr
{
    public class VXROverlayManager : MonoSingleton<VXROverlayManager>
    {

        private bool _isStartIssueUpdate = false;

        internal void SendCrateAndroidSurfaceEvent()
        {
            GL.IssuePluginEvent(VXRPlugin.AndroidSurfaceEvent(), (int)OverlayAndroidSurfaceEvent.Create);
        }

        internal void SendShutdownAndroidSurfaceEvent()
        {
            GL.IssuePluginEvent(VXRPlugin.AndroidSurfaceEvent(), (int)OverlayAndroidSurfaceEvent.Shutdown);
        }

        internal void StartIssueUpdate()
        {
            if (_isStartIssueUpdate)
            {
                return;
            }
            _isStartIssueUpdate = true;
            StartCoroutine(UpdateAndroidSurface());
        }

        IEnumerator UpdateAndroidSurface()
        {
            while (true)
            {                
                yield return new WaitForEndOfFrame();
                GL.IssuePluginEvent(VXRPlugin.AndroidSurfaceEvent(), (int)OverlayAndroidSurfaceEvent.Update);
            }            
        }
    }

}
