using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.波源.Rigol_DSG_3060
{
    public partial class RFSignalGenerator : TCPIPInternalInterface, TCPIPOuterInterface
    {
        VISAResourceTCPIPHelper TCPVISA { get; set; } = new VISAResourceTCPIPHelper(GetProductName);

        public bool ConnectTCPIP(string DeviceName, out Exception exc)
        {
            return TCPVISA.ConnectTCPIP(this, DeviceName, out exc);
        }

        public List<string> GetTCPIPDeviceNames()
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

        List<string> TCPIPInternalInterface.GetRawTCPIPDeviceNames()
        {
            return new List<string>();
        }

        bool TCPIPInternalInterface.IsTCPIPOpen()
        {
            return TCPVISA.IsTCPIPOpen(this);
        }

        object TCPIPInternalInterface.OpenTCPIPPort(List<object> param)
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
