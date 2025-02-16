using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.仪器列表.板卡.Spincore_PulseBlaster
{
    /// <summary>
    /// 
    /// </summary>
    public enum CommandTypes
    {
        /// <summary>
        /// 继续执行下一条指令
        /// </summary>
        Continue = 0,
        /// <summary>
        /// 等待Trigger
        /// </summary>
        Wait = 1,
        /// <summary>
        /// 结束指令
        /// </summary>
        End = 2,
    }
}
