using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.射频源
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class SignalGeneratorBase : PortObject
    {
        /// <summary>
        /// 通道列表
        /// </summary>
        public List<SignalChannelBase> Channels { get; set; } = new List<SignalChannelBase>();
    }
}
