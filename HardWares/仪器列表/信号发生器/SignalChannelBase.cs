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
    public abstract class SignalChannelBase : PortElement
    {
        /// <summary>
        /// 波源频率(MHz)
        /// </summary>
        public abstract double Frequency { get; set; }

        /// <summary>
        /// 强度（dBm）
        /// </summary>
        public abstract double Amplitude { get; set; }

        /// <summary>
        /// 偏移（V）
        /// </summary>
        public abstract double Offset { get; set; }

        /// <summary>
        /// 开关状态
        /// </summary>
        public abstract bool IsOutOpen { get; set; }

        /// <summary>
        /// 射频偏移
        /// </summary>
        public abstract double PhaseOffset { get; set; }

        /// <summary>
        /// 输出波形
        /// </summary>
        public abstract OutputShapes WaveShape { get; set; }
    }
}
