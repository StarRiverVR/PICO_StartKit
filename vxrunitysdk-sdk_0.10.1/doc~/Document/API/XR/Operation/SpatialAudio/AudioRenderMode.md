## 命名空间
```CSharp
com.vivo.openxr.VXRSpatialAudio
```
## 声明
```CSharp
public enum AuidoRenderMode
```

音频渲染模式

---------------------

## 枚举类型
类型 | 描述
------ | ------
 HRTFDisable | 禁用基于HRTF的渲染
 HRTFAmbisonics8 | 基于HRTF的渲染，使用一阶Ambisonics， 8个扬声器的虚拟阵列
 HRTFAmbisonics12 | 基于HRTF的渲染，二阶Ambisonics，12个扬声器的虚拟阵列
 HRTFAmbisonics26 | 基于HRTF的渲染，三阶Ambisonics，26个扬声器的虚拟阵列
 Room | 仅房间效果渲染。这将禁用基于HRTF的渲染和声音对象的直接（干）输出