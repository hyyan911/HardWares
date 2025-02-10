using HardWares.仪器列表.射频波源.RIGOL_DSG_3000;
using HardWares.数据处理.SCPI指令;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.波源.Rigol_DSG_3060
{
    public partial class RFSignalGenerator : RFSignalGeneratorBase
    {
        VISAResourceHelper VISA { get; set; } = new VISAResourceHelper() { GetProductName = GetProductName };

        /// <summary>
        /// 获取ProductName
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private static string GetProductName(string source)
        {
            try
            {
                if (source.Contains("Rigol Technologies"))
                {
                    string[] ss = source.Split(',');
                    if (ss[1].Contains("DSG3060"))
                    {
                        return "DSG3060 " + ss[2];
                    }
                }
                return "";
            }
            catch (Exception) { return ""; }
        }

        public override string ProductIdentifier { get; internal set; } = "Rigol DSG3060";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            res.Add(new Parameter("RFFrequency", "射频源频率（MHz）", RFFrequency.GetType(), this, true));
            res.Add(new Parameter("RFPhaseOffset", "射频相位偏移(度)", RFPhaseOffset.GetType(), this, true));
            res.Add(new Parameter("RFAmplitude", "射频强度（dBm）", RFAmplitude.GetType(), this, true));
            res.Add(new Parameter("RFAmplitudeLimit", "射频输出上限（dBm）", RFAmplitudeLimit.GetType(), this, true));
            res.Add(new Parameter("IsRFOutOpen", "射频源输出状态", IsRFOutOpen.GetType(), this, true));

            res.Add(new Parameter("LFAmplitude", "低频源幅度（V）", LFAmplitude.GetType(), this, true));
            res.Add(new Parameter("LFFrequency", "低频输出源频率（Hz）", LFFrequency.GetType(), this, true));
            res.Add(new Parameter("IsLFOutOpen", "低频源开关状态", IsLFOutOpen.GetType(), this, true));
            res.Add(new Parameter("LFOutputShape", "低频输出波形", LFOutputShape.GetType(), this, true));
            return res;
        }

        public override void ValidateParams()
        {
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return VISA.VISAThreadUnsafeQuery(this, messagetosend, timeout);
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
                return double.Parse(value.Replace("MHz", "")) * 1e+6;
            }
            if (value.Contains("kHz"))
            {
                return double.Parse(value.Replace("kHz", "")) * 1e+3;
            }
            return double.Parse(value);
        }

        #region 设备属性
        /// <summary>
        /// 射频频率（MHz）
        /// </summary>
        public override double RFFrequency
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "FREQ"), 2000);
                    return ConvertFreqValue(value) * 1e+6;
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { false }, new string[] { value.ToString() + "MHz" }, "FREQ"));
            }
        }
        /// <summary>
        /// 射频强度（dBm）
        /// </summary>
        public override double RFAmplitude
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LEV"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "LEV"));
            }
        }

        /// <summary>
        /// 射频输出上限（dBm）
        /// </summary>
        public override double RFAmplitudeLimit
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LEV", "LIM"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "LEV", "LIM"));
            }
        }

        /// <summary>
        /// 射频源输出状态
        /// </summary>
        public override bool IsRFOutOpen
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "OUTP"), 2000);
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
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value ? "1" : "0" }, "OUTP"));
            }
        }

        /// <summary>
        /// 射频相位偏移(度)
        /// </summary>
        public override double RFPhaseOffset
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "PHAS"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "PHAS"));
            }
        }

        /// <summary>
        /// 低频输出源频率
        /// </summary>
        public override double LFFrequency
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "FREQ"), 2000);
                    return ConvertFreqValue(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "LFO", "FREQ"));
            }
        }

        /// <summary>
        /// 射频幅度（V）
        /// </summary>
        public override double LFAmplitude
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "LEV"), 2000);
                    return ConvertFreqValue(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "LFO", "LEV"));
            }
        }

        /// <summary>
        /// 低频输出波形
        /// </summary>
        public override OutputShapes LFOutputShape
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO", "SHAP"), 2000);
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
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { false }, new string[] { value == OutputShapes.Sine ? "SINE" : "SQU" }, "LFO", "SHAP"));
            }
        }

        /// <summary>
        /// 低频源开关状态
        /// </summary>
        public override bool IsLFOutOpen
        {
            get
            {
                try
                {
                    string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "LFO"), 2000);
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
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value ? "1" : "0" }, "LFO"));
            }
        }
        #endregion
    }
}
