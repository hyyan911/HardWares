using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Visa;
using Ivi.Visa;
using System.Xml.Linq;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;

namespace HardWares.端口基类部分.PortHelper
{
    internal class VISAResourceUSBHelper : VISAResourceHelperBase
    {
        public VISAResourceUSBHelper(Func<string, string> getProductName, char TerminateCharacter)
        {
            GetProductName = getProductName;
            Terminate = TerminateCharacter;
        }

        #region USB部分

        public void ReceiveUSBAct(USBInternalInterface device)
        {
            return;
        }

        public byte[] USBPortRead(USBInternalInterface device)
        {
            return new byte[0];
        }

        public bool IsUSBOpen(USBInternalInterface device)
        {
            return true;
        }

        public object OpenUSBPort(USBInternalInterface device, USBDeviceInfo param)
        {
            using (ResourceManager m = new ResourceManager())
            {
                UsbSession res = (UsbSession)m.Open(param.USBIdentification, AccessModes.None, 1000, out ResourceOpenStatus stat);
                if (Terminate == '\0')
                    res.TerminationCharacterEnabled = false;
                else
                {
                    res.TerminationCharacter = (byte)Terminate;
                    res.TerminationCharacterEnabled = true;
                }
                return res;
            }
        }

        public void CloseUSBPort(USBInternalInterface device)
        {
            if ((device as PortObject).Instance == null) return;
            UsbSession session = (device as PortObject).Instance as UsbSession;

            session.Clear();
            session.Dispose();
        }

        public bool TestUSBPort(USBInternalInterface device)
        {
            PortObject port = device as PortObject;
            string res = port.ThreadSafeQuery("*IDN?\n", 1000);
            if (res == "") return false;
            port.ProductName = GetProductName?.Invoke(res);
            if (port.ProductName == "") return false;
            return true;
        }

        public void USBPortWrite(USBInternalInterface device, byte[] value)
        {
            ((device as PortObject).Instance as UsbSession).RawIO.Write(value);
        }

        public void USBInitAction(USBInternalInterface device, object PortInstance)
        {
            try
            {
                ((device as PortObject).Instance as UsbSession).Clear();
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 获取USB设备
        /// </summary>
        public List<USBDeviceInfo> EnumerateUSBDevices(USBInternalInterface device)
        {
            using (ResourceManager m = new ResourceManager())
            {
                try
                {
                    List<string> strs = m.Find("USB?*").ToList();
                    List<USBDeviceInfo> result = new List<USBDeviceInfo>();
                    foreach (var item in strs)
                    {
                        try
                        {
                            if (EndJudgeFunc != null)
                            {
                                if (EndJudgeFunc.Invoke() == true) return result;
                            }
                            using (var ss = m.Open(item) as UsbSession)
                            {
                                string res = VISAThreadUnsafeQuery(ss, "*IDN?\n", 500);
                                if (res == "") continue;
                                if (GetProductName == null)
                                {
                                    result.Add(new USBDeviceInfo(res, item));
                                    continue;
                                }
                                string product = GetProductName?.Invoke(res);
                                if (product == "") continue;
                                result.Add(new USBDeviceInfo(product, item));
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return new List<USBDeviceInfo>();
                }
            }

        }

        /// <summary>
        /// 连接USB
        /// </summary>
        /// <param name="device"></param>
        /// <param name="usbName"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectUSB(PortObject device, USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return device.Connect(info, out exc, reconnect);
        }
        #endregion
    }
}
