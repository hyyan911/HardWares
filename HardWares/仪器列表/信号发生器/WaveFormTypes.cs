using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.仪器列表.信号发生器
{
    /// <summary>
    /// 波形
    /// </summary>
    public enum WaveFormTypes
    {
        /// <summary>
        /// 正弦
        /// </summary>
        Sine = 0,
        /// <summary>
        /// 矩形波
        /// </summary>
        Square = 1,
        /// <summary>
        /// CMOS波
        /// </summary>
        CMOS = 2,
        /// <summary>
        /// 直流波
        /// </summary>
        DC = 4,
        /// <summary>
        /// 三角波
        /// </summary>
        Triangle = 5,
        /// <summary>
        /// 正锯齿波
        /// </summary>
        P_SawTooth = 6,
        /// <summary>
        /// 反锯齿波
        /// </summary>
        N_SawTooth = 7,
        /// <summary>
        /// 正阶梯波
        /// </summary>
        P_Step = 9,
        /// <summary>
        /// 反阶梯波
        /// </summary>
        N_Step = 10,
        /// <summary>
        /// 正指数波
        /// </summary>
        P_Exp = 11,
        /// <summary>
        /// 反指数波
        /// </summary>
        N_Exp = 12,
    }
}
