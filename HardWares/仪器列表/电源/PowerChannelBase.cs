using HardWares.源表;
using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uc480.Defines;

namespace HardWares.电源
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public abstract class PowerChannelBase : PortElement
    {
        /// <summary>
        /// 通道名
        /// </summary>
        public abstract string ChannelName { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public abstract double Voltage { get; set; }

        /// <summary>
        /// 电流限流值
        /// </summary>
        public abstract double CurrentLimit { get; set; }

        /// <summary>
        /// 输出状态
        /// </summary>
        public abstract bool Output { get; set; }
    }
}
