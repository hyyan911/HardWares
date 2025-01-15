using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using InTheHand.Net;
using System.Diagnostics;
using HardWares.端口基类;
using HardWares.端口基类部分;

namespace HardWares.角度传感器
{
    /// <summary>
    /// 角度传感器
    /// </summary>
    public partial class AngleSensor : PortObject
    {

        /// <summary>
        /// 产品名
        /// </summary>
        public new string ProductName { get; internal set; } = "角度传感器";

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "角度传感器IMU948";

        /// <summary>
        /// 参数改变事件
        /// </summary>
        public override event ParamsChangeEventHandler ParamsChangedEvent = null;

        private double angleX = 0;
        /// <summary>
        /// 欧拉角X
        /// </summary>
        public double AngleX
        {
            get { return Thread.VolatileRead(ref angleX); }
            internal set
            {
                Thread.VolatileWrite(ref angleX, value);
            }
        }

        private double angleY = 0;
        /// <summary>
        /// 欧拉角Y
        /// </summary>
        public double AngleY
        {
            get { return Thread.VolatileRead(ref angleY); }
            internal set
            {
                Thread.VolatileWrite(ref angleY, value);
            }
        }

        private double angleZ = 0;

        /// <summary>
        /// 欧拉角Z
        /// </summary>
        public double AngleZ
        {
            get { return Thread.VolatileRead(ref angleZ); }
            internal set
            {
                Thread.VolatileWrite(ref angleZ, value);
            }
        }


        /// <summary>
        /// 连接成功后的操作
        /// </summary>
        public void ConnectedAction()
        {
            //开启设备主动上报(只订阅欧拉角)
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x03 }, true));

            //设置设备参数(只订阅欧拉角)
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x12, 255, 0, 0, 0, 20, 2, 2, 4, 0x40, 0x00 }, true));

            Thread.Sleep(100);

            //开启设备主动上报(只订阅欧拉角)
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x19 }, true));

            Thread.Sleep(100);
        }

        /// <summary>
        /// 接收到信息的操作
        /// </summary>
        /// <param name="port"></param>
        public void ReceiveAct()
        {
            if (ReceiveBuffer.Count == 0) return;
            List<byte> data = ReadFrame(out byte tag);
            if (tag == 0) return;
            //读取串口通信地址
            if (tag == 0x31)
            {
                id = data[0];
                //判定连接成功
                Internal_Is_Connected = true;
                return;
            }
            //获取订阅的数据（角度）
            if (tag == 0x11)
            {
                //获取并设置欧拉角
                AngleY = 0.0054931640625f * BitConverter.ToInt16(data.GetRange(6, 2).ToArray(), 0);
                AngleX = 0.0054931640625f * BitConverter.ToInt16(data.GetRange(8, 2).ToArray(), 0);
                AngleZ = 0.0054931640625f * BitConverter.ToInt16(data.GetRange(10, 2).ToArray(), 0);
                ParamsChangedEvent?.Invoke(this, "Angle");
                return;
            }

        }

        /// <summary>
        /// 刷新参数
        /// </summary>
        public override void ValidateParams()
        {
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x11 }, true));
        }

        /// <summary>
        /// 销毁的操作
        /// </summary>
        internal void DisposeAction()
        {
            if (PortArranger != null)
            {
                //关闭设备主动上报(只订阅欧拉角)
                PortArranger.AddMessage(GenerateFrame(new byte[] { 0x18 }, true));
                //休眠
                PortArranger.AddMessage(GenerateFrame(new byte[] { 0x02 }, true));

                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 初始化操作
        /// </summary>
        internal void InitAction()
        {
            //关闭设备主动上报(只订阅欧拉角)
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x18 }, true));
        }

        /// <summary>
        /// 测试行为
        /// </summary>
        internal void TestAction()
        {
            ///起始帧，读取串口地址            
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x31 }, true));
        }


        /// <summary>
        /// 生成数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<byte> GenerateFrame(byte[] data, bool isStartFrame)
        {
            List<byte> result = new List<byte>();

            //是起始帧，需要加50位前导码
            if (isStartFrame)
            {
                result = Enumerable.Repeat((byte)0, 46).ToList();
                result.Add(0);
                result.Add(0xFF);
                result.Add(0);
                result.Add(0xFF);
            }
            //起始码
            result.Add(0x49);
            //地址(0xff表示所有设备均能接收到)
            result.Add(0xFF);
            //数据长度
            result.Add((byte)data.Length);
            //数据体
            result.AddRange(data.ToList());
            //校验码
            byte checksum = (byte)(0xFF + data.Length);
            foreach (byte item in data)
            {
                checksum += item;
            }
            result.Add(checksum);

            //结束码
            result.Add(0x4D);

            return result;
        }

        /// <summary>
        /// 读取数据(并删除缓存区中数据)
        /// </summary>
        /// <returns></returns>
        private List<byte> ReadFrame(out byte tag)
        {
            try
            {
                int ind = 1;
                int start = ReceiveBuffer.IndexOf(0x49);
                int end = ReceiveBuffer.LastIndexOf(0x4d);
                if (end == -1 || start == -1) { tag = 0; ReceiveBuffer.Clear(); return new List<byte>(); }
                //求校验和
                int sum = 0;
                for (int i = start + 1; i < end - 1; i++)
                {
                    sum += ReceiveBuffer[i];
                }
                sum = sum % 256;
                if (sum != ReceiveBuffer[end - 1]) { tag = 0; ReceiveBuffer.RemoveRange(0, end + 1); return new List<byte>(); }
                //获取数据体
                List<byte> seg = ReceiveBuffer.GetRange(start, end - start + 1);
                ReceiveBuffer.RemoveRange(0, end + 1);
                tag = seg[3];
                if (seg[2] == 0)
                {
                    return new List<byte>();
                }
                else
                {
                    return seg.GetRange(4, seg[2]);
                }
            }
            catch
            {
                tag = 0;
                return new List<byte>();
            }
        }

        /// <summary>
        /// 将当前位置设置成Z轴的零点
        /// </summary>
        public void SetZZero(int maxWaittime)
        {
            PortArranger.AddMessage(GenerateFrame(new byte[] { 0x05 }, true));
        }

        internal string Query(string tosend, int timeout)
        {
            throw new NotImplementedException();
        }

        public override List<Parameter> AvailableParameterNames()
        {
            throw new NotImplementedException();
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            throw new NotImplementedException();
        }
    }

}
