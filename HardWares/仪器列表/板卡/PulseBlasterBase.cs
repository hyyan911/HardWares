using HardWares.仪器列表.板卡.Spincore_PulseBlaster;
using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.板卡
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PulseBlasterBase : PortObject
    {
        /// <summary>
        /// 开始输出信号
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 停止输出信号
        /// </summary>
        public abstract void End();

        /// <summary>
        /// 
        /// </summary>
        public abstract void SetCommands(List<CommandLine> lines);


        /// <summary>
        /// 通道序号列表
        /// </summary>
        public IEnumerable<int> ChannelInds { get; internal set; }
    }
}
