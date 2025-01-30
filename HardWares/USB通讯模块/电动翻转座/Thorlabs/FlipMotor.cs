using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thorlabs.MotionControl.DeviceManagerCLI;
using Thorlabs.MotionControl.FilterFlipperCLI;

namespace HardWares.仪器列表.电动翻转座
{
    public partial class FlipMotor : FlipMotorBase, WinUSBOuterInterface, USBInternalInterface
    {
        public bool ConnectUSB(string usbName, out Exception exc)
        {
            return Connect(PortType.USB, out exc, Encoding.ASCII, false, usbName);
        }

        /// <summary>
        /// 获取设备名
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsbDeviceNames()
        {
            int s = 1;
            DeviceManagerCLI.BuildDeviceList();
            return DeviceManagerCLI.GetDeviceList(37);
        }

        void USBInternalInterface.CloseUSBPort()
        {
            (Instance as FilterFlipper).DisableDevice();
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
        }

        object USBInternalInterface.CreateUSBInstance(List<object> param)
        {
            return FilterFlipper.CreateFilterFlipper(param[0] as string);
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as FilterFlipper).IsConnected;
        }

        void USBInternalInterface.OpenUSBPort()
        {
            (Instance as FilterFlipper).Connect((Instance as FilterFlipper).SerialNo);
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            return;
        }

        bool USBInternalInterface.TestUSBAction()
        {
            if ((Instance as FilterFlipper).USBConnected == ThorlabsConnectionManager.ConnectionStates.Connected)
            {
                ProductName = "Thorlabs Filter Flipper " + (Instance as FilterFlipper).SerialNo;
                return true;
            }
            return false;
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            //复位
            (Instance as FilterFlipper).Home(3000);
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
