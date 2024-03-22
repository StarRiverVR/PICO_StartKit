## 命名空间 
>com.vivo.openxr

## 类名
```CSharp
public class VXROverlay
```

合成层功能组件，用于创建一个规定范围内形状的合成层

---------------------

## 成员方法
名称 | 描述
------ | ------
 [SetTextures](#SetTextures) | 动态设置合成层显示纹理

<span id="SetTextures"></span>
**SetTextures**

```CSharp
public void SetTextures(Texture leftEyeTexture,Texture rightEyeTexture)
```

>*参数列表*
> 参数 | 类型 | 描述
>------ | ------ | ------
> leftEyeTexture | Texture| 设置左眼纹理
> rightEyeTexture | Texture| 设置右眼纹理
>
>*返回值*
>* 无

## 成员变量
变量名 | static |Editor | 类型| 描述
------ | ------ | ------ | ------ |  ------ 
 CurShape | false | false  | [OverlayShape](#OverlayShape) | 当前合成层形状
 CurOverlayType | false | false  | [OverlayType](#OverlayType) | 当前合成层显示类型
  CurOverlayDepth | false | false  | int | 当前合成层的深度
 IsDynamic | false | false  | bool | true：使用动态纹理；fasle：使用静态纹理

## 枚举类型
<span id="OverlayShape"></span>
**OverlayShape**
类型 | 描述
------ | ------
 Quad | 具有四个顶点的平面纹理
 Cylinder | 具有柱面弧度的圆柱形纹理

<span id="OverlayType"></span>
**OverlayType**
类型 | 描述
------ | ------
 Overlay | 将纹理呈现在 Eye Buffer 前面
 Underlay | 将纹理呈现在 Eye Buffer 后面