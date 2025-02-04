using HardWares.端口基类;
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
        bool ConnectTCPIP(string TCPIPName, int baudrate, out Exception exc);

    }
}
