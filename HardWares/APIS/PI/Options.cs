using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.APIS.PI
{
    /// <summary>
    /// 反馈状态
    /// </summary>
    public enum ServoState
    {
        /// <summary>
        /// 开启反馈控制
        /// </summary>
        On = 0,
        /// <summary>
        /// 关闭反馈控制
        /// </summary>
        Off = 1

    }

    /// <summary>
    /// Reference类型
    /// </summary>
    public enum ReferenceMovingTypes
    {

    }

    /// <summary>
    /// Reference类型
    /// </summary>
    public enum ReferenceModes
    {
        None = -1,
        /// <summary>
        /// 由系统给定Reference点
        /// </summary>
        System = 0,

        /// <summary>
        /// 自行设置Reference点
        /// </summary>
        Custom = 1
    }
}
