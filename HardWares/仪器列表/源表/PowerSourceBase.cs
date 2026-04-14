using HardWares.源表;
using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uc480.Defines;

namespace HardWares.源表
{
    /// <summary>
    /// 源表基类
    /// </summary>
    public abstract class PowerSourceBase : PortObject
    {
        /// <summary>
        /// 设备种类
        /// </summary>
        public override string ProductType { get; internal set; } = "源表";

        internal abstract double InternalTargetVoltage { get; set; }
        /// <summary>
        /// 设置电压
        /// </summary>
        public double TargetVoltage
        {
            get { return InternalTargetVoltage; }
            set
            {
                SetVoltageWithRamp(value);
            }
        }

        /// <summary>
        /// 电流限流值
        /// </summary>
        public abstract double CurrentLimit { get; set; }

        /// <summary>
        /// 输出状态
        /// </summary>
        public abstract bool Output { get; set; }

        /// <summary>
        /// 电压变压步长
        /// </summary>
        public double VoltageRampStep { get; set; } = 0.1;

        /// <summary>
        /// 电压变压间隔(毫秒)
        /// </summary>
        public int VoltageRampGap { get; set; } = 1000;

        /// <summary>
        /// 电流是否限流
        /// </summary>
        public abstract bool IsCurrentLimited();

        /// <summary>
        /// 连续变化电压
        /// </summary>
        /// <param name="value"></param>
        internal void SetVoltageWithRamp(double value)
        {
            //获取当前电压设置值
            double curvolt = InternalTargetVoltage;

            if (double.IsNaN(curvolt) == true) return;

            int sgn = (value - curvolt) < 0 ? -1 : 1;

            double volt = curvolt;
            while (sgn * (volt - value) < 0)
            {
                InternalTargetVoltage = volt;
                volt += sgn * VoltageRampStep;
                Thread.Sleep(VoltageRampGap);
            }
            InternalTargetVoltage = value;
        }

        /// <summary>
        /// 执行单次测量,执行完成后挂起
        /// </summary>
        /// <returns></returns>
        public abstract MeasureGroup Measure();

    }
}
