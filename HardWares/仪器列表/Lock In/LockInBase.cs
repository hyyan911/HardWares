using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.Lock_In
{
    /// <summary>
    /// LockIn锁相放大器
    /// </summary>
    public abstract class LockInBase : PortObject
    {
        /// <summary>
        /// P值
        /// </summary>
        public abstract double P { get; set; }

        /// <summary>
        /// I值
        /// </summary>
        public abstract double I { get; set; }

        /// <summary>
        /// D值
        /// </summary>
        public abstract double D { get; set; }

        /// <summary>
        /// PID设定值
        /// </summary>
        public abstract double SetPoint { get; set; }
    }
}
