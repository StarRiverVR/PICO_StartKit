## 命名空间
>com.vivo.openxr

## 声明
```CSharp
public class VXRRefreshRate
```

描述

---------------------
 ## 成员方法
 方法名 | static | 描述
 ------ | ------ | ------ 
   [SetRefreshRate](#SetRefreshRate) |   true  |  设置刷新率
   [GetRefreshRate](#GetRefreshRate) |   true  |  获取刷新率
   [GetRefreshRates](#GetRefreshRates) |   true  | 获取有效刷新率列表
   [AddChangeListener](#AddChangeListener) |   true  | 添加刷新率变化事件监听
   [RemoveChangeListener](#RemoveChangeListener) |   true  | 移除指定刷新率变化事件监听
   [RemoveAllChangeListener](#RemoveAllChangeListener) |   true  | 移除所有刷新率变化事件监听
 
 <span id="SetRefreshRate"></span>
 **SetRefreshRate**
 
 ```CSharp
 public void SetRefreshRate(float rate)
 ```
 
 > *参数列表*
 > 参数 | 类型 | 描述
 > ------ | ------ | ------
 > rate |  float |  刷新率
 >
 > *返回值*
 > * 无

  <span id="GetRefreshRate"></span>
 **GetRefreshRate**
 
 ```CSharp
 public float GetRefreshRate()
 ```
 
 > *参数列表*
 > * 无
 >
 > *返回值*
 > * 当前刷新率

 
  <span id="GetRefreshRates"></span>
 **GetRefreshRates**
 
 ```CSharp
 public float GetRefreshRates()
 ```
 
 > *参数列表*
 > * 无
 >
 > *返回值*
 > * 当前设备支持的刷新率列表
 
   <span id="AddChangeListener"></span>
 **AddChangeListener**
 
 ```CSharp
  public static void AddChangeListener(Action<float,float> listener)
 ```
 
 > *参数列表*
 > 参数 | 类型 | 描述
 > ------ | ------ | ------
 > listener |  Action\<float,float> |  刷新率变化监听回调函数 </br>**参数1** 变化前刷新率</br> **参数2** 变化后刷新率
 >
 > *返回值*
 > * 无

  <span id="RemoveChangeListener"></span>
 **RemoveChangeListener**
 
 ```CSharp
       public static void RemoveChangeListener(Action<RefreshRateChangedData> listener)
 ```
 
 > *参数列表*
 > 参数 | 类型 | 描述
 > ------ | ------ | ------
 > listener |  Action\<float,float> |  刷新率变化监听回调函数 </br>**参数1** 变化前刷新率</br> **参数2** 变化后刷新率
 >
 > *返回值*
 > * 无

<span id="RemoveAllChangeListener"></span>
**RemoveAllChangeListener**

```CSharp
public static void RemoveAllChangeListener()
```

>*参数列表*
>* 无
>
>*返回值*
>* 无