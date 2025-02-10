using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类部分
{
    internal class DeviceInfo
    {
        public PortObject Device { get; set; } = null;

        public PortType PortType { get; set; } = PortType.USB;

        public List<object> PortParams { get; set; } = new List<object>();

        public bool CompareParam(Type devicetype, PortType type, object[] param)
        {
            if (devicetype.Name != Device.GetType().Name) return false;

            if (type == PortType.USB)
            {
                if (PortParams[0].ToString() == param[0].ToString())
                {
                    return true;
                }
                return false;
            }

            if (type == PortType.COM)
            {
                if (PortParams[0].ToString() == param[0].ToString() && PortParams[1].ToString() == param[1].ToString())
                {
                    return true;
                }
                return false;
            }

            if (type == PortType.TCPIP)
            {
                if (PortParams[0].ToString() == param[0].ToString() && PortParams[1].ToString() == param[1].ToString())
                {
                    return true;
                }
                return false;
            }

            return false;
        }
    }
}
