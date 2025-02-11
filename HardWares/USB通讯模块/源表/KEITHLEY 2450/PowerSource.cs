using HardWares.数据处理;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using HardWares.端口基类部分.设备信息;
using HardWares.端口基类部分.PortHelper;

namespace HardWares.源表.KEITHLEY_2450
{
    public partial class PowerSource : PowerSourceBase, USBInternalInterface, WinUSBOuterInterface
    {

        private VISAResourceUSBHelper VISA = new VISAResourceUSBHelper(GetUSBProductName);

        private static string GetUSBProductName(string source)
        {
            if (source.Contains("KEITHLEY") && source.Contains("MODEL 2450"))
            {
                //获取序列号
                string strsegs = source.Split(',')[2];
                return "KEITHLEY 2450 " + strsegs;
            }
            return "";
        }

        void USBInternalInterface.CloseUSBPort()
        {
            VISA.CloseUSBPort(this);
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            VISA.USBInitAction(this, PortInstance);
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return VISA.USBPortRead(this);
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            VISA.USBPortWrite(this, value);
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return VISA.IsUSBOpen(this);
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo param)
        {
            return VISA.OpenUSBPort(this, param);
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            VISA.ReceiveUSBAct(this);
        }

        bool USBInternalInterface.TestUSBAction()
        {
            return VISA.TestUSBPort(this);
        }

        public bool ConnectUSB(USBDeviceInfo info, out Exception exc)
        {
            return VISA.ConnectUSB(this, info, out exc);
        }

        /// <summary>
        /// 获取USB设备
        /// </summary>
        /// <returns></returns>
        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            return VISA.EnumerateUSBDevices(this);
        }
    }
}
