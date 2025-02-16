using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.仪器列表.板卡.Spincore_PulseBlaster
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PulseBlaster : PulseBlasterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Spincore PulseBlaster";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>();
        }

        /// <summary>
        /// 开始编程
        /// </summary>
        public override void BeginProgram()
        {
        }

        /// <summary>
        /// 终止执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void End()
        {
        }

        /// <summary>
        /// 终止编程
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void EndProgram()
        {
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Start()
        {
        }

        public override void ValidateParams()
        {
        }

        internal override Encoding GetCoder()
        {
            return Encoding.UTF8;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }
    }
}
