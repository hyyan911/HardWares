using HardWares.源表;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uc480.Defines;

namespace HardWares.电源.Rigol_DP811
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public partial class Power : PortObject
    {
        /// <summary>
        /// 产品名称 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Rigol DP811";

        public override event ParamsChangeEventHandler ParamsChangedEvent;
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> result = new List<Parameter>();
            result.Add(new Parameter("CurrentLimit", "电流限流值", typeof(double), this, true) { IsReadOnly = false });
            result.Add(new Parameter("Output", "输出状态", typeof(bool), this, true) { IsReadOnly = false });
            result.Add(new Parameter("Voltage", "电压", typeof(double), this, true) { IsReadOnly = false });
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

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

    }
}
