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
            return AxisName.Substring(ind, AxisName.Length - ind - 1);
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public override void MoveTo(double targetvalue)
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
        public override void MoveToAndWait(double targetvalue, int timeout = 50)
        {
            MoveTo(targetvalue);
            int time = 0;
            while (Math.Abs(Position - targetvalue) > 0.01 && time < timeout)
            {
                Thread.Sleep(50);
                time += 50;
            }
        }

        /// <summary>
        /// 移动指定偏移量
        /// </summary>
        /// <param name="step"></param>
        /// <param name="timeout"></param>
        public override void MoveStepAndWait(double step, int timeout)
        {
            NanoController c = ParentDevice as NanoController;
            c.AddMessage(c.ProcessCmd(GetAxisInd(), "MVR", step.ToString()));
            target += step;
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

            return result;
        }

        #endregion
    }
}
