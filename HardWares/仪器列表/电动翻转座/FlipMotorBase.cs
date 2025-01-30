using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.仪器列表.电动翻转座
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class FlipMotorBase : PortObject
    {
        /// <summary>
        /// 翻转座开关
        /// </summary>
        public abstract bool Switch { get; set; }
    }
}
