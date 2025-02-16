using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares
{
    /// <summary>
    /// PID闭环控制类
    /// </summary>
    public class PIDControlHelper
    {
        /// <summary>
        /// P参数
        /// </summary>
        public double P { get; set; } = 1;

        /// <summary>
        /// I参数 
        /// </summary>
        public double I { get; set; } = 0;

        /// <summary>
        /// D参数
        /// </summary>
        public double D { get; set; } = 0;

        /// <summary>
        /// 衰减系数(在输出结果上乘以一个系数)
        /// </summary>
        public double Attenuation { get; set; } = 1;

        /// <summary>
        /// 信号输入的时间间隔
        /// </summary>
        public double TimeGap_MillSec { get; set; } = 0;

        /// <summary>
        /// 积分限幅
        /// </summary>
        public double IntegralLimit { get; set; } = 10000;

        /// <summary>
        /// 输出范围限制
        /// </summary>
        public double OutputLimit { get; set; } = 10000;

        /// <summary>
        /// 设定值
        /// </summary>
        public double SetPoint { get; set; } = 0;

        public PIDControlHelper(double p, double i, double d, double timegap_MillSec)
        {
            P = p;
            I = i;
            D = d;
            TimeGap_MillSec = timegap_MillSec;
        }

        private double Sum { get; set; } = 0;

        private double HistoryValue { get; set; } = double.NaN;

        /// <summary>
        /// 输入一个实际值，PID根据SetPoint输出一个反馈值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double Input(double value)
        {
            if (TimeGap_MillSec == 0 && D != 0) return 0;

            double result = 0;
            //计算误差
            double err = SetPoint - value;

            //P部分
            result += P * err;

            Sum += err;
            //积分限幅
            if (Sum > IntegralLimit) Sum = IntegralLimit;
            if (Sum < -IntegralLimit) Sum = -IntegralLimit;

            //I部分
            result += I * Sum;

            if (double.IsNaN(HistoryValue) == false && D != 0)
            {
                result += (err - HistoryValue) / TimeGap_MillSec;
            }

            HistoryValue = err;

            double finalresult = result * Attenuation;

            if (finalresult > OutputLimit) finalresult = OutputLimit;
            if (finalresult < -OutputLimit) finalresult = -OutputLimit;

            return finalresult;

        }

        public double GetHostoryValue()
        {
            return HistoryValue;
        }

        public void Init()
        {
            Sum = 0;
            HistoryValue = double.NaN;
        }


        public double GetError(double value)
        {
            return SetPoint - value;
        }

        /// <summary>
        /// 清除积分值 
        /// </summary>
        public void ClearIntegrateValue()
        {
            Sum = 0;
            return;
        }
    }
}
