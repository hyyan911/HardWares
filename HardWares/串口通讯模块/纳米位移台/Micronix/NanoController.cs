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
using System.Runtime.ConstrainedExecution;

namespace HardWares.纳米位移台.Micronix
{
    public partial class NanoController : NanoControllerBase, COMInternalInterface, COMOuterInterface
    {
        COMHelper COMHelper = new COMHelper('\n', Encoding.ASCII);
        public bool ConnectCOM(COMDeviceInfo info, out Exception exc)
        {
            return COMHelper.ConnectCOM(this, info, out exc);
        }

        public List<COMDeviceInfo> GetCOMDeviceInfos()
        {
            return COMHelper.ScanSerialCOMs(new Func<SerialPort, string>((ser) =>
            {
                ser.Write("1VER?\n\r");
                Thread.Sleep(50);
                ser.Write("2VER?\n\r");
                Thread.Sleep(50);
                ser.Write("3VER?\n\r");
                Thread.Sleep(50);
                ser.Write("4VER?\n\r");
                Thread.Sleep(50);
                ser.Write("5VER?\n\r");
                Thread.Sleep(50);
                ser.Write("6VER?\n\r");
                Thread.Sleep(50);
                ser.Write("7VER?\n\r");
                Thread.Sleep(50);
                ser.Write("8VER?\n\r");
                Thread.Sleep(400);
                string result = ser.ReadExisting();
                if (DevNames.Where(x => result.Contains(x)).Count() != 0)
                {
                    return "Micronix" + ser.PortName.Replace("COM", "");
                }
                return "";
            }));
        }

        void COMInternalInterface.CloseCOMPort()
        {
            COMHelper.CloseCOMPort(this);
        }

        void COMInternalInterface.COMInitAction(object PortInstance)
        {
        }

        byte[] COMInternalInterface.COMPortRead()
        {
            return COMHelper.COMPortRead(this);
        }

        void COMInternalInterface.COMPortWrite(byte[] value)
        {
            COMHelper.COMPortWrite(this, value);
        }

        void COMInternalInterface.ConnectedCOMAction()
        {
            for (int i = 1; i <= 8; i++)
            {
                string result = ThreadSafeQuery(i.ToString() + "VER?\n\r", 500);
                if (result.Trim() != "")
                {
                    NanoStage stage = new NanoStage(result + "_" + i.ToString(), this);
                    stage.target = stage.Position;
                    Stages.Add(stage);
                }
            }

        }

        bool COMInternalInterface.IsCOMOpen()
        {
            return COMHelper.IsCOMOpen(this);
        }

        object COMInternalInterface.OpenCOMPort(COMDeviceInfo info)
        {
            return COMHelper.OpenCOMPort(this, info);
        }

        void COMInternalInterface.ReceiveCOMAct()
        {
            COMHelper.ReceiveCOMAct(this);
        }

        bool COMInternalInterface.TestCOMAction()
        {
            string result = ThreadSafeQuery("1VER?\n\r2VER?\n\r3VER?\n\r4VER?\n\r5VER?\n\r6VER?\n\r7VER?\n\r8VER?\n\r", 400);
            if (result.Trim() != "")
            {
                return true;
            }
            return false;
        }
    }
}
