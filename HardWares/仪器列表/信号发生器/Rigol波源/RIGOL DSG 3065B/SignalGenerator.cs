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

namespace HardWares.射频源.Rigol_DSG_3065B
{
    /// <summary>
    /// 
    /// </summary>
    public partial class SignalGenerator : SignalGeneratorBase
    {
        public override string ProductIdentifier { get; internal set; } = "Rigol DSG 3065B";

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
            return VISAResourceHelperBase.ThreadUnsafeQuery(this, messagetosend, timeout);
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

        /// <summary>
        /// 将字符串格式转换成数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal double ConvertFreqValue(string value)
        {
            if (value.Contains("MHz"))
            {
                return double.Parse(value.Replace("MHz", "").Replace(" ", "")) * 1e+6;
            }
            if (value.Contains("kHz"))
            {
                return double.Parse(value.Replace("kHz", "").Replace(" ", "")) * 1e+3;
            }
            if (value.Contains("GHz"))
            {
                return double.Parse(value.Replace("GHz", "").Replace(" ", "")) * 1e+9;
            }
            if (value.Contains("Hz"))
            {
                return double.Parse(value.Replace("Hz", "").Replace(" ", ""));
            }
            return double.Parse(value.Replace(" ", ""));
        }

        #region 设备属性
        #endregion
    }
}
