using HardWares.数据处理.SCPI指令;
using HardWares.源表;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.源表.KEITHLEY_2400
{
    public partial class PowerSource : PowerSourceBase
    {
        /// <summary>
        /// 产品名称 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "KEITHLEY 2400";

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
            return;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            QueryState = false;
            QueryReturnedData = null;
            AddMessage(messagetosend);
            int time = 0;
            while (!QueryState && time < timeout)
            {
                Thread.Sleep(20);
                time += 20;
            }
            if (QueryReturnedData == null) return "";
            string res = "";
            foreach (var item in QueryReturnedData)
            {
                res += item.Replace("\n", "") + ",";
            }
            if (res != "") res = res.Remove(res.Length - 1, 1);
            return res;
        }


        #region 设备参数

        /// <summary>
        /// 执行单次测量
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

            string value = "";
            value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "VOLT").Replace("\r", "") + ";" + SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "CURR"), 3000);
            if (value == "") return new MeasureGroup();
            string[] vc = value.Split(';');
            string[] v = vc[0].Split(',');
            string[] c = vc[1].Split(',');

            try
            {
                MeasureGroup g = new MeasureGroup();
                g.Voltage = double.Parse(v[0]);
                g.Current = double.Parse(c[1]);
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
                string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SENS", "CURR", "PROT"), 1000);
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
                AddMessage(SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "SENS", "CURR", "PROT"));
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

        /// <summary>
        /// 电流是否限流
        /// </summary>
        public override bool IsCurrentLimited()
        {
            string value = ThreadSafeQuery(SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "SENS", "CURR", "PROT", "TRIP"), 1000);
            if (value == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
