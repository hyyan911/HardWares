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
    public abstract class SensorChannelBase : PortElement
    {
        /// <summary>
        /// 单位
        /// </summary>
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
            ParentDevice = parent;
            ChannelName = name;
            Unit = unit;
        }
    }
}
