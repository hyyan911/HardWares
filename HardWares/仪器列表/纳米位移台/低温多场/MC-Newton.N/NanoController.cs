using HardWares.数据处理;
using HardWares.纳米位移台;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using HardWares.APIS.PI;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.IO.Ports;
using System.Windows;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类部分;
using HardWares.纳米位移台.低温多场旋转台;
using HardWares.端口基类部分.设备信息;
using HardWares.端口基类部分.PortHelper;
using HardWares.纳米位移台.PI;

namespace HardWares.纳米位移台.低温多场.MC_Newton_N
{
    public partial class NanoController : NanoControllerBase
    {
        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "低温多场MC.Newton.N";

        /// <summary>
        /// 位移台列表
        /// </summary>
        public override List<StageBase> Stages { get; set; } = new List<StageBase>();


        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ValidateParams()
        {
        }

        #region 基本通讯方法
        internal override string ThreadUnsafeQuery(string instruction, int waittingtime)
        {
            return ProcessResult(COMHelper.ThreadUnsafeQuery(this, instruction, waittingtime));
        }

        internal string ProcessResult(List<string> result)
        {
            List<string> res = new List<string>();
            foreach (var item in result)
            {
                string v = item.Replace("[", "");
                v = v.Replace("]", "");
                if (item == "") continue;
                if (item.Contains("ReachTarg")) continue;
                if (item.Contains("MovStop")) continue;
                if (!item.Contains(":")) continue;
                res.Add(v);
            }
            if (res.Count == 0) return "";
            var r = res[0].Split(':');
            if (r.Count() < 1)
            {
                return "";
            }
            return r[1];
        }

        #endregion


        #region 参数筛选
        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> result = new List<Parameter>();
            return result;
        }
        #endregion
    }
}
