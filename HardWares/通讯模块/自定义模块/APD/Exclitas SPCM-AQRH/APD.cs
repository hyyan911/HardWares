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

        string DaqContinusTriggerTaskName = "";
        string DaqContinusReceiveTaskName = "";

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
            return new List<CustomDeviceInfo>() { new CustomDeviceInfo("Exclitas_SPCM_AQRH " + count.ToString()) };
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
