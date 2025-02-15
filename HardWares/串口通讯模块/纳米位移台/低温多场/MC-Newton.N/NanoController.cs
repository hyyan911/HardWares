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

namespace HardWares.纳米位移台.低温多场.MC_Newton_N
{
    public partial class NanoController : NanoControllerBase, COMInternalInterface, COMOuterInterface
    {
        COMHelper COMHelper = new COMHelper(']', Encoding.ASCII);
        public bool ConnectCOM(COMDeviceInfo info, out Exception exc)
        {
            return COMHelper.ConnectCOM(this, info, out exc);
        }

        public List<COMDeviceInfo> GetCOMDeviceInfos()
        {
            return COMHelper.ScanSerialCOMs(new Func<SerialPort, string>((ser) =>
            {
                ser.Write("[1-CurPosi?] ");
                Thread.Sleep(200);
                string result = ser.ReadExisting();
                if (result.Contains("[") && result.Contains("]"))
                {
                    return "MC_Newton_N " + ser.PortName.Replace("COM", "");
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
            COMHelper.ReceiveCOMAct(this, new List<string>() { "MovStop", "ReachTarg" });
        }

        bool COMInternalInterface.TestCOMAction()
        {
            string result = ThreadSafeQuery("[1-CurPosi?]", 300);
            if (double.Parse(result) != 0)
            {
                NanoStage stage = new NanoStage("1", this);
                stage.target = stage.Position;
                Stages.Add(stage);
                AddMessage("[1-ServOn]");
            }
            Thread.Sleep(100);
            result = ThreadSafeQuery("[2-CurPosi?]", 300);
            if (double.Parse(result) != 0)
            {
                NanoStage stage = new NanoStage("2", this);
                stage.target = stage.Position;
                Stages.Add(stage);
                AddMessage("[2-ServOn]");
            }
            return true;
        }
    }
}
