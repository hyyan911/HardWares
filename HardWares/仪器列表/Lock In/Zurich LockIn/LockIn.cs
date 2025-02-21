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
            return new List<Parameter>();
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
