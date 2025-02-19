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

        bool IsAPDSampling = false;
        bool IsReading = false;
        /// <summary>
        /// 获取计数(/s)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<int> GetCounts()
        {
            if (!IsAPDSampling)
            {
                return new List<int>();
            }
            try
            {
                IsReading = true;
                var counts = countReader.ReadMultiSampleInt32(-1);
                IsReading = false;
                return counts.ToList();
            }
            catch (Exception ex)
            {
                IsReading = false;
                return new List<int>();
            }
        }

        private double sampleFreq = 100;
        CounterSingleChannelReader countReader = null;

        /// <summary>
        /// 开始连续采样(IsOuterTrigger表示是否使用外部源触发计数,sampleCount表示采样数，仅在IsOutTrigger为True时有效,SampleFreq表示连续采样频率，仅在IsOutTrigger为False时有效)
        /// </summary>
        public override void BeginSample(bool IsOuterTrigger, double SampleFreq, int sampleCount)
        {
            try
            {
                sampleFreq = SampleFreq;
                IsReading = false;
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
                try
                {
                    DaqOutTriggerTask?.Dispose();
                }
                catch (Exception) { }
                try
                {
                    DaqOutTriggerTask?.Dispose();
                }
                catch (Exception) { }
                #endregion

                if (!IsOuterTrigger)
                {
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
                }
                else
                {
                    DaqOutTriggerTask = new NationalInstruments.DAQmx.Task();
                    CIChannel channel = DaqOutTriggerTask.CIChannels.CreateCountEdgesChannel(DaqCounterOutTriggerChannelName, string.Empty, CICountEdgesActiveEdge.Rising, 0, CICountEdgesCountDirection.Up);
                    DaqContinusReceiveTask.Timing.ConfigureSampleClock(APDDataChannelName, 1, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, sampleCount);
                    channel.DataTransferMechanism = CIDataTransferMechanism.Dma;
                    DaqOutTriggerTask.Timing.ConfigureImplicit(SampleQuantityMode.FiniteSamples, sampleCount);
                    DaqOutTriggerTask.Start();


                    countReader = new CounterSingleChannelReader(DaqOutTriggerTask.Stream);
                }
                //设置采样
                IsAPDSampling = true;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 终止连续采样
        /// </summary>
        public override void EndSample()
        {
            while (IsReading)
            {
                Thread.Sleep(30);
            }
            IsAPDSampling = false;
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
            try
            {
                DaqOutTriggerTask?.Stop();
            }
            catch (Exception) { }
            try
            {
                DaqOutTriggerTask?.Dispose();
            }
            catch (Exception) { }
            #endregion
            return;
        }

    }
}
