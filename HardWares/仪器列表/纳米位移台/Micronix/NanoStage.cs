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

namespace HardWares.纳米位移台.Micronix
{
    /// <summary>
    /// Micronix位移台
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
                return c.ProcessNum(c.ThreadSafeQuery(c.ProcessCmd(GetAxisInd(), "POS?"), 300));
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

        /// <summary>
        /// 速度
        /// </summary>
        public override double Velocity
        {
            get
            {
                NanoController c = ParentDevice as NanoController;
                return c.ProcessNum(c.ThreadSafeQuery(c.ProcessCmd(GetAxisInd(), "VEL?"), 300));
            }
            set
            {
                NanoController c = ParentDevice as NanoController;
                c.AddMessage(c.ProcessCmd(GetAxisInd(), "VEL", value.ToString()));
            }
        }

        /// <summary>
        /// 速度上限
        /// </summary>
        public double VelocityLimit
        {
            get
            {
                NanoController c = ParentDevice as NanoController;
                return c.ProcessNum(c.ThreadSafeQuery(c.ProcessCmd(GetAxisInd(), "VMX?"), 300));
            }
        }


        /// <summary>
        ///位置单位
        /// </summary>
        public override string PositionUnit { get; internal set; } = "mm";

        /// <summary>
        /// 加速度
        /// </summary>
        public double Acceleration
        {
            get
            {
                NanoController c = ParentDevice as NanoController;
                return c.ProcessNum(c.ThreadSafeQuery(c.ProcessCmd(GetAxisInd(), "ACC?"), 300));
            }
            set
            {
                NanoController c = ParentDevice as NanoController;
                c.AddMessage(c.ProcessCmd(GetAxisInd(), "ACC", value.ToString()));
            }
        }

        /// <summary>
        /// 加速度上限
        /// </summary>
        public double AccelerationLimit
        {
            get
            {
                NanoController c = ParentDevice as NanoController;
                return c.ProcessNum(c.ThreadSafeQuery(c.ProcessCmd(GetAxisInd(), "AMX?"), 300));
            }
        }

        private string GetAxisInd()
        {
            int ind = AxisName.LastIndexOf('_');
            return AxisName.Substring(ind + 1, AxisName.Length - ind - 1);
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public override void InnerMoveTo(double targetvalue)
        {
            NanoController c = ParentDevice as NanoController;
            c.AddMessage(c.ProcessCmd(GetAxisInd(), "MVA", targetvalue.ToString()));
            target = targetvalue;
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public override void InnerMoveToAndWait(double targetvalue, int timeout)
        {
            MoveTo(targetvalue);
            int time = 0;
            double det = Math.Abs(Position - target);
            while ((double.IsNaN(det) || det > 1e-4) && time < timeout)
            {
                Thread.Sleep(50);
                time += 50;
                det = Math.Abs(Position - target);
            }
            target = Position;
        }

        /// <summary>
        /// 移动指定偏移量
        /// </summary>
        /// <param name="step"></param>
        /// <param name="timeout"></param>
        public override void MoveStepAndWait(double step, int timeout)
        {
            if (target + step < CustomRangeLo || target + step > CustomRangeHi) return;
            NanoController c = ParentDevice as NanoController;
            c.AddMessage(c.ProcessCmd(GetAxisInd(), "STP", step.ToString()));
            c.AddMessage(c.ProcessCmd(GetAxisInd(), "MVR", step.ToString()));
            int time = 0;
            double det = Math.Abs(Position - target);
            while ((double.IsNaN(det) || det > 1e-4) && time < timeout)
            {
                Thread.Sleep(50);
                time += 50;
                det = Math.Abs(Position - target);
            }
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

            result.Add(new Parameter("Velocity", "速度", Velocity.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("VelocityLimit", "速度上限", VelocityLimit.GetType(), this, true) { IsReadOnly = true });

            result.Add(new Parameter("Acceleration", "加速度", Acceleration.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("AccelerationLimit", "加速度上限", AccelerationLimit.GetType(), this, true) { IsReadOnly = true });

            result.Add(new Parameter("CustomRangeLo", "自定义位置下限(mm)", CustomRangeLo.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("CustomRangeHi", "自定义位置上限(mm)", CustomRangeHi.GetType(), this, true) { IsReadOnly = false });

            return result;
        }

        #endregion
    }
}
