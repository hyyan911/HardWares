using CodeHelper;
using HardWares.APIS.PI;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.纳米位移台;
using HardWares.纳米位移台.DAQmxController;
using HardWares.纳米位移台.PI;
using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Task = NationalInstruments.DAQmx.Task;

namespace HardWares.纳米位移台.DAQmxController
{
    /// <summary>
    /// DAQmx位移台
    /// </summary>
    public class NanoStage : StageBase
    {

        /// <summary>
        /// 
        /// </summary>
        public override PortObject ParentDevice { get; internal set; } = null;
        /// <summary>
        /// 轴序号
        /// </summary>
        public override string AxisName { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="axisName"></param>
        /// <param name="parent"></param>
        public NanoStage(string axisName, NanoController parent)
        {
            AxisName = axisName;
            ParentDevice = parent;
            var filename = "DAQmx LicationLog " + ParentDevice.ProductName;
            foreach (var item in Path.GetInvalidPathChars())
            {
                filename = filename.Replace(item.ToString(), "");
            }
            foreach (var item in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(item.ToString(), "");
            }
            LocationLogSavePath = Path.Combine(Environment.CurrentDirectory, filename);
        }


        Task AOTask = null;
        /// <summary>
        /// 设置电压,默认步长为0.0001V
        /// </summary>
        /// <param name="volt"></param>
        internal void SetVoltage(double volt, int timeout = 10000)
        {
            DisposeTask();
            try
            {
                AOTask = new Task();

                AOTask.AOChannels.CreateVoltageChannel(VoltageSetChannelName, string.Empty, CustomRangeLo, CustomRangeHi, AOVoltageUnits.Volts);
                AOTask.Timing.SampleTimingType = SampleTimingType.SampleClock;
                double det = volt - target;
                int counts = (int)(Math.Abs(det) / 0.0001);

                double[] listtosend = Enumerable.Range(0, counts).Select(x => target + x * (volt - target) / (counts - 1)).ToArray();
                if (counts == 0) listtosend = new double[] { volt };

                AOTask.Timing.ConfigureSampleClock("", Velocity / 0.0001, SampleClockActiveEdge.Rising, SampleQuantityMode.ContinuousSamples, listtosend.Length);

                AOTask.Timing.SampleClockRate = Velocity / 0.0001;
                var writer = new AnalogSingleChannelWriter(AOTask.Stream);
                writer.WriteMultiSample(true, listtosend);
                int time = 0;
                while (AOTask.Stream.AvailableSamplesPerChannel != 0 && time < timeout)
                {
                    Thread.Sleep(10);
                    time += 10;
                }
                if (time >= timeout)
                {
                    target = target + (counts - AOTask.Stream.AvailableSamplesPerChannel * (volt - target)) / (counts - 1);
                }
                else
                    target = volt;
                //保存当前位置到文件
                FileObject fobj = new FileObject();
                fobj.Descriptions.Add("CurrentVolt", target.ToString());
                fobj.SaveToFile(LocationLogSavePath);
                DisposeTask();
            }
            catch (Exception e)
            {
                DisposeTask();
            }
        }


        internal string LocationLogSavePath = "";

        internal void DisposeTask()
        {
            try
            {
                AOTask?.Stop();
            }
            catch (Exception ex)
            {
            }
            try
            {
                AOTask?.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

        #region 电压设置通道
        string VoltageSetChannelName = "";
        /// <summary>
        /// APD输入计数器通道(get方法返回所有可用的通道列表，set方法设置通道名)
        /// </summary>
        public object VoltageSetChannelNames
        {
            get
            {
                return new KeyValuePair<string, List<string>>(VoltageSetChannelName, DaqSystem.Local.GetPhysicalChannels(PhysicalChannelTypes.AO, PhysicalChannelAccess.All).ToList());
            }
            set
            {
                VoltageSetChannelName = value.ToString();
            }
        }
        #endregion

        /// <summary>
        /// 当前位置(V)
        /// </summary>
        public override double Position
        {
            get
            {
                return Target;
            }
        }

        internal double target = 0;
        /// <summary>
        /// 目标位置
        /// </summary>
        public override double Target
        {
            get { return target; }
        }

        /// <summary>
        /// 速度(V/s)
        /// </summary>
        public override double Velocity { get; set; } = 0.1;

        /// <summary>
        ///位置单位
        /// </summary>
        public override string PositionUnit { get; internal set; } = "V";


        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        internal override void InnerMoveTo(double targetvalue)
        {
            SetVoltage(targetvalue, 10000);
        }

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="targetvalue"></param>
        /// <returns></returns>
        internal override void InnerMoveToAndWait(double targetvalue, int timeout)
        {
            SetVoltage(targetvalue, timeout);
        }

        /// <summary>
        /// 移动指定偏移量
        /// </summary>
        /// <param name="step"></param>
        /// <param name="timeout"></param>
        public override void MoveStepAndWait(double step, int timeout)
        {
            double target = Target;
            target += step;
            SetVoltage(target, timeout);
        }


        #region 获取可用参数
        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            NanoStage st = this;
            List<Parameter> result = new List<Parameter>();

            result.Add(new Parameter("CustomRangeLo", "自定义位置下限(V)", CustomRangeLo.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("CustomRangeHi", "自定义位置上限(V)", CustomRangeHi.GetType(), this, true) { IsReadOnly = false });

            result.Add(new Parameter("Velocity", "速度(V/s)", Velocity.GetType(), this, true) { IsReadOnly = false });

            return result;
        }

        #endregion
    }
}