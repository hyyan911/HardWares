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
    internal abstract class VISAResourceHelper
    {
        protected List<KeyValuePair<string, string>> DevNames = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// 根据ProductName寻找USB名称
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        protected string FindResourceName(string identifier)
        {
            foreach (var item in DevNames)
            {
                if (item.Key == identifier || item.Value == identifier) return item.Value;
            }
            return "";
        }

        public static string VISAThreadUnsafeQuery(PortObject device, string messagetosend, int timeout)
        {
            if (device.Instance is UsbSession)
            {
                return VISAUSBThreadUnsafeQuery(device.Instance as UsbSession, messagetosend, timeout);
            }
            if (device.Instance is SerialSession)
            {
                return VISACOMThreadUnsafeQuery(device.Instance as SerialSession, messagetosend, timeout);
            }
            if (device.Instance is TcpipSession)
            {
                return VISATCPIPThreadUnsafeQuery(device.Instance as TcpipSession, messagetosend, timeout);
            }
            return "";
        }

        public static string VISAThreadUnsafeQuery(Session session, string messagetosend, int timeout)
        {
            if (session is UsbSession)
            {
                return VISAUSBThreadUnsafeQuery(session as UsbSession, messagetosend, timeout);
            }
            if (session is SerialSession)
            {
                return VISACOMThreadUnsafeQuery(session as SerialSession, messagetosend, timeout);
            }
            if (session is TcpipSession)
            {
                return VISATCPIPThreadUnsafeQuery(session as TcpipSession, messagetosend, timeout);
            }
            return "";
        }

        private static string VISAUSBThreadUnsafeQuery(UsbSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }

        private static string VISACOMThreadUnsafeQuery(SerialSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }

        private static string VISATCPIPThreadUnsafeQuery(TcpipSession device, string messagetosend, int timeout)
        {
            try
            {
                device.TimeoutMilliseconds = timeout;
                device.RawIO.Write(messagetosend);
                return (device.RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }
    }
}
