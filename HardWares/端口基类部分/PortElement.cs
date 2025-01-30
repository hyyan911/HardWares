using HardWares.端口基类;
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
    public class PortElement
    {
        /// <summary>
        /// 所属设备
        /// </summary>
        public PortObject ParentDevice { get; set; } = null;
    }
}
