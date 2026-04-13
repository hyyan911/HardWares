using HardWares.温度控制器.SRS_PTC10;
using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.温度控制器
{
    /// <summary>
    /// 温度控制器基类
    /// </summary>
    public abstract class TemperatureControllerBase : ElementPortObject
    {
        /// <summary>
        /// 开关状态
        /// </summary>
        public abstract bool OutputEnable { get; set; }
    }
}
