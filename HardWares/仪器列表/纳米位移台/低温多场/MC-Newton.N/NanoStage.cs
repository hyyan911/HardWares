using HardWares.APIS.PI;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.纳米位移台;
using HardWares.纳米位移台.PI;
using HardWares.纳米位移台.低温多场.MC_Newton_N;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.纳米位移台.低温多场.MC_Newton_N
{
    /// <summary>
    /// 低温多场旋转台
    /// </summary>
    public class NanoStage : StageBase
    {

        /// <summary>
        /// 
        /// </summary>
        public override PortObject ParentDevice { get; internal set; } = null;

        private string name = "";
        /// <summary>
        /// 轴序号
        /// </summary>
        public override string AxisName
        {
            get
            {
                return name;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="parent"></param>
        public NanoStage(string axisName, NanoController parent)
        {
            name = axisName;
            ParentDevice = parent;
        }

        #region 通讯部分

        /// <summary>
        /// 当前位置
        /// </summary>
        public override double Position
        {
            get
            {
                NanoController c = ParentDevice as NanoController;
                string result = c.ThreadSafeQuery("[" + AxisName + "-CurPosi?]", 500);
                if (result == "") return double.NaN;
                return double.Parse(result);
            }
        }

        internal double target = double.NaN;
        /// <summary>
        /// 闭环目标位置
        /// </summary>
        public override double Target
        {
            get
            {
                return target;
            }
        }

        internal double velocity = 5;
        /// <summary>
        /// 速度
        /// </summary>
        public override double Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                NanoController c = ParentDevice as NanoController;
                string result = c.ThreadSafeQuery("[" + AxisName + "-SetSpeed:" + value.ToString() + "]", 500);
                if (result == "") return;
                velocity = value;
            }
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        internal override void InnerMoveTo(double targetvalue)
        {
            target = targetvalue;
            NanoController c = ParentDevice as NanoController;
            string result = c.ThreadSafeQuery("[" + AxisName + "-SetTarg:" + Math.Round(target, 6).ToString() + "]", 500);
            if (result == "") return;
            c.AddMessage("[" + AxisName + "-MovTarg]");
        }
        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        internal override void InnerMoveToAndWait(double targetvalue, int timeout)
        {
            MoveTo(targetvalue);
            int time = 0;
            double det = Math.Abs(Position - targetvalue);
            while ((double.IsNaN(det) || det > 1e-2) && time < timeout)
            {
                if (det < 0.1)
                {
                    (ParentDevice as NanoController).AddMessage("[" + AxisName + "-MovTarg]");
                    Thread.Sleep(100);
                    (ParentDevice as NanoController).AddMessage("[" + AxisName + "-MovStop]");
                }
                else
                {
                    Thread.Sleep(100);
                }
                time += 100;
                det = Math.Abs(Position - targetvalue);
            }
        }


        /// <summary>
        ///位置单位
        /// </summary>
        public override string PositionUnit { get; internal set; } = "mm";

        /// <summary>
        /// 移动指定偏移量
        /// </summary>
        /// <param name="step"></param>
        /// <param name="timeout"></param>
        public override void MoveStepAndWait(double step, int timeout)
        {
            MoveToAndWait(target + step, timeout);
        }

        #endregion
        #region 获取可用参数
        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> result = new List<Parameter>();

            result.Add(new Parameter("AxisName", "轴名称", AxisName.GetType(), this, true) { IsReadOnly = true });

            result.Add(new Parameter("Position", "当前位置", Position.GetType(), this, true) { IsReadOnly = true });

            result.Add(new Parameter("Target", "目标位置", Target.GetType(), this, true) { IsReadOnly = true });

            result.Add(new Parameter("Velocity", "速度(°/s或mm/s)", Velocity.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("CustomRangeLo", "自定义位置下限(°或mm)", CustomRangeLo.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("CustomRangeHi", "自定义位置上限(°或mm)", CustomRangeHi.GetType(), this, true) { IsReadOnly = false });

            return result;
        }

        #endregion
    }
}
