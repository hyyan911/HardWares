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

namespace HardWares.纳米位移台.PI
{
    /// <summary>
    /// PI位移台C-863
    /// </summary>
    public partial class PIController : NanoControllerBase
    {

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "PI位移台";


        public new Encoding Coder = Encoding.ASCII;

        /// <summary>
        /// 位移台列表
        /// </summary>
        public override List<StageBase> Stages { get; set; } = new List<StageBase>();

        #region 属性部分
        internal bool IsGCS2 { get; set; } = true;

        /// <summary>
        /// 已经存在的设备列表
        /// </summary>
        internal static Dictionary<string, int> ExistDevices = new Dictionary<string, int>();

        /// <summary>
        /// 支持的函数
        /// </summary>
        internal List<string> CommandSet = new List<string>();

        /// <summary>
        /// 分隔符
        /// </summary>
        private char LF = (char)10;

        #endregion


        /// <summary>
        /// 
        /// </summary>
        public override void ValidateParams()
        {
        }

        #region 基本通讯方法
        /// <summary>
        /// 获取设备支持的函数名
        /// </summary>
        /// <param name="help"></param>
        /// <returns></returns>
        internal List<string> GetSupportedCommands()
        {
            List<string> help = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("HLP?", "", ""), 3000));
            if (help == null) return new List<string>();
            help.RemoveAt(0);
            help.RemoveAt(help.Count - 1);
            List<string> commands = new List<string>();
            foreach (var item in help)
            {
                string funcName = item.ToUpper().Split()[0];
                commands.Add(funcName);

            }
            return commands;
        }

        internal override string ThreadUnsafeQuery(string instruction, int waittingtime)
        {
            //检查是否有对应函数
            string cmd = instruction.Split(' ')[0].Trim();
            if (CommandSet.Contains(cmd) == false && !(cmd.Length == 1) && instruction != "*IDN?\n" && instruction != "HLP?\n") return "";

            QueryReturnedData = null;
            QueryState = false;
            if (instruction.Length % 2 != 0)
            {
                instruction += "\0";
            }
            AddMessage(Coder.GetBytes(instruction).ToList());
            int time = 0;
            Thread.Sleep(50);
            while (QueryState == false && time < waittingtime)
            {
                Thread.Sleep(30);
                time += 30;
            }
            if (QueryReturnedData == null) return "";
            string query = "";
            foreach (var item in QueryReturnedData)
            {
                item.Replace("\n", "");
                query += item + "\n";
            }
            if (query.Length > 1) query = query.Remove(query.Length - 1, 1);
            return query;
        }


        internal List<string> ProcessQueryResult(string queryresult)
        {
            List<string> result = new List<string>();
            List<string> querylist = queryresult.Split('\n').ToList();
            foreach (var item in querylist)
            {
                if (item == "") continue;
                if (item.Contains("="))
                {
                    List<string> res = item.Split('=').ToList();
                    if (res.Count >= 2)
                    {
                        result.Add(res[1].Trim());
                    }
                    else
                    {
                        result.Add(res[0].Trim());
                    }
                }
                result.Add(item);
            }
            if (result.Count == 0)
            {
                result.Add("");
            }
            return result;
        }

        /// <summary>
        /// 处理输入指令
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="axisname"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        internal string ProcessCmd(string instruction, string axisname, string value)
        {
            if (instruction.Contains("#"))
            {
                instruction = Encoding.ASCII.GetString(new byte[] { byte.Parse(instruction.Replace("#", "")) });
            }
            if (axisname != "")
            {
                instruction += " " + axisname;
            }
            if (value != "")
            {
                instruction += " " + value;
            }
            instruction += LF;

            return instruction;
        }

        /// <summary>
        /// 发送指令
        /// </summary>
        /// <returns></returns>
        internal bool Send(string instruction)
        {
            if (IsGCS2)
            {
                string cmd = instruction.Split(' ')[0].Trim();
                //检查是否有对应函数
                if (CommandSet.Contains(cmd) == false) return false;

                AddMessage(instruction);
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion


        #region 设备查重部分
        internal int HasRepeatDevice(string serialnumber)
        {
            if (ExistDevices.TryGetValue(serialnumber, out var device))
            {
                return device;
            }
            return -1;
        }
        #endregion

        #region GCS版本部分

        List<string> GCS1Devices = new List<string>()
        {
            "C-843", "C-702.00", "C-880", "C-848", "E-621", "E-625", "E-665", "E-816", "E-516",
               "C-663.10", "C-863.10", "MERCURY", "HEXAPOD", "TRIPOD", "E-710", "F-206", "E-761"
        };

        List<string> GCS2Devices = new List<string>()
        {
             "C-663", "C-867", "C-863", "E-135", "E-861", "E-871", "E-872", "E-873", "E-874", "C-413K011", "C-877",
               "C-891", "C-867K040", "E-712", "C-413", "E-517", "E-518", "E-723", "E-725", "E-727", "E-753", "E-754",
               "C-887"
        };

        /// <summary>
        /// 判断控制器是否使用GCS3
        /// </summary>
        /// <returns></returns>
        internal bool IsGCS3()
        {
            return false;
            //string result = ThreadSafeQuery(ProcessCmd("CSV?", "", ""), 300);
            //bool check = double.TryParse(result, out double version);
            //if (check == true)
            //{
            //    if (version > 2.0) return true;
            //}
            //List<string> results = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("*IDN?", "", ""), 300))[0].Split(',').ToList();
            //if (results.Count < 2) return false;

            //if (GCS1Devices.Contains(results[1].Trim().TrimEnd()))
            //{
            //    return false;
            //}

            //if (GCS2Devices.Contains(results[1].Trim().TrimEnd()))
            //{
            //    return false;
            //}

            //if (results.Count < 4) return false;

            //if (results[3].Split('.').ToList().Count < 4)
            //{
            //    return false;
            //}
            //return true;
        }

        #endregion


        #region 参数筛选
        /// <summary>
        /// 获取可用参数
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            PIStage st = new PIStage("", this);
            List<Parameter> result = new List<Parameter>();
            return result;
        }

        internal Parameter CreateParameterRef(string commandwrite, string commandread, string paramName, Type paramType, object parent)
        {
            if (CommandSet.Contains(commandread) || CommandSet.Contains(commandwrite))
            {
                bool readOnly = false;
                if (!CommandSet.Contains(commandwrite))
                    readOnly = true;
                if (parent.GetType().GetProperty(paramName).CanWrite == false)
                {
                    readOnly = true;
                }
                Parameter p = new Parameter(paramName, "", paramType, parent, false) { IsReadOnly = readOnly };
                return p;
            }
            return null;
        }
        #endregion

        /// <summary>
        /// 停止所有轴运动
        /// </summary>
        /// <param name="ID">设备ID</param>
        /// <returns></returns>
        public void StopAll()
        {
            Send(ProcessCmd("STP", "", ""));
        }

        /// <summary>
        /// 判断控制器是否可以接收指令
        /// </summary>
        public bool IsControllerReady()
        {
            if (IsGCS2)
            {
                string retu = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("#7", "", ""), 1000))[0];

                if (retu == "") return false;

                if (retu.Trim() == "1")
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
    }
}
