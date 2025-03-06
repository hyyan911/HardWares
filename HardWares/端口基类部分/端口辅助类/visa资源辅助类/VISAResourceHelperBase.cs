using HardWares.端口基类;
using NationalInstruments.Visa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.端口基类部分.PortHelper
{
    internal abstract class VISAResourceHelperBase : PortHelper
    {
        public Func<string, string> GetProductName { get; protected set; } = null;

        protected char Terminate = '\0';

        public static string ThreadUnsafeQuery(PortObject obj, string message, int timeout)
        {
            if (obj.Instance is UsbSession)
            {
                string str = VISAThreadUnsafeQuery(obj.Instance as UsbSession, message, timeout);
                int ind = str.LastIndexOf("\n");
                if (ind != -1)
                {
                    return str.Remove(ind, 1);
                }
            }
            if (obj.Instance is SerialSession)
            {
                string str = VISAThreadUnsafeQuery(obj.Instance as SerialSession, message, timeout);
                int ind = str.LastIndexOf("\n");
                if (ind != -1)
                {
                    return str.Remove(ind, 1);
                }
            }
            if (obj.Instance is TcpipSession)
            {
                string str = VISAThreadUnsafeQuery(obj.Instance as TcpipSession, message, timeout);
                int ind = str.LastIndexOf("\n");
                if (ind != -1)
                {
                    return str.Remove(ind, 1);
                }
            }
            return "";
        }

        protected static string VISAThreadUnsafeQuery(UsbSession ss, string v1, int v2)
        {
            try
            {
                ss.Clear();
                ss.TimeoutMilliseconds = v2;
                ss.RawIO.Write(v1);
                string res = ss.RawIO.ReadString();
                ss.Clear();
                return res;
            }
            catch (Exception ex) { return ""; }
        }

        protected static string VISAThreadUnsafeQuery(SerialSession ss, string v1, int v2)
        {
            try
            {
                ss.Clear();
                ss.TimeoutMilliseconds = v2;
                ss.RawIO.Write(v1);
                return ss.RawIO.ReadString();
                ss.Clear();
            }
            catch (Exception ex) { return ""; }
        }

        protected static string VISAThreadUnsafeQuery(TcpipSession ss, string v1, int v2)
        {
            try
            {
                ss.Clear();
                ss.TimeoutMilliseconds = v2;
                ss.RawIO.Write(v1);
                return ss.RawIO.ReadString();
                ss.Clear();
            }
            catch (Exception ex) { return ""; }
        }
    }
}
