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
            List<Parameter> parameters = new List<Parameter>();
            parameters.Add(new Parameter("IsInTriggerMode", "内部触发模式", IsInTriggerMode.GetType(), this, true));
            return parameters;
        }

        public override void ValidateParams()
        {
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            var res = COMHelper.ThreadUnsafeQuery(this, messagetosend, timeout);
            return res[0];
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
        /// <summary>
        /// 是否处于内部触发状态
        /// </summary>
        public bool IsInTriggerMode
        {
            get
            {
                var res = ThreadSafeQuery("RPF\n", 1000);
                if (res == "3")
                {
                    return true;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    //触发模式
                    ThreadSafeQuery("WPF" + "3" + "\n", 1000);
                    //CH2触发模式
                    ThreadSafeQuery("WPM" + "0" + "\n", 1000);
                }
            }
        }
        #endregion
    }
}
