using HardWares.数据处理.SCPI指令;
using HardWares.源表;
using HardWares.电源.Rigol_DP811;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.电源
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public class PowerChannel : PowerChannelBase
    {
        public override PortObject ParentDevice { get; internal set; } = null;

        internal int ChannelInd = 0;

        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> result = new List<Parameter>();
            result.Add(new Parameter("CurrentLimit", "电流限流值(A)", typeof(double), this, true) { IsReadOnly = false });
            result.Add(new Parameter("Output", "输出状态", typeof(bool), this, true) { IsReadOnly = false });
            result.Add(new Parameter("Voltage", "测量电压值(V)", typeof(double), this, true) { IsReadOnly = false });
            return result;
        }

        private void SelectChannel()
        {
            if ((ParentDevice as Power).ChannelCounts > 1)
            {
                ParentDevice.AddMessage(":" + SCPIGenerator.GenerateSCPICommannd(false, new bool[] { false }, new string[] { "CH" + ChannelInd.ToString() }, "INST"));
            }
        }

        #region 通道属性
        public override string ChannelName { get; set; } = "电源通道";
        /// <summary>
        /// 电压设置值
        /// </summary>
        public override double Voltage
        {
            get
            {
                try
                {
                    SelectChannel();
                    string value = ParentDevice.ThreadSafeQuery(":" + SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "VOLT"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                SelectChannel();
                ParentDevice.AddMessage(":" + SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "VOLT"));
            }
        }

        /// <summary>
        /// 电流限流值
        /// </summary>
        public override double CurrentLimit
        {
            get
            {
                try
                {
                    SelectChannel();
                    string value = ParentDevice.ThreadSafeQuery(":" + SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "CURR", "PROT"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
            set
            {
                ParentDevice.AddMessage(":" + SCPIGenerator.GenerateSCPICommannd(false, new bool[] { true }, new string[] { value.ToString() }, "CURR", "PROT"));
            }
        }

        /// <summary>
        /// 测量电压(V)
        /// </summary>
        public override double MeasuredVoltage
        {
            get
            {
                try
                {
                    SelectChannel();
                    string value = ParentDevice.ThreadSafeQuery(":" + SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
        }

        /// <summary>
        /// 测量电流（A）
        /// </summary>
        public override double MeasuredCurrent
        {
            get
            {
                try
                {
                    SelectChannel();
                    string value = ParentDevice.ThreadSafeQuery(":" + SCPIGenerator.GenerateSCPICommannd(true, new bool[] { }, new string[] { }, "MEAS", "CURR"), 2000);
                    return double.Parse(value);
                }
                catch (Exception) { return double.NaN; }
            }
        }

        /// <summary>
        /// 输出
        /// </summary>
        public override bool Output
        {
            get
            {
                try
                {
                    SelectChannel();
                    string value = ParentDevice.ThreadSafeQuery(":OUTP? " + "CH" + ChannelInd.ToString() + "\n", 2000);
                    if (value == "ON") return true;
                    return false;
                }
                catch (Exception) { return false; }
            }
            set
            {
                SelectChannel();
                ParentDevice.AddMessage(":OUTP " + "CH" + ChannelInd.ToString() + "," + (value ? "ON" : "OFF") + "\n");
            }
        }

        #endregion
    }
}
