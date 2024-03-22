using System;

namespace com.vivo.openxr
{
    public class VXREvent
    {

        /// <summary>
        /// 表冠逆时针旋转最小角度
        /// </summary>
        public const int CROWN_ROTATE_MIN_ANGLE = -360;

        /// <summary>
        /// 表冠顺时针旋转最大角度
        /// </summary>
        public const int CROWN_ROTATE_MAX_ANGLE = 360;

#if VXR_USE_CROWN_TOUCH
        /// <summary>
        /// 表冠触摸类型
        /// </summary>
        public enum CrownTouchType
        {
            /// <summary>
            /// 空事件
            /// </summary>
            NoneType,

            /// <summary>
            /// 点击事件
            /// </summary>
            Click = 1,

            /// <summary>
            ///双击事件
            /// </summary>
            DoubleClick = 2,

            /// <summary>
            /// 长按事件
            /// </summary>
            LongPress = 3,

            /// <summary>
            /// 抬起按键事件
            /// </summary>
            PressUp = 4,

            /// <summary>
            /// 按下按键事件
            /// </summary>
            PressDown = 5,
        }

#endif
        /// <summary>
        /// 表冠单次旋转信息
        /// </summary>
        public struct CrownRotateData
        {
            /// <summary>
            /// 单次旋转变化量，重新旋转时会清零；正/负代表旋转方向，顺时针为正，逆时针为负；正常低速情况下变化量的绝对值恒为 1度。
            /// </summary>
            public int RotateDelta;

            /// <summary>
            /// 旋转方向，表冠顺时针是返回true，逆时针返回 false 。
            /// </summary>
            public bool Direction;
        }
        
        /// <summary>
        /// 表冠归一化旋转信息
        /// </summary>
        
        public struct CrownRotate360Data
        {
            /// <summary>
            /// 旋转角度，一圈360度
            /// </summary>
            public int RotateAngle;

            /// <summary>
            /// 表冠顺时针旋转最大角度
            /// </summary>
            public int MaxAngle { get => CROWN_ROTATE_MAX_ANGLE; }

            /// <summary>
            /// 表冠逆时针旋转最小角度
            /// </summary>
            public int MinAngle { get => CROWN_ROTATE_MIN_ANGLE; }
        }

#if VXR_USE_CROWN_ROTATE_TOTAL
        /// <summary>
        /// 表冠累计旋转信息
        /// </summary>
        public struct CrownRotateTotalData
        {
            /// <summary>
            /// 累计旋转角度
            /// </summary>
            public int RotateAngle;
        }
#endif

        private static Action _openXREnteringCallBack;
        private static Action _openXRExitingCallBack;
        private static Action<bool> _openXRFocusChangedCallback;
#if VXR_USE_CROWN_TOUCH
        private static Action<CrownTouchType> _crownTouchCalBack;
#endif
#if VXR_USE_CROWN_ROTATE_TOTAL
        private static Action<CrownRotateTotalData> _crownRotateTotalCallBack;
#endif
        private static Action<CrownRotate360Data> _crownRotate360CallBack;
        private static Action<CrownRotateData> _crownRotateCallBack;

        #region // OpenXR事件

        /// <summary>
        /// 添加OpenXR进入事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddOpenXREnteringListener(Action listener)
        {
            _openXREnteringCallBack += listener;
        }

        /// <summary>
        /// 移除注册OpenXR进入事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveOpenXREnteringListener(Action listener)
        {
            _openXREnteringCallBack -= listener;
        }

        /// <summary>
        /// 添加OpenXR退出事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddOpenXRExitingListener(Action listener)
        {
            _openXRExitingCallBack += listener;
        }

        /// <summary>
        /// 移除注册OpenXR退出事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveOpenXRExitingListener(Action listener)
        {
            _openXRExitingCallBack -= listener;
        }

        /// <summary>
        /// 添加OpenXR焦点变化事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddOpenXRFocusChangedListener(Action<bool> listener)
        {
            _openXRFocusChangedCallback += listener;
        }

        /// <summary>
        /// 移除注册OpenXR焦点变化事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveOpenXRFocusChangedListener(Action<bool> listener)
        {
            _openXRFocusChangedCallback -= listener;
        }
        #endregion

        #region// 表冠事件

#if VXR_USE_CROWN_TOUCH
        /// <summary>
        /// 添加表冠触摸事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddCrownTouchListener(Action<CrownTouchType> listener)
        {
            _crownTouchCalBack += listener;
        }

        /// <summary>
        /// 移除注册表冠触摸事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveCrownTouchListener(Action<CrownTouchType> listener)
        {
            _crownTouchCalBack -= listener;
        }
#endif

        /// <summary>
        /// 添加表冠单次旋转事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddCrownRotateListener(Action<CrownRotateData> listener)
        {
            _crownRotateCallBack += listener;
        }

        /// <summary>
        /// 移除注册表冠单次旋转事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveCrownRotateListener(Action<CrownRotateData> listener)
        {
            _crownRotateCallBack -= listener;
        }

        /// <summary>
        /// 添加表冠旋转事件
        /// 旋转角范围为 -360度 ~ +360度
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddCrownRotate360Listener(Action<CrownRotate360Data> listener)
        {
            _crownRotate360CallBack += listener;
        }

        /// <summary>
        /// 移除注册表冠旋转事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveCrownRotate360Listener(Action<CrownRotate360Data> listener)
        {
            _crownRotate360CallBack -= listener;
        }

#if VXR_USE_CROWN_ROTATE_TOTAL
        /// <summary>
        /// 添加表冠累计旋转事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void AddCrownRotateTotalListener(Action<CrownRotateTotalData> listener)
        {
            _crownRotateTotalCallBack += listener;
        }

        /// <summary>
        /// 移除注册表冠旋转事件
        /// </summary>
        /// <param name="listener">事件函数</param>
        public static void RemoveCrownRotateTotalListener(Action<CrownRotateTotalData> listener)
        {
            _crownRotateTotalCallBack -= listener;
        }
#endif


#endregion

        /// <summary>
        /// 事件响应
        /// </summary>
        /// <param name="bufferData">事件数据</param>
        /// <returns>是否阻断事件</returns>
        internal static bool EventCallBack(VXRPlugin.EventDataBuffer bufferData)
        {
            switch (bufferData.EventType)
            {
                case VXRPlugin.EventType.Entering:
                    _openXREnteringCallBack?.Invoke();
                    return true;
                case VXRPlugin.EventType.Exiting:
                    _openXRExitingCallBack?.Invoke();
                    return true;
                case VXRPlugin.EventType.FocusedChange:
                    var focus = VXRDeserialize.ByteArrayToStructure<bool>(bufferData.EventData);
                    _openXRFocusChangedCallback?.Invoke(focus);
                    return true;
                case VXRPlugin.EventType.CrownTouch:
#if VXR_USE_CROWN_TOUCH
                    VXRPlugin.CrownTouchData crownTouchData = VXRDeserialize.ByteArrayToStructure<VXRPlugin.CrownTouchData>(bufferData.EventData);
                    _crownTouchCalBack?.Invoke((CrownTouchType)crownTouchData.TouchType);
#endif
                    return true;
                case VXRPlugin.EventType.CrownRotateTotal:
#if VXR_USE_CROWN_ROTATE_TOTAL
                    var crownRotateTotalData = VXRDeserialize.ByteArrayToStructure<VXRPlugin.CrownRotateData>(bufferData.EventData);
                    _crownRotateTotalCallBack?.Invoke(new CrownRotateTotalData() { RotateAngle = crownRotateTotalData.RotateAngle });
#endif
                    return true;
                case VXRPlugin.EventType.CrownRotate360:
                    var crownRotateData360 = VXRDeserialize.ByteArrayToStructure<VXRPlugin.CrownRotateData>(bufferData.EventData);
                    _crownRotate360CallBack?.Invoke(new CrownRotate360Data() { RotateAngle = crownRotateData360.RotateAngle });
                    return true;
                case VXRPlugin.EventType.CrownRotateDelta:
                    var crownRotateData = VXRDeserialize.ByteArrayToStructure<VXRPlugin.CrownRotateData>(bufferData.EventData);
                    _crownRotateCallBack?.Invoke(new CrownRotateData() { RotateDelta = crownRotateData.RotateAngle,Direction = crownRotateData.Direction });
                    return true;
                default:
                    break;
            }
            return false;
        }
    }
}
