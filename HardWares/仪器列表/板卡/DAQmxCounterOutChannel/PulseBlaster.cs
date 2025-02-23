using HardWares.仪器列表.板卡.Spincore_PulseBlaster;
using HardWares.端口基类部分;
using NationalInstruments.DAQmx;
using SpinCore.SpinAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.板卡.DAQmxCounterSignalChannel
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PulseBlaster : PulseBlasterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "DAQmx计数器信号输出通道";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 输出频率(Hz)
        /// </summary>
        public double PulseFrequency { get; set; } = 100;

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> pms = new List<Parameter>();
            pms.Add(new Parameter("PulseFrequency", "输出信号频率(Hz)", PulseFrequency.GetType(), this, true) { IsReadOnly = false });
            return pms;
        }

        /// <summary>
        /// 停止输出波形
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void End()
        {
            try
            {
                OutputTask?.Stop();
            }
            catch (Exception)
            {
            }
            try
            {
                OutputTask?.Dispose();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 开始输出波形
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Start()
        {
            try
            {
                OutputTask?.Stop();
            }
            catch (Exception)
            {
            }
            try
            {
                OutputTask?.Dispose();
            }
            catch (Exception)
            {
            }
            try
            {

                var chs = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CO, PhysicalChannelAccess.External);
                foreach (var item in chs)
                {
                    try
                    {
                        OutputTask = new NationalInstruments.DAQmx.Task();
                        var ch = OutputTask.COChannels.CreatePulseChannelFrequency(item, string.Empty, COPulseFrequencyUnits.Hertz, COPulseIdleState.Low, 0, PulseFrequency, 0.5);
                        ch.PulseTerminal = Instance as string;
                        OutputTask.Timing.ConfigureImplicit(SampleQuantityMode.ContinuousSamples, 2);
                        OutputTask.Start();
                        return;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            OutputTask?.Stop();
                        }
                        catch (Exception)
                        {
                        }
                        try
                        {
                            OutputTask?.Dispose();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetCommands(List<CommandBase> lines)
        {
            return;
        }

        public override void ValidateParams()
        {
        }

        internal override Encoding GetCoder()
        {
            return Encoding.UTF8;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }
    }
}
