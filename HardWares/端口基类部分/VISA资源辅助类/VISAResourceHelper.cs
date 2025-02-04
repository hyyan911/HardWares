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

namespace HardWares.端口基类部分
{
    internal class VISAResourceHelper
    {
        internal delegate string ProductNameGenerater(string source);

        /// <summary>
        /// 获取ProductName
        /// </summary>
        public ProductNameGenerater GetProductName = null;

        #region COM部分

        public void COMPortWrite(TCPIPInternalInterface device, byte[] value)
        {
            ((device as PortObject).Instance as SerialSession).RawIO.Write(value);
        }

        public byte[] COMPortRead(TCPIPInternalInterface device)
        {
            return new byte[0];
        }

        public void CloseCOMPort(TCPIPInternalInterface device)
        {
            if ((device as PortObject).Instance == null) return;
            SerialSession session = (device as PortObject).Instance as SerialSession;

            session.Clear();
            session.UnlockResource();
            session.Dispose();
        }


        public object OpenCOMPort(TCPIPInternalInterface device, List<object> param)
        {
            using (ResourceManager m = new ResourceManager())
            {
                SerialSession res = (SerialSession)m.Open(param[0] as string, AccessModes.None, 1000, out ResourceOpenStatus stat);
                res.TerminationCharacterEnabled = false;
                string name = param[0] as string;
                try
                {
                    ((device as PortObject).Instance as SerialSession).LockResource(2000);
                }
                catch (Exception ex) { }
                return res;
            }
        }

        public bool IsCOMOpen(TCPIPInternalInterface device)
        {
            return true;
        }

        public void ReceiveCOMAct(TCPIPInternalInterface device)
        {
            return;
        }

        public bool TestCOMPort(TCPIPInternalInterface device)
        {
            PortObject port = device as PortObject;
            string res = port.ThreadSafeQuery("*IDN?\n", 1000);
            if (res == "") return false;
            port.ProductName = GetProductName?.Invoke(res);
            if (port.ProductName == "") return false;
            return true;
        }

        public void COMInitAction(TCPIPInternalInterface device, object PortInstance)
        {
            try
            {
                ((device as PortObject).Instance as SerialSession).Clear();
            }
            catch (Exception ex) { }
        }
        #endregion

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

        public object OpenUSBPort(USBInternalInterface device, List<object> param)
        {
            using (ResourceManager m = new ResourceManager())
            {
                UsbSession res = (UsbSession)m.Open(param[0] as string, AccessModes.None, 1000, out ResourceOpenStatus stat);
                res.TerminationCharacterEnabled = false;
                try
                {
                    ((device as PortObject).Instance as UsbSession).LockResource(2000);
                }
                catch (Exception ex) { }
                return res;
            }
        }

        public void CloseUSBPort(USBInternalInterface device)
        {
            if ((device as PortObject).Instance == null) return;
            UsbSession session = (device as PortObject).Instance as UsbSession;

            session.Clear();
            session.UnlockResource();
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


        List<KeyValuePair<string, string>> DevNames = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// 获取USB设备
        /// </summary>
        public List<string> EnumerateUSBDevices(USBInternalInterface device)
        {
            using (ResourceManager m = new ResourceManager())
            {
                DevNames.Clear();
                try
                {
                    List<string> strs = m.Find("USB?*").ToList();
                    List<string> result = new List<string>();
                    foreach (var item in strs)
                    {
                        using (var ss = m.Open(item) as UsbSession)
                        {
                            try
                            {
                                string res = VISAUSBThreadUnsafeQuery(ss, "*IDN?\n", 200);
                                if (res == "") continue;
                                if (GetProductName == null) result.Add(res);
                                string product = GetProductName?.Invoke(res);
                                if (product == "") continue;
                                result.Add(product);
                                DevNames.Add(new KeyValuePair<string, string>(product, item));
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }

        /// <summary>
        /// 根据ProductName寻找USB名称
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string FindUSBName(string identifier)
        {
            foreach (var item in DevNames)
            {
                if (item.Key == identifier || item.Value == identifier) return item.Value;
            }
            return "";
        }
        #endregion

        #region TCPIP部分

        public void ReceiveTCPIPAct(TCPIPInternalInterface device)
        {
            return;
        }

        public byte[] TCPIPPortRead(TCPIPInternalInterface device)
        {
            return new byte[0];
        }

        public bool IsTCPIPOpen(TCPIPInternalInterface device)
        {
            return true;
        }

        public object OpenTCPIPPort(TCPIPInternalInterface device, List<object> param)
        {
            using (ResourceManager m = new ResourceManager())
            {
                TcpipSession res = (TcpipSession)m.Open(param[0] as string, AccessModes.None, 1000, out ResourceOpenStatus stat);
                res.TerminationCharacterEnabled = false;
                try
                {
                    ((device as PortObject).Instance as TcpipSession).LockResource(2000);
                }
                catch (Exception ex) { }
                return res;
            }
        }

        public void CloseTCPIPPort(TCPIPInternalInterface device)
        {
            if ((device as PortObject).Instance == null) return;
            TcpipSession session = (device as PortObject).Instance as TcpipSession;

            session.Clear();
            session.UnlockResource();
            session.Dispose();
        }

        public bool TestTCPIPPort(TCPIPInternalInterface device)
        {
            PortObject port = device as PortObject;
            string res = port.ThreadSafeQuery("*IDN?\n", 1000);
            if (res == "") return false;
            port.ProductName = GetProductName?.Invoke(res);
            if (port.ProductName == "") return false;
            return true;
        }

        public void TCPIPPortWrite(TCPIPInternalInterface device, byte[] value)
        {
            ((device as PortObject).Instance as TcpipSession).RawIO.Write(value);
        }

        public void TCPIPInitAction(TCPIPInternalInterface device, object PortInstance)
        {
            try
            {
                ((device as PortObject).Instance as TcpipSession).Clear();
            }
            catch (Exception ex) { }
        }
        /// <summary>
        /// 获取USB设备
        /// </summary>
        public List<string> EnumerateTCPIPDevices(TCPIPInternalInterface device)
        {
            using (ResourceManager m = new ResourceManager())
            {
                DevNames.Clear();
                try
                {
                    List<string> strs = m.Find("TCPIP?*").ToList();
                    List<string> result = new List<string>();
                    foreach (var item in strs)
                    {
                        using (var ss = m.Open(item) as TcpipSession)
                        {
                            try
                            {
                                string res = VISATCPIPThreadUnsafeQuery(ss, "*IDN?\n", 200);
                                if (res == "") continue;
                                if (GetProductName == null) result.Add(res);
                                string product = GetProductName?.Invoke(res);
                                if (product == "") continue;
                                result.Add(product);
                                DevNames.Add(new KeyValuePair<string, string>(product, item));
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }

        /// <summary>
        /// 根据ProductName寻找USB名称
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string FindTCPIPName(string identifier)
        {
            foreach (var item in DevNames)
            {
                if (item.Key == identifier || item.Value == identifier) return item.Value;
            }
            return "";
        }
        #endregion

        #region 读写操作
        public string VISAThreadUnsafeQuery(PortObject device, string messagetosend, int timeout)
        {
            if (device.Instance is UsbSession)
            {
                VISAUSBThreadUnsafeQuery(device.Instance as UsbSession, messagetosend, timeout);
            }
            if (device.Instance is SerialSession)
            {
                VISACOMThreadUnsafeQuery(device.Instance as SerialSession, messagetosend, timeout);
            }
            if (device.Instance is GpibSession)
            {
                VISAGPIBThreadUnsafeQuery(device.Instance as GpibSession, messagetosend, timeout);
            }
            if (device.Instance is TcpipSession)
            {
                VISATCPIPThreadUnsafeQuery(device.Instance as TcpipSession, messagetosend, timeout);
            }
            return "";
        }

        private string VISAUSBThreadUnsafeQuery(UsbSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }

        private string VISACOMThreadUnsafeQuery(SerialSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }

        private string VISAGPIBThreadUnsafeQuery(GpibSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }

        private string VISATCPIPThreadUnsafeQuery(TcpipSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }
        #endregion
    }
}
