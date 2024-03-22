using System;

namespace com.vivo.openxr
{
    public static class VXRRefreshRate
    {
        private static Action<float, float> _refreshRateChange;

        /// <summary>
        /// 设置显示刷新率。
        /// 注意：设置的刷新率必须为设备支持的刷新率。
        /// 设备支持刷新率参考接口<see cref="GetRefreshRates"/>
        /// </summary>
        /// <param name="rate">设置的刷新率</param>
        public static void SetRefreshRate(float rate)
        {
            VXRPlugin.SetDisplayRefreshRate(rate);
        }

        /// <summary>
        /// 获取显示刷新率。
        /// </summary>
        /// <returns>当前的显示刷新率</returns>
        public static float GetRefreshRate()
        {
            VXRPlugin.GetDisplayRefreshRate(out float rate);
            return rate;
        }
        
        /// <summary>
        /// 获取支持的显示刷新率列表。
        /// </summary>
        /// <returns>当前支持的刷新率数组</returns>
        public static float[] GetRefreshRates()
        {
            return VXRPlugin.GetRefreshRatesAvailable();
        }

        internal static void OnRefreshRateChange(VXRPlugin.RefreshRateChangedData change)
        {
            _refreshRateChange?.Invoke(change.FromRefreshRate, change.ToRefreshRate);
        }

        /// <summary>
        /// 添加刷新率变化事件监听。
        /// </summary>
        /// <param name="listener">监听事件</param>
        public static void AddChangeListener(Action<float,float> listener)
        {
            _refreshRateChange += listener;
        }

        /// <summary>
        /// 移除指定刷新率变化事件监听。
        /// </summary>
        /// <param name="listener">监听事件</param>
        public static void RemoveChangeListener(Action<float, float> listener)
        {
            _refreshRateChange -= listener;
        }

        /// <summary>
        /// 移除所有刷新率变化事件监听。
        /// </summary>
        public static void RemoveAllChangeListener()
        {
            _refreshRateChange = null;
        }
    }
}
