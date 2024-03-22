## 命名空间 
>com.vivo.openxr

## 类名
```CSharp
public class VXRVirtualKeyboard
```

虚拟键盘功能组件，实现一个可在 XR 空间中进行文字输入的键盘

---------------------

## 成员方法
名称 | 描述
------ | ------
 [ShowKeyBoard](#ShowKeyBoard) | 显示键盘
 [HideKeyBoard](#HideKeyBoard) | 隐藏键盘
 [AddCommitTextListener](#AddCommitTextListener) | 添加键盘文本提交事件监听
 [RemoveCommitTextListener](#RemoveCommitTextListener) | 移除键盘文本提交事件监听
 [AddBackSpaceListener](#AddBackSpaceListener) | 添加键盘退格键事件监听
 [RemoveBackSpaceListener](#RemoveBackSpaceListener) | 移除键盘退格键事件监听
 [AddEnterListener](#AddEnterListener) | 添加键盘回车键事件监听
 [RemoveEnterListener](#RemoveEnterListener) | 移除键盘回车键事件监听
 [AddShowListener](#AddShowListener) | 添加键盘显示事件监听
 [RemoveShowListener](#RemoveShowListener) | 移除键盘显示事件监听
 [AddHideListener](#AddHideListener) | 添加键盘隐藏事件监听
 [RemoveHideListener](#RemoveHideListener) | 移除键盘隐藏事件监听
 [UseRecommendLocation](#UseRecommendLocation) | 使用推荐键盘位置
 [AddRecordStartListener](#AddRecordStartListener) | 添加语音输入开始录音事件监听
 [RemoveRecordStartListener](#RemoveRecordStartListener) | 移除语音输入开始录音事件监听
 [AddRecordEndListener](#AddRecordEndListener) | 添加语音输入结束录音事件监听
 [RemoveRecordEndListener](#RemoveRecordEndListener) | 移除语音输入结束录音事件监听



<span id="ShowKeyBoard"></span>
**ShowKeyBoard**

```CSharp
public void ShowKeyBoard(VirtualKeyBoardLayout keyboardLayout = VirtualKeyBoardLayout.Text)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> keyboardLayout | [VirtualKeyBoardLayout](#VirtualKeyBoardLayout)| 键盘布局类型，默认为常规文本键盘
>
>*返回值*
>* 无

<span id="HideKeyBoard"></span>
**HideKeyBoard**

```CSharp
  public void HideKeyBoard()
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
>
>*返回值*
>* 无


<span id="AddCommitTextListener"></span>
**AddCommitTextListener**

```CSharp
public void AddCommitTextListener(Action<string> listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action<string>| 键盘文本提交事件响应时需要回调的函数
>
>*返回值*
>* 无

<span id="RemoveCommitTextListener"></span>
**RemoveCommitTextListener**

```CSharp
 public void RemoveCommitTextListener(Action<string> listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action<string>| 键盘文本提交事件响应时需要回调的函数
>
>*返回值*
>* 无


<span id="AddBackSpaceListener"></span>
**AddBackSpaceListener**

```CSharp
 public void AddBackSpaceListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘退格键事件响应时需要回调的函数
>
>*返回值*
>* 无



<span id="RemoveBackSpaceListener"></span>
**RemoveBackSpaceListener**

```CSharp
 public void RemoveBackSpaceListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘退格键事件响应时需要回调的函数
>
>*返回值*
>* 无



<span id="AddEnterListener"></span>
**AddEnterListener**

```CSharp
public void AddEnterListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘回车键事件响应时需要回调的函数
>
>*返回值*
>* 无


<span id="RemoveEnterListener"></span>
**RemoveEnterListener**

```CSharp
public void RemoveEnterListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘回车键事件响应时需要回调的函数
>
>*返回值*
>* 无


<span id="AddShowListener"></span>
**AddShowListener**

```CSharp
public void AddShowListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘显示事件响应时需要回调的函数
>
>*返回值*
>* 无

<span id="RemoveShowListener"></span>
**RemoveShowListener**

```CSharp
public void RemoveShowListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘显示事件响应时需要回调的函数
>
>*返回值*
>* 无

<span id="AddHideListener"></span>
**AddHideListener**

```CSharp
public void AddHideListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘隐藏事件响应时需要回调的函数
>
>*返回值*
>* 无

<span id="RemoveHideListener"></span>
**RemoveHideListener**

```CSharp
public void RemoveHideListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘隐藏事件响应时需要回调的函数
>
>*返回值*
>* 无


<span id="AddRecordStartListener"></span>
**AddRecordStartListener**

```CSharp
public void AddRecordStartListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘语音输入开始事件响应时需要回调的函数
>
>*返回值*
>* 无

<span id="RemoveRecordStartListener"></span>
**RemoveRecordStartListener**

```CSharp
public void RemoveRecordStartListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘语音输入开始事件响应时需要回调的函数
>
>*返回值*
>* 无



<span id="AddRecordEndListener"></span>
**AddRecordEndListener**

```CSharp
public void AddRecordEndListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘语音输入结束事件响应时需要回调的函数
>
>*返回值*
>* 无


<span id="RemoveRecordEndListener"></span>
**RemoveRecordEndListener**

```CSharp
public void RemoveRecordEndListener(Action listener)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> listener | Action| 键盘语音输入结束事件响应时需要回调的函数
>
>*返回值*
>* 无

<span id="UseRecommendLocation"></span>
**UseRecommendLocation**

```CSharp
 public void UseRecommendLocation(KeyboardLocationType locationType)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> locationType | [KeyboardLocationType](#KeyboardLocationType)| 要使用的位置类型
>
>*返回值*
>* 无





## 成员变量
变量名 | static |Editor | 类型| 描述
------ | ------ | ------ | ------ |  ------ 
 InputFieldCommits | false | false  | InputField | 当前虚拟键盘响应的输入框列表
 ZAxisGameObjects | false | false  | GameObject | 射线和键盘交互的载体，一般为可视化的射线模型，注意载体自身坐标的Z轴朝向必须为前方
 TargetActionAsset | false | false  | InputActionAsset | 当前使用的控制器输入的InputSystem Asset配置
 ActionReferences | false | false  | InputActionReference | 当前使用的控制器输入Action选项


## 枚举类型
<span id="KeyboardLocationType"></span>
**KeyboardLocationType**  
键盘位置类型枚举

类型 | 描述
------ | ------
 Near | 近场显示
 Far | 远场显示
 Custom | 自定义移动位置

<span id="VirtualKeyBoardLayout"></span>
**VirtualKeyBoardLayout**
键盘布局类型

类型 | 描述
------ | ------
 Text | 常规全键盘
 Number | 数字键盘