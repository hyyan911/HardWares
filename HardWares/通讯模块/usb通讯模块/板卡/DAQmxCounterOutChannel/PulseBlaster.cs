using HardWares.仪器列表.板卡.Spincore_PulseBlaster;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
using SpinCore.SpinAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using Task = NationalInstruments.DAQmx.Task;

namespace HardWares.板卡.DAQmxCounterSignalChannel
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PulseBlaster : WinUSBOuterInterface, USBInternalInterface
    {
        private Task OutputTask = null;

        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, false, false);
        }

        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            return DaqSystem.Local.GetTerminals(TerminalTypes.All).Select(x => new USBDeviceInfo(x, x)).ToList();
        }

        void USBInternalInterface.CloseUSBPort()
        {
            try
            {
                OutputTask?.Stop();
            }
            catch (Exception) { }
            try
            {
                OutputTask?.Dispose();
            }
            catch (Exception) { }
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            return;
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return true;
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo info)
        {
            return info.USBIdentification;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            throw new NotImplementedException();
        }

        bool USBInternalInterface.TestUSBAction()
        {
            ChannelInds = new List<int>().AsEnumerable();
            return true;
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            return;
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return new byte[0];
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            return;
        }
    }
}
