using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.继电器模块
{
    internal abstract class SwitchBase : PortObject
    {
        /// <summary>
        /// 是否打开
        /// </summary>
        public abstract bool IsOpen { get; set; }
    }
}
