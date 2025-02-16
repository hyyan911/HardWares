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

    }
    /// <summary>
    /// 
    /// </summary>
    public class CommandLine : CommandBase
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
        public CommandTypes CommandTpye { get; set; } = CommandTypes.Continue;

        public CommandLine(List<int> channelIndexes, int timeLength)
        {
            ChannelIndexes = channelIndexes;
            TimeLength = timeLength;
        }
    }

}
