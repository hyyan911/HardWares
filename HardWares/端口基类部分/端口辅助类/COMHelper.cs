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
using HardWares.数据处理;
using HardWares.温度控制器.SRS_PTC10;
using System.Threading;

namespace HardWares.端口基类部分.PortHelper
{
    internal class COMHelper : PortHelper
    {

        private char LF = '\n';

        private Encoding coder = Encoding.ASCII;

        public COMHelper(char LF, Encoding encoding)
        {
            this.LF = LF;
            coder = encoding;
        }

        static int[] ScanBaudRates = new int[] { 9600, 115200, 19200, 38400 };

        /// <summary>
        /// 扫描COM口，遍历所有波特率
        /// </summary>
        /// <param name="PortTestFunc">端口检验函数，遍历端口时执行此函数，如果无返回结果或出错则返回空字符串，如果有返回结果则要进行处理，最终返回ProductName(COMDeviceInfo的DeviceName)</param>
        /// <returns></returns>
        internal static List<COMDeviceInfo> ScanSerialCOMs(Func<SerialPort, string> PortTestFunc, List<int> BaudRatestoScan = null)
        {
            List<COMDeviceInfo> result = new List<COMDeviceInfo>();
            var ports = SerialPort.GetPortNames();
            List<int> scanlist = BaudRatestoScan == null ? ScanBaudRates.ToList() : BaudRatestoScan;
            foreach (var baud in scanlist)
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


        #region 常用方法
        public void CloseCOMPort(PortObject device)
        {
            (device.Instance as SerialPort).Close();
        }

        public byte[] COMPortRead(PortObject device)
        {
            int count = (device.Instance as SerialPort).BytesToRead;
            byte[] result = new byte[count];
            (device.Instance as SerialPort).Read(result, 0, count);
            return result;
        }

        public void COMPortWrite(PortObject device, byte[] value)
        {
            (device.Instance as SerialPort).Write(value, 0, value.Length);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectCOM(PortObject device, COMDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return device.Connect(info, out exc, reconnect);
        }

        public bool IsCOMOpen(PortObject device)
        {
            return (device.Instance as SerialPort).IsOpen;
        }

        public object OpenCOMPort(PortObject device, COMDeviceInfo param)
        {
            SerialPort port = new SerialPort(param.COMName, param.BaudRate);
            port.Open();
            return port;
        }

        public void ReceiveCOMAct(PortObject device, List<string> FilterStr = null)
        {
            List<List<byte>> retu = DataProcess.ProcessReceivedSerialData(LF, device.ReceiveBuffer, out List<byte> result);
            device.ReceiveBuffer = result;
            if (retu.Count != 0)
            {
                List<string> res = new List<string>();
                foreach (var item in retu)
                {
                    string st = coder.GetString(item.ToArray());
                    if (FilterStr == null)
                    {
                        res.Add(st);
                        continue;
                    }
                    if (FilterStr.Where(x => st.Contains(x)).Count() == 0) res.Add(st);
                }
                device.QueryState = true;
                //只取第一条指令
                device.QueryReturnedData = res;
            }
        }

        public List<string> ThreadUnsafeQuery(PortObject device, string command, int timeout)
        {
            device.QueryReturnedData = null;
            device.QueryState = false;
            device.PortArranger.AddMessage(command);
            int time = 0;
            while (device.QueryState == false && time < timeout)
            {
                Thread.Sleep(30);
                time += 30;
            }

            if (device.QueryReturnedData == null) return new List<string>();
            if (device.QueryReturnedData.Count == 0) return new List<string>();

            //去除分隔符
            for (int i = 0; i < device.QueryReturnedData.Count; i++)
            {
                device.QueryReturnedData[i] = device.QueryReturnedData[0].Replace(LF.ToString(), "");
            }
            return device.QueryReturnedData;
        }

        #endregion
    }
}
