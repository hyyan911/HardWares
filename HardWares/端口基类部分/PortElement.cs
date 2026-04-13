using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares
{
    /// <summary>
    /// 设备元素
    /// </summary>
    public abstract class PortElement
    {
        /// <summary>
        /// 所属设备
        /// </summary>
        public abstract PortObject ParentDevice { get; internal set; }

        /// <summary>
        /// 通道名
        /// </summary>
        public virtual string ChannelName { get; internal set; } = "";

        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public abstract List<Parameter> AvailableParameterNames();
    }
}
