## 命名空间 
>com.vivo.openxr

## 类名
```CSharp
public static class VXRPassthrough
```

Passthrough相关能力开放接口

---------------------

## 成员方法
名称 | 描述
------ | ------
 [SetPassthroughEnable](#SetPassthroughEnable) | 设置透视通道开关，可用于开启或关闭VST
 [SetPassthroughBlendMode](#SetPassthroughBlendMode) | 设置透视通道混合模式
 



<span id="SetPassthroughEnable"></span>
**SetPassthroughEnable**

```CSharp
public static void SetPassthroughEnable(bool isEnable)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> isEnable | bool| 开启或关闭VST，true：开启、false：关闭
>
>*返回值*
>* 无

<span id="SetPassthroughBlendMode"></span>
**SetPassthroughBlendMode**

```CSharp
public static void SetPassthroughBlendMode(PassthroughBlendMode mode)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> mode | [PassthroughBlendMode](#PassthroughBlendMode)| 混合模式类型，Opaque：不透明模式 Additive：附加模式 AlphaBlend：透明混合模式
>
>*返回值*
>* 无




## 枚举类型
<span id="PassthroughBlendMode"></span>
**PassthroughBlendMode**  
键盘位置类型枚举

类型 | 描述
------ | ------
 Opaque | 不透明模式
 Additive | 附加模式
 AlphaBlend | 透明混合模式
