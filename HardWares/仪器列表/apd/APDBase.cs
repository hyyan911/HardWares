using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.APD
{
    public abstract class APDBase : PortObject
    {
        #region 连续计数部分

        /// <summary>
        /// 开始连续采样
        /// </summary>
        public abstract void BeginContinusSample();
        /// <summary>
        /// 获取计数
        /// </summary>
        /// <returns></returns>
        public abstract int GetContinusCount();
        /// <summary>
        /// 光子数采样频率
        /// </summary>
        public abstract double SampleFreq { get; set; }

        /// <summary>
        /// 结束连续采样
        /// </summary>
        public abstract void EndContinusSample();
        #endregion
    }
}
