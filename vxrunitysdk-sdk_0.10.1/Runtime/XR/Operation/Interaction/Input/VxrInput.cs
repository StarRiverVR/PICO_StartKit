using System;

namespace com.vivo.openxr
{
    /// <summary>
    /// 设备型号
    /// </summary>
    public enum DeviceModel
    {
        V1 = 0,        
    }

    /// <summary>
    /// 设备部件
    /// </summary>
    public enum DeviceUnit
    {
        //左手柄
        Left = 0,
        //右手柄
        Right = 1,
        //头显
        Head = 2,
    }

    /// <summary>
    /// 监听类型
    /// </summary>
    public enum HandleListenerType
    {
        // 手柄状态监听
        TYPE_STICK_STATE_LISTEN = 0,
        // 手柄电量监听
        TYPE_STICK_POWER_LISTEN = 1,
        // 手柄升级监听
        TYPE_STICK_UPDATE_LISTEN = 2,
        // 手柄按键监听
        TYPE_STICK_KEY_LISTEN = 3,
        // 手写笔监听
        TYPE_STYLUS_LISTEN = 4,
    }

    /// <summary>
    /// 返回操作结果
    /// </summary>
    public enum ResultOperateType
    {
        //操作无效
        RESULT_OPERATE_INVAILD = -2,
        //操作失败
        RESULT_OPERATE_FAIL = -1,
        //操作成功
        RESULT_OPERATE_SUCCESS = 0,
    }

    /// <summary>
    /// 扳机力反馈设置参数结构
    /// </summary>
    public struct TriggerForceFeedbackOptions
    {
        public DeviceUnit unit;                    //设备单元
        public int mode;                           //振动模式
        public int dampingForce;                   //阻尼值
        public int dampingStartPos;                //阻尼起始位置
        public int vibrationForce;                 //振动值
        public int vibrationFrequency;             //振动频率
        public int vibrationStartPos;              //振动开始位置
        public int vibrationStartForce;            //振动启动力度
        public int breakthroughStartPos;           //突破起始位置
        public int breakthroughContinuousJourney;  //突破连续行程
        public int breakthroughResistance;         //突破阻力
    }
    
    /// <summary>
    /// 手柄触摸震动反馈设置参数结构
    /// </summary>
    public struct TouchpadVibrationFeedbackOptions
    {
        public DeviceUnit unit;//设备单元
        public int mode;       //模式
        public int frequency;  //频率
        public int amplitude;  //振幅
        public int duration;   //期间
        public int startTime;  //开始时间
        public int brakeTime;  //制动时间
    }

    /// <summary>
    /// 手柄主马达振动反馈设置参数结构
    /// </summary>
    public struct MainMotorVibrationFeedbackOptions
    {
        public DeviceUnit unit;//设备单元
        public int mode;       //模式
        public int frequency;  //频率
        public int amplitude;  //振幅
        public int duration;   //期间
        public int startTime;  //开始时间
        public int brakeTime;  //制动时间
    }

    public static class VXRInput
    {
        /// <summary>
        /// 手柄SDK是否连接
        /// </summary>
        /// <returns>true: 已连接  fasel: 未连接</returns>
        public static bool IsConnected()
        {
            return VXRControllerPlugin.IsConnected();
        }

        /// <summary>
        /// 获取头显版本号
        /// </summary>
        /// <returns>返回头显的版本号信息</returns>
        public static string GetHeadVersion()
        {

            return VXRControllerPlugin.GetHeadVersion();
        }

        /// <summary>
        /// 获取手柄版本号
        /// </summary>
        /// <param name="unit">设备单元,Left：左手柄，Right:右手柄 </param>
        /// <returns>返回对应手柄的版本号信息</returns>
        public static string GetHandVersion(DeviceUnit unit)
        {
            return VXRControllerPlugin.GetHandVersion(unit);
        }

        /// <summary>
        /// 获取手柄SN
        /// </summary>
        /// <param name="unit">设备单元,Left：左手柄，Right:右手柄</param>
        /// <returns>返回对应手柄的SN</returns>
        public static string GetHandleSN(DeviceUnit unit)
        {
            return VXRControllerPlugin.GetHandleSN(unit);
        }

        /// <summary>
        /// 绑定手柄
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType PairHandleMode(DeviceUnit unit)
        {            
            return (ResultOperateType)VXRControllerPlugin.PairHandleMode(unit);
        }

        /// <summary>
        /// 解除手柄绑定
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType UnpairHandleMode(DeviceUnit unit)
        {            
            return (ResultOperateType)VXRControllerPlugin.UnpairHandleMode(unit);            
        }

        /// <summary>
        /// 升级头显
        /// </summary>
        /// <param name="file">头显升级指令</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType OtaHead(string file)
        {
            return (ResultOperateType)VXRControllerPlugin.OtaHead(file);
        }

        /// <summary>
        /// 升级手柄
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <param name="file">手柄升级指令</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType OtaHandle(DeviceUnit unit, string file)
        {
            return (ResultOperateType)VXRControllerPlugin.OtaHandle(unit, file);
        }


        /// <summary>
        /// 获取手柄状态
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <returns>返回手柄状态值</returns>
        public static int GetHandleState(DeviceUnit unit)
        {
            return VXRControllerPlugin.GetHandleState(unit);
        }

        /// <summary>
        /// 获取手柄电量
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <returns>返回手柄电量值</returns>
        public static int GetHandlePower(DeviceUnit unit)
        {
            return VXRControllerPlugin.GetHandlePower(unit);
        }

        /// <summary>
        /// 设置Trigger键振动反馈
        /// </summary>
        /// <param name="options">振动反馈设置参数</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType SetTriggerForceFeedback(TriggerForceFeedbackOptions options)
        {
            return (ResultOperateType)VXRControllerPlugin.SetTriggerForceFeedback(options);
        }

        /// <summary>
        /// 只设置Trigger键振动反馈模式
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <param name="mode">模式类型</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType SetTriggerForceFeedbackModeOnly(DeviceUnit unit, int mode)
        {
            return (ResultOperateType)VXRControllerPlugin.SetTriggerForceFeedbackModeOnly(unit, mode);
        }


        /// <summary>
        /// 设置手柄触摸板振动反馈
        /// </summary>
        /// <param name="options">手柄触摸震动反馈设置参数结构</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType SetTouchpadVibrationFeedback(TouchpadVibrationFeedbackOptions options)
        {
            return (ResultOperateType)VXRControllerPlugin.SetTouchpadVibrationFeedback(options);
        }


        /// <summary>
        /// 只设置手柄触摸板振动反馈模式
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <param name="mode">模式类型</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType SetTouchpadVibrationFeedbackModeOnly(DeviceUnit unit, int mode)
        {
            return (ResultOperateType)VXRControllerPlugin.SetTouchpadVibrationFeedbackModeOnly(unit, mode);
        }


        /// <summary>
        /// 设置主手柄马达振动反馈
        /// </summary>
        /// <param name="options">主手柄马达振动反馈设置参数结构</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType SetMainMotorVibrationFeedback(MainMotorVibrationFeedbackOptions options)
        {
            return (ResultOperateType)VXRControllerPlugin.SetMainMotorVibrationFeedback(options);
        }

        /// <summary>
        /// 只设置主手柄马达振动反馈模式
        /// </summary>
        /// <param name="unit">设备单元</param>
        /// <param name="mode">模式类型</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType SetMainMotorVibrationFeedbackModeOnly(DeviceUnit unit, int mode)
        {
            return (ResultOperateType)VXRControllerPlugin.SetMainMotorVibrationFeedbackModeOnly(unit, mode);
        }

        /// <summary>
        /// 注册状态监听
        /// </summary>
        /// <param name="type">监听类型</param>
        /// <param name="listener">监听响应对象</param>
        /// <returns>返回操作结果</returns>
        public static ResultOperateType RegisterStickStateListener(HandleListenerType type, HandStickStateListener listener)
        {
            return (ResultOperateType)VXRControllerPlugin.RegisterStickStateListener(type,listener);
        }

        /// <summary>
        /// 解除状态监听
        /// </summary>
        /// <param name="listener">监听响应对象</param>
        /// <returns>返回操作结果</returns> 
        public static ResultOperateType UnregisterStickStateListener(HandStickStateListener listener)
        {
            return (ResultOperateType)VXRControllerPlugin.UnregisterStickStateListener(listener);
        }

        /// <summary>
        /// 应用退出
        /// </summary>
        public static void OnApplicationQuit()
        {
            VXRControllerPlugin.OnApplicationQuit();
        }

        /// <summary>
        /// 绑定message监听事件回调
        /// </summary>
        /// <param name="objName">场景中挂载监听脚本的游戏对象</param>
        /// <param name="funcName">监听脚本中用于监听的函数名</param>
        public static void SetMessageByUnity(string objName, string funcName)
        {
            VXRControllerPlugin.SetMessageByUnity(objName, funcName);
        }

        /// <summary>
        /// 绑定message监听事件回调
        /// </summary>
        /// <param name="obj">回调函数所在的对象实例</param>
        /// <param name="listenerEvent">监听响应回调函数</param>
        public static void BindListenerEvent(object obj, Action<string> listenerEvent = null)
        {
            VXRInputListener.Instance.BindListenerEvent(obj, listenerEvent);
        }

        /// <summary>
        /// 解除message监听绑定
        /// </summary>
        /// <param name="obj">监听函数所在的实例对象</param>
        public static void UnBindListenerEvent(object obj)
        {
            if (VXRInputListener.IsCreateInputListener)
            {
                VXRInputListener.Instance.UnBindListenerEvent(obj);
            }
        }
    }
}
