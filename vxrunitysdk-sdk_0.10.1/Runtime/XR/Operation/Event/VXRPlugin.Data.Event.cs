using System.Runtime.InteropServices;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {

        public enum EventType
        {
            Unknown = -1,
            Entering = 1,
            Exiting = 2,
            FocusedChange = 3,
            RefreshRateChanged = 4,

            CrownTouch = 5,
            CrownRotateTotal = 6,
            CrownRotate360 = 7,
            CrownRotateDelta = 8,
        }

        //表冠触摸类型
        public enum CrownTouchType
        {
            NoneType,
            Click = 1,
            DoubleClick = 2,
            LongPress = 3,
            PressUp = 4,
            PressDown = 5,
        }
        public struct CrownTouchData
        {
            public CrownTouchType TouchType; //表冠触摸类型
        }

        public struct CrownRotateData
        {
            public bool Direction;//旋转方向，表冠顺时针是返回true，逆时针返回 false。
            public int RotateAngle;//旋转角度，正/负代表旋转方向，顺时针为正，逆时针为负；
        }

        public struct RefreshRateChangedData
        {
            public float FromRefreshRate;
            public float ToRefreshRate;
        }

        public const int k_eventDataBufferSize = 4000;


        /// <summary>
        /// 通用事件数据结构
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct EventDataBuffer
        {
            /// <summary>
            /// 事件类型
            /// </summary>
            public EventType EventType;

            /// <summary>
            /// 事件二进制数据
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = k_eventDataBufferSize)]
            public byte[] EventData;
        }
    }
}
