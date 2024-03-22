using System.Collections.Generic;
using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public class EventManager : MonoSingleton<EventManager>
    {
        /// <summary>
        /// 事件响应后的事件数据池
        /// </summary>
        private List<VXRPlugin.EventDataBuffer> _dataBufferList = new List<VXRPlugin.EventDataBuffer>();

        public void Init()
        {
        }

        public void AddEventDataBuffer(VXRPlugin.EventDataBuffer dataBuff)
        {
            _dataBufferList.Add(dataBuff);
        }

        void Update()
        {
            // Native Events
            VXRPlugin.EventDataBuffer eventDataBuffer = new VXRPlugin.EventDataBuffer();
            // TODO 使用 out ，此处不需要考虑内存循环使用问题
            while (VXRPlugin.PollEvent(ref eventDataBuffer))
            {
                ExcuteEvent(eventDataBuffer);
            }

            // Android Events
            _dataBufferList.Clear();
            // TODO : 不要使用数组传递
            VXRPlugin.PollAndroidEvent(ref _dataBufferList);
            while(_dataBufferList.Count > 0)
            {
                var buffer =_dataBufferList[0];
                _dataBufferList.RemoveAt(0);
                ExcuteEvent(buffer);
            }
        }
        
        private void ExcuteEvent(VXRPlugin.EventDataBuffer dataBuffer)
        {
            // VXREvent 未阻断事件 继续判断事件派发
            if (!VXREvent.EventCallBack(dataBuffer))
            {
                switch (dataBuffer.EventType)
                {
                    case VXRPlugin.EventType.RefreshRateChanged:
                        VXRRefreshRate.OnRefreshRateChange(VXRDeserialize.ByteArrayToStructure<VXRPlugin.RefreshRateChangedData>(dataBuffer.EventData));
                        break;
                    default:
                        VLog.Warning($"Unknow Native Event Type[{dataBuffer.EventType}]");
                        break;
                }
            }
        }

        protected override void OnApplicationQuit()
        {
            VXRPlugin.ReleaseAndroidVXREvent();
        }
    }

}
