# VXRInput
|Functgion          |Description            |
|:---                |:---                    |
|[IsConnected](#IsConnected)        |是否已经链接|
|[GetHeadVersion](#GetHeadVersion)        |获取头显版本号|
|[GetHandVersion](#GetHandVersion)        |获取手柄版本号|
|[GetHandleSN](#GetHandleSN)        |获取手柄SN|
|[PairHandleMode](#PairHandleMode)        |绑定手柄|
|[UnpairHandleMode](#UnpairHandleMode)        |解除手柄绑定|
|[OtaHead](#OtaHead)        |升级头显|
|[OtaHandle](#OtaHandle)        |升级手柄|
|[GetHandleState](#GetHandleState)        |获取手柄状态|
|[GetHandlePower](#GetHandlePower)        |获取手柄电量    |
|[SetTriggerForceFeedback](#SetTriggerForceFeedback)        |设置Trigger键振动反馈|
|[SetTriggerForceFeedbackModeOnly](#SetTriggerForceFeedbackModeOnly)        |只设置Trigger振动反馈模式|
|[SetTouchpadVibrationFeedback](#SetTouchpadVibrationFeedback)        |设置手柄触摸板振动反馈|
|[SetTouchpadVibrationFeedbackModeOnly](#SetTouchpadVibrationFeedbackModeOnly)        |只设置手柄触摸板振动反馈模式|
|[SetMainMotorVibrationFeedback](#SetMainMotorVibrationFeedback)        |设置主手柄马达振动反馈|
|[SetMainMotorVibrationFeedbackModeOnly](#SetMainMotorVibrationFeedbackModeOnly)        |只设置手柄马达振动反馈模式|
|[RegisterStickStateListener](#RegisterStickStateListener)        |注册状态监听|
|[UnregisterStickStateListener](#UnregisterStickStateListener)        |解除状态监听|
|[OnApplicationQuit](#OnApplicationQuit)        |退出应用|
|[SetMessageByUnity](#SetMessageByUnity)        |设置Unity端监听游戏对象和监听函数|
|[BindListenerEvent](#BindListenerEvent)        |绑定message监听事件回调|
|[UnBindListenerEvent](#UnBindListenerEvent)        |解除message监听事件回调|


<span id="IsConnected"></span>
## IsConnected
|bool IsConnected()  | 
|:-----------------------| 

手柄SDK是否连接
### Returns
_true:_ 已连接  
_fasel:_ 未连接


<span id="GetHeadVersion"></span>
## GetHeadVersion
| string GetHeadVersion() | 
| :-----------------------| 

获取头显版本号
### Returns
返回头显的版本号信息


<span id="GetHandVersion"></span>
## GetHandVersion
| string GetHandVersion(DeviceUnit unit) | 
| :-----------------------| 

获取手柄版本号信息

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
### Returns
返回对应手柄的版本号信息


<span id="GetHandleSN"></span>
## GetHandleSN
| string GetHandleSN(DeviceUnit unit)  | 
| :-----------------------| 

获取手柄SN

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
### Returns
返回对应手柄的SN


<span id="PairHandleMode"></span>
## PairHandleMode
| ResultOperateType PairHandleMode(DeviceUnit unit)         | 
| :-----------------------| 

绑定手柄

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功

<span id="UnpairHandleMode"></span>
## UnpairHandleMode
| ResultOperateType UnpairHandleMode(DeviceUnit unit)         | 
| :-----------------------| 

解除手柄绑定

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功



<span id="OtaHead"></span>
## OtaHead
|  ResultOperateType OtaHead(string file)        | 
| :-----------------------| 

升级头显

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|file        |头显升级指令 |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功



<span id="OtaHandle"></span>
## OtaHandle
| ResultOperateType OtaHandle(DeviceUnit unit, string file)  | 
| :-----------------------| 

升级手柄

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
|file        |手柄升级指令|
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功



<span id="GetHandleState"></span>
## GetHandleState
| int GetHandleState(DeviceUnit unit)       | 
| :-----------------------| 

获取手柄状态

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
### Returns
返回手柄状态值


<span id="GetHandlePower"></span>
## GetHandlePower
| int GetHandlePower(DeviceUnit unit)      | 
| :-----------------------| 

获取手柄电量

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
### Returns
返回手柄电量值


<span id="SetTriggerForceFeedback"></span>
## SetTriggerForceFeedback
| ResultOperateType SetTriggerForceFeedback(TriggerForceFeedbackOptions options)   | 
| :-----------------------| 

设置Trigger键振动反馈

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|options        |振动反馈设置参数[TriggerForceFeedbackOptions](#TriggerForceFeedbackOptions) |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功



<span id="SetTriggerForceFeedbackModeOnly"></span>
## SetTriggerForceFeedbackModeOnly
| ResultOperateType SetTriggerForceFeedbackModeOnly(DeviceUnit unit, int mode)     | 
| :-----------------------| 

只设置Trigger键振动反馈模式

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
|mode        |模式类型|

### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="SetTouchpadVibrationFeedback"></span>
## SetTouchpadVibrationFeedback
| ResultOperateType SetTouchpadVibrationFeedback(TouchpadVibrationFeedbackOptions options)         | 
| :-----------------------| 

设置手柄触摸板振动反馈

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|options        |手柄触摸震动反馈设置参数结构[TouchpadVibrationFeedbackOptions](#TouchpadVibrationFeedbackOptions)  |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="SetTouchpadVibrationFeedbackModeOnly"></span>
## SetTouchpadVibrationFeedbackModeOnly
| ResultOperateType SetTouchpadVibrationFeedbackModeOnly(DeviceUnit unit, int mode)        | 
| :-----------------------| 

只设置手柄触摸板振动反馈模式

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
|mode        |模式类型|
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="SetMainMotorVibrationFeedback"></span>
## SetMainMotorVibrationFeedback
| ResultOperateType SetMainMotorVibrationFeedback(MainMotorVibrationFeedbackOptions options)        | 
| :-----------------------| 

设置主手柄马达振动反馈

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|options        |主手柄马达振动反馈设置参数结构[MainMotorVibrationFeedbackOptions](#MainMotorVibrationFeedbackOptions)  |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="SetMainMotorVibrationFeedbackModeOnly"></span>
## SetMainMotorVibrationFeedbackModeOnly
| ResultOperateType SetMainMotorVibrationFeedbackModeOnly(DeviceUnit unit, int mode)         | 
| :-----------------------| 

只设置主手柄马达振动反馈模式

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|unit        |设备单元[DeviceUnit](#DeviceUnit)，Left：左手柄，Right:右手柄 |
|mode        |模式类型|
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="RegisterStickStateListener"></span>
## RegisterStickStateListener
| ResultOperateType RegisterStickStateListener(HandleListenerType type, HandStickStateListener listener)  | 
| :-----------------------| 

注册状态监听

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|type            |监听类型[HandleListenerType](#HandleListenerType)|
|listener        |监听响应对象HandStickStateListener|
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="UnregisterStickStateListener"></span>
## UnregisterStickStateListener
| ResultOperateType UnregisterStickStateListener(HandStickStateListener listener)       | 
| :-----------------------| 

解除状态监听

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|listener        |监听响应对象HandStickStateListener |
### Returns
[ResultOperateType](#ResultOperateType)：返回操作结果  
_RESULT_OPERATE_INVAILD:_ 操作无效  
_RESULT_OPERATE_FAIL:_ 操作炒作失败  
_RESULT_OPERATE_SUCCESS:_ 操作成功


<span id="OnApplicationQuit"></span>
## OnApplicationQuit
| void OnApplicationQuit()   | 
| :-----------------------| 

应用退出



<span id="SetMessageByUnity"></span>
## SetMessageByUnity
| void SetMessageByUnity(string objName, string funcName)     | 
| :-----------------------| 

绑定message监听事件回调

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|objName         |场景中挂载监听脚本的游戏对象 |
|funcName        |监听脚本中用于监听的函数名|


<span id="BindListenerEvent"></span>
## BindListenerEvent
| void BindListenerEvent(object obj, Action<string> listenerEvent = null)     | 
| :-----------------------| 

绑定message监听事件回调

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|obj        |回调函数所在的对象实例 |
|listenerEvent        |监听响应回调函数 |


<span id="UnBindListenerEvent"></span>
## UnBindListenerEvent
| void UnBindListenerEvent(object obj)    | 
| :-----------------------| 

解除message监听绑定

### Parameters
|Parameter          |Description            |
|:---              |:---                    |
|obj        |监听函数所在的实例对象 |  

# **Enum**
<span id="DeviceUnit"></span>
**<font size=5>DeviceUnit</font>**  
设备单元

|Parameter   |value       |Description            |
|:---           |:---              |:---                    |
|Left    |   0   |左手柄 |
|Right   |   1   |右手柄 |
|Head    |   2   |头显 |

<span id="HandleListenerType"></span>
**<font size=5>HandleListenerType</font>**  
监听类型

|Parameter  |value        |Description            |
|:---       |:---       |:---                    |
|TYPE_STICK_STATE_LISTEN   |   0    |手柄状态监听 |
|TYPE_STICK_POWER_LISTEN   |   1    |手柄电量监听 |
|TYPE_STICK_UPDATE_LISTEN  |   2    |手柄升级监听 |
|TYPE_STICK_KEY_LISTEN     |   3    |手柄按键监听 |
|TYPE_STYLUS_LISTEN        |   4    |手写笔监听 |

<span id="ResultOperateType"></span>
**<font size=5>ResultOperateType</font>**  
返回操作结果

|Parameter |Value          |Description            |
|:---    |:---              |:---                    |
|RESULT_OPERATE_INVAILD  |   -2    |操作无效 |
|RESULT_OPERATE_FAIL     |   -1    |操作失败 |
|RESULT_OPERATE_SUCCESS  |    0     |操作成功 |

*********************************
# **Struct**
<span id="TriggerForceFeedbackOptions"></span>
**<font size=5>TriggerForceFeedbackOptions</font>**  
Trriger键振动反馈设置参数结构

|Parameter    |type      |Description            |
|:---          |:---              |:---                    |
|unit      | [DeviceUnit](#DeviceUnit)     |设备单元|
|mode                               | int  |振动模式|
|dampingForce                       | int  |阻尼值|
|dampingStartPos                    | int  |阻尼起始位置|
|vibrationForce                     | int  |振动值|
|vibrationFrequency                 | int  |振动频率|
|vibrationStartPos                  | int  |振动开始位置|
|vibrationStartForce                | int  |振动启动力度|
|breakthroughStartPos               | int  |突破起始位置|
|breakthroughContinuousJourney      | int  |突破连续行程|
|breakthroughResistance             | int  |突破阻力|


<span id="TouchpadVibrationFeedbackOptions"></span>
**<font size=5>TouchpadVibrationFeedbackOptions</font>**  
手柄触摸震动反馈参数设置结构

|Parameter    |type      |Description            |
|:---          |:---              |:---                    |
|unit      | [DeviceUnit](#DeviceUnit)     |设备单元|
|mode                               | int  |模式|
|frequency                          | int  |频率|
|amplitude                          | int  |振幅|
|duration                           | int  |期间|
|startTime                          | int  |开始时间|
|brakeTime                          | int  |制动时间|


<span id="MainMotorVibrationFeedbackOptions"></span>
**<font size=5>MainMotorVibrationFeedbackOptions</font>**  
手柄主马达振动反馈设置参数结构

|Parameter    |type      |Description            |
|:---          |:---              |:---                    |
|unit      | [DeviceUnit](#DeviceUnit)     |设备单元|
|mode                               | int  |模式|
|frequency                          | int  |频率|
|amplitude                          | int  |振幅|
|duration                           | int  |期间|
|startTime                          | int  |开始时间|
|brakeTime                          | int  |制动时间|


                                      
