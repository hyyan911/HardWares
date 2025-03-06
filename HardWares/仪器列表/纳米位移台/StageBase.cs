using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.纳米位移台.PI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        /// 自定义位置下限(mm)
        /// </summary>
        public double CustomRangeLo { get; set; } = 0;

        /// <summary>
        /// 自定义位置上限(mm)
        /// </summary>
        public double CustomRangeHi { get; set; } = 0;

        /// <summary>
        /// 闭环目标位置
        /// </summary>
        public abstract double Target { get; }

        /// <summary>
        /// 速度
        /// </summary>
        public abstract double Velocity { get; set; }

        /// <summary>
        /// 位置单位
        /// </summary>
        public abstract string PositionUnit { get; internal set; }

        /// <summary>
        /// 移动到目标位置
        /// </summary>
        /// <param name="targetvalue"></param>
        public void MoveTo(double targetvalue)
        {
            if (Target < CustomRangeLo || Target > CustomRangeHi) return;
            if (targetvalue <= CustomRangeLo)
            {
                InnerMoveTo(CustomRangeLo);
                return;
            };
            if (targetvalue >= CustomRangeHi)
            {
                InnerMoveTo(CustomRangeHi);
                return;
            };
            InnerMoveTo(targetvalue);
        }

        internal abstract void InnerMoveTo(double target);

        /// <summary>
        /// 移动到指定位置并等待移动完成
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public void MoveToAndWait(double targetvalue, int timeout)
        {
            if (Target < CustomRangeLo || Target > CustomRangeHi) return;
            if (targetvalue <= CustomRangeLo)
            {
                InnerMoveToAndWait(CustomRangeLo, timeout);
                return;
            };
            if (targetvalue >= CustomRangeHi)
            {
                InnerMoveToAndWait(CustomRangeHi, timeout);
                return;
            };
            InnerMoveToAndWait(targetvalue, timeout);
        }

        internal abstract void InnerMoveToAndWait(double target, int timeout);

        /// <summary>
        /// 移动指定偏移量
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public abstract void MoveStepAndWait(double step, int timeout);
    }
}
