using HardWares.源表;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uc480.Defines;

namespace HardWares.电源.Rigol_DP811
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public partial class Power : PowerBase
    {
        /// <summary>
        /// 产品名称 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Rigol DP811";

        public override event ParamsChangeEventHandler ParamsChangedEvent;
        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>();
        }

        public override void ValidateParams()
        {
            return;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return VISAResourceHelperBase.ThreadUnsafeQuery(this, messagetosend, timeout);
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

    }
}
