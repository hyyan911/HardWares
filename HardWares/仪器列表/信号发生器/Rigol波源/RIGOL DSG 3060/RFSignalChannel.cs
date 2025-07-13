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
    public partial class RFSignalChannel : SignalChannelBase
    {
        public override PortObject ParentDevice { get; internal set; } = null;

        public override string ChannelName { get; set; } = "射频输出通道";

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            res.Add(new Parameter("Frequency", "射频频率（MHz）", Frequency.GetType(), this, true));
            res.Add(new Parameter("PhaseOffset", "射频相位偏移(度)", PhaseOffset.GetType(), this, true));
            res.Add(new Parameter("Amplitude", "射频强度（dBm）", Amplitude.GetType(), this, true));
            res.Add(new Parameter("AmplitudeLimit", "射频输出上限（dBm）", AmplitudeLimit.GetType(), this, true));
            res.Add(new Parameter("IsOutOpen", "射频输出状态", IsOutOpen.GetType(), this, true));
            res.Add(new Parameter("WaveShape", "输出波形", WaveShape.GetType(), this, true) { IsReadOnly = true });
            return res;
        }

        /// <summary>
        /// 将字符串格式转换成数字
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double ConvertFreqValue(string value)
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
        /// <summary>
        /// 射频频率（MHz）
        /// </summary>
        public override double Frequency
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "FREQ"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "FREQ"), 2000);
                    return ConvertFreqValue(value) * 1e-6;
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() + "MHz" }, "FREQ"));
            }
        }
        /// <summary>
        /// 射频强度（dBm）
        /// </summary>
        public override double Amplitude
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LEV"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LEV"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() + "dBm" }, "LEV"));
            }
        }

        /// <summary>
        /// 射频输出上限（dBm）
        /// </summary>
        public double AmplitudeLimit
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LEV", "LIM"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LEV", "LIM"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() + "dBm" }, "LEV", "LIM"));
            }
        }

        /// <summary>
        /// 射频源输出状态
        /// </summary>
        public override bool IsOutOpen
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "OUTP"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "OUTP"), 2000);
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
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value ? "1" : "0" }, "OUTP"));
            }
        }

        /// <summary>
        /// 射频相位偏移(度)
        /// </summary>
        public override double PhaseOffset
        {
            get
            {
                try
                {
                    string value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "PHAS"), 2000);
                    value = ParentDevice.ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "PHAS"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "PHAS"));
            }
        }

        public override OutputShapes WaveShape
        {
            get
            {
                return OutputShapes.Sine;
            }
            set
            {
                return;
            }
        }
        #endregion
    }
}
