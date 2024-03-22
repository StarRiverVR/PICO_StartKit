# Passthrough

> 实现虚拟现实和现实世界之间的透传，达成虚拟现实和现实世界场景的混合效果。



当前VST特性在开启URP时，使用URP材质可能存在问题，解决方案如下：

* 在URP Assets inspect面板关闭Quality -> HDR选项；

## 开启或关闭VST

创建一个MonoBehaviour脚本挂载到场景中，在需要的时候，调用VXRPassthrough.SetPassthroughEnable(bool isEnable)开启或关闭VST


*  示例
 ``` 
 private void OnChangeBlendModelBtn()
    {
        IsVSTState = !IsVSTState;
        if (IsVSTState)
        {
            VXRPassthrough.SetPassthroughEnable(true);            
        }
        else
        {
            VXRPassthrough.SetPassthroughEnable(false);
        }
    }   
    
    
  ```

