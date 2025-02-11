using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.VISAHelper;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.射频源.Rigol_DSG_3060
{
    public partial class RFSource : TCPIPInternalInterface, TCPIPOuterInterface
    {
        VISAResourceTCPIPHelper TCPVISA { get; set; } = new VISAResourceTCPIPHelper(GetTCPIPProductName);

        /// <summary>
        /// 获取ProductName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string GetTCPIPProductName(string source)
        {
            try
            {
                if (source.Contains("Rigol Technologies"))
                {
                    string[] ss = source.Split(',');
                    if (ss[1].Contains("DSG3060"))
                    {
                        return "DSG3060 " + ss[2];
                    }
                }
                return "";
            }
            catch (Exception) { return ""; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="DeviceName"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectTCPIP(TCPIPDeviceInfo DeviceName, out Exception exc)
        {
            return TCPVISA.ConnectTCPIP(this, DeviceName, out exc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TCPIPDeviceInfo> GetTCPIPDeviceInfos()
        {
            return TCPVISA.EnumerateTCPIPDevices(this);
        }

        void TCPIPInternalInterface.CloseTCPIPPort()
        {
            TCPVISA.CloseTCPIPPort(this);
        }

        void TCPIPInternalInterface.ConnectedTCPIPAction()
        {
        }

        bool TCPIPInternalInterface.IsTCPIPOpen()
        {
            return TCPVISA.IsTCPIPOpen(this);
        }

        object TCPIPInternalInterface.OpenTCPIPPort(TCPIPDeviceInfo param)
        {
            return TCPVISA.OpenTCPIPPort(this, param);
        }

        void TCPIPInternalInterface.ReceiveTCPIPAct()
        {
            TCPVISA.ReceiveTCPIPAct(this);
        }

        void TCPIPInternalInterface.TCPIPInitAction(object PortInstance)
        {
            TCPVISA.TCPIPInitAction(this, PortInstance);
        }

        byte[] TCPIPInternalInterface.TCPIPPortRead()
        {
            return TCPVISA.TCPIPPortRead(this);
        }

        void TCPIPInternalInterface.TCPIPPortWrite(byte[] value)
        {
            TCPVISA.TCPIPPortWrite(this, value);
        }

        bool TCPIPInternalInterface.TestTCPIPAction()
        {
            return TCPVISA.TestTCPIPPort(this);
        }
    }
}
