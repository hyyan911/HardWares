using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    /// <summary>
    /// 静态参数特性
    /// </summary>
    public class StaticParameter : Attribute
    {
    }
}
