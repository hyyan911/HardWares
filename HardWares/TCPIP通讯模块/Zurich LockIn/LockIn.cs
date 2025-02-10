using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using zhinst;

namespace HardWares.Lock_In.Zurich_LockIn
{
    public partial class LockIn : LockInBase, TCPIPInternalInterface, TCPIPOuterInterface
    {
        private static ziDotNET ZurichClient = null;

        public bool ConnectTCPIP(string TCPIPName, out Exception exc)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <returns></returns>
        public List<string> GetTCPIPDeviceNames()
        {
            if (ZurichClient == null) ZurichClient = new ziDotNET();
            return ZurichClient.discoveryFindAll();
        }

        void TCPIPInternalInterface.CloseTCPIPPort()
        {
            (Instance as ziDotNET).disconnect();
        }

        void TCPIPInternalInterface.ConnectedTCPIPAction()
        {
        }

        bool TCPIPInternalInterface.IsTCPIPOpen()
        {
            return false;
        }

        object TCPIPInternalInterface.OpenTCPIPPort(List<object> param)
        {
            string name = param[0] as string;
            ziDotNET core = new ziDotNET();
            core.connect();
            return core;
        }

        void TCPIPInternalInterface.ReceiveTCPIPAct()
        {
            throw new NotImplementedException();
        }

        void TCPIPInternalInterface.TCPIPInitAction(object PortInstance)
        {
            throw new NotImplementedException();
        }

        byte[] TCPIPInternalInterface.TCPIPPortRead()
        {
            throw new NotImplementedException();
        }

        void TCPIPInternalInterface.TCPIPPortWrite(byte[] value)
        {
            throw new NotImplementedException();
        }

        bool TCPIPInternalInterface.TestTCPIPAction()
        {
            throw new NotImplementedException();
        }
    }
}
