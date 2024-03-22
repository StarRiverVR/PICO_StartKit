## 命名空间 
>com.vivo.openxr

## 类名
```CSharp
public class VXRFoveationRendering
```

注视点设置

---------------------

## 成员方法
名称 | static | 描述
------ | ------ | ------
 [SetFoveationLevel](#SetFoveationLevel) | true | 设置注视点等级
 [GetFoveationLevel](#GetFoveationLevel) | true | 获取注视点等级
 [SetUseDynamicFoveationLevel](#SetUseDynamicFoveationLevel) | true | 获取注视点等级
 [GetUseDynamicFoveationLevel](#GetUseDynamicFoveationLevel) | true | 获取是否自适应注视点等级


<span id="SetFoveationLevel"></span>
**SetFoveationLevel**

```CSharp
public void SetFoveationLevel(Level level)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> level | [Level](FoveationLevel.md) | 注视点等级
>
>*返回值*
>* 无

<span id="GetFoveationLevel"></span>
**GetFoveationLevel**

```CSharp
public Level GetFoveationLevel()
```

>*参数列表*
>* 无
>
>*返回值*
>* [Level](FoveationLevel.md) 注视点等级

<span id="SetUseDynamicFoveationLevel"></span>
**SetUseDynamicFoveationLevel**

```CSharp
public void SetUseDynamicFoveationLevel()
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> use | bool | 是否自适应注视点等级
>
>*返回值*
>* 无

<span id="GetUseDynamicFoveationLevel"></span>
**GetUseDynamicFoveationLevel**

```CSharp
public bool GetUseDynamicFoveationLevel()
```

>*参数列表*
>* 无
>
>*返回值*
>* bool 是否自适应注视点等级