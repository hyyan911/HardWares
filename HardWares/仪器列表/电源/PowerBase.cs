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
    public abstract class PowerBase : PortObject
    {
        /// <summary>
        /// 通道列表
        /// </summary>
        public List<PowerChannelBase> Channels { get; set; } = new List<PowerChannelBase>();
    }
}
