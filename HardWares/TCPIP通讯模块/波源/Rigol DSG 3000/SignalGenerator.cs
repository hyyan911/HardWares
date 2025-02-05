using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.波源.Rigol_DSG_3000
{
    internal partial class RFSignalGenerator : TCPIPInternalInterface, TCPIPOuterInterface
    {
        public bool ConnectTCPIP(string DeviceName, int baudrate, out Exception exc)
        {
            string res = VISA.FindTCPIPName(DeviceName);
            if (res == "")
            {
                exc = new Exception("未找到设备");
                return false;
            }
            return Connect(PortType.TCPIP, out exc, Encoding.ASCII, DeviceName);
        }

        void TCPIPInternalInterface.CloseTCPIPPort()
        {
            VISA.CloseTCPIPPort(this);
        }

        void TCPIPInternalInterface.ConnectedTCPIPAction()
        {
        }

        bool TCPIPInternalInterface.IsTCPIPOpen()
        {
            return VISA.IsTCPIPOpen(this);
        }

        object TCPIPInternalInterface.OpenTCPIPPort(List<object> param)
        {
            return VISA.OpenTCPIPPort(this, param);
        }

        void TCPIPInternalInterface.ReceiveTCPIPAct()
        {
            VISA.ReceiveTCPIPAct(this);
        }

        void TCPIPInternalInterface.TCPIPInitAction(object PortInstance)
        {
            VISA.TCPIPInitAction(this, PortInstance);
        }

        byte[] TCPIPInternalInterface.TCPIPPortRead()
        {
            return VISA.TCPIPPortRead(this);
        }

        void TCPIPInternalInterface.TCPIPPortWrite(byte[] value)
        {
            VISA.TCPIPPortWrite(this, value);
        }

        bool TCPIPInternalInterface.TestTCPIPAction()
        {
            return VISA.TestTCPIPPort(this);
        }
    }
}
