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
    public partial class SignalChannel : SignalChannelBase
    {
        public override PortObject ParentDevice { get; internal set; } = null;

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            res.Add(new Parameter("Frequency", "频率（Hz）", Frequency.GetType(), this, true));
            res.Add(new Parameter("PhaseOffset", "相位偏移(度)", PhaseOffset.GetType(), this, true));
            res.Add(new Parameter("Amplitude", "幅度（V）", Amplitude.GetType(), this, true));
            res.Add(new Parameter("Offset", "输出偏置（V）", Offset.GetType(), this, true));
            res.Add(new Parameter("IsOutOpen", "输出状态", IsOutOpen.GetType(), this, true));
            res.Add(new Parameter("WaveShape", "输出波形", WaveShape.GetType(), this, true));
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
                ParentDevice.AddMessage(dev.ProcessCmd("W", ChannelName, "F", value.ToString()));
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
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                ParentDevice.AddMessage(dev.ProcessCmd("W", ChannelName, "A", value.ToString()));
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
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "N", ""), 2000);
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
                var dev = ParentDevice as SignalGenerator;
                ParentDevice.AddMessage(dev.ProcessCmd("W", ChannelName, "N", value ? "1" : "0"));
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
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                ParentDevice.AddMessage(dev.ProcessCmd("W", ChannelName, "P", value.ToString()));
            }
        }

        /// <summary>
        /// 输出偏置(V)
        /// </summary>
        public double Offset
        {
            get
            {
                try
                {
                    var dev = ParentDevice as SignalGenerator;
                    string value = ParentDevice.ThreadSafeQuery(dev.ProcessCmd("R", ChannelName, "O", ""), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                var dev = ParentDevice as SignalGenerator;
                ParentDevice.AddMessage(dev.ProcessCmd("W", ChannelName, "O", value.ToString()));
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
                if (value == "1") return OutputShapes.Square;
                else { return OutputShapes.Unknown; }
            }
            set
            {
                if (value == OutputShapes.Unknown) return;
                string v = "0";
                if (value == OutputShapes.Sine) v = "0";
                if (value == OutputShapes.Square) v = "1";
                var dev = ParentDevice as SignalGenerator;
                ParentDevice.AddMessage(dev.ProcessCmd("W", ChannelName, "N", v));
            }
        }
        #endregion
    }
}
