
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类
{
    /// <summary>
    /// CAN指令标准数据帧(11个字节)
    /// </summary>
    internal class CANFrame
    {
        public int Identifier { get; set; } = 0;

        public int DataLength { get; set; } = 0;

        public CANFrame(int identifier, int dataLength)
        {
            Identifier = identifier;
            DataLength = dataLength;
            Buf2 = (byte)(identifier >> 3);
            Buf3 = (byte)((identifier & 0b111) << 5);
        }

        private byte Buf2;

        private byte Buf3;

        /// <summary>
        /// 生成数据帧
        /// </summary>
        /// <returns></returns>
        public List<byte> GenerateFrame(byte[] data)
        {
            //对于8位数据帧，需要发送的位数一共有17位

            List<byte> result = new List<byte>();
            //AT=0x41 54为CAN适配器的指令符
            result.Add(0x41);
            result.Add(0x54);
            ///创建标识符
            result.Add(Buf2);
            result.Add(Buf3);
            //0表示数据帧
            result.Add(0);
            //0x00 08表示数据位有8位
            result.Add(0);
            result.Add((byte)DataLength);
            result.AddRange(data.ToList());
            //换行符=0x0D 0A
            result.Add(0x0D);
            result.Add(0x0A);
            return result;
        }

        /// <summary>
        /// 读取InputBuffer中遇到的第一个数据帧并将其从列表中删除 
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="identifier"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public List<byte> ReadFrame(List<byte> InputBuffer, out List<byte> data)
        {
            for (int i = 0; i < InputBuffer.Count - 1; i++)
            {
                ///检测到数据帧
                if (InputBuffer[i] == 0x41 && InputBuffer[i + 1] == 0x54)
                {
                    if (i + 9 + DataLength <= InputBuffer.Count)
                    {
                        data = InputBuffer.GetRange(i + 7, DataLength);
                        List<byte> result = InputBuffer.GetRange(i, 9 + DataLength);
                        InputBuffer.RemoveRange(i, 9 + DataLength);
                        return result;
                    }
                }
            }

            data = null;
            return null;
        }
    }
}
