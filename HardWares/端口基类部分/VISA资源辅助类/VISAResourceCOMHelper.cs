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
    internal class VISAResourceCOMHelper : VISAResourceHelper
    {
        /// <summary>
        /// 获取ProductName
        /// </summary>
        private ProductNameGenerater GetProductName = null;

        public VISAResourceCOMHelper(ProductNameGenerater getProductName)
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
        public List<string> EnumerateCOMDevices(COMInternalInterface device)
        {
            using (ResourceManager m = new ResourceManager())
            {
                DevNames.Clear();
                try
                {
                    List<string> strs = m.Find("ASRL?*INSTR").ToList();
                    List<string> result = new List<string>();
                    foreach (var item in strs)
                    {
                        using (var ss = m.Open(item) as SerialSession)
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
        public bool ConnectCOM(PortObject device, string comname, int baudrate, out Exception exc)
        {
            EnumerateCOMDevices(device as COMInternalInterface);
            string res = FindResourceName(comname);
            if (res == "")
            {
                exc = new Exception("未找到设备");
                return false;
            }
            return device.Connect(PortType.COM, out exc, Encoding.ASCII, res, baudrate);
        }
        #endregion
    }
}
