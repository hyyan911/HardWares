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
using HardWares.端口基类部分.设备信息;
using HardWares.端口基类部分.PortHelper;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectCOM(COMDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect);
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

        object COMInternalInterface.OpenCOMPort(COMDeviceInfo param)
        {
            SerialPort port = new SerialPort(param.COMName, param.BaudRate);
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
            string result = ThreadSafeQuery("\n\"*IDN?\"\n", 500);
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

        public List<COMDeviceInfo> GetCOMDeviceInfos()
        {
            return COMHelper.ScanSerialCOMs(new Func<SerialPort, string>((ser) =>
            {
                try
                {
                    byte[] bs = Encoding.ASCII.GetBytes("\n\"*IDN?\"\n");
                    ser.Write(bs, 0, bs.Length);
                    Thread.Sleep(200);
                    string res = ser.ReadExisting();
                    if (res.Contains("Stanford Research Systems, PTC10 Programmable Temperature Controller"))
                    {
                        string serialnumber = res.Split(',')[2];
                        return "SRS PTC10 " + serialnumber;
                    }
                    return "";
                }
                catch (Exception) { return ""; }
            }));
        }
    }

}
