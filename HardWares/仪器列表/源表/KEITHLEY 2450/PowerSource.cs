using HardWares.数据处理.SCPI指令;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using NationalInstruments.Visa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.源表.KEITHLEY_2450
{
    public partial class PowerSource : PowerSourceBase
    {
        /// <summary>
        /// 产品名称 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "KEITHLEY 2450";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> result = new List<Parameter>();
            result.Add(new Parameter("CurrentLimit", "电流限流值", typeof(double), this, true) { IsReadOnly = false });
            result.Add(new Parameter("Output", "输出状态", typeof(bool), this, true) { IsReadOnly = false });
            result.Add(new Parameter("TargetVoltage", "设置电压", typeof(double), this, true) { IsReadOnly = false });
            return result;
        }

        public override void ValidateParams()
        {
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return VISAResourceHelperBase.ThreadUnsafeQuery(this, messagetosend, timeout);
        }

        #region 设备参数

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCurrentLimited()
        {
            string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SOUR", "VOLT", "ILIM", "TRIP"), 1000);
            if (value == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 测量
        /// </summary>
        /// <returns></returns>
        public override MeasureGroup Measure()
        {
            //如果电源未打开则返回NAN
            if (Output == false)
            {
                return new MeasureGroup() { TimeStamp = DateTime.Now };
            }

            DateTime time = new DateTime();

            string valuev = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "VOLT"), 3000);
            string valuec = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "CURR"), 3000);

            try
            {
                MeasureGroup g = new MeasureGroup();
                g.Voltage = double.Parse(valuev);
                g.Current = double.Parse(valuec);
                g.TimeStamp = DateTime.Now;
                g.IsCurrentLimited = IsCurrentLimited();
                return g;
            }
            catch (Exception)
            {
                return new MeasureGroup() { TimeStamp = DateTime.Now };
            }
        }

        /// <summary>
        /// 限流值
        /// </summary>
        public override double CurrentLimit
        {
            get
            {
                string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SOUR", "VOLT", "ILIM"), 1000);
                try
                {
                    return Convert.ToDouble(value);
                }
                catch (Exception ex)
                {
                    return double.NaN;
                }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "SOUR", "VOLT", "ILIM"));
            }
        }


        /// <summary>
        /// 输出状态
        /// </summary>
        public override bool Output
        {
            get
            {
                string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "OUTP"), 1000);
                if (value == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value ? "1" : "0" }, "OUTP"));
            }
        }

        /// <summary>
        /// 设定电压
        /// </summary>
        internal override double InternalTargetVoltage
        {
            get
            {
                string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SOUR", "VOLT"), 1000);
                try
                {
                    return Convert.ToDouble(value);
                }
                catch (Exception ex)
                {
                    return double.NaN;
                }
            }
            set
            {
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "SOUR", "VOLT"));
            }
        }

        #endregion

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }
    }
}
