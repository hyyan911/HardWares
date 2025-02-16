using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类部分.设备信息
{
    public abstract class DeviceInfoBase
    {
        /// <summary>
        /// 设备名
        /// </summary>
        public string DeviceName { get; protected set; } = "";

        /// <summary>
        /// 
        /// </summary>
        internal PortType PortType { get; set; } = PortType.USB;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceName"></param>
        public DeviceInfoBase(string deviceName)
        {
            DeviceName = deviceName;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool CompareParam(DeviceInfoBase target)
        {
            if (target.GetType().Name != GetType().Name) return false;

            if (this is COMDeviceInfo)
            {
                COMDeviceInfo source = this as COMDeviceInfo;
                COMDeviceInfo tat = target as COMDeviceInfo;
                if (source.DeviceName == tat.DeviceName && source.COMName == tat.COMName && source.BaudRate == tat.BaudRate) return true;
                return false;
            }

            if (this is USBDeviceInfo)
            {
                USBDeviceInfo source = this as USBDeviceInfo;
                USBDeviceInfo tat = target as USBDeviceInfo;
                if (source.DeviceName == tat.DeviceName && source.USBIdentification == tat.USBIdentification) return true;
                return false;
            }

            if (this is TCPIPDeviceInfo)
            {
                TCPIPDeviceInfo source = this as TCPIPDeviceInfo;
                TCPIPDeviceInfo tat = target as TCPIPDeviceInfo;
                if (source.DeviceName == tat.DeviceName && source.IPAddress == tat.IPAddress && source.Port == tat.Port) return true;
                return false;
            }

            if (this is CustomDeviceInfo)
            {
                CustomDeviceInfo source = this as CustomDeviceInfo;
                CustomDeviceInfo tat = target as CustomDeviceInfo;
                if (source.Params.Count != tat.Params.Count) return false;
                for (int i = 0; i < source.Params.Count; i++)
                {
                    if (source.Params[i] != tat.Params[i]) return false;
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public DeviceInfoBase Copy()
        {
            DeviceInfoBase result = null;
            if (this is COMDeviceInfo)
            {
                var v = this as COMDeviceInfo;
                result = new COMDeviceInfo(v.DeviceName, v.COMName, v.BaudRate);
            }

            if (this is USBDeviceInfo)
            {
                var v = this as USBDeviceInfo;
                result = new USBDeviceInfo(v.DeviceName, v.USBIdentification);
            }

            if (this is TCPIPDeviceInfo)
            {
                var v = this as TCPIPDeviceInfo;
                result = new TCPIPDeviceInfo(v.DeviceName, v.IPAddress, v.Port);
            }

            if (this is CustomDeviceInfo)
            {
                var v = this as CustomDeviceInfo;
                result = new CustomDeviceInfo(v.DeviceName, v.Params);
            }

            return result;
        }
    }
}
