using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.仪器列表.板卡
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class PulseBlasterBase : PortObject
    {
        /// <summary>
        /// 开始编程
        /// </summary>
        public abstract void BeginProgram();

        /// <summary>
        /// 停止编程
        /// </summary>
        public abstract void EndProgram();

        /// <summary>
        /// 开始输出信号
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// 停止输出信号
        /// </summary>
        public abstract void End();
    }
}
