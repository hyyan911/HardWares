
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类
{
    /// <summary>
    /// RS485或TTL指令(11个字节)
    /// </summary>
    internal class RS485Frame
    {
        public byte Header { get; set; } = 0x3E;

        /// <summary>
        /// 设备ID号
        /// </summary>
        public int ID { get; set; } = 1;

        public RS485Frame(int id)
        {
            ID = id;
        }

        /// <summary>
        /// 生成数据帧
        /// </summary>
        /// <returns></returns>
        public List<byte> GenerateFrame(byte command, byte[] data)
        {
            //数据格式：帧头+数据
            List<byte> datalist = data.ToList();
            List<byte> result = new List<byte>();

            result.Add(Header);
            result.Add(command);
            result.Add((byte)ID);
            result.Add((byte)datalist.Count);
            //帧头校验和字节
            byte checksum = 0;
            foreach (byte item in result)
            {
                checksum += item;
            }
            result.Add(checksum);
            //数据部分
            if (datalist.Count == 0)
            {
                return result;
            }
            result.AddRange(datalist);
            checksum = 0;
            foreach (byte item in datalist)
            {
                checksum += item;
            }
            result.Add(checksum);
            return result;
        }

        /// <summary>
        /// 读取InputBuffer中遇到的第一个数据帧并将其从列表中删除 
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="identifier"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<byte> ReadFrame(List<byte> InputBuffer, out List<byte> data, out byte commandType)
        {
            for (int i = 0; i < InputBuffer.Count - 1; i++)
            {
                ///检测到数据帧
                if (InputBuffer[i] == Header)
                {
                    try
                    {
                        //获取数据位数
                        int dataLength = InputBuffer[i + 3];
                        //获取数据,验证校验和
                        byte checksum1 = 0;
                        checksum1 += InputBuffer[i];
                        checksum1 += InputBuffer[i + 1];
                        checksum1 += InputBuffer[i + 2];
                        checksum1 += InputBuffer[i + 3];
                        byte checksum2 = 0;
                        for (int j = 0; j < dataLength; j++)
                        {
                            checksum2 += InputBuffer[i + 5 + j];
                        }
                        if (dataLength == 0)
                        {
                            if (checksum1 == InputBuffer[i + 4])
                            {
                                data = new List<byte>();
                                commandType = InputBuffer[i + 1];
                                List<byte> result = InputBuffer.GetRange(i, 5);
                                InputBuffer.RemoveRange(i, 5);
                                return result;
                            }
                            data = null;
                            commandType = 0;
                            InputBuffer.RemoveRange(i, 5);
                            return null;
                        }
                        if (checksum1 == InputBuffer[i + 4] && checksum2 == InputBuffer[i + 4 + dataLength + 1])
                        {
                            data = InputBuffer.GetRange(i + 5, dataLength);
                            commandType = InputBuffer[i + 1];
                            List<byte> result = InputBuffer.GetRange(i, 5 + dataLength + 1);
                            InputBuffer.RemoveRange(i, 5 + dataLength + 1);
                            return result;
                        }
                        else
                        {
                            data = null;
                            commandType = 0;
                            InputBuffer.RemoveRange(i, 5 + dataLength + 1);
                            return null;
                        }
                    }
                    catch
                    {
                        data = null;
                        commandType = 0;
                        return null;
                    }
                }
            }
            data = null;
            commandType = 0;
            return null;
        }
    }
}
