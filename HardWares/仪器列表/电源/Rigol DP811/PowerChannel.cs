using HardWares.源表;
using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uc480.Defines;

namespace HardWares.电源
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public abstract class PowerChannel : PowerChannelBase
    {
        public override PortObject ParentDevice { get; internal set; } = null;

        public override string ChannelName { get; set; } = "电源通道";

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> res = new List<Parameter>();
            res.Add(new Parameter("Amplitude", "低频源幅度（V）", Amplitude.GetType(), this, true));
            res.Add(new Parameter("Frequency", "低频输出源频率（Hz）", Frequency.GetType(), this, true));
            res.Add(new Parameter("IsOutOpen", "低频源开关状态", IsOutOpen.GetType(), this, true));
            res.Add(new Parameter("WaveShape", "低频输出波形", WaveShape.GetType(), this, true));
            return res;
        }
    }
}
