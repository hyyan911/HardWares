using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HardWares.端口基类部分.设备信息
{
    /// <summary>
    /// 
    /// </summary>
    public class COMDeviceInfo : DeviceInfoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="comname"></param>
        /// <param name="baudrate"></param>
        public COMDeviceInfo(string deviceName, string comname, int baudrate) : base(deviceName)
        {
            DeviceName = deviceName;
            PortType = PortType.COM;
            COMName = comname;
            BaudRate = baudrate;
        }

        /// <summary>
        /// 串口名
        /// </summary>
        public string COMName { get; private set; } = "";

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; private set; } = 9600;



        static int[] ScanBaudRates = new int[] { 9600, 115200, 19200, 38400 };
        /// <summary>
        /// 扫描COM口，遍历所有波特率
        /// </summary>
        /// <param name="PortTestFunc">端口检验函数，遍历端口时执行此函数，如果无返回结果或出错则返回空字符串，如果有返回结果则要进行处理，最终返回ProductName(COMDeviceInfo的DeviceName)</param>
        /// <returns></returns>
        internal static List<COMDeviceInfo> ScanSerialCOMs(Func<SerialPort, string> PortTestFunc)
        {
            List<COMDeviceInfo> result = new List<COMDeviceInfo>();
            var ports = SerialPort.GetPortNames();
            foreach (var baud in ScanBaudRates)
            {
                foreach (var portname in ports)
                {
                    if (result.FindIndex(x => x.COMName == portname) != -1) continue;
                    SerialPort port = new SerialPort(portname, baud);
                    try
                    {
                        port.Open();
                        string res = PortTestFunc(port);
                        port.DiscardInBuffer();
                        port.DiscardOutBuffer();
                        port.Close();
                        if (string.IsNullOrEmpty(res) || res == "") continue;
                        result.Add(new COMDeviceInfo(res, portname, baud));
                    }
                    catch (Exception ex) { continue; }
                }
            }
            return result;
        }

    }
}
