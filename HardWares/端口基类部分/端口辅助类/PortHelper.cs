using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类.TCPIP串口;

namespace HardWares.端口基类部分.VISA资源辅助类
{
    internal abstract class PortResourceHelper
    {
        protected List<KeyValuePair<string, string>> DevNames = new List<KeyValuePair<string, string>>();
        /// <summary>
        /// 获取ProductName
        /// </summary>
        protected Func<string, string> GetProductName = null;

        protected PortResourceHelper(Func<string, string> getProductName)
        {
            GetProductName = getProductName;
        }
    }
}
