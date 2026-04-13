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

namespace HardWares.纳米位移台.DAQmxController
{
    /// <summary>
    /// DAQmx控制器
    /// </summary>
    public partial class NanoController : NanoControllerBase
    {

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "DAQmx AnalogOutput Controlled Scanner";

        #region 属性部分
        internal bool IsGCS2 { get; set; } = true;

        /// <summary>
        /// 支持的函数
        /// </summary>
        internal List<string> CommandSet = new List<string>();

        /// <summary>
        /// 分隔符
        /// </summary>
        private char LF = (char)10;

        #endregion

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

        internal Parameter CreateParameterRef(string commandwrite, string commandread, string paramName, Type paramType, object parent)
        {
            if (CommandSet.Contains(commandread) || CommandSet.Contains(commandwrite))
            {
                bool readOnly = false;
                if (!CommandSet.Contains(commandwrite))
                    readOnly = true;
                if (parent.GetType().GetProperty(paramName).CanWrite == false)
                {
                    readOnly = true;
                }
                Parameter p = new Parameter(paramName, "", paramType, parent, false) { IsReadOnly = readOnly };
                return p;
            }
            return null;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }
        #endregion
    }
}
