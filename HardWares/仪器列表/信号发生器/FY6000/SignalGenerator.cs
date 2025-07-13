using HardWares.射频源;
using HardWares.数据处理.SCPI指令;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.射频源.FY6000
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SignalGenerator : SignalGeneratorBase
    {
        public override string ProductIdentifier { get; internal set; } = "FY 6000";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>();
        }

        public override void ValidateParams()
        {
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return COMHelper.ThreadUnsafeQuery(this, messagetosend, timeout)[0];
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

        internal string ProcessCmd(string writereadmode, string channel, string signaltoget, string data)
        {
            string wr = writereadmode;
            if (channel == "CH1")
            {
                return wr + "M" + signaltoget + data + (char)0x0a;
            }
            else
            {
                return wr + "F" + signaltoget + data + (char)0x0a;
            }
        }

        #region 设备属性
        #endregion
    }
}
