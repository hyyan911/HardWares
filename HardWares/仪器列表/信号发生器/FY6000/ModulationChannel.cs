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

namespace HardWares.射频源.FY6000
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ModulationChannel : SignalChannelBase
    {
        public override PortObject ParentDevice { get; internal set; } = null;

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            res.Add(new Parameter("Frequency", "频率（Hz）", Frequency.GetType(), this, true));
            res.Add(new Parameter("PhaseOffset", "相位偏移(度)", PhaseOffset.GetType(), this, true));
            res.Add(new Parameter("AmplitudeLimit", "幅度上限（V）", AmplitudeLimit.GetType(), this, true));
            res.Add(new Parameter("Amplitude", "幅度（V）", Amplitude.GetType(), this, true));
            res.Add(new Parameter("Offset", "输出偏置（V）", Offset.GetType(), this, true));
            res.Add(new Parameter("Duty", "占空比（仅方波时有效）", Duty.GetType(), this, true));
            res.Add(new Parameter("WaveShape", "输出波形", WaveShape.GetType(), this, true));
            res.Add(new Parameter("TriggerPulseCount", "输出脉冲数", TriggerPulseCount.GetType(), this, true));
            res.Add(new Parameter("IsOutOpen", "输出状态", IsOutOpen.GetType(), this, true));
            return res;
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
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "F", ""), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "F", value.ToString()), 1000);
                dev.IsInTriggerMode = trigger;
            }
        }

        /// <summary>
        /// 输出脉冲数
        /// </summary>
        public int TriggerPulseCount
        {
            get
            {
                try
                {
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery("RPN\n", 2000);
                    return int.Parse(value);
                }
                catch (Exception) { return -1; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery("WPN" + value.ToString() + "\n", 1000);
                dev.IsInTriggerMode = trigger;
            }
        }
        /// <summary>
        /// 强度(V）
        /// </summary>
        public override double Amplitude
        {
            get
            {
                try
                {
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "A", ""), 2000);
                    return double.Parse(value) / 10000;
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                double v = value;
                if (v > AmplitudeLimit) v = AmplitudeLimit;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "A", v.ToString()), 1000);
                dev.IsInTriggerMode = trigger;
            }
        }

        /// <summary>
        /// 输出上限(V)
        /// </summary>
        public double AmplitudeLimit { get; set; } = 3;

        /// <summary>
        /// 射频源输出状态
        /// </summary>
        public override bool IsOutOpen
        {
            get
            {
                try
                {
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "N", ""), 2000);
                    if (value == "255")
                    {
                        return true;
                    }
                    return false;
                }
                catch (Exception) { return false; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "N", value ? "1" : "0"), 1000);
                dev.IsInTriggerMode = trigger;
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
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "P", ""), 2000);
                    return double.Parse(value) / 1000;
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "P", value.ToString()), 1000);
                dev.IsInTriggerMode = trigger;
            }
        }

        /// <summary>
        /// 输出偏置(V)
        /// </summary>
        public override double Offset
        {
            get
            {
                try
                {
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "O", ""), 2000);
                    return double.Parse(value) / 1000;
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "O", value.ToString()), 1000);
                dev.IsInTriggerMode = trigger;
            }
        }

        /// <summary>
        /// 波形
        /// </summary>
        public override OutputShapes WaveShape
        {
            get
            {
                var dev = ParentDevice as SignalGenerator;
                string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "W", ""), 2000);
                if (value == "0") return OutputShapes.Sine;
                if (value == "2") return OutputShapes.Square;
                if (value == "38") return OutputShapes.NarrowNegativePulse;
                else { return OutputShapes.Unknown; }
            }
            set
            {
                if (value == OutputShapes.Unknown) return;
                string v = "0";
                if (value == OutputShapes.Sine) v = "0";
                if (value == OutputShapes.Square) v = "2";
                if (value == OutputShapes.NarrowNegativePulse) v = "38";
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "W", v), 1000);
                dev.IsInTriggerMode = trigger;
            }
        }
        #endregion

        /// <summary>
        /// 占空比
        /// </summary>
        public double Duty
        {
            get
            {
                var dev = ParentDevice as SignalGenerator;
                string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "D", ""), 2000);
                try
                {
                    return double.Parse(value) / 1000.0 / 100.0;
                }
                catch (Exception)
                {
                    return double.NaN;
                }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                bool trigger = dev.IsInTriggerMode;
                ParentDevice.ThreadSafeQuery(dev.ProcessCmd("W", ChannelName, "D", Math.Round(value * 100.0, 1).ToString()), 1000);
                dev.IsInTriggerMode = trigger;
            }
        }
    }
}
