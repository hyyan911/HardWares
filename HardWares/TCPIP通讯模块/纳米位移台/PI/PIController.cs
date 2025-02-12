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
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;

namespace HardWares.纳米位移台.PI
{
    /// <summary>
    /// PI位移台
    /// </summary>
    public partial class PIController : NanoControllerBase, TCPIPInternalInterface, TCPIPOuterInterface
    {

        private static string GetTCPIPProductName(string arg)
        {
            int ind = arg.IndexOf("--");
            if (ind != -1)
            {
                return arg.Substring(0, ind);
            }
            return arg;
        }


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

        object TCPIPInternalInterface.OpenTCPIPPort(TCPIPDeviceInfo info)
        {
            if (IsGCS2)
            {
                return PIAPI2.PI_ConnectTCPIP(info.IPAddress, info.Port);
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
            var retun = ProcessQueryResult(PIAPI2.GetAxisNames((int)Instance));
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

        public bool ConnectTCPIP(TCPIPDeviceInfo info, out Exception exc)
        {
            return Connect(info, out exc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<TCPIPDeviceInfo> GetTCPIPDeviceInfos()
        {
            List<string> names = PIAPI2.EnumerateTCPIP();

            names = names.Where((x) =>
            {
                foreach (var item in DevTypes)
                {
                    if (x.Contains(item)) return true;
                }
                return false;
            }).ToList();

            return names.Select(x =>
            {
                Regex reg = new Regex("[(].*[)]");
                var res = reg.Match(x);
                string address = res.Value.Replace("(", "").Replace(")", "");
                int ind = address.LastIndexOf(':');
                address = address.Substring(0, ind);
                return new TCPIPDeviceInfo(GetTCPIPProductName(x), address, 50000);
            }).ToList();
        }
    }
}
