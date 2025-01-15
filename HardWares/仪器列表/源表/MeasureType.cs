using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.源表
{
    public enum MeasureTypes
    {
        /// <summary>
        /// 仅测量电压，返回结果中包含测量的电压值，电流为NAN
        /// </summary>
        MeasureVoltage = 0,

        /// <summary>
        /// 仅测量电流，返回结果中包含设置的电压值，测量的电流值
        /// </summary>
        MeasureCurrent = 1,

        /// <summary>
        /// 测量电压和电流值，返回结果中包含测量的电压值测量的电流值
        /// </summary>
        MeasureVoltageAndCurrent = 3,
    }
}
