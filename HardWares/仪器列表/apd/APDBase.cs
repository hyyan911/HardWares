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
        public abstract void BeginContinusSample(double SampleFreq);
        /// <summary>
        /// 获取计数率(/s)
        /// </summary>
        /// <returns></returns>
        public abstract double GetContinusCountRatio();

        /// <summary>
        /// 结束连续采样
        /// </summary>
        public abstract void EndContinusSample();
        #endregion
    }
}
