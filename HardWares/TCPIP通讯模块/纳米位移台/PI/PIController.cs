using HardWares.数据处理;
using HardWares.纳米位移台;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using HardWares.APIS.PI;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.IO.Ports;
using System.Windows;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类.TCPIP串口;
using System.Text.RegularExpressions;

namespace HardWares.纳米位移台.PI
{
    /// <summary>
    /// PI位移台
    /// </summary>
    public partial class PIController : NanoControllerBase, TCPIPInternalInterface, TCPIPOuterInterface
    {

        bool TCPIPInternalInterface.TestTCPIPAction()
        {
            string s = PIAPI2.QIDN((int)Instance);
            List<string> result = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("*IDN?", "", ""), 500));
            if (result.Count == 0) return false;
            if (result[0].Contains("Physik Instrumente"))
            {
                return true;
            }
            return false;
        }

        void TCPIPInternalInterface.TCPIPPortWrite(byte[] value)
        {
            if (IsGCS2)
            {
                PIAPI2.PI_GcsCommandset((int)Instance, value);
            }
            else
            {
                return;
            }
        }

        byte[] TCPIPInternalInterface.TCPIPPortRead()
        {
            if (IsGCS2)
            {
                return PIAPI2.GetAnswer((int)Instance, out bool state).ToArray();
            }
            else
            {
                return new byte[0];
            }
        }

        void TCPIPInternalInterface.CloseTCPIPPort()
        {
            if (Instance == null) return;
            if ((int)Instance == -1) return;
            if (IsGCS2)
            {
                PIAPI2.PI_CloseConnection((int)Instance);
            }
            else
            {
                return;
            }
        }

        object TCPIPInternalInterface.OpenTCPIPPort(List<object> param)
        {
            if (IsGCS2)
            {
                string str = param[0] as string;
                //Regex reg = new Regex("[(].*[)]");
                //var res = reg.Match(str);
                //int id = PIAPI2.PI_ConnectTCPIP(res.Value.Replace("(", "").Replace(")", "").Replace(":50000", ""), 50000);
                Regex reg = new Regex("[S][N][ ]*[0-9]*[ ]?");
                var res = reg.Match(str);
                int id = PIAPI2.PI_ConnectTCPIPByDescription(str);
                ProductName = param[0] as string;
                return id;
            }
            return -1;
        }

        bool TCPIPInternalInterface.IsTCPIPOpen()
        {
            return PIAPI2.PI_IsConnected((int)Instance);
        }

        void TCPIPInternalInterface.ConnectedTCPIPAction()
        {
            //获取函数列表
            CommandSet = GetSupportedCommands();
            //判断是否是GCS2版本
            IsGCS2 = !IsGCS3();
            //获取轴列表
            var retun = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("SAI?", "", ""), 1000));
            Stages.Clear();
            foreach (var item in retun)
            {
                //添加位移台
                Stages.Add(new PIStage(item.Trim(), this));
            }
        }

        void TCPIPInternalInterface.ReceiveTCPIPAct()
        {
            if (ReceiveBuffer.Count == 0) return;
            List<List<byte>> retu = DataProcess.ProcessReceivedSerialData(LF, ReceiveBuffer, out List<byte> result);
            ReceiveBuffer = result;
            if (retu.Count != 0)
            {
                QueryState = true;
                QueryReturnedData = new List<string>();
                foreach (var item in retu)
                {
                    QueryReturnedData.Add(Encoding.ASCII.GetString(item.ToArray()));
                }
            }
        }

        void TCPIPInternalInterface.TCPIPInitAction(object PortInstance)
        {
        }

        public bool ConnectTCPIP(string TCPIPName, out Exception exc)
        {
            return Connect(PortType.TCPIP, out exc, Encoding.ASCII, TCPIPName);
        }

        public List<string> GetTCPIPDeviceNames()
        {
            return PIAPI2.EnumerateTCPIP();
        }
    }
}
