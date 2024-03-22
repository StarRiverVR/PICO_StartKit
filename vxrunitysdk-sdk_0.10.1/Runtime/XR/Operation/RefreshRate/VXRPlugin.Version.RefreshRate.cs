using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        private sealed partial class VXRVersion_0_0_2
        {
            #region // RefreshRate
            [DllImport(pluginName, EntryPoint = "vxr_SetDisplayRefreshRate")]
            public static extern Result vxr_SetDisplayRefreshRate(float rate);
            [DllImport(pluginName, EntryPoint = "vxr_GetDisplayRefreshRate")]
            public static extern Result vxr_GetDisplayRefreshRate(out float rate);
            [DllImport(pluginName, EntryPoint = "vxr_EnumerateDisplayRefreshRates")]
            public static extern Result vxr_EnumerateDisplayRefreshRates(IntPtr ratePtr, out int rateCountOutput);
            #endregion RefreshRate
        }

    }
}
