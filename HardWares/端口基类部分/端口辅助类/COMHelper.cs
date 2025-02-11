using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Visa;
using Ivi.Visa;
using System.Xml.Linq;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
using System.IO.Ports;

namespace HardWares.端口基类部分.PortHelper
{
    internal class COMHelper : PortHelper
    {
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
                    if (EndJudgeFunc != null)
                    {
                        if (EndJudgeFunc.Invoke() == true) return result;
                    }
                    if (result.FindIndex(x => x.COMName == portname) != -1) continue;
                    SerialPort port = new SerialPort(portname, baud);
                    try
                    {
                        port.Open();
                        port.DiscardInBuffer();
                        port.DiscardOutBuffer();
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
