using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thorlabs.MotionControl.FilterFlipperCLI;

namespace HardWares.仪器列表.电动翻转座
{
    /// <summary>
    /// 
    /// </summary>
    public partial class FlipMotor : FlipMotorBase
    {
        /// <summary>
        /// 开关状态
        /// </summary>
        public override bool Switch
        {
            get
            {
                try
                {
                    int pos = (Instance as FilterFlipper).Position;
                    if (pos == 1) return true;
                    else return false;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            set
            {
                try
                {
                    (Instance as FilterFlipper).SetPosition(value ? (uint)1 : (uint)2, 3000);
                }
                catch (Exception)
                {
                    return;
                }
            }
        }

        public override string ProductIdentifier { get; internal set; } = "Thorlabs Filter Flipper";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter> { new Parameter("Switch", "开关状态", typeof(bool), this, true) { IsReadOnly = false } };
        }

        public override void ValidateParams()
        {
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }
    }
}
