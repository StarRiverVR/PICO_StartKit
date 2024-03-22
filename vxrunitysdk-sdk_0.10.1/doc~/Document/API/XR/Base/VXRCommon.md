<details> 
    <summary>VXR API 文档</summary>
</details>

**NameSpace**
>>com.vivo.openxr.VXRCommon

**描述**
>>全局通用设置与接口。

**静态变量**

变量名     | Editor | 类型 |描述
-------- | ----- | --------| --------
ReleaseMode  |false| VXRAsset.BuildReleaseMode | 构建模式
isUnityXROn  |false| bool| 当前是否已经安装了UnityXR插件包
isUnityURPOn  |false| bool| 当前是否已经安装了UnityURP插件包
isUnityXRManagementOn  |false| bool| 当前是否已经安装了UnityXRManagement插件包
isDeviceSupported  |false| bool| 设备是否支持XR
TargetGroups  |true| BuildTargetGroup[]| SDK支持的平台
BuildTargets  |true| BuildTarget[]| SDK支持的平台
ActiveBuildTarget  |true| BuildTarget| 当前构建的平台
ActiveBuildTargetGroup  |true| BuildTargetGroup| 当前构建的平台
SelectedBuildTargetGroup  |true| BuildTargetGroup| 当前选择的平台
CurrentSelectedVivoOpenXRFeature  |true| VXRFeature| 当前选择平台的VXRFeature
CurrentActiveVivoOpenXRFeature  |true| VXRFeature| 当前构建平台的VXRFeature
CurrentSelectedVivoOpenXRControllerProfile  |true| VXRControllerProfile| 当前选择平台的VXRControllerProfile
CurrentActiveVivoOpenXRControllerProfile  |true| VXRControllerProfile| 当前选择平台的VXRControllerProfile
UnityRunningInBatchmode  |false| bool| Unity当前运行模式是否为Batchmode
IsURP  |false| bool| 当前是否为URP模式
UrpAsset  |false| UniversalRenderPipelineAsset| 获取URP配置
MsaaSample  |false| MSAASamples| MSAA抗锯齿设置 None-关闭，MSAA2x-2倍抗锯齿，MSAA4x-4倍抗锯齿，MSAA8x-8倍抗锯齿
MsaaSampleCount  |false| int| MSAA抗锯齿设置 1-关闭，2-2倍抗锯齿，4-4倍抗锯齿，8-8倍抗锯齿
EyeTextureResolutionScale  |false| float| 分辨率缩放 0.5-2
RenderViewportScale  |false| float| 视口缩放 0-1
RefreshRate  |false| float| 屏幕刷新率
RefreshRatesAvailable  |false| float[] | 屏幕刷新率列表
**静态函数**

函数名     | Editor |描述
-------- | ----- | --------
GetXRGeneralSettingsForBuildTarget  |true| 获得Unity XR配置
GetVivoOpenXRFeature  |true| 获得对应平台的 VXRFeature
GetVivoOpenXRControllerProfile  |true| 获得对应平台的 VXRControllerProfile
SetColorSpace  |true| 设置颜色空间，通常为Linear
IsPermissionGranted  |false| 判断安卓权限是否开启
PermissionRequest  |false| 安卓权限申请
IsPackageInstalled  |true| 是否有安装Package
GetDisplaySubsystem  |false| 获得当前使用的显示子系统
GetInputSubsystem  |false| 获得当前使用的输入子系统
StartFadeInOutAnim  |false| 淡入淡出效果





