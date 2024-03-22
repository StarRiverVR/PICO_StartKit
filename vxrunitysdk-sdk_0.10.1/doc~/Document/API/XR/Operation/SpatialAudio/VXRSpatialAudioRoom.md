## 命名空间
>com.vivo.openxr

## 声明
```CSharp
public class VXRSpatialAudioRoom : MonoBehaviour
```

空间音频静态房间模型初始化配置脚本

---------------------

## 父类
* [MonoBehaviour](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/MonoBehaviour.html)

## 成员变量
变量名 | static |Editor | 类型| 描述
------ | ------ | ------ | ------ |  ------ 
 AttachTransform  | false | true | [Transform](https://docs.unity3d.com/2018.3/Documentation/ScriptReference/Transform.html) | 房间中心点物体
 length | false | true  | float | 房间.长
 width | false | true  | float | 房间.宽
 height | false | true  | float | 房间.高
 left  | false | true | [ReflectionMaterial](ReflectionMaterial.md) | 左边墙面预设材质
 right  | false | true | [ReflectionMaterial](ReflectionMaterial.md) | 右边墙面预设材质
 down  | false | true | [ReflectionMaterial](ReflectionMaterial.md) | 下边墙面预设材质
 up  | false | true | [ReflectionMaterial](ReflectionMaterial.md) | 上边墙面预设材质
 front  | false | true | [ReflectionMaterial](ReflectionMaterial.md) | 前边墙面预设材质
 back  | false | true | [ReflectionMaterial](ReflectionMaterial.md) | 后边墙面预设材质
 roomReflectionScalar  | false | true | float | 反射系数比例因子
 roomReverbGain  | false | true | float | 混响增益
 roomReverbTime  | false | true | float | 混响时间</br>**注：设置为1.0f时没有效果**
 roomReverbBrightness  | false | true | float | 混响明亮度
