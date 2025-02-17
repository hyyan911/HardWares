using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares
{
    /// <summary>
    /// 端口类型
    /// </summary>
    public enum PortType
    {
        /// <summary>
        /// USB设备
        /// </summary>
        USB = 0,
        /// <summary>
        /// COM设备(包括RS232以及USB生成的虚拟串口设备)
        /// </summary>
        COM = 1,
        /// <summary>
        /// 网络设备
        /// </summary>
        TCPIP = 2,
        /// <summary>
        /// 自定义设备
        /// </summary>
        CUSTOM = 3

    }
}
