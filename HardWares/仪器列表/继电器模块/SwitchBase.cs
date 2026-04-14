using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.继电器模块
{
    public abstract class SwitchBase : PortObject
    {
        /// <summary>
        /// 设备种类
        /// </summary>
        public override string ProductType { get; internal set; } = "继电器";

        /// <summary>
        /// 是否打开
        /// </summary>
        public abstract bool IsOpen { get; set; }
    }
}
