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
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;

namespace HardWares.端口基类部分.PortHelper
{
    internal class VISAResourceTCPIPHelper : VISAResourceHelperBase
    {
        public VISAResourceTCPIPHelper(Func<string, string> getProductName)
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

        public object OpenTCPIPPort(TCPIPInternalInterface device, TCPIPDeviceInfo info)
        {
            using (ResourceManager m = new ResourceManager())
            {
                TcpipSession res = (TcpipSession)m.Open(info.IPAddress, AccessModes.None, 1000, out ResourceOpenStatus stat);
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
        public List<TCPIPDeviceInfo> EnumerateTCPIPDevices(TCPIPInternalInterface device)
        {
            using (ResourceManager m = new ResourceManager())
            {
                try
                {
                    List<string> strs = m.Find("TCPIP?*").ToList();
                    List<TCPIPDeviceInfo> result = new List<TCPIPDeviceInfo>();
                    foreach (var item in strs)
                    {
                        try
                        {
                            if (EndJudgeFunc != null)
                            {
                                if (EndJudgeFunc.Invoke() == true) return result;
                            }
                            using (var ss = m.Open(item) as TcpipSession)
                            {
                                string res = VISAThreadUnsafeQuery(ss, "*IDN?\n", 200);
                                if (res == "") continue;
                                if (GetProductName == null)
                                {
                                    result.Add(new TCPIPDeviceInfo(res, item, 0));
                                    continue;
                                }
                                string product = GetProductName?.Invoke(res);
                                if (product == "") continue;
                                result.Add(new TCPIPDeviceInfo(product, item, 0));
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
                    return new List<TCPIPDeviceInfo>();
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
        public bool ConnectTCPIP(PortObject device, TCPIPDeviceInfo info, out Exception exc)
        {
            return device.Connect(info, out exc);
        }
        #endregion
    }
}
