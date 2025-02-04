using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.数据处理.SCPI指令
{
    internal class SCPIGenerator
    {
        /// <summary>
        /// 生成SCPI指令
        /// </summary>
        /// <returns></returns>
        internal static string GenerateSCPICommannd(bool isQuery, bool[] isnumber, string[] values, params string[] commands)
        {
            string command = "";
            foreach (var item in commands)
            {
                command += ":" + item;
            }
            if (isQuery)
            {
                command += "?";
            }
            else
            {
                for (int i = 0; i < isnumber.Count(); ++i)
                {
                    if (isnumber[i])
                    {
                        command += " " + values[i];
                    }
                    else
                    {
                        command += " " + "\"" + values[i] + "\"";
                    }
                }
            }
            return command.Remove(0, 1) + "\n";
        }
    }
}
