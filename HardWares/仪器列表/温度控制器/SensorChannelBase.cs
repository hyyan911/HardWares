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
    /// 温度传感器通道
    /// </summary>
    public abstract class SensorChannelBase
    {
        public TemperatureControllerBase Parent { get; internal set; } = null;

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
        /// 温度
        /// </summary>
        public abstract double Temperature { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        public SensorChannelBase(string name, TemperatureControllerBase parent, string unit)
        {
            Parent = parent;
            Name = name;
            Unit = unit;
        }

        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public abstract List<Parameter> GetAvailableParams();
    }
}
