using HardWares.端口基类;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HardWares.端口基类
{
    /// <summary>
    /// Custom接口
    /// </summary>
    public interface CustomOuterInterface
    {
        /// <summary>
        /// 连接Custom口
        /// </summary>
        /// <param name="CustomName"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        bool ConnectCustom(CustomDeviceInfo info, out Exception exc, bool reconnect = false);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<CustomDeviceInfo> GetCustomDeviceInfos();

    }
}
