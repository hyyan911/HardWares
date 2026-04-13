using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace HardWares.端口基类部分
{
    public class Parameter
    {
        /// <summary>
        /// 参数名
        /// </summary>
        public string ParameterName { get; internal set; } = "";

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; internal set; } = "";

        /// <summary>
        /// 参数是否只读
        /// </summary>
        public bool IsReadOnly { get; internal set; } = false;

        /// <summary>
        /// 是否是静态参数
        /// </summary>
        public bool IsStatic { get; internal set; } = true;

        /// <summary>
        /// 参数类型
        /// </summary>
        public Type ParamType { get; internal set; } = null;

        /// <summary>
        /// 所属设备
        /// </summary>
        public object Parent { get; internal set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public Parameter(string Name, string description, Type type, object parent, bool isStatic)
        {
            ParameterName = Name;
            Description = description;
            ParamType = type;
            Parent = parent;
            IsStatic = isStatic;
        }

        /// <summary>
        /// 写参数
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="value"></param>
        public void WriteValue(dynamic value)
        {
            if (IsReadOnly) return;
            Parent.GetType().GetProperty(ParameterName).SetValue(Parent, value);
        }

        /// <summary>
        /// 写参数
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="value"></param>
        public dynamic ReadValue()
        {
            return Parent.GetType().GetProperty(ParameterName).GetValue(Parent);
        }
    }
}
