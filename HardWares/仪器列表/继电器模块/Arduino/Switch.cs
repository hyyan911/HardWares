using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.继电器模块.Arduino
{
    public partial class Switch : SwitchBase
    {
        public override bool IsOpen
        {
            get
            {
                var res = ThreadSafeQuery("RSWI\n", 500);
                if (res.Replace("\n", "").Last() == '0') return false;
                if (res.Replace("\n", "").Last() == '1') return true;
                return false;
            }
            set
            {
                var res = ThreadSafeQuery("SWI" + (value ? "1" : "0") + "\n", 500);
            }
        }
        public override string ProductIdentifier { get; internal set; } = "Arduino Switch";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>()
            {
                new Parameter("IsOpen","是否打开",IsOpen.GetType(),this,true){ IsReadOnly=false}
            };
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
            return COMHelper.ThreadUnsafeQuery(this, messagetosend, timeout)[0];
        }
    }
}
