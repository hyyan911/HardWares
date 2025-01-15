using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.数据处理
{
    internal class DataProcess
    {
        /// <summary>
        /// 处理接收到的数据(如果有分隔符则按照分隔符读取，同时从源数据中删除数据,返回数据中包含分隔符)
        /// </summary>
        /// <returns></returns>
        public static List<List<byte>> ProcessReceivedSerialData(char Terminate, List<byte> source, out List<byte> processedSource)
        {
            List<List<byte>> result = new List<List<byte>>();
            int start = 0;
            int end = 0;
            for (int i = 0; i < source.Count; i++)
            {
                if (source[i] == '\0')
                {
                    source.RemoveAt(i);
                    --i;
                    continue;
                }
                if (source[i] == Terminate)
                {
                    end = i;
                    result.Add(source.GetRange(start, end - start + 1));
                    //删除原来的数据
                    source.RemoveRange(start, end - start + 1);
                    i = 0;
                    continue;
                }
            }
            processedSource = source;
            return result;
        }

        /// <summary>
        /// 浮点数转字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDecimalString(double value)
        {
            return value.ToString("0.##################################################");
        }
    }
}
