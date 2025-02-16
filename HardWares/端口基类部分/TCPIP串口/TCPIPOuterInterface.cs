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
    /// TCPIP接口
    /// </summary>
    public interface TCPIPOuterInterface
    {
        /// <summary>
        /// 连接TCPIP口
        /// </summary>
        /// <param name="TCPIPName"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        bool ConnectTCPIP(TCPIPDeviceInfo info, out Exception exc, bool reconnect = false);

        /// <summary>
        /// 获取USB设备名称
        /// </summary>
        /// <returns></returns>
        List<TCPIPDeviceInfo> GetTCPIPDeviceInfos();
    }
}
