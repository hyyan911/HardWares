using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thorlabs.MotionControl.DeviceManagerCLI;
using Thorlabs.MotionControl.FilterFlipperCLI;
using Thorlabs.MotionControl.FilterFlipperCLI.Native;
using HardWares.端口基类部分.设备信息;

namespace HardWares.仪器列表.电动翻转座
{
    public partial class FlipMotor : FlipMotorBase, WinUSBOuterInterface, USBInternalInterface
    {
        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect);
        }

        /// <summary>
        /// 获取设备名
        /// </summary>
        /// <returns></returns>
        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            DeviceManagerCLI.BuildDeviceList();
            return DeviceManagerCLI.GetDeviceList(37).Select(x => new USBDeviceInfo(x, x)).ToList();
        }

        void USBInternalInterface.CloseUSBPort()
        {
            try
            {
                (Instance as FilterFlipper).StopPolling();
                (Instance as FilterFlipper).Disconnect(true);
            }
            catch (Exception) { }
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            (Instance as FilterFlipper).WaitForSettingsInitialized(10000);
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as FilterFlipper).IsConnected;
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo param)
        {
            DeviceManagerCLI.BuildDeviceList();
            FilterFlipper dev = FilterFlipper.CreateFilterFlipper(param.USBIdentification);
            dev.Connect(param.USBIdentification);
            dev.EnableDevice();
            FilterFlipperIOSettings currentDeviceSettings = dev.GetIOSettings();
            currentDeviceSettings.TransitTime = 300;
            currentDeviceSettings.IO1PulseWidth = 200;
            currentDeviceSettings.IO2PulseWidth = 200;
            dev.SetIOSettings(currentDeviceSettings);
            dev.StartPolling(250);
            return dev;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            return;
        }

        bool USBInternalInterface.TestUSBAction()
        {
            if ((Instance as FilterFlipper).USBConnected == ThorlabsConnectionManager.ConnectionStates.Disconnected)
            {
                return false;
            }
            ProductName = "Thorlabs Filter Flipper " + (Instance as FilterFlipper).SerialNo;
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
            return;
        }
    }
}
