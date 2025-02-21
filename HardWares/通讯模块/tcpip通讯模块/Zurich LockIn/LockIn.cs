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
        private string DevID { get; set; } = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectTCPIP(TCPIPDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect, false);
        }

        /// <summary>
        /// 获取设备名称
        /// </summary>
        /// <returns></returns>
        public List<TCPIPDeviceInfo> GetTCPIPDeviceInfos()
        {
            var ZurichClient = new ziDotNET();
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
            var ZurichClient = new ziDotNET();
            DevID = ZurichClient.discoveryFind(param.DeviceName);
            string host = ZurichClient.discoveryGetValueS(param.DeviceName, "serveraddress");
            long port = ZurichClient.discoveryGetValueI(param.DeviceName, "serverport");
            long api = ZurichClient.discoveryGetValueI(param.DeviceName, "apilevel");
            ZurichClient.init(host, Convert.ToUInt16(port), (ZIAPIVersion_enum)api);
            return ZurichClient;
        }

        void TCPIPInternalInterface.ReceiveTCPIPAct()
        {
            return;
        }

        void TCPIPInternalInterface.TCPIPInitAction(object PortInstance)
        {
        }

        byte[] TCPIPInternalInterface.TCPIPPortRead()
        {
            return new byte[0];
        }

        void TCPIPInternalInterface.TCPIPPortWrite(byte[] value)
        {
            return;
        }

        bool TCPIPInternalInterface.TestTCPIPAction()
        {
            return true;
        }
    }
}
