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
    /// COM接口
    /// </summary>
    public interface COMOuterInterface
    {
        /// <summary>
        /// 连接COM口
        /// </summary>
        /// <param name="COMName"></param>
        /// <param name="baudrate"></param>
        /// <returns></returns>
        bool ConnectCOM(string COMName, int baudrate, out Exception exc);
    }
}
