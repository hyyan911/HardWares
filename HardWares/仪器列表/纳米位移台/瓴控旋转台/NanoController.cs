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
using HardWares.端口基类部分.设备信息;
using HardWares.端口基类部分.PortHelper;
using HardWares.纳米位移台.PI;

namespace HardWares.纳米位移台.瓴控旋转台
{
    public partial class NanoController : NanoControllerBase
    {
        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "LingKong MS Rotator";

        internal override Encoding GetCoder()
        {
            return Encoding.UTF8;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ValidateParams()
        {
        }

        #region 基本通讯方法
        internal override string ThreadUnsafeQuery(string instruction, int waittingtime)
        {
            QueryReturnedByte = null;
            QueryState = false;
            PortArranger.AddMessage(Encoding.ASCII.GetBytes(instruction).ToList());
            int time = 0;
            while (QueryState == false && time < waittingtime)
            {
                Thread.Sleep(30);
                time += 30;
            }

            if (QueryReturnedByte == null) return "";
            if (QueryReturnedByte.Count == 0) return "";

            string result = "";
            foreach (var item in QueryReturnedByte)
            {
                result += item.ToString() + "_";
            }
            if (result != "") result = result.Remove(result.Length - 1, 1);
            return result;
        }

        internal bool CheckResult(List<byte> result, out byte command, out List<byte> data)
        {
            command = 0;
            data = new List<byte>();
            int ind = result.IndexOf(0x3E);
            if (ind == -1)
            {
                return false;
            }
            //计算校验和
            if (ind + 4 > result.Count - 1)
            {
                return false;
            }
            int sum = result[ind] + result[ind + 1] + result[ind + 2] + result[ind + 3];
            if (result[ind + 4] != (byte)(sum % 256))
            {
                return false;
            }
            command = result[1];
            //检查数据格式
            if (result[ind + 3] == 0) return true;
            int datacount = result[ind + 3];
            int datasum = 0;
            if (ind + 4 + datacount + 1 > result.Count - 1) return false;
            for (int j = 0; j < datacount; j++)
            {
                data.Add(result[ind + 5 + j]);
                datasum += result[ind + 5 + j];
            }
            if (result[ind + 4 + datacount + 1] != (byte)(datasum % 256))
            {
                return false;
            }
            return true;
        }

        internal List<byte> ProcessResult(string result)
        {
            if (result == "") return new List<byte>();
            return result.Split('_').Select(x => byte.Parse(x)).ToList();
        }

        internal override string ThreadUnsafeQuery(List<byte> messagetosend, int timeout)
        {
            QueryReturnedByte = null;
            QueryState = false;
            PortArranger.AddMessage(messagetosend);
            int time = 0;
            while (QueryState == false && time < timeout)
            {
                Thread.Sleep(30);
                time += 30;
            }

            if (QueryReturnedByte == null) return "";
            if (QueryReturnedByte.Count == 0) return "";

            string result = "";
            foreach (var item in QueryReturnedByte)
            {
                result += item.ToString() + "_";
            }
            if (result != "") result = result.Remove(result.Length - 1, 1);
            return result;
        }

        internal List<byte> ProcessCommand(byte command, byte id, List<byte> data)
        {
            List<byte> cmd = new List<byte>();
            cmd.Add(0x3e);
            cmd.Add(command);
            cmd.Add(id);
            cmd.Add((byte)data.Count);
            //求校验和
            int sum = (int)command + 0x3e + (int)id + data.Count;
            cmd.Add((byte)(sum % 256));
            int datasum = 0;
            foreach (var item in data)
            {
                cmd.Add(item);
                datasum += item;
            }
            if (data.Count != 0)
                cmd.Add((byte)(datasum % 256));
            return cmd;
        }
        #endregion


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
        #endregion
    }
}
