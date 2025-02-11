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

namespace HardWares.端口基类部分.VISAHelper
{
    internal class VISAResourceCOMHelper : VISAResourceHelperBase
    {
        public VISAResourceCOMHelper(Func<string, string> getProductName)
        {
            GetProductName = getProductName;
        }

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
                    res.LockResource(2000);
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

        /// <summary>
        /// 获取USB设备
        /// </summary>
        public List<COMDeviceInfo> EnumerateCOMDevices(COMInternalInterface device)
        {
            using (ResourceManager m = new ResourceManager())
            {
                try
                {
                    List<string> strs = m.Find("ASRL?*INSTR").ToList();
                    List<COMDeviceInfo> result = new List<COMDeviceInfo>();
                    foreach (var item in strs)
                    {
                        using (var ss = m.Open(item) as SerialSession)
                        {
                            try
                            {
                                string res = VISAThreadUnsafeQuery(ss, "*IDN?\n", 200);
                                if (res == "") continue;
                                if (GetProductName == null)
                                {
                                    result.Add(new COMDeviceInfo(res, item, 9600));
                                    continue;
                                }
                                string product = GetProductName?.Invoke(res);
                                if (product == "") continue;
                                result.Add(new COMDeviceInfo(product, item, 9600));
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
                    return new List<COMDeviceInfo>();
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
        public bool ConnectCOM(PortObject device, COMDeviceInfo info, out Exception exc)
        {
            return device.Connect(info, out exc);
        }
        #endregion
    }
}
