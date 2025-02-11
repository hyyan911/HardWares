using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.纳米位移台.PI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HardWares.纳米位移台
{
    /// <summary>
    /// 纳米位移台
    /// </summary>
    public abstract class StageBase : PortElement
    {
        /// <summary>
        /// 轴名称
        /// </summary>
        public abstract string AxisName { get; set; }

        /// <summary>
        /// 当前位置
        /// </summary>
        public abstract double Position { get; }

        /// <summary>
        /// 闭环目标位置
        /// </summary>
        public abstract double Target { get; }

        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="targetvalue"></param>
        public abstract void MoveTo(double targetvalue);

        /// <summary>
        /// 移动到指定位置并等待移动完成
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public abstract void MoveToAndWait(double targetvalue, int timeout);

        /// <summary>
        /// 移动指定偏移量
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public abstract void MoveStepAndWait(double step, int timeout);
    }
}
