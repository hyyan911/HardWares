using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.端口基类部分.设备信息
{
    /// <summary>
    /// 
    /// </summary>
    public class USBDeviceInfo : DeviceInfoBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="portType"></param>
        /// <param name="identification"></param>
        public USBDeviceInfo(string deviceName, string identification) : base(deviceName)
        {
            DeviceName = deviceName;
            PortType = PortType.USB;
            USBIdentification = identification;
        }

        /// <summary>
        /// USB标识符
        /// </summary>
        public string USBIdentification { get; private set; } = "";

    }
}
