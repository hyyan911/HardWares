using HardWares.端口基类部分;
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
            List<Parameter> parameters = new List<Parameter>();
            return parameters;
        }

        public override void ValidateParams()
        {
            return;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            try
            {
                (Instance as UsbSession).TimeoutMilliseconds = timeout;
                (Instance as UsbSession).RawIO.Write(messagetosend);
                return ((Instance as UsbSession).RawIO.ReadString().Replace("\n", ""));
            }
            catch (Exception ex) { return ""; }
        }

        /// <summary>
        /// 生成SCPI指令
        /// </summary>
        /// <returns></returns>
        internal string GenerateSCPICommannd(bool isQuery, bool[] isnumber, string[] values, params string[] commands)
        {
            string command = "";
            foreach (var item in commands)
            {
                command += ":" + item;
            }
            if (isQuery)
            {
                command += "?";
            }
            for (int i = 0; i < isnumber.Count(); ++i)
            {
                if (isnumber[i])
                {
                    command += " " + values[i];
                }
                else
                {
                    command += " " + "\"" + values[i] + "\"";
                }
            }

            return command.Remove(0, 1) + "\n";
        }

        #region 设备参数

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool IsCurrentLimited()
        {
            string value = ThreadSafeQuery(GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SOUR", "VOLT", "ILIM", "TRIP"), 1000);
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

            string valuev = ThreadSafeQuery(GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "VOLT"), 3000);
            string valuec = ThreadSafeQuery(GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "CURR"), 3000);

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
                string value = ThreadSafeQuery(GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SOUR", "VOLT", "ILIM"), 1000);
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
                AddMessage(GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "SOUR", "VOLT", "ILIM"));
            }
        }


        /// <summary>
        /// 输出状态
        /// </summary>
        public override bool Output
        {
            get
            {
                string value = ThreadSafeQuery(GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "OUTP"), 1000);
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
                AddMessage(GenerateSCPICommannd(false, new bool[] { true }, new string[] { value ? "1" : "0" }, "OUTP"));
            }
        }

        /// <summary>
        /// 设定电压
        /// </summary>
        internal override double InternalTargetVoltage
        {
            get
            {
                string value = ThreadSafeQuery(GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SOUR", "VOLT"), 1000);
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
                AddMessage(GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "SOUR", "VOLT"));
            }
        }

        #endregion
    }
}
