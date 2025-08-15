using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.继电器模块.Arduino
{
    public partial class Switch : SwitchBase, COMInternalInterface, COMOuterInterface
    {
        COMHelper COMHelper = new COMHelper('\n', Encoding.ASCII);

        public bool ConnectCOM(COMDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return COMHelper.ConnectCOM(this, info, out exc, reconnect);
        }

        public List<COMDeviceInfo> GetCOMDeviceInfos()
        {
            return COMHelper.ScanSerialCOMs(new Func<SerialPort, string>((ser) =>
              {
                  ser.Write("TEST" + "\n");
                  System.Threading.Thread.Sleep(200);
                  string result = ser.ReadExisting();
                  if (result.Contains("Arduino"))
                  {
                      return result.Replace("\n", "");
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
            COMHelper.ReceiveCOMAct(this);
        }

        bool COMInternalInterface.TestCOMAction()
        {
            var result = COMHelper.ThreadUnsafeQuery(this, "TEST\n", 500);
            if (result[0].Contains("Arduino Switch"))
            {
                return true;
            }
            return false;
        }
    }
}
