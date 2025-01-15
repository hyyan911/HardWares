using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.源表
{
    /// <summary>
    /// 测量组
    /// </summary>
    public class MeasureGroup
    {
        /// <summary>
        /// 电压
        /// </summary>
        public double Voltage { get; internal set; } = double.NaN;

        /// <summary>
        /// 电流
        /// </summary>
        public double Current { get; internal set; } = double.NaN;

        /// <summary>
        /// 取样时间
        /// </summary>
        public DateTime TimeStamp { get; internal set; }

        /// <summary>
        /// 电流是否限流
        /// </summary>
        public bool IsCurrentLimited { get; set; } = false;
    }
}
