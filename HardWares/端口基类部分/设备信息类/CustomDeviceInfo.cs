using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类部分.设备信息
{
    /// <summary>
    /// 自定义设备信息
    /// </summary>
    public class CustomDeviceInfo : DeviceInfoBase
    {
        public List<string> Params = new List<string>();

        public CustomDeviceInfo(string deviceName, params string[] @params) : base(deviceName)
        {
            DeviceName = deviceName;
            PortType = PortType.CUSTOM;
            Params = @params.ToList();
        }

        public CustomDeviceInfo(string deviceName, List<string> @params) : base(deviceName)
        {
            DeviceName = deviceName;
            PortType = PortType.CUSTOM;
            Params = @params.ToList();
        }
    }
}
