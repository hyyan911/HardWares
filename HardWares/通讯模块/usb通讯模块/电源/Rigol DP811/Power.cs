using HardWares.源表;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uc480.Defines;

namespace HardWares.电源.Rigol_DP811
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public partial class Power : PortObject, USBInternalInterface, WinUSBOuterInterface
    {
        VISAResourceUSBHelper VISAHelper = new VISAResourceUSBHelper(, '\n');

        private string GetName(string idnstring)
        {
            try
            {
                if (idnstring.Contains("Rigol Technologies"))
                {
                    string[] ss = idnstring.Split(',');
                    if (ss[1].Contains("DP811"))
                    {
                        return "DP811 " + ss[2];
                    }
                }
                return "";
            }
            catch (Exception) { return ""; }
        }

        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return VISAHelper.ConnectUSB(this, info, out exc, reconnect);
        }

        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            return VISAHelper.EnumerateUSBDevices(this);
        }

        void USBInternalInterface.CloseUSBPort()
        {
            throw new NotImplementedException();
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            throw new NotImplementedException();
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            throw new NotImplementedException();
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo info)
        {
            throw new NotImplementedException();
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            throw new NotImplementedException();
        }

        bool USBInternalInterface.TestUSBAction()
        {
            throw new NotImplementedException();
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            throw new NotImplementedException();
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            throw new NotImplementedException();
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            throw new NotImplementedException();
        }
    }
}
