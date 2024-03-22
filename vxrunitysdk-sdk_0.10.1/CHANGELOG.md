# Changelog
开发包修改记录

## [0.10.1] - 2024-02-06

### Fixed
- 虚拟键盘修复两个手柄输入错乱问题；
- 表冠修复长按按钮闪退问题；
- 眼动Sample增加注释坐标刷新面板；

## [0.10.0] - 2024-01-19

### Added
- 虚拟键盘支持语音输入，新增语音快捷键及其操作指示板；
- 合成层新增支持underlay类型；
- 旋转表冠新增单次旋转角度事件，返回单次旋转变化量；

## [0.9.0] - 2023-12-27

### Changed
- plugin数据结构按开放Feature层和内部plugin层做解耦重构；

### Added
- 虚拟键盘增加震动、远近、多个输入文本框绑定和切换；
- overlay增加自定义层级排序功能；

## [0.8.0] - 2023-12-19

### Fixed
- 虚拟键盘重写服务接口plugin；
- plugin数据结构按模块解耦重构，修复一些已知问题；

### Added
- 空间音频增加房间模型及环境混响；
- editor模块增加配置导出支持；

## [0.7.1] - 2023-12-14

### Fixed
- 优化环境说明文档，修复空间音频等Sample的体验问题；

## [0.7.0] - 2023-12-01

### Fixed
- 修复main Sample的urp场景切换菜单偏移难以操作问题；

### Added
- 增加空间音频能力，支持自由声场3D音频特性和音频自定义开发；
- 增加虚拟键盘，支持应用通过input组件绑定和文本监听回调两种方式使用（当前支持在Android Launcher版本上使用）；
- FFR 增加dynamic自动匹配开放和使用介绍，sample增加自动匹配示例；

## [0.6.1] - 2023-11-16

### Changed
- 回收空间锚点接口，后续择机开放；

### Fixed
- 修复sample适配系统退出菜单等问题；

### Added
- 更新Openxr loader；
  
## [0.6.0] - 2023-11-15

### Fixed
- 修复loader访问runtime broker缺失权限需手工添加的问题；
- 修复空间锚点示例交互问题，支持左右手柄；

### Added
- 新增SDK 版本获取接口；
- 新增VXREvent-旋转表冠事件，支持顺时针和逆时针响应；

## [0.5.0] - 2023-10-30

### Fixed
- 修复合成层显示图片色彩变暗；

### Added
- 新增空间锚点，支持锚点的创建、销毁、持久化、反持久化、重新加载、获取锚点位置；
- 合成层Sample增加空间图片、Camera RenderTexture使用示例；


## [0.4.0] - 2023-10-17

### Fixed
- 淡入淡出兼容URP和内置管线；
- 部分Sample的材质兼容问题；

### Added
- "Interaction Profiles"增加可选"Vivo Controller profile"；
- 增加VXREvent模块，支持刷新率、Openxr session进入退出、焦点得失 的事件监听；
- VXROverlay合成层组件，支持Quad、Cylinder形状；
- 眼动交互接入XRI，支持Gaze Interactor眼动交互器；
- doc增加硬件设备和开发环境搭建介绍；

## [0.3.0] - 2023-09-08

### Changed
- "Interaction Profiles"可选择"Oculus touch profile"

### Added
- FFR固定注视点渲染。

## [0.2.1] - 2023-09-22

### Changed
- 将c++_shared库文件提出来以供可选；

### Fixed
- 修复Editor、Build相关脚本打包问题；

## [0.2.0] - 2023-09-08

### Added
- 抗锯齿、视口调节、淡入淡出、延迟锁定、VST开关、眼动追踪。


## [0.0.1] - 2023-05-30

### Changed
- 开发包集成Openxr loader

### Fixed

### Added
- VivoXRPlugin开发包。

