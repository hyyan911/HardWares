using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.DAQmx;
using uc480.Defines;

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
            return new List<Parameter>();
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
        /// <summary>
        /// 获取计数(/s)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override double GetContinusCountRatio()
        {
            if (!IsContinusSampling)
            {
                throw new Exception("在获取连续计数前必须调用BeginContinusSample方法");
            }
            try
            {
                var counts = countReader.ReadMultiSampleInt32(2);
                return (counts[1] - counts[0]) * sampleFreq;
            }
            catch (Exception)
            {
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
            sampleFreq = SampleFreq;
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
            DaqContinusReceiveTask.Timing.ConfigureImplicit(SampleQuantityMode.ContinuousSamples, 2);
            #endregion

            DaqContinusTriggerTask.Start();
            DaqContinusReceiveTask.Start();

            countReader = new CounterSingleChannelReader(DaqContinusReceiveTask.Stream);
            //设置采样
            IsContinusSampling = true;
        }

        /// <summary>
        /// 终止连续采样
        /// </summary>
        public override void EndContinusSample()
        {
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
            IsContinusSampling = false;
            return;
        }

    }
}
