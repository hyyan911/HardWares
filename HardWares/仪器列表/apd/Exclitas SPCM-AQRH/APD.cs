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
            list.Add(new Parameter("APDReceiveChannelNames", "APD输入计数器通道", APDReceiveChannelNames.GetType(), this, true) { IsReadOnly = false });
            list.Add(new Parameter("CounterOutTriggerChannel1Names", "外部触发计数通道1", CounterOutTriggerChannel1Names.GetType(), this, true) { IsReadOnly = false });
            list.Add(new Parameter("CounterOutTriggerChannel2Names", "外部触发计数通道2", CounterOutTriggerChannel1Names.GetType(), this, true) { IsReadOnly = false });
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
        int sampleCount = 0;
        /// <summary>
        /// 获取计数(/s)
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<int> GetCounts(int timeout)
        {
            int time = 0;
            while (time < timeout && stream.AvailableSamplesPerChannel < sampleCount)
            {
                Thread.Sleep(20);
                time += 20;
            }
            if (time < timeout)
            {
                return countReader.ReadMultiSampleInt32(-1).ToList();
            }
            var ll = countReader.ReadMultiSampleInt32(-1).ToList();
            return new List<int>();
        }

        CounterSingleChannelReader countReader = null;
        DaqStream stream = null;

        /// <summary>
        /// 开始采样 sampleCount表示采样数
        /// </summary>
        public override void BeginSample(APDTriggerChannels channel, int sampleCount)
        {
            try
            {
                IsReading = false;
                #region 结束旧任务
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
                var strs = DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External);
                foreach (var item in strs)
                {
                    try
                    {
                        DaqContinusReceiveTask = new NationalInstruments.DAQmx.Task();
                        CIChannel ch = DaqContinusReceiveTask.CIChannels.CreateCountEdgesChannel(item, string.Empty, CICountEdgesActiveEdge.Rising, 0, CICountEdgesCountDirection.Up);
                        ch.CountEdgesTerminal = APDReceiveChannelName;
                        string connectchannelName = channel == APDTriggerChannels.Channel1 ? CounterOutTriggerChannel1Name : CounterOutTriggerChannel2Name;
                        DaqContinusReceiveTask.Timing.ConfigureSampleClock(connectchannelName, 1e+7, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, sampleCount == 1 ? 2 : sampleCount + 100);
                        ch.DataTransferMechanism = CIDataTransferMechanism.Dma;
                        DaqContinusReceiveTask.Start();
                        this.sampleCount = sampleCount == 1 ? 2 : sampleCount;
                        DaqContinusReceiveTask.Stream.Timeout = 50;
                        stream = DaqContinusReceiveTask.Stream;
                        countReader = new CounterSingleChannelReader(stream);
                        IsAPDSampling = true;
                        Thread.Sleep(15);
                        return;
                    }
                    catch (Exception ex)
                    {
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
                        continue;
                    }
                }

                //设置采样
                IsAPDSampling = false;
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
            IsAPDSampling = false;
            #region 结束任务
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
