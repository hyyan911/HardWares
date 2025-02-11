using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectTCPIP(TCPIPDeviceInfo info, out Exception exc)
        {
            return Connect(info, out exc);
        }

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <returns></returns>
        public List<TCPIPDeviceInfo> GetTCPIPDeviceInfos()
        {
            if (ZurichClient == null) ZurichClient = new ziDotNET();
            return ZurichClient.discoveryFindAll().Select(x => new TCPIPDeviceInfo(x, "", 0)).ToList();
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

        object TCPIPInternalInterface.OpenTCPIPPort(TCPIPDeviceInfo param)
        {
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
