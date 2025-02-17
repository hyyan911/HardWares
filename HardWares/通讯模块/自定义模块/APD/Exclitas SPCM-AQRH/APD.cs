using HardWares.端口基类;
using HardWares.端口基类.Custom串口;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using Task = NationalInstruments.DAQmx.Task;

namespace HardWares.APD.Exclitas_SPCM_AQRH
{
    public partial class APD : APDBase, CustomInternalInterface, CustomOuterInterface
    {
        /// <summary>
        /// 输出信号触发任务
        /// </summary>
        Task DaqContinusTriggerTask = null;
        /// <summary>
        /// 接收任务
        /// </summary>
        Task DaqContinusReceiveTask = null;

        string DaqContinusTriggerChannelName = "";
        /// <summary>
        /// 连续测量输出信号触发通道(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object DaqContinusTriggerChannelNames
        {
            get
            {
                return new KeyValuePair<string, List<string>>(DaqContinusTriggerChannelName, DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CO, PhysicalChannelAccess.All).ToList());
            }
            set
            {
                DaqContinusTriggerChannelName = value.ToString();
            }
        }

        string DaqContinusReceiveChannelName = "";
        /// <summary>
        /// APD输入计数器通道(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object DaqContinusReceiveChannelNames
        {
            get
            {
                return new KeyValuePair<string, List<string>>(DaqContinusReceiveChannelName, DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.All).ToList());
            }
            set
            {
                DaqContinusReceiveChannelName = value.ToString();
            }
        }

        string APDDataChannelName = "";
        /// <summary>
        /// APD数据采集通道(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object APDDataChannelNames
        {
            get
            {
                return new KeyValuePair<string, List<string>>(APDDataChannelName, DaqSystem.Local.GetTerminals(TerminalTypes.All).ToList());
            }
            set
            {
                APDDataChannelName = value.ToString();
            }
        }

        /// <summary>
        /// 连接，参数列表：1：输入DAQmx通道名,2.输出触发信号DAXmx通道名
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <param name="reconnect"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ConnectCustom(CustomDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect);
        }

        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <returns></returns>
        public List<CustomDeviceInfo> GetCustomDeviceInfos()
        {
            int count = 0;
            foreach (var item in DeviceInfos)
            {
                if (item.Key.DeviceName.Contains("Exclitas_SPCM_AQRH"))
                {
                    ++count;
                }
            }
            return new List<CustomDeviceInfo>() { new CustomDeviceInfo("Exclitas_SPCM_AQRH " + count.ToString()) { Params = new List<string>() { "", "" } } };
        }

        void CustomInternalInterface.CloseCustomPort()
        {
            try
            {
                DaqContinusReceiveTask?.Stop();
                DaqContinusReceiveTask?.Dispose();
            }
            catch (Exception) { }
            try
            {
                DaqContinusReceiveTask?.Stop();
                DaqContinusReceiveTask?.Dispose();
            }
            catch (Exception) { }
        }

        void CustomInternalInterface.ConnectedCustomAction()
        {
        }

        void CustomInternalInterface.CustomInitAction(object PortInstance)
        {
            return;
        }

        byte[] CustomInternalInterface.CustomPortRead()
        {
            return new byte[0];
        }

        void CustomInternalInterface.CustomPortWrite(byte[] value)
        {
        }

        bool CustomInternalInterface.IsCustomOpen()
        {
            return true;
        }

        object CustomInternalInterface.OpenCustomPort(CustomDeviceInfo info)
        {
            DaqContinusReceiveChannelName = info.Params[0];
            DaqContinusTriggerChannelName = info.Params[1];
            return null;
        }

        void CustomInternalInterface.ReceiveCustomAct()
        {
            return;
        }

        bool CustomInternalInterface.TestCustomAction()
        {
            return true;
        }
    }
}
