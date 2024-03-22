namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        public enum ImeType
        {
            Default = 0,
        }

        public enum VirtualKeyBoardLayout
        {
            Text = 1,
            Number = 2,
        }

        public enum ImeError
        {
            UNKNOWN = 0,
            SERVICE_NOT_CONNECTED = 1
        }

        //for commit code. -1 to commit string, other to commit key
        public enum ImeKey
        {
            KEYCODE_COMMIT = -1,
            KEYCODE_UNKNOWN = 0,
            KEYCODE_ENTER = 66,
            KEYCODE_DEL = 67,
            KEYCODE_VOICE_START = 1000,
            KEYCODE_VOICE_END = 1001,
        }

        public enum ImeInputType
        {
            TYPE_CLASS_TEXT = 1,
            TYPE_CLASS_NUMBER = 2,
            TYPE_CLASS_PHONE = 3,
            TYPE_CLASS_DATETIME = 4,
            TYPE_CLASS_MUTE_TEXT = 5,
        }

        public enum ImeTextType
        {
            TYPE_TEXT_VARIATION_NORMAL = 0,
            TYPE_TEXT_VARIATION_URI = 0x10,
            TYPE_TEXT_VARIATION_EMAIL_ADDRESS = 0x20,
            TYPE_TEXT_VARIATION_EMAIL_SUBJECT = 0x30,
            TYPE_TEXT_VARIATION_SHORT_MESSAGE = 0x40,
            TYPE_TEXT_VARIATION_LONG_MESSAGE = 0x50,
            TYPE_TEXT_VARIATION_PERSON_NAME = 0x60,
            TYPE_TEXT_VARIATION_POSTAL_ADDRESS = 0x70,
            TYPE_TEXT_VARIATION_PASSWORD = 0x80,
            TYPE_TEXT_VARIATION_VISIBLE_PASSWORD = 0x90,
            TYPE_TEXT_VARIATION_WEB_EDIT_TEXT = 0xa0,
            TYPE_TEXT_VARIATION_FILTER = 0xb0,
            TYPE_TEXT_VARIATION_PHONETIC = 0xc0,
            TYPE_TEXT_VARIATION_WEB_EMAIL_ADDRESS = 0xd0,
            TYPE_TEXT_VARIATION_WEB_PASSWORD = 0xe0
        }

        public enum ImeMotionEventType
        {
            ACTION_DOWN,
            ACTION_UP,
            ACTION_MOVE,
            ACTION_CANCEL,
            ACTION_OUTSIDE,
            ACTION_POINTER_DOWN,
            ACTION_POINTER_UP,
            ACTION_HOVER_MOVE,
            ACTION_SCROLL,
            ACTION_HOVER_ENTER,
            ACTION_HOVER_EXIT,
            ACTION_BUTTON_PRESS,
            ACTION_BUTTON_RELEASE,
            ACTION_LONGPRESS = 100,
            ACTION_HANDLER_JOYSTICK_BASE = 200,
            ACTION_HANDLER_JOYSTICK_BUTT = 299,
        }

        public enum ImeKeyboardLayoutType
        {
            TYPE_KEYBOARD_LAYOUT_NORMAL,
            TYPE_KEYBOARD_LAYOUT_NUMBER,
            TYPE_KEYBOARD_LAYOUT_VOICE
        }

        public enum ImeBlockType
        {
            TYPE_BLOCK_LEFT,
            TYPE_BLOCK_MIDDLE,
            TYPE_BLOCK_RIGHT,
            TYPE_BLOCK_NUMBER,
            TYPE_BLOCK_VOICE_DEFAULT,
            TYPE_BLOCK_VOICE_EDITING,
            TYPE_BLOCK_VOICE_ERROR,
            TYPE_BLOCK_VOICE_SYMBOL,
            TYPE_BLOCK_BUTT
        }

        public enum ImeDeviceCommandType
        {
            TYPE_DEVICE_JOYSTICK_COMMAND_RELEASE = 0,
            TYPE_DEVICE_JOYSTICK_COMMAND_PRESS,
            TYPE_DEVICE_JOYSTICK_COMMAND_LEFT,
            TYPE_DEVICE_JOYSTICK_COMMAND_RIGHT,
            TYPE_DEVICE_JOYSTICK_COMMAND_UP,
            TYPE_DEVICE_JOYSTICK_COMMAND_DOWN,
            TYPE_DEVICE_JOYSTICK_COMMAND_BUTT
        }

        public enum HandlerPhase
        {
            HANDLER_PHASE_DEFAULT,
            HANDLER_PHASE_EDITING,
            HANDLER_PHASE_BUTT
        }

        public enum ImeSceneType
        {
            SCENE_TYPE_NEAR,
            SCENE_TYPE_FAR,
            SCENE_TYPE_CUSTOM,
        }

        public enum ImeHandlerType
        {
            HANDLER_TYPE_LEFT_HAND = 0,
            HANDLER_TYPE_RIGHT_HAND = 1
        }
    }
    
}
