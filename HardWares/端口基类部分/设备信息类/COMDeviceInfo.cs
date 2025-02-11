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

    }
}
