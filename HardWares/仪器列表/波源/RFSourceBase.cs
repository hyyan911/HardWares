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
    public abstract class RFSourceBase : PortObject
    {
        /// <summary>
        /// 射频波源频率(MHz)
        /// </summary>
        public abstract double RFFrequency { get; set; }

        /// <summary>
        /// 射频强度（dBm）
        /// </summary>
        public abstract double RFAmplitude { get; set; }

        /// <summary>
        /// 射频强度上限
        /// </summary>
        public abstract double RFAmplitudeLimit { get; set; }

        /// <summary>
        /// RF源开关状态
        /// </summary>
        public abstract bool IsRFOutOpen { get; set; }

        /// <summary>
        /// 低频源频率(Hz)
        /// </summary>
        public abstract double LFFrequency { get; set; }

        /// <summary>
        /// 低频源幅度(dBm)
        /// </summary>
        public abstract double LFAmplitude { get; set; }

        /// <summary>
        /// 低频输出波形
        /// </summary>
        public abstract OutputShapes LFOutputShape { get; set; }

        /// <summary>
        /// 低频源开关状态
        /// </summary>
        public abstract bool IsLFOutOpen { get; set; }

        /// <summary>
        /// 射频偏移
        /// </summary>
        public abstract double RFPhaseOffset { get; set; }
    }
}
