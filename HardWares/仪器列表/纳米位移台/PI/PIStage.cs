using HardWares.APIS.PI;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.纳米位移台;
using HardWares.纳米位移台.PI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.纳米位移台.PI
{
    /// <summary>
    /// PI位移台
    /// </summary>
    public class PIStage : StageBase
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
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("SAI", name, value));
                //检查是否修改成功
                List<string> res = c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("SAI?", "", ""), 1000));
                if (res.Contains(value))
                {
                    name = value;
                }
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="parent"></param>
        public PIStage(string axisName, PIController parent)
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
                PIController c = ParentDevice as PIController;
                if (!double.TryParse(c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("POS?", AxisName, ""), 1000))[0], out double position))
                {
                    return double.NaN;
                }
                return position;
            }
        }

        /// <summary>
        /// 闭环目标位置
        /// </summary>
        public override double Target
        {
            get
            {
                PIController c = ParentDevice as PIController;
                if (!double.TryParse(c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("MOV?", AxisName, ""), 1000))[0], out double position))
                {
                    return double.NaN;
                }
                return position;
            }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable
        {
            get
            {
                PIController c = ParentDevice as PIController;
                string result = c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("EAX?", AxisName, ""), 1000))[0];
                if (result.Trim() == "0" || result.Trim() == "")
                {
                    return false;
                }
                return true;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("EAX", AxisName, value ? "1" : "0"));
            }
        }

        /// <summary>
        /// 开环位置下限
        /// </summary>
        public double OpenRangeLo
        {
            get
            {
                PIController c = ParentDevice as PIController;
                if (!double.TryParse(c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("TMN?", AxisName, ""), 1000))[0], out double position))
                {
                    return double.NaN;
                }
                return position;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("TMN", AxisName, value.ToString()));
            }
        }

        /// <summary>
        /// 开环位置上限
        /// </summary>
        public double OpenRangeHi
        {
            get
            {
                PIController c = ParentDevice as PIController;
                if (!double.TryParse(c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("TMX?", AxisName, ""), 1000))[0], out double position))
                {
                    return double.NaN;
                }
                return position;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("TMX", AxisName, value.ToString()));
            }
        }

        /// <summary>
        /// 闭环位置下限
        /// </summary>
        public double ServoRangeLo
        {
            get
            {
                PIController c = ParentDevice as PIController;
                if (!double.TryParse(c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("CMN?", AxisName, ""), 1000))[0], out double position))
                {
                    return double.NaN;
                }
                return position;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("CMN", AxisName, value.ToString()));
            }
        }

        /// <summary>
        /// 闭环位置上限
        /// </summary>
        public double ServoRangeHi
        {
            get
            {
                PIController c = ParentDevice as PIController;
                if (!double.TryParse(c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("CMX?", AxisName, ""), 1000))[0], out double position))
                {
                    return double.NaN;
                }
                return position;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("CMX", AxisName, value.ToString()));
            }
        }

        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool IsMoving
        {
            get
            {
                PIController c = ParentDevice as PIController;
                string result = c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("#5", AxisName, ""), 1000))[0];
                if (result.Trim() == "0" || result.Trim() == "")
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// 是否启用闭环控制
        /// </summary>
        public bool ServoState
        {
            get
            {
                PIController c = ParentDevice as PIController;
                string result = c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("SVO?", AxisName, ""), 1000))[0];
                if (result.Trim() == "0" || result.Trim() == "")
                {
                    return false;
                }
                return true;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("SVO", AxisName, value ? "1" : "0"));
            }
        }

        /// <summary>
        /// Reference模式
        /// </summary>
        public ReferenceModes ReferenceMode
        {
            get
            {
                PIController c = ParentDevice as PIController;
                string result = c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("RON?", AxisName, ""), 1000))[0];
                if (result.Trim() == "0")
                {
                    return ReferenceModes.Custom;
                }
                if (result.Trim() == "")
                {
                    return ReferenceModes.None;
                }
                return ReferenceModes.System;
            }
            set
            {
                PIController c = ParentDevice as PIController;
                c.Send(c.ProcessCmd("RON", AxisName, value == ReferenceModes.Custom ? "0" : "1"));
            }
        }

        /// <summary>
        /// 指定轴是否已经处于Reference状态
        /// </summary>
        public bool IsReferenced
        {
            get
            {
                PIController c = ParentDevice as PIController;
                string result = c.ProcessQueryResult(c.ThreadSafeQuery(c.ProcessCmd("FRF?", AxisName, ""), 1000))[0];
                if (result.Trim() == "0" || result.Trim() == "")
                {
                    return false;
                }
                return true;
            }
        }


        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public override void MoveTo(double targetvalue)
        {
            PIController c = ParentDevice as PIController;
            c.Send(c.ProcessCmd("MOV", AxisName, ""));
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        public override void MoveToAndWait(double targetvalue, int timeout)
        {
            PIController c = ParentDevice as PIController;
            c.Send(c.ProcessCmd("MOV", AxisName, targetvalue.ToString()));
            int time = 0;
            while (IsMoving && time < timeout)
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
            if (ServoState)
            {
                double target = Target;
                target += step;
                MoveToAndWait(target, timeout);
            }
        }
        #region Reference
        /// <summary>
        /// 将轴当前位置设置为位置的参考零点,返回设置状态，只有在ReferenceMode为Custom时才有效
        /// </summary>
        public void SetCurrentPositoionAsReference(double refValue, out Exception exc)
        {
            PIController c = ParentDevice as PIController;
            if (!c.CommandSet.Contains("POS"))
            {
                exc = new Exception("此设备不包含此指令");
                return;
            }
            if (!(ReferenceMode == ReferenceModes.Custom))
            {
                exc = new Exception("只有当ReferenceMode为True时才能使用此方法");
                return;
            }
            c.Send(c.ProcessCmd("POS", AxisName, refValue.ToString()));
            exc = null;
        }

        /// <summary>
        /// 进行Reference操作并等待，将设备的内置零点设置为参考点,返回结果
        /// </summary>
        public bool Reference(int timeout)
        {
            PIController c = ParentDevice as PIController;
            c.Send(c.ProcessCmd("FRF", AxisName, ""));
            int time = 0;
            while (c.IsControllerReady() == false && time < timeout)
            {
                Thread.Sleep(30);
                time += 30;
            }
            if (time >= timeout) return false;
            return true;
        }

        #endregion
        #endregion
        #region 获取可用参数
        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            PIStage st = this;
            List<Parameter> result = new List<Parameter>();

            PIController c = ParentDevice as PIController;
            Parameter p = c.CreateParameterRef("SAI", "SAI?", "AxisName", st.AxisName.GetType(), st);
            if (p != null)
            {
                p.Description = "轴名称";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("#5", "#5?", "IsMoving", st.IsMoving.GetType(), st);
            if (p != null)
            {
                p.Description = "是否正在移动";
                p.IsStatic = false;
                result.Add(p);
            }

            p = c.CreateParameterRef("EAX", "EAX?", "Enable", st.Enable.GetType(), st);
            if (p != null)
            {
                p.Description = "是否启用";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("TMX", "TMX?", "OpenRangeHi", st.OpenRangeHi.GetType(), st);
            if (p != null)
            {
                p.Description = "开环位置上限";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("TMN", "TMN?", "OpenRangeLo", st.OpenRangeLo.GetType(), st);
            if (p != null)
            {
                p.Description = "开环位置下限";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("CMX", "CMX?", "ServoRangeHi", st.ServoRangeHi.GetType(), st);
            if (p != null)
            {
                p.Description = "闭环位置下限";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("CMN", "CMN?", "ServoRangeLo", st.ServoRangeLo.GetType(), st);
            if (p != null)
            {
                p.Description = "闭环位置上限";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("SVO", "SVO?", "ServoState", st.ServoState.GetType(), st);
            if (p != null)
            {
                p.Description = "是否启用闭环控制";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("MOV", "MOV?", "Target", st.Target.GetType(), st);
            if (p != null)
            {
                p.Description = "闭环目标位置";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("POS", "POS?", "Position", st.Position.GetType(), st);
            if (p != null)
            {
                p.Description = "当前位置";
                p.IsStatic = false;
                result.Add(p);
            }

            p = c.CreateParameterRef("FRF", "FRF?", "IsReferenced", st.IsReferenced.GetType(), st);
            if (p != null)
            {
                p.Description = "是否处于Referenced状态";
                p.IsStatic = true;
                result.Add(p);
            }

            p = c.CreateParameterRef("RON", "RON?", "ReferenceMode", st.ReferenceMode.GetType(), st);
            if (p != null)
            {
                p.Description = "Reference模式";
                p.IsStatic = true;
                result.Add(p);
            }
            return result;
        }

        #endregion
    }
}
