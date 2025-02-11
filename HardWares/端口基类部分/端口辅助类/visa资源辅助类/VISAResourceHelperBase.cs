using HardWares.端口基类;
using NationalInstruments.Visa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类部分.VISAHelper
{
    internal abstract class VISAResourceHelperBase
    {
        public Func<string, string> GetProductName { get; protected set; } = null;

        public static string ThreadUnsafeQuery(PortObject obj, string message, int timeout)
        {
            if (obj.Instance is UsbSession)
            {
                return VISAThreadUnsafeQuery(obj.Instance as UsbSession, message, timeout);
            }
            if (obj.Instance is SerialSession)
            {
                return VISAThreadUnsafeQuery(obj.Instance as SerialSession, message, timeout);
            }
            if (obj.Instance is TcpipSession)
            {
                return VISAThreadUnsafeQuery(obj.Instance as TcpipSession, message, timeout);
            }
            return "";
        }

        protected static string VISAThreadUnsafeQuery(UsbSession ss, string v1, int v2)
        {
            try
            {
                ss.TimeoutMilliseconds = v2;
                ss.RawIO.Write(v1);
                return ss.RawIO.ReadString();
            }
            catch (Exception ex) { return ""; }
        }

        protected static string VISAThreadUnsafeQuery(SerialSession ss, string v1, int v2)
        {
            try
            {
                ss.TimeoutMilliseconds = v2;
                ss.RawIO.Write(v1);
                return ss.RawIO.ReadString();
            }
            catch (Exception ex) { return ""; }
        }

        protected static string VISAThreadUnsafeQuery(TcpipSession ss, string v1, int v2)
        {
            try
            {
                ss.TimeoutMilliseconds = v2;
                ss.RawIO.Write(v1);
                return ss.RawIO.ReadString();
            }
            catch (Exception ex) { return ""; }
        }
    }
}
