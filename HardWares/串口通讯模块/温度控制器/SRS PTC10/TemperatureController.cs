using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using HardWares.数据处理;
using HardWares.端口基类.COM串口;
using HardWares.端口基类;

namespace HardWares.温度控制器.SRS_PTC10
{
    public partial class TemperatureController : TemperatureControllerBase, COMInternalInterface, COMOuterInterface
    {

        void COMInternalInterface.CloseCOMPort()
        {
            (Instance as SerialPort).Close();
        }

        void COMInternalInterface.COMInitAction(object PortInstance)
        {
        }

        byte[] COMInternalInterface.COMPortRead()
        {
            int count = (Instance as SerialPort).ReadBufferSize;
            byte[] result = new byte[count];
            (Instance as SerialPort).Read(result, 0, count);
            return result;
        }

        void COMInternalInterface.COMPortWrite(byte[] value)
        {
            (Instance as SerialPort).Write(value, 0, value.Length);
        }

        public bool ConnectCOM(string COMName, int baudrate, out Exception exc)
        {
            return Connect(PortType.COM, out exc, Encoding.ASCII, COMName, baudrate);
        }

        void COMInternalInterface.ConnectedCOMAction()
        {
            //查找所有通道
            List<string> names = ThreadSafeQuery("getOutput.names\n", 1000).Split(',').ToList();
            List<string> units = ThreadSafeQuery("getOutput.units\n", 1000).Split(',').ToList();
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].Trim() == "W" || units[i].Trim() == "mW")
                {
                    OutputChannels.Add(new OutputChannel(names[i].Trim(), this, units[i].Trim()));
                }
                else
                {
                    if (units[i].Trim() == "ê")
                    {
                        SensorChannels.Add(new SensorChannel(names[i].Trim(), this, "Ω"));
                    }
                    else
                    {
                        SensorChannels.Add(new SensorChannel(names[i].Trim(), this, units[i].Trim()));
                    }
                }
            }
        }

        bool COMInternalInterface.IsCOMOpen()
        {
            return (Instance as SerialPort).IsOpen;
        }

        object COMInternalInterface.OpenCOMPort(List<object> param)
        {
            SerialPort port = new SerialPort(param[0] as string, (int)param[1]);
            port.Open();
            return port;
        }

        void COMInternalInterface.ReceiveCOMAct()
        {
            List<List<byte>> retu = DataProcess.ProcessReceivedSerialData('\n', ReceiveBuffer, out List<byte> result);
            ReceiveBuffer = result;
            if (retu.Count != 0)
            {
                QueryState = true;
                //只取第一条指令
                QueryReturnedData = new List<string>() { System.Text.Encoding.UTF7.GetString(retu[0].ToArray()) };
            }
        }

        bool COMInternalInterface.TestCOMAction()
        {
            string result = ThreadSafeQuery("\"" + "*IDN?" + "\"\n", 500);
            if (result.Contains("Stanford Research Systems, PTC10 Programmable Temperature Controller"))
            {
                ProductName = "SRS PTC10";
                return true;
            }
            return false;
        }

        public override void ValidateParams()
        {
            return;
        }

    }

}
