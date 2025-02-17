using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using uc480.Defines;
using System.Threading;

namespace HardWares.APD.Exclitas_SPCM_AQRH
{
    /// <summary>
    /// 
    /// </summary>
    public partial class APD : APDBase
    {
        public override string ProductIdentifier { get; internal set; } = "Exclitas SPCM-AQRH";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            var list = new List<Parameter>();
            list.Add(new Parameter("DaqContinusTriggerChannelNames", "连续测量输出信号通道", DaqContinusTriggerChannelNames.GetType(), this, true) { IsReadOnly = false });
            list.Add(new Parameter("DaqContinusReceiveChannelNames", "APD输入计数器通道", DaqContinusReceiveChannelNames.GetType(), this, true) { IsReadOnly = false });
            list.Add(new Parameter("APDDataChannelNames", "APD数据采集通道", APDDataChannelNames.GetType(), this, true) { IsReadOnly = false });
            return list;
        }

        public override void ValidateParams()
        {
            return;
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }

        bool IsContinusSampling = false;
        bool IsSampling = false;
        /// <summary>
        /// 获取计数(/s)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override double GetContinusCountRatio()
        {
            if (!IsContinusSampling)
            {
                return 0;
            }
            try
            {
                IsSampling = true;
                var counts = countReader.ReadMultiSampleInt32(2);
                IsSampling = false;
                return (counts[1] - counts[0]) * sampleFreq;
            }
            catch (Exception ex)
            {
                IsSampling = false;
                return 0;
            }
        }

        private double sampleFreq = 100;
        CounterSingleChannelReader countReader = null;

        /// <summary>
        /// 开始连续采样
        /// </summary>
        public override void BeginContinusSample(double SampleFreq)
        {
            try
            {
                sampleFreq = SampleFreq;
                IsSampling = false;
                #region 结束旧任务
                try
                {
                    DaqContinusTriggerTask?.Stop();
                }
                catch (Exception) { }
                try
                {
                    DaqContinusTriggerTask?.Dispose();
                }
                catch (Exception) { }
                try
                {
                    DaqContinusReceiveTask?.Stop();
                }
                catch (Exception) { }
                try
                {
                    DaqContinusReceiveTask?.Dispose();
                }
                catch (Exception) { }
                #endregion

                #region 触发DAQ任务
                DaqContinusTriggerTask = new NationalInstruments.DAQmx.Task();
                DaqContinusTriggerTask.COChannels.CreatePulseChannelFrequency(DaqContinusTriggerChannelName, string.Empty, COPulseFrequencyUnits.Hertz, COPulseIdleState.Low, 0, SampleFreq, 0.5);
                DaqContinusTriggerTask.Timing.ConfigureImplicit(SampleQuantityMode.ContinuousSamples, 2);
                #endregion

                #region 采样DAQ任务
                DaqContinusReceiveTask = new NationalInstruments.DAQmx.Task();
                CIChannel channel = DaqContinusReceiveTask.CIChannels.CreateCountEdgesChannel(DaqContinusReceiveChannelName, string.Empty, CICountEdgesActiveEdge.Rising, 0, CICountEdgesCountDirection.Up);
                channel.DataTransferMechanism = CIDataTransferMechanism.Dma;
                DaqContinusReceiveTask.Timing.ConfigureSampleClock(APDDataChannelName, 10, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, 2);
                #endregion

                DaqContinusTriggerTask.Start();
                DaqContinusReceiveTask.Start();

                countReader = new CounterSingleChannelReader(DaqContinusReceiveTask.Stream);
                //设置采样
                IsContinusSampling = true;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 终止连续采样
        /// </summary>
        public override void EndContinusSample()
        {
            while (IsSampling)
            {
                Thread.Sleep(30);
            }
            IsContinusSampling = false;
            #region 结束任务
            try
            {
                DaqContinusTriggerTask?.Stop();
            }
            catch (Exception) { }
            try
            {
                DaqContinusTriggerTask?.Dispose();
            }
            catch (Exception) { }
            try
            {
                DaqContinusReceiveTask?.Stop();
            }
            catch (Exception) { }
            try
            {
                DaqContinusReceiveTask?.Dispose();
            }
            catch (Exception) { }
            #endregion
            return;
        }

    }
}
