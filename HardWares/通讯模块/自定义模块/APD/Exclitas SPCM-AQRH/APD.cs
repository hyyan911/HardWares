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
        /// 接收任务
        /// </summary>
        Task DaqContinusReceiveTask = null;

        string APDReceiveChannelName = "";
        /// <summary>
        /// APD输入计数器通道(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object APDReceiveChannelNames
        {
            get
            {
                return new KeyValuePair<string, List<string>>(APDReceiveChannelName, DaqSystem.Local.GetTerminals(TerminalTypes.All).ToList());
            }
            set
            {
                APDReceiveChannelName = value.ToString();
            }
        }

        string CounterChannelName = "";
        /// <summary>
        /// 计数器通道(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object CounterChannelNames
        {
            get
            {
                return new KeyValuePair<string, List<string>>(CounterChannelName, DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External).ToList());
            }
            set
            {
                CounterChannelName = value.ToString();
            }
        }

        string CounterOutTriggerChannel1Name = "";
        /// <summary>
        /// 外部触发计数通道1(比如外部板卡信号，外部信号边沿触发一次，计数器返回当前计数)(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object CounterOutTriggerChannel1Names
        {
            get
            {
                return new KeyValuePair<string, List<string>>(CounterOutTriggerChannel1Name, DaqSystem.Local.GetTerminals(TerminalTypes.All).ToList());
            }
            set
            {
                CounterOutTriggerChannel1Name = value.ToString();
            }
        }

        string CounterOutTriggerChannel2Name = "";
        /// <summary>
        /// 外部触发计数通道2(比如外部板卡信号，外部信号边沿触发一次，计数器返回当前计数)(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object CounterOutTriggerChannel2Names
        {
            get
            {
                return new KeyValuePair<string, List<string>>(CounterOutTriggerChannel2Name, DaqSystem.Local.GetTerminals(TerminalTypes.All).ToList());
            }
            set
            {
                CounterOutTriggerChannel2Name = value.ToString();
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
            if (info.Params.Count == 3)
            {
                APDReceiveChannelName = info.Params[0];
                CounterChannelName = info.Params[2];
                CounterOutTriggerChannel1Name = info.Params[1];
                CounterOutTriggerChannel2Name = info.Params[2];
            }
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
