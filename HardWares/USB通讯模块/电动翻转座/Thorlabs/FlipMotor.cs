using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thorlabs.MotionControl.DeviceManagerCLI;
using Thorlabs.MotionControl.FilterFlipperCLI;
using Thorlabs.MotionControl.GenericMotorCLI;
using Thorlabs.MotionControl.GenericMotorCLI.ControlParameters;
using Thorlabs.MotionControl.GenericMotorCLI.AdvancedMotor;
using Thorlabs.MotionControl.GenericMotorCLI.Settings;

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
            (Instance as FilterFlipper).Disconnect(false);
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
        }

        private string SerialNO = "";
        object USBInternalInterface.CreateUSBInstance(List<object> param)
        {
            SerialNO = param[0] as string;
            return FilterFlipper.CreateFilterFlipper(param[0] as string);
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as FilterFlipper).IsConnected;
        }

        void USBInternalInterface.OpenUSBPort()
        {
            (Instance as FilterFlipper).Connect(SerialNO);
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
