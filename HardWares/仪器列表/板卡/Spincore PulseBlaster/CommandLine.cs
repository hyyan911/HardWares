using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.仪器列表.板卡.Spincore_PulseBlaster
{

    public abstract class CommandBase
    {
        /// <summary>
        /// 需要拉到高电平的通道序号
        /// </summary>
        public List<int> ChannelIndexes { get; set; } = new List<int>();
        /// <summary>
        /// 持续时间(ns)
        /// </summary>
        public int TimeLength { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        internal CommandTypes CommandTpye { get; set; } = CommandTypes.Continue;
    }
    /// <summary>
    /// 
    /// </summary>
    public class CommandLine : CommandBase
    {
        public CommandLine(List<int> channelIndexes, int timeLength)
        {
            ChannelIndexes = channelIndexes;
            TimeLength = timeLength;
        }
    }

    /// <summary>
    /// 分支语句
    /// </summary>
    public class BranchCommandLine : CommandBase
    {
        /// <summary>
        /// 需要跳转的指令序号
        /// </summary>
        public int BranchTo { get; set; } = 0;

        public BranchCommandLine(List<int> channelIndexes, int timeLength, int branchTo)
        {
            ChannelIndexes = channelIndexes;
            TimeLength = timeLength;
            BranchTo = branchTo;
        }
    }

}
