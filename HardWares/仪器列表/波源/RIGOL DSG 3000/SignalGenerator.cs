using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.TCPIP通讯模块.波源.Rigol_DSG_3000
{
    internal partial class SignalGenerator : SignalGeneratorBase
    {
        VISAResourceHelper VISA { get; set; } = new VISAResourceHelper();

        public override string ProductIdentifier { get; internal set; } = "Rigol DSG3000";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            return res;
        }

        public override void ValidateParams()
        {
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return VISA.VISAThreadUnsafeQuery(this, messagetosend, timeout);
        }
    }
}
