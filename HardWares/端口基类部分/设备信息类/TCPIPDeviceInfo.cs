using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类部分.设备信息
{
    /// <summary>
    /// 
    /// </summary>
    public class TCPIPDeviceInfo : DeviceInfoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="portType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public TCPIPDeviceInfo(string deviceName, string ip, int port) : base(deviceName)
        {
            DeviceName = deviceName;
            PortType = PortType.TCPIP;
            IPAddress = ip;
            Port = port;
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; private set; } = "";
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; } = 0;

    }
}
