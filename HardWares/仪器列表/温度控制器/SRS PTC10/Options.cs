using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.温度控制器.SRS_PTC10
{
    /// <summary>
    /// 单位类型
    /// </summary>
    public enum UnitType
    {
        /// <summary>
        /// 摄氏度
        /// </summary>
        C = 0,
        /// <summary>
        /// 开尔文
        /// </summary>
        K = 1,
        /// <summary>
        /// 华氏度
        /// </summary>
        F = 2,
        /// <summary>
        /// 传感器值
        /// </summary>
        Sensor = 3
    }

    /// <summary>
    /// PID模式
    /// </summary>
    public enum PIDMode
    {
        /// <summary>
        /// 开启
        /// </summary>
        On = 0,
        /// <summary>
        /// 关闭
        /// </summary>
        Off = 1,
        /// <summary>
        /// 跟随输入值
        /// </summary>
        Follow = 2
    }


    /// <summary>
    /// 传感器类型
    /// </summary>
    public enum SensorType
    {
        /// <summary>
        /// 电阻传感器
        /// </summary>
        RTD = 0,

        Thermistors = 1,
        /// <summary>
        /// 二极管传感器
        /// </summary>
        Diode = 2,
        ROX = 3,
        Unknown = 4,
        /// <summary>
        /// 热电偶传感器_E
        /// </summary>
        Thermocouple_E = 5,
        /// <summary>
        /// 热电偶传感器_J
        /// </summary>
        Thermocouple_J = 6,
        /// <summary>
        /// 热电偶传感器_K
        /// </summary>
        Thermocouple_K = 7,
        /// <summary>
        /// 热电偶传感器_N
        /// </summary>
        Thermocouple_N = 8,
        /// <summary>
        /// 热电偶传感器_T
        /// </summary>
        Thermocouple_T = 9,

    }

    /// <summary>
    /// 输出状态
    /// </summary>
    public enum OutputState
    {
        /// <summary>
        /// 关闭
        /// </summary>
        Off = 0,
        /// <summary>
        /// 开启
        /// </summary>
        On = 1
    }

    public enum IOTypes
    {
        /// <summary>
        /// 显示需求功率
        /// </summary>
        Set_Out = 0,
        /// <summary>
        /// 显示实际功率
        /// </summary>
        Measure_Out = 1,
        /// <summary>
        /// 显示外部参考信号
        /// </summary>
        Input = 2
    }
}
