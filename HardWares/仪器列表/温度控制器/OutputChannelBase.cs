using HardWares.温度控制器.SRS_PTC10;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.温度控制器
{
    /// <summary>
    /// 温控输出通道
    /// </summary>
    public abstract class OutputChannelBase
    {
        public TemperatureControllerBase Parent { get;internal set; } = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public OutputChannelBase(string name, TemperatureControllerBase parent, string unit)
        {
            Parent = parent;
            Name = name;
            Unit = unit;
        }

        /// <summary>
        /// 名称
        /// </summary>
        [StaticParameter]
        public string Name { get; internal set; }


        /// <summary>
        /// 单位
        /// </summary>
        [StaticParameter]
        public string Unit { get; internal set; }


        /// <summary>
        /// 输出功率(W)
        /// </summary>
        public abstract double Power { get; }


        /// <summary>
        /// PID参数:P
        /// </summary>
        [StaticParameter]
        public abstract double P { get; set; }

        /// <summary>
        /// PID参数:P
        /// </summary>
        [StaticParameter]
        public abstract double I { get; set; }

        /// <summary>
        /// PID参数:P
        /// </summary>
        [StaticParameter]
        public abstract double D { get; set; }

        /// <summary>
        /// 设定温度(度)
        /// </summary>
        [StaticParameter]
        public abstract double SetPoint { get; set; }

        /// <summary>
        /// 功率限制下限
        /// </summary>
        [StaticParameter]
        public abstract double LowPowerLimit { get; set; }

        /// <summary>
        /// 功率限制上限
        /// </summary>
        [StaticParameter]
        public abstract double HiPowerLimit { get; set; }

        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public abstract List<Parameter> GetAvailableParams();

    }
}
