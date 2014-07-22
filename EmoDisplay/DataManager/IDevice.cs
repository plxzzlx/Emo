using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    public abstract class IDevice
    {
        public enum Channel
        {
            F3, F4, P7, P8, F7, F8,
            AF3, AF4, FC5, T7, O1,
            O2, T8, FC6
        };
        public enum ConnectQuality
        {
            EEG_CQ_NO_SIGNAL,
            EEG_CQ_VERY_BAD,
            EEG_CQ_POOR,
            EEG_CQ_FAIR,
            EEG_CQ_GOOD
        };
        //获取设备名称
        abstract public String GetDeviceName();
        //获取设备的通道
        abstract public Channel[] GetChannels();
        //获取设备的通道数
        abstract public int GetChNum();
        //获取采样率
        abstract public int GetSampleRate();
        //采集一次数据
        abstract public Object GetData();
        //重建数据（重新封装成统一格式 即Dictionary<Channel, double[]>形式）
        abstract public Object RebuildData(Object obj);
        //获取信号状态
        abstract public ConnectQuality[] GetCQStates();
    }
}
