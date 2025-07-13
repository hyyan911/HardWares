using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;

namespace HardWares.射频源.Rigol_DSG_3065B
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SignalGenerator : USBInternalInterface, WinUSBOuterInterface
    {

        VISAResourceUSBHelper USBVISA = new VISAResourceUSBHelper(GetUSBProductName, '\n');

        /// <summary>
        /// 获取ProductName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string GetUSBProductName(string source)
        {
            try
            {
                if (source.Contains("Rigol Technologies"))
                {
                    string[] ss = source.Split(',');
                    if (ss[1].Contains("DSG3065B"))
                    {
                        return "DSG3065B " + ss[2];
                    }
                }
                return "";
            }
            catch (Exception) { return ""; }
        }

        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return USBVISA.ConnectUSB(this, info, out exc, reconnect);
        }

        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            return USBVISA.EnumerateUSBDevices(this);
        }

        void USBInternalInterface.CloseUSBPort()
        {
            USBVISA.CloseUSBPort(this);
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            //添加射频和低频通道
            Channels.Clear();
            Channels.Add(new RFSignalChannel() { ParentDevice = this });
            Channels.Add(new LFSignalChannel() { ParentDevice = this });
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return USBVISA.IsUSBOpen(this);
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo param)
        {
            return USBVISA.OpenUSBPort(this, param);
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            USBVISA.ReceiveUSBAct(this);
        }

        bool USBInternalInterface.TestUSBAction()
        {
            return USBVISA.TestUSBPort(this);
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            USBVISA.USBInitAction(this, PortInstance);
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return USBVISA.USBPortRead(this);
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            USBVISA.USBPortWrite(this, value);
        }
    }
}
