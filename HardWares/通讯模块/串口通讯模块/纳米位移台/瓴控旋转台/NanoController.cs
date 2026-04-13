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
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
using HardWares.端口基类部分.PortHelper;

namespace HardWares.纳米位移台.瓴控旋转台
{
    public partial class NanoController : NanoControllerBase, COMInternalInterface, COMOuterInterface
    {
        public bool ConnectCOM(COMDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect);
        }

        public List<COMDeviceInfo> GetCOMDeviceInfos()
        {
            return COMHelper.ScanSerialCOMs(new Func<SerialPort, string>((ser) =>
            {
                var cmd = ProcessCommand(0x12, 1, new List<byte>()).ToArray();
                ser.Write(cmd, 0, cmd.Length);
                Thread.Sleep(200);
                byte[] res = new byte[ser.BytesToRead];
                ser.Read(res, 0, res.Length);
                bool result = CheckResult(res.ToList(), out byte cmdnum, out var data);
                if (result == true)
                {
                    string driver = Encoding.ASCII.GetString(data.GetRange(0, 20).ToArray()).Replace("\0", "");
                    string name = Encoding.ASCII.GetString(data.GetRange(20, 20).ToArray()).Replace("\0", "");
                    return "LingKong MS " + driver + " " + name;
                }
                return "";
            }));
        }

        void COMInternalInterface.CloseCOMPort()
        {
            (Instance as SerialPort).Close();
        }

        void COMInternalInterface.COMInitAction(object PortInstance)
        {
        }

        byte[] COMInternalInterface.COMPortRead()
        {
            int count = (Instance as SerialPort).BytesToRead;
            byte[] result = new byte[count];
            (Instance as SerialPort).Read(result, 0, count);
            return result;
        }

        void COMInternalInterface.COMPortWrite(byte[] value)
        {
            (Instance as SerialPort).Write(value, 0, value.Length);
        }

        void COMInternalInterface.ConnectedCOMAction()
        {
        }

        bool COMInternalInterface.IsCOMOpen()
        {
            return (Instance as SerialPort).IsOpen;
        }

        object COMInternalInterface.OpenCOMPort(COMDeviceInfo info)
        {
            SerialPort port = new SerialPort(info.COMName, info.BaudRate);
            port.Open();
            return port;
        }

        List<byte> QueryReturnedByte = null;

        void COMInternalInterface.ReceiveCOMAct()
        {
            var result = CheckResult(ReceiveBuffer, out byte command, out var data);
            if (result == true)
            {
                QueryState = true;
                //只取第一条指令
                QueryReturnedByte = data;
                ReceiveBuffer.Clear();
            }
        }

        bool COMInternalInterface.TestCOMAction()
        {
            var result = ProcessResult(ThreadSafeQuery(ProcessCommand(0x12, 1, new List<byte>()), 300));
            if (result.Count != 0)
            {
                string driver = Encoding.ASCII.GetString(result.GetRange(0, 20).ToArray()).Replace("\0", "");
                string name = Encoding.ASCII.GetString(result.GetRange(20, 20).ToArray()).Replace("\0", "");
                ProductName = "LingKong MS " + driver + " " + name;
                NanoStage stage = new NanoStage("0", this);
                stage.target = stage.Position;
                Channels.Add(stage);
                return true;
            }
            return false;
        }
    }
}
