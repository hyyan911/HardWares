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
    internal delegate string ProductNameGenerater(string source);

    internal class VISAResourceTCPIPHelper : VISAResourceHelper
    {
        /// <summary>
        /// 获取ProductName
        /// </summary>
        private ProductNameGenerater GetProductName = null;

        public VISAResourceTCPIPHelper(ProductNameGenerater getProductName)
        {
            GetProductName = getProductName;
        }

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
                    res.LockResource(2000);
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
                                string res = VISAThreadUnsafeQuery(ss, "*IDN?\n", 200);
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
        /// 连接USB
        /// </summary>
        /// <param name="device"></param>
        /// <param name="usbName"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectTCPIP(PortObject device, string tcpipname, out Exception exc)
        {
            EnumerateTCPIPDevices(device as TCPIPInternalInterface);
            string res = FindResourceName(tcpipname);
            if (res == "")
            {
                exc = new Exception("未找到设备");
                return false;
            }
            return device.Connect(PortType.TCPIP, out exc, Encoding.ASCII, res);
        }
        #endregion
    }
}
