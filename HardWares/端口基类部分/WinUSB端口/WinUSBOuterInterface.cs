using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类
{
    /// <summary>
    /// 
    /// </summary>
    public interface WinUSBOuterInterface
    {
        /// <summary>
        /// 连接USB
        /// </summary>
        /// <param name="USBName"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        bool ConnectUSB(string usbName, out Exception exc);

        /// <summary>
        /// 获取USB设备名称
        /// </summary>
        /// <returns></returns>
        List<string> GetUsbDeviceNames();
    }
}
