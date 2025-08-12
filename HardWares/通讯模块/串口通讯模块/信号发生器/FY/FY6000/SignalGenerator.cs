using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace HardWares.射频源.FY6000
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SignalGenerator : SignalGeneratorBase, COMInternalInterface, COMOuterInterface
    {

        COMHelper COMHelper = new COMHelper('\n', System.Text.Encoding.ASCII);

        public bool ConnectCOM(COMDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return COMHelper.ConnectCOM(this, info, out exc, reconnect);
        }

        public List<COMDeviceInfo> GetCOMDeviceInfos()
        {
            return COMHelper.ScanSerialCOMs(new Func<System.IO.Ports.SerialPort, string>((ser) =>
            {
                try
                {
                    ser.Write("UMO" + (char)0x0a);
                    Thread.Sleep(200);
                    string model = ser.ReadExisting();
                    ser.Write("UID" + (char)0x0a);
                    Thread.Sleep(200);
                    string id = ser.ReadExisting();
                    ser.Write("UMO" + (char)0x0a);
                    Thread.Sleep(200);
                    model = ser.ReadExisting();
                    if (model.Contains("FY"))
                    {
                        return model.Replace("\n", "") + " " + id.Replace("\n", "");
                    }
                    return "";
                }
                catch (Exception) { return ""; }
            }), new List<int>() { 115200 });
        }

        void COMInternalInterface.CloseCOMPort()
        {
            COMHelper.CloseCOMPort(this);
        }

        void COMInternalInterface.ConnectedCOMAction()
        {
            //添加射频和低频通道
            Channels.Clear();
            Channels.Add(new ModulationChannel() { ParentDevice = this, ChannelName = "CH1" });
            Channels.Add(new SignalChannel() { ParentDevice = this, ChannelName = "CH2" });
        }

        bool COMInternalInterface.IsCOMOpen()
        {
            return COMHelper.IsCOMOpen(this);
        }

        object COMInternalInterface.OpenCOMPort(COMDeviceInfo param)
        {
            return COMHelper.OpenCOMPort(this, param);
        }

        void COMInternalInterface.ReceiveCOMAct()
        {
            COMHelper.ReceiveCOMAct(this);
        }

        bool COMInternalInterface.TestCOMAction()
        {
            (Instance as SerialPort).DiscardInBuffer();
            (Instance as SerialPort).DiscardOutBuffer();
            string model = ThreadSafeQuery("UMO\n", 1000);
            string id = ThreadSafeQuery("UID\n", 1000);
            if (model.Contains("FY"))
            {
                return true;
            }
            return false;
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
    }
}
