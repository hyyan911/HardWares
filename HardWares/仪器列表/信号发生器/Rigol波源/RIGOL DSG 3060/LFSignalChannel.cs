using HardWares.射频源;
using HardWares.数据处理.SCPI指令;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.射频源.Rigol_DSG_3060
{
    /// <summary>
    /// 
    /// </summary>
    public partial class LFSignalChannel : SignalChannelBase
    {
        public override PortObject ParentDevice { get; internal set; } = null;

        public override string ChannelName { get; set; } = "低频输出通道";

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            res.Add(new Parameter("Amplitude", "低频源幅度（V）", Amplitude.GetType(), this, true));
            res.Add(new Parameter("Frequency", "低频输出源频率（Hz）", Frequency.GetType(), this, true));
            res.Add(new Parameter("IsOutOpen", "低频源开关状态", IsOutOpen.GetType(), this, true));
            res.Add(new Parameter("WaveShape", "低频输出波形", WaveShape.GetType(), this, true));
            return res;
        }

        #region 设备属性
        /// <summary>
        /// 低频输出源频率
        /// </summary>
        public override double Frequency
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "FREQ"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "FREQ"), 2000);
                    return (ParentDevice as SignalGenerator).ConvertFreqValue(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "LFO", "FREQ"));
            }
        }

        /// <summary>
        /// 射频幅度（V）
        /// </summary>
        public override double Amplitude
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "LEV"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "LEV"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "LFO", "LEV"));
            }
        }
        public override double PhaseOffset
        {
            get
            {
                return double.NaN;
            }
            set
            {
                return;
            }
        }

        /// <summary>
        /// 低频输出波形
        /// </summary>
        public override OutputShapes WaveShape
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "SHAP"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "SHAP"), 2000);
                    if (value == "SINE")
                    {
                        return OutputShapes.Sine;
                    }
                    if (value == "SQU")
                    {
                        return OutputShapes.Square;
                    }
                    return OutputShapes.Unknown;
                }
                catch (Exception) { return OutputShapes.Unknown; }
            }
            set
            {
                if (value == OutputShapes.Unknown) return;
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value == OutputShapes.Sine ? "SINE" : "SQU" }, "LFO", "SHAP"));
            }
        }

        /// <summary>
        /// 低频源开关状态
        /// </summary>
        public override bool IsOutOpen
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO"), 2000);
                    if (value == "1")
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception) { return false; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value ? "1" : "0" }, "LFO"));
            }
        }
        #endregion
    }
}
