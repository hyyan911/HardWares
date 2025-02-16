using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.APD.Exclitas_SPCM_AQRH
{
    /// <summary>
    /// 
    /// </summary>
    public partial class APD : APDBase
    {
        public override string ProductIdentifier { get; internal set; } = "Exclitas SPCM-AQRH";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>();
        }

        public override void ValidateParams()
        {
            return;
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }

        /// <summary>
        /// 获取计数(/s)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override int GetContinusCount()
        {
            return 0;
        }

        public override void BeginContinusSample()
        {
            return;
        }

        public override void EndContinusSample()
        {
            return;
        }

        /// <summary>
        /// 取样频率
        /// </summary>
        public override double SampleFreq { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
