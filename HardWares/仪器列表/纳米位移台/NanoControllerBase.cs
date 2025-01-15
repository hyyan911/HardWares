using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.纳米位移台
{
    /// <summary>
    /// 纳米位移台
    /// </summary>
    public abstract class NanoControllerBase : PortObject
    {

        /// <summary>
        /// 位移台列表
        /// </summary>
        public abstract List<StageBase> Stages { get; set; }

    }
}
