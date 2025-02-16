using HardWares.仪器列表.板卡.Spincore_PulseBlaster;
using HardWares.端口基类部分;
using SpinCore.SpinAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.板卡.Spincore_PulseBlaster
{
    /// <summary>
    /// 
    /// </summary>
    public partial class PulseBlaster
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Spincore PulseBlaster";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        private double frequency = 400;
        /// <summary>
        /// 板卡频率(MHz)
        /// </summary>
        public double Frequency
        {
            get
            {
                return frequency;
            }
            set
            {
                (Instance as SpinAPI).SetFrequency(value);
                frequency = value;
            }
        }

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>();
        }
        /// <summary>
        /// 开始编程
        /// </summary>
        internal void BeginProgram()
        {
            (Instance as SpinAPI).StartProgramming(ProgramTarget.PULSE_PROGRAM);
        }

        /// <summary>
        /// 终止执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void End()
        {
            (Instance as SpinAPI).Stop();
        }

        /// <summary>
        /// 终止编程
        /// </summary>
        internal void EndProgram()
        {
            (Instance as SpinAPI).StopProgramming();
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Start()
        {
            (Instance as SpinAPI).Start();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetCommands(List<CommandLine> lines)
        {
            BeginProgram();
            foreach (var item in lines)
            {
                if (item == lines.Last()) item.CommandTpye = CommandTypes.End;
                AddCommand(item);
            }
            EndProgram();
        }

        internal void AddCommand(CommandLine line)
        {
            int flag = 0;
            for (int i = 0; i < line.ChannelIndexes.Count; i++)
            {
                if (line.ChannelIndexes[i] > 20 || line.ChannelIndexes[i] < 0)
                {
                    continue;
                }
                int temp = 0b1 << line.ChannelIndexes[i];
                flag |= temp;
            }
            int noperiod = 0b111 << 21;
            (Instance as SpinAPI).PBInstDirect(noperiod | flag, ConvertToOpCode(line.CommandTpye), 0, line.TimeLength);
        }

        internal OpCode ConvertToOpCode(CommandTypes commandtype)
        {
            if (commandtype == CommandTypes.Continue)
            {
                return OpCode.CONTINUE;
            }
            if (commandtype == CommandTypes.Wait)
            {
                return OpCode.CONTINUE;
            }
            if (commandtype == CommandTypes.Wait)
            {
                return OpCode.WAIT;
            }

            return OpCode.CONTINUE;
        }

        public override void ValidateParams()
        {
        }

        internal override Encoding GetCoder()
        {
            return Encoding.UTF8;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }
    }
}
