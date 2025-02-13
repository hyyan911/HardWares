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
using HardWares.端口基类部分.设备信息;
using HardWares.端口基类部分.PortHelper;
using HardWares.纳米位移台.PI;

namespace HardWares.纳米位移台.Micronix
{
    public partial class NanoController : NanoControllerBase
    {
        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Micronix";

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

        internal string ProcessCmd(string axisind, string cmd, params string[] ps)
        {
            string res = "";
            res += axisind + cmd;
            foreach (var item in ps)
            {
                res += item + ",";
            }
            if (res.Last() == ',') res = res.Remove(res.Length - 1, 1);
            res += "\n\r";
            return res;
        }

        internal string ProcessResult(List<string> result)
        {
            foreach (var item in result)
            {
                string str = item.Replace("\n", "");
                str = str.Replace("\n", "");
                if (str.Trim() != "") return str.Trim();
            }
            return "";
        }

        /// <summary>
        /// 处理数字
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        internal double ProcessNum(string result)
        {
            try
            {
                string num = result.Substring(4, result.Length - 4 + 1);
                return double.Parse(num);
            }
            catch (Exception)
            {

                return double.NaN;
            }
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
