using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using zhinst;

namespace HardWares.Lock_In.Zurich_LockIn
{
    public partial class LockIn : LockInBase
    {
        /// <summary>
        /// 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Zurich MFLI";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public override List<Parameter> AvailableParameterNames()
        {
            var param = new List<Parameter>();
            param.Add(new Parameter("P", "P值", P.GetType(), this, true));
            param.Add(new Parameter("I", "I值", I.GetType(), this, true));
            param.Add(new Parameter("D", "D值", D.GetType(), this, true));
            param.Add(new Parameter("SetPoint", "设定值", SetPoint.GetType(), this, true));
            param.Add(new Parameter("PIDOutput", "PID输出状态", PIDOutput.GetType(), this, true));
            return param;
        }

        public override void ValidateParams()
        {
        }

        internal override Encoding GetCoder()
        {
            return Encoding.ASCII;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }

        #region 参数设置
        public override double P
        {
            get
            {
                return (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "p"));
            }
            set
            {
                (Instance as ziDotNET).setDouble(CombinePath(DevID, "pids", "0", "p"), value);
            }
        }

        public override double I
        {
            get
            {
                return (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "i"));
            }
            set
            {
                (Instance as ziDotNET).setDouble(CombinePath(DevID, "pids", "0", "i"), value);
            }
        }

        public override double D
        {
            get
            {
                return (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "d"));
            }
            set
            {
                (Instance as ziDotNET).setDouble(CombinePath(DevID, "pids", "0", "d"), value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override double SetPoint
        {
            get
            {
                return (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "setpoint"));
            }
            set
            {
                (Instance as ziDotNET).setDouble(CombinePath(DevID, "pids", "0", "setpoint"), value);
            }
        }

        public override bool PIDOutput
        {
            get
            {
                var value = (Instance as ziDotNET).getInt(CombinePath(DevID, "pids", "0", "enable"));
                if (value == 0) return true;
                return false;
            }
            set
            {
                (Instance as ziDotNET).setInt(CombinePath(DevID, "pids", "0", "enable"), value ? 0 : 1);
            }
        }

        public override double DemodX
        {
            get
            {
                return (Instance as ziDotNET).getDemodSample(CombinePath(DevID, "demods", "0", "sample")).x;
            }
        }

        public override double DemodY
        {
            get
            {
                return (Instance as ziDotNET).getDemodSample(CombinePath(DevID, "demods", "0", "sample")).y;
            }
        }

        public override double DemodR
        {
            get
            {
                var sample = (Instance as ziDotNET).getDemodSample(CombinePath(DevID, "demods", "0", "sample"));
                return Math.Sqrt(sample.x * sample.x + sample.y * sample.y);
            }
        }

        public override double DemodAngle
        {
            get
            {
                var sample = (Instance as ziDotNET).getDemodSample(CombinePath(DevID, "demods", "0", "sample"));
                return Math.Atan2(sample.x, sample.y);
            }
        }

        public override double PIDValue
        {
            get
            {
                var sample = (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "value"));
                return sample;
            }
        }

        public override double PIDOutputUpperLimit
        {
            get
            {
                var sample = (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "limitupper"));
                return sample;
            }
        }

        public override double PIDOutputLowerLimit
        {
            get
            {
                var sample = (Instance as ziDotNET).getDouble(CombinePath(DevID, "pids", "0", "limitlower"));
                return sample;
            }
        }
        #endregion

        private string CombinePath(params string[] paths)
        {
            string res = "/";
            foreach (var item in paths)
            {
                res += item + "/";
            }
            return res.Remove(res.Length - 1, 1);
        }
    }
}
