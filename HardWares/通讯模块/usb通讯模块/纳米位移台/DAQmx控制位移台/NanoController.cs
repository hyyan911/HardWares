using HardWares.数据处理;
using HardWares.纳米位移台;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using HardWares.APIS.PI;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.IO.Ports;
using System.Windows;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
using NationalInstruments.DAQmx;
using System.IO;
using CodeHelper;

namespace HardWares.纳米位移台.DAQmxController
{
    /// <summary>
    /// DAQmx控制器
    /// </summary>
    public partial class NanoController : NanoControllerBase, USBInternalInterface, WinUSBOuterInterface
    {
        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            return DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.All).Select(x => new USBDeviceInfo("DAQmx控制器" + x, x)).ToList();
        }

        void USBInternalInterface.CloseUSBPort()
        {
            foreach (var item in Stages)
            {
                (item as NanoStage).DisposeTask();
            }
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
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
        }

        bool USBInternalInterface.TestUSBAction()
        {
            var stage = new NanoStage("DAQmx位移台控制轴" + Instance as string, this);
            if (!File.Exists(stage.LocationLogSavePath))
                stage.target = 0;
            else
            {
                FileObject obj = FileObject.ReadFromFile(stage.LocationLogSavePath);
                stage.target = double.Parse(obj.Descriptions.First().Value);
            }
            Stages.Add(stage);
            return true;
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return new byte[0];
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
        }
    }
}
