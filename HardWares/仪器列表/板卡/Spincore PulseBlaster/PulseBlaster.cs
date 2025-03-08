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
    public partial class PulseBlaster : PulseBlasterBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Spincore PulseBlaster";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        override public double PulseFrequency { get; set; } = 500;

        private double frequency = 500;
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
                (Instance as SpinAPI).Init();
                (Instance as SpinAPI).SetFrequency(value);
                (Instance as SpinAPI).Close();
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
            (Instance as SpinAPI).Init();
            (Instance as SpinAPI).SetClock(500);
            (Instance as SpinAPI).StartProgramming(ProgramTarget.PULSE_PROGRAM);
        }

        /// <summary>
        /// 终止执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void End()
        {
            (Instance as SpinAPI).Init();
            (Instance as SpinAPI).Stop();
            (Instance as SpinAPI).Close();
        }

        /// <summary>
        /// 终止编程
        /// </summary>
        internal void EndProgram()
        {
            (Instance as SpinAPI).StopProgramming();
            (Instance as SpinAPI).Close();
        }

        /// <summary>
        /// 开始执行
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void Start()
        {
            (Instance as SpinAPI).Init();
            (Instance as SpinAPI).Start();
            (Instance as SpinAPI).Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void SetCommands(List<CommandBase> lines)
        {
            BeginProgram();
            foreach (var item in lines)
            {
                AddCommand(item);
            }
            EndProgram();
        }

        internal void AddCommand(CommandBase line)
        {
            int flag = 0;
            OpCode commandtype = OpCode.CONTINUE;
            int time = 10000;
            List<int> inds = new List<int>();
            if (line is CommandLine)
            {
                inds = (line as CommandLine).ChannelIndexes;
                time = (line as CommandLine).TimeLength;
            }
            for (int i = 0; i < inds.Count; i++)
            {
                if (inds[i] > 20 || inds[i] < 0)
                {
                    continue;
                }
                int temp = 0b1 << inds[i];
                flag |= temp;
            }
            int noperiod = 0b111 << 21;
            int instruct = 0;
            if (line is BranchCommandLine)
            {
                instruct = (line as BranchCommandLine).BranchTo;
                commandtype = OpCode.BRANCH;
            }
            if (line is LoopStartCommandLine)
            {
                instruct = (line as LoopStartCommandLine).NumberOfLoop;
                commandtype = OpCode.LOOP;
            }
            if (line is LoopEndCommandLine)
            {
                instruct = (line as LoopEndCommandLine).IndexOfLoopStart;
                commandtype = OpCode.END_LOOP;
            }
            if (line is EndCommandLine)
            {
                instruct = 0;
                commandtype = OpCode.STOP;
            }

            try
            {
                (Instance as SpinAPI).PBInstDirect(noperiod | flag, commandtype, instruct, time);
            }
            catch (Exception)
            {
            }
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
