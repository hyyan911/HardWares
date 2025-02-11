using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using HardWares.数据处理;
using HardWares.端口基类;
using HardWares.端口基类部分;

namespace HardWares.温度控制器.SRS_PTC10
{
    /// <summary>
    /// 
    /// </summary>
    public partial class TemperatureController : TemperatureControllerBase
    {
        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "SRS PTC10";

        #region 设备参数
        /// <summary>
        /// 温度传感器通道
        /// </summary>
        public override List<SensorChannelBase> SensorChannels { get; internal set; } = new List<SensorChannelBase>();

        /// <summary>
        /// 输出通道
        /// </summary>
        public override List<OutputChannelBase> OutputChannels { get; internal set; } = new List<OutputChannelBase>();

        /// <summary>
        /// 开关状态
        /// </summary>
        public override bool OutputEnable
        {
            get
            {
                string result = ThreadSafeQuery(ProcessCommand("", "outputEnable?", ""), 1000);
                if (result == "On")
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
                if (value == false)
                {
                    Send(ProcessCommand("", "outputEnable", "Off"));
                }
                else
                {
                    Send(ProcessCommand("", "outputEnable", "On"));
                }
            }
        }
        #endregion

        #region 具体操作
        /// <summary>
        /// 发送指令并等待返回数据
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="waittingtime"></param>
        /// <returns></returns>
        internal override string ThreadUnsafeQuery(string instruction, int waittingtime)
        {
            QueryReturnedData = null;
            QueryState = false;
            PortArranger.AddMessage(instruction);
            int time = 0;
            while (QueryState == false && time < waittingtime)
            {
                Thread.Sleep(30);
                time += 30;
            }

            if (QueryReturnedData == null) return "";
            if (QueryReturnedData.Count == 0) return "";

            //去除分隔符
            QueryReturnedData[0] = QueryReturnedData[0].Replace("\r", "");
            QueryReturnedData[0] = QueryReturnedData[0].Replace("\n", "");

            return QueryReturnedData[0];
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <returns></returns>
        internal void Send(string instruction)
        {
            PortArranger.AddMessage(instruction);
        }

        /// <summary>
        /// 指令预处理
        /// </summary>
        /// <param name="channelName"></param>
        /// <param name="command"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal string ProcessCommand(string channelName, string command, string value)
        {
            string leftv = channelName + command;
            //包含空格加引号
            if (channelName.Contains(' '))
            {
                leftv = "\"" + channelName + command + "\"";
            }
            if (value != "")
            {
                if (value.Contains(" "))
                {
                    value = "\"" + value + "\"";
                }
                leftv += " = " + value;
            }
            leftv += "\n";
            return leftv;
        }

        /// <summary>
        /// 获取所有可用参数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override List<Parameter> AvailableParameterNames()
        {
            throw new NotImplementedException();
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }
        #endregion
    }

    /// <summary>
    /// 温度传感器通道
    /// </summary>
    public partial class SensorChannel : SensorChannelBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parent"></param>
        /// <param name="unit"></param>
        public SensorChannel(string name, TemperatureController parent, string unit) : base(name, parent, unit)
        {
            ParentDevice = parent;
        }

        /// <summary>
        /// 温度值
        /// </summary>
        public override double Temperature
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".value?", ""), 1000);
                double value = 0;
                if (!double.TryParse(result, out value))
                    return double.NaN;
                return value;
            }
        }

        /// <summary>
        /// 传感器类型
        /// </summary>

        public SensorType Sensor
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".sensor?", ""), 1000);
                if (result == "RTD")
                {
                    return SensorType.RTD;
                }
                if (result == "Thermistor")
                {
                    return SensorType.Thermistors;
                }
                if (result == "ROX")
                {
                    return SensorType.ROX;
                }
                if (result == "Diode")
                {
                    return SensorType.Diode;
                }
                if (result == "E")
                {
                    return SensorType.Thermocouple_E;
                }
                if (result == "J")
                {
                    return SensorType.Thermocouple_J;
                }
                if (result == "K")
                {
                    return SensorType.Thermocouple_K;
                }
                if (result == "N")
                {
                    return SensorType.Thermocouple_N;
                }
                if (result == "T")
                {
                    return SensorType.Thermocouple_T;
                }
                return SensorType.Unknown;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                if (value == SensorType.RTD)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "RTD"));
                }
                if (value == SensorType.Thermistors)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "Thermistor"));
                }
                if (value == SensorType.Diode)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "Diode"));
                }
                if (value == SensorType.ROX)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "ROX"));
                }
                if (value == SensorType.Thermocouple_E)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "E"));
                }
                if (value == SensorType.Thermocouple_J)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "J"));
                }
                if (value == SensorType.Thermocouple_K)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "K"));
                }
                if (value == SensorType.Thermocouple_N)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "N"));
                }
                if (value == SensorType.Thermocouple_T)
                {
                    control.Send(control.ProcessCommand(Name, ".sensor", "T"));
                }
            }
        }

        /// <summary>
        /// 参数A
        /// </summary>
        public double Param_A
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".cal.A?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".cal.A", DataProcess.ToDecimalString(value)));
            }
        }
        /// <summary>
        /// 参数B
        /// </summary>

        public double Param_B
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".cal.B?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".cal.B", DataProcess.ToDecimalString(value)));
            }
        }
        /// <summary>
        /// 参数C
        /// </summary>

        public double Param_C
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".cal.C?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".cal.C", DataProcess.ToDecimalString(value)));
            }
        }
        /// <summary>
        /// 参数R0
        /// </summary>

        public double Param_R0
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".cal.R0?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".cal.R0", DataProcess.ToDecimalString(value)));
            }
        }

        public override PortObject ParentDevice { get; internal set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> param = new List<Parameter>();
            param.Add(new Parameter("Param_A", "参数A", typeof(double), this, true));
            param.Add(new Parameter("Param_B", "参数B", typeof(double), this, true));
            param.Add(new Parameter("Param_C", "参数C", typeof(double), this, true));
            param.Add(new Parameter("Param_R0", "参数R0", typeof(double), this, true));
            param.Add(new Parameter("Sensor", "传感器类型", typeof(SensorType), this, true));
            param.Add(new Parameter("Temperature", "温度", typeof(double), this, false));
            return param;
        }
    }

    /// <summary>
    /// 输出通道
    /// </summary>
    public partial class OutputChannel : OutputChannelBase
    {

        public OutputChannel(string name, TemperatureController parent, string unit) : base(name, parent, unit)
        {
            ParentDevice = parent;
        }



        /// <summary>
        /// 输出功率(W)
        /// </summary>
        public override double Power
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".value?", ""), 1000);
                double value = 0;
                if (!double.TryParse(result, out value))
                    return double.NaN;
                return value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> param = new List<Parameter>();
            param.Add(new Parameter("LowPowerLimit", "功率限制下限", typeof(double), this, true));
            param.Add(new Parameter("HiPowerLimit", "功率限制上限", typeof(double), this, true));
            param.Add(new Parameter("P", "PID参数:P", typeof(double), this, true));
            param.Add(new Parameter("I", "PID参数:I", typeof(double), this, true));
            param.Add(new Parameter("D", "PID参数:D", typeof(double), this, true));
            param.Add(new Parameter("IOType", "IO测量方式", typeof(IOTypes), this, true));
            param.Add(new Parameter("I", "PID参数:I", typeof(double), this, true));
            param.Add(new Parameter("Ramp", "变温速率", typeof(double), this, true));
            param.Add(new Parameter("PIDMode", "PID模式", typeof(PIDMode), this, true));
            param.Add(new Parameter("Power", "输出功率", typeof(double), this, false));
            param.Add(new Parameter("Ramp_T", "当前设定温度", typeof(double), this, false));

            return param;
        }

        /// <summary>
        /// 功率限制下限
        /// </summary>

        public override double LowPowerLimit
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = ParentDevice.ThreadSafeQuery(control.ProcessCommand(Name, ".Low lmt?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".Low lmt", value.ToString()));
            }
        }

        /// <summary>
        /// 功率限制上限
        /// </summary>

        public override double HiPowerLimit
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".Hi lmt?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".Hi lmt", value.ToString()));
            }
        }

        /// <summary>
        /// IO测量方式
        /// </summary>

        public IOTypes IOType
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".IO type?", ""), 1000);
                if (result == "Input")
                {
                    return IOTypes.Input;
                }
                if (result == "Set out")
                {
                    return IOTypes.Set_Out;
                }
                if (result == "Meas out")
                {
                    return IOTypes.Measure_Out;
                }
                return IOTypes.Set_Out;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                if (value == IOTypes.Input)
                {
                    control.Send(control.ProcessCommand(Name, ".IO type", "Input"));
                }
                if (value == IOTypes.Measure_Out)
                {
                    control.Send(control.ProcessCommand(Name, ".IO type", "Meas out"));
                }
                if (value == IOTypes.Set_Out)
                {
                    control.Send(control.ProcessCommand(Name, ".IO type", "Set out"));
                }
            }
        }

        /// <summary>
        /// PID参数:P
        /// </summary>

        public override double P
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.P?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".PID.P", value.ToString()));
            }
        }

        /// <summary>
        /// PID参数:P
        /// </summary>

        public override double I
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.I?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".PID.I", value.ToString()));
            }
        }

        /// <summary>
        /// PID参数:P
        /// </summary>

        public override double D
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.D?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".PID.D", value.ToString()));
            }
        }

        /// <summary>
        /// 设定温度(度)
        /// </summary>

        public override double SetPoint
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.Setpoint?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".PID.Setpoint", value.ToString()));
            }
        }

        /// <summary>
        /// PID输入通道名
        /// </summary>

        public string PIDInput
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.Input?", ""), 1000);
                return result;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".PID.Input", value));
            }
        }

        /// <summary>
        /// 变温速率
        /// </summary>

        public double Ramp
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.Ramp?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                control.Send(control.ProcessCommand(Name, ".PID.Ramp", value.ToString()));
            }
        }

        /// <summary>
        /// 当前设定温度
        /// </summary>
        public double Ramp_T
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.RampT?", ""), 1000);
                double value = 0;
                double.TryParse(result, out value);
                return value;
            }
        }

        /// <summary>
        /// PID模式
        /// </summary>

        public PIDMode PIDMode
        {
            get
            {
                TemperatureController control = ParentDevice as TemperatureController;
                string result = control.ThreadSafeQuery(control.ProcessCommand(Name, ".PID.Mode?", ""), 1000);
                if (result == "Off")
                {
                    return PIDMode.Off;
                }
                if (result == "On")
                {
                    return PIDMode.On;
                }
                if (result == "Follow")
                {
                    return PIDMode.Follow;
                }
                return PIDMode.Off;
            }
            set
            {
                TemperatureController control = ParentDevice as TemperatureController;
                if (value == PIDMode.On)
                {
                    control.Send(control.ProcessCommand(Name, ".PID.Mode", "On"));
                }
                if (value == PIDMode.Off)
                {
                    control.Send(control.ProcessCommand(Name, ".PID.Mode", "Off"));
                }
                if (value == PIDMode.Follow)
                {
                    control.Send(control.ProcessCommand(Name, ".PID.Mode", "Follow"));
                }
            }
        }

        public override PortObject ParentDevice { get; internal set; } = null;
    }

}
