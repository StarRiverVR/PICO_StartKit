## 命名空间
>com.vivo.openxr.VXREvent

## 声明
```CSharp
public struct CrownRotateData
```

表冠单次旋转信息，重新旋转时会清零；正常低速情况下变化量的绝对值恒为1度。

---------------------

## 成员变量
变量名 | 类型| 描述
------ | ------ | ------
 RotateDelta  | int | 单次旋转变化量，重新旋转时会清零；正/负代表旋转方向，顺时针为正，逆时针为负；正常低速情况下变化量的绝对值恒为 1度。
 Direction  | bool | 旋转方向，表冠顺时针是返回true，逆时针返回 false 。
