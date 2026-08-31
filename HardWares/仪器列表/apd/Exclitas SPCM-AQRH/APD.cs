using HardWares.端口基类部分;
using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HardWares.APD.Exclitas_SPCM_AQRH
{
    public partial class APD : APDBase
    {
        public override string ProductIdentifier { get; internal set; } = "Exclitas SPCM-AQRH";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        private Task _task;
        private DaqStream _stream;
        private CounterReader _reader;

        private int _sampleCount;
        private string _continusHistoryChannel;
        private string _pulseHistoryChannel;

        private static readonly string[] _allCiChannels =
            DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.CI, PhysicalChannelAccess.External);

        #region Base 接口

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>
            {
                new Parameter("APDReceiveChannelNames", "APD输入计数器通道", APDReceiveChannelName.GetType(), this, true),
                new Parameter("CounterOutTriggerChannel1Names", "外部触发计数通道1", CounterOutTriggerChannel1Name.GetType(), this, true),
                new Parameter("CounterOutTriggerChannel2Names", "外部触发计数通道2", CounterOutTriggerChannel2Name.GetType(), this, true)
            };
        }

        public override void ValidateParams() { }

        internal override Encoding GetCoder() => Encoding.ASCII;
        internal override string ThreadUnsafeQuery(string messagetosend, int timeout) => "";

        public override void SetSampleCount(int samplecount)
        {
            if (samplecount <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplecount));

            _sampleCount = samplecount == 1 ? 2 : samplecount;
        }

        /// <summary>
        /// 开始采样（每次都是全新 Task）
        /// </summary>
        public override void BeginSample(APDTriggerChannels channel, int sampleCount)
        {
            SetSampleCount(sampleCount);

            // ✅ 强制确保旧资源已释放
            ForceDisposeTask();

            string ciChannel = ResolveCiChannel(channel, false);
            CreateTask(channel, ciChannel);
            _task.Start(); // ✅ 此时资源一定是空闲的
        }

        public override List<int> GetCounts(int timeout)
        {
            if (_reader == null || _task == null)
                throw new InvalidOperationException("APD 任务未启动");

            try
            {
                _stream.Timeout = timeout;
                int[] data = _reader.ReadMultiSampleInt32(_sampleCount);
                return data.ToList();
            }
            finally
            {
                // ✅ 无论成功失败，立刻释放
                ForceDisposeTask();
            }
        }

        public override void EndSample()
        {
            ForceDisposeTask();
        }

        public override void ClearBuffer() { }

        #endregion

        #region 连续采样
        private bool _isContinuousMode;
        public bool IsContinuousMode => _isContinuousMode;
        /// <summary>
        /// 启动连续采样（不创建线程）
        /// </summary>
        /// <param name="channel">触发通道</param>
        /// <param name="samplesPerRead">每次 Read 读取的点数</param>
        public void StartContinuous(APDTriggerChannels channel, int samplesPerRead)
        {
            if (_isContinuousMode)
                throw new InvalidOperationException("连续采样已启动");

            if (samplesPerRead <= 0)
                throw new ArgumentOutOfRangeException(nameof(samplesPerRead));

            _sampleCount = samplesPerRead == 1 ? 2 : samplesPerRead;

            // 固定使用已验证过的 CI 通道
            string ciChannel = ResolveCiChannel(channel, true);
            CreateContinuousTask(channel, ciChannel);

            _task.Start();
            _isContinuousMode = true;
        }

        /// <summary>
        /// 读取连续采样数据（由外部线程周期性调用）
        /// </summary>
        /// <param name="timeout">读取超时（ms）</param>
        /// <returns>采样数据</returns>
        public IList<int> GetContinuousCount(int timeout = 1000)
        {
            if (!_isContinuousMode || _reader == null)
                throw new InvalidOperationException("连续采样未启动");

            try
            {
                _stream.Timeout = timeout;
                int[] data = _reader.ReadMultiSampleInt32(_sampleCount);
                return data;
            }
            catch (DaqException ex) when (ex.Error == -200284)
            {
                // 超时：返回空集合，由外部决定如何处理
                return Array.Empty<int>();
            }
        }

        /// <summary>
        /// 停止连续采样
        /// </summary>
        public void StopContinuous()
        {
            if (!_isContinuousMode)
                return;

            try
            {
                _task?.Stop();
            }
            finally
            {
                ForceDisposeTask();
                _isContinuousMode = false;
                _sampleCount = 0;
            }
        }

        private string ResolveClockSource(APDTriggerChannels channel)
        {
            return channel == APDTriggerChannels.Channel1
                ? CounterOutTriggerChannel1Name
                : CounterOutTriggerChannel2Name;
        }

        private void CreateContinuousTask(APDTriggerChannels channel, string ciChannel)
        {
            ForceDisposeTask();

            string clockSource = ResolveClockSource(channel);

            _task = new Task();

            var ch = _task.CIChannels.CreateCountEdgesChannel(
                ciChannel,
                "",
                CICountEdgesActiveEdge.Rising,
                0,
                CICountEdgesCountDirection.Up);

            ch.CountEdgesTerminal = APDReceiveChannelName;
            ch.DataTransferMechanism = CIDataTransferMechanism.Dma;

            // ✅ Continuous Samples
            _task.Timing.ConfigureSampleClock(
                clockSource,
                _sampleCount * 10,   // 缓冲区放大，防溢出
                SampleClockActiveEdge.Rising,
                SampleQuantityMode.ContinuousSamples,
                _sampleCount);

            _stream = _task.Stream;
            _reader = new CounterReader(_stream);
        }
        #endregion

        #region DAQmx 核心（防 -50103 专用）

        private void CreateTask(APDTriggerChannels channel, string ciChannel)
        {
            _task = new Task();

            var ch = _task.CIChannels.CreateCountEdgesChannel(
                ciChannel,
                "",
                CICountEdgesActiveEdge.Rising,
                0,
                CICountEdgesCountDirection.Up);

            ch.CountEdgesTerminal = APDReceiveChannelName;
            ch.DataTransferMechanism = CIDataTransferMechanism.Dma;

            string clockSource = channel == APDTriggerChannels.Channel1
                ? CounterOutTriggerChannel1Name
                : CounterOutTriggerChannel2Name;

            _task.Timing.ConfigureSampleClock(
                clockSource,
                _sampleCount,
                SampleClockActiveEdge.Rising,
                SampleQuantityMode.FiniteSamples,
                _sampleCount);

            _stream = _task.Stream;
            _reader = new CounterReader(_stream);
        }

        private string ResolveCiChannel(APDTriggerChannels channel, bool isContinus)
        {
            // ✅ 已经找到过，直接用
            if (!string.IsNullOrEmpty(isContinus ? _continusHistoryChannel : _pulseHistoryChannel))
                return isContinus ? _continusHistoryChannel : _pulseHistoryChannel;

            foreach (var ci in _allCiChannels)
            {
                if (TryStartChannel(channel, ci))
                {
                    if (isContinus)
                    {
                        _continusHistoryChannel = ci;
                    }
                    else
                    {
                        _pulseHistoryChannel = ci;
                    }
                    return ci;
                }
            }

            throw new InvalidOperationException("未找到可用的 CI 计数通道（-50103）");
        }

        private bool TryStartChannel(APDTriggerChannels channel, string ciChannel)
        {
            Task testTask = null;
            try
            {
                testTask = new Task();

                // CI 计数通道
                var ch = testTask.CIChannels.CreateCountEdgesChannel(
                    ciChannel,
                    "",
                    CICountEdgesActiveEdge.Rising,
                    0,
                    CICountEdgesCountDirection.Up);

                ch.CountEdgesTerminal = APDReceiveChannelName;
                ch.DataTransferMechanism = CIDataTransferMechanism.Dma;

                // 采样时钟（关键：可能和 CI 冲突）
                string clockSource = channel == APDTriggerChannels.Channel1
                    ? CounterOutTriggerChannel1Name
                    : CounterOutTriggerChannel2Name;

                testTask.Timing.ConfigureSampleClock(
                    clockSource,
                    _sampleCount,
                    SampleClockActiveEdge.Rising,
                    SampleQuantityMode.FiniteSamples,
                    _sampleCount);

                // ✅ 真正占用硬件
                testTask.Start();

                // 能跑到这里，说明这个 CI 通道真的可用
                return true;
            }
            catch (DaqException ex) when (ex.Error == -50103 || ex.Error == -200220)
            {
                // -50103：资源被占用
                // -200220：终端已被保留
                return false;
            }
            catch
            {
                return false;
            }
            finally
            {
                // ✅ 必须彻底释放，否则下一个通道还会 -50103
                try { testTask?.Stop(); } catch { }
                try { testTask?.Dispose(); } catch { }

                // ✅ 给 DAQmx 一点时间释放硬件
                Thread.Sleep(20);
            }
        }

        private bool IsChannelAvailable(APDTriggerChannels channel, string ciChannel)
        {
            try
            {
                using (var testTask = new Task())
                {
                    var ch = testTask.CIChannels.CreateCountEdgesChannel(
                        ciChannel, "", CICountEdgesActiveEdge.Rising, 0, CICountEdgesCountDirection.Up);

                    ch.CountEdgesTerminal = APDReceiveChannelName;

                    string clockSource = channel == APDTriggerChannels.Channel1
                        ? CounterOutTriggerChannel1Name
                        : CounterOutTriggerChannel2Name;

                    testTask.Timing.ConfigureSampleClock(
                        clockSource,
                        _sampleCount,
                        SampleClockActiveEdge.Rising,
                        SampleQuantityMode.FiniteSamples,
                        _sampleCount);

                    testTask.Control(TaskAction.Verify);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// ✅ 强制释放 + 延迟保护（解决 -50103 的关键）
        /// </summary>
        private void ForceDisposeTask()
        {
            if (_task == null)
                return;

            try { _task.Stop(); } catch { }
            try { _task.Dispose(); } catch { }

            _task = null;
            _stream = null;
            _reader = null;

            // ✅ 关键：给 DAQmx 驱动一点时间
            Thread.Sleep(20);
        }

        #endregion

        ~APD()
        {
            ForceDisposeTask();
        }
    }
}