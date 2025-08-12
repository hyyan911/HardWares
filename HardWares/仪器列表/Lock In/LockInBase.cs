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

        /// <summary>
        /// PID输出
        /// </summary>
        public abstract bool PIDOutput { get; set; }

        /// <summary>
        /// 解调器X值
        /// </summary>
        public abstract double DemodX { get; }

        /// <summary>
        /// 解调器Y值
        /// </summary>
        public abstract double DemodY { get; }

        /// <summary>
        /// 解调器R值
        /// </summary>
        public abstract double DemodR { get; }

        /// <summary>
        /// 解调器幅角值
        /// </summary>
        public abstract double DemodAngle { get; }

        /// <summary>
        /// 当前PID值
        /// </summary>
        public abstract double PIDValue { get; }
        /// <summary>
        /// PID输出下限
        /// </summary>
        public abstract double PIDOutputLowerLimit { get; set; }

        /// <summary>
        /// PID输出上限
        /// </summary>
        public abstract double PIDOutputUpperLimit { get; set; }
    }
}
