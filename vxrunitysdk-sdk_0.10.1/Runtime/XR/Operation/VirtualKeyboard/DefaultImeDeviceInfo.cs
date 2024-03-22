using System.Collections.Generic;
using UnityEngine.XR;

namespace com.vivo.openxr
{
    public class DefaultImeDeviceInfo
    {

        static public bool ExistDeviceByRole(InputDeviceRole role)
        {
            var gameControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithRole(role, gameControllers);
            if (gameControllers.Count > 0)
            {
                return true;
            }
            return false;
        }

        static public InputDevice GetDeviceByRole(InputDeviceRole role)
        {
            InputDevice inputDevice = new InputDevice();
            var gameControllers = new List<InputDevice>();
            InputDevices.GetDevicesWithRole(role, gameControllers);
            if (gameControllers.Count > 0)
            {
                inputDevice = gameControllers[0];
            }
            return inputDevice;
        }
    }

}
