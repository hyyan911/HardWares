using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.示波器.汉泰
{
    public unsafe partial class OscilloScope
    {

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <param name="DeviceIndex"></param>
        /// <returns></returns>
        [DllImport("HTMarch.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern short dsoOpenDevice(ushort DeviceIndex);

        /// <summary>
        /// 设置通道电压档位
        /// </summary>
        /// <param name="DeviceIndex"></param>
        /// <param name="nCH"></param>
        /// <param name="nVoltDIV"></param>
        /// <returns></returns>
        [DllImport("HTMarch.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern short dsoSetVoltDIV(ushort DeviceIndex, int nCH, int nVoltDIV);

        /// <summary>
        /// 设置采样率档位
        /// </summary>
        /// <param name="DeviceIndex"></param>
        /// <param name="nTimeDIV"></param>
        /// <returns></returns>
        [DllImport("HTMarch.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern short dsoSetTimeDIV(ushort DeviceIndex, int nTimeDIV);

        /// <summary>
        /// 数据读取
        /// </summary>
        /// <param name="DeviceIndex"></param>
        /// <param name="pCH1Data"></param>
        /// <param name="pCH2Data"></param>
        /// <param name="nReadLen"></param>
        /// <param name="pCalLevel"></param>
        /// <param name="nCH1VoltDIV"></param>
        /// <param name="nCH2VoltDIV"></param>
        /// <param name="nTrigSweep"></param>
        /// <param name="nTrigSrc"></param>
        /// <param name="nTrigLevel"></param>
        /// <param name="nSlope"></param>
        /// <param name="nTimeDIV"></param>
        /// <param name="nHTrigPos"></param>
        /// <param name="nDisLen"></param>
        /// <param name="nTrigPoint"></param>
        /// <param name="nInsertMode"></param>
        /// <returns></returns>
        [DllImport("HTMarch.dll", CallingConvention = CallingConvention.StdCall)]
        private static unsafe extern short dsoReadHardData(ushort deviceIndex, short* pCH1Data, short* pCH2Data, uint nReadLen, short* pCalLevel, int nCH1VoltDIV, int nCH2VoltDIV, short nTrigSweep, short nTrigSrc, short nTrigLevel, short nSlope, int nTimeDIV, short nHTrigPos, uint nDisLen, uint* nTrigPoint, short nInsertMode);

        /// <summary>
        /// 获取设备的校对数据
        /// </summary>
        /// <param name="DeviceIndex"></param>
        /// <param name="level"></param>
        /// <param name="nLen"></param>
        /// <returns></returns>
        [DllImport("HTMarch.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern short dsoGetCalLevel(ushort DeviceIndex, short* level, short nLen);

    }

    /// <summary>
    /// 连接部分
    /// </summary>
    public unsafe partial class OscilloScope
    {
        /// <summary>
        /// 设备索引
        /// </summary>
        int ID { get; set; } = 0;

        /// <summary>
        /// 连接状态监听线程
        /// </summary>
        private Thread ConnectThread { get; set; } = null;

        /// <summary>
        /// 校准数据
        /// </summary>
        private short[] level = new short[32];

        /// <summary>
        /// 断开连接事件
        /// </summary>
        public event EventHandler ConnectStateChangeEvent = null;

        private int isConnected = 0;
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool IsConnected
        {
            get { return Thread.VolatileRead(ref isConnected) == 0 ? false : true; }
            set
            {
                Thread.VolatileWrite(ref isConnected, value ? 1 : 0);
            }
        }

        public OscilloScope(int id, Channels channel, VoltScales volt)
        {
            ID = id;
            VoltScale = volt;
        }

        /// <summary>
        /// 自动尝试连接
        /// </summary>
        /// <returns></returns>
        public bool AutoConnect()
        {
            try
            {
                short result = dsoOpenDevice((ushort)ID);

                if (result == 1)
                {
                    //创建监听线程
                    ConnectThread?.Abort();
                    dsoSetVoltDIV((ushort)ID, Convert.ToInt16(Channels.CH1), Convert.ToInt16(VoltScale));
                    ConnectThread = new Thread(() =>
                      {
                          while (true)
                          {
                              if (dsoOpenDevice((ushort)ID) == 0)
                              {
                                  IsConnected = false;
                                  ConnectStateChangeEvent?.Invoke(this, new EventArgs());
                              }
                              else
                              {
                                  IsConnected = true;
                                  ConnectStateChangeEvent?.Invoke(this, new EventArgs());
                              }
                              Thread.Sleep(500);
                          }
                      });
                    ConnectThread.Start();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void Dispose()
        {
            ConnectThread?.Abort();
        }
    }

    /// <summary>
    /// 物理参数
    /// </summary>
    public unsafe partial class OscilloScope
    {
        /// <summary>
        /// 电压刻度
        /// </summary>
        public enum VoltScales
        {
            /// <summary>
            /// 20mV
            /// </summary>
            mV_20 = 0,
            /// <summary>
            /// 50mV
            /// </summary>
            mV_50 = 1,
            /// <summary>
            /// 100mV
            /// </summary>
            mV_100 = 2,
            /// <summary>
            /// 200mV
            /// </summary>
            mV_200 = 3,
            /// <summary>
            /// 500mV
            /// </summary>
            mV_500 = 4,
            /// <summary>
            /// 1V
            /// </summary>
            V_1 = 5,
            /// <summary>
            /// 2V
            /// </summary>
            V_2 = 6,
            /// <summary>
            /// 5V
            /// </summary>
            V_5 = 7,
        }

        /// <summary>
        /// 采样率
        /// </summary>
        public enum SampleRates
        {
            /// <summary>
            /// 100KSa/s
            /// </summary>
            K_100 = 27,
            /// <summary>
            /// 200KSa/s
            /// </summary>
            K_200 = 26,
            /// <summary>
            /// 500KSa/s
            /// </summary>
            K_500 = 25,
            /// <summary>
            /// 1MSa/s
            /// </summary>
            M_1 = 15,
            /// <summary>
            /// 4MSa/s
            /// </summary>
            M_4 = 13,
            /// <summary>
            /// 8MSa/s
            /// </summary>
            M_8 = 12,
            /// <summary>
            /// 16MSa/s
            /// </summary>
            M_16 = 11,
            /// <summary>
            /// 48MSa/s
            /// </summary>
            M_48 = 10,
        }

        /// <summary>
        /// 通道
        /// </summary>
        public enum Channels
        {
            CH1 = 0,
            CH2 = 1,
        }
        /// <summary>
        /// 电压刻度
        /// </summary>
        public VoltScales VoltScale { get; set; } = VoltScales.mV_500;

        /// <summary>
        /// 采样率
        /// </summary>
        public SampleRates SampleRate { get; set; } = SampleRates.M_1;


        private static double Factor = 32;
        /// <summary>
        /// 读取指定通道示波器数据(单位：V)
        /// </summary>
        /// <returns></returns>
        public List<double> ReadData(Channels selectedChannel, uint datalength)
        {

            short[] bufferCH1 = new short[datalength];
            short[] bufferCH2 = new short[datalength];

            short value;
            unsafe
            {
                uint trigPt = 0;
                fixed (short* pBufferCH1 = bufferCH1)
                fixed (short* pBufferCH2 = bufferCH2)
                fixed (short* pLevel = level)
                {
                    value = dsoReadHardData((ushort)ID, pBufferCH1, pBufferCH2, datalength, pLevel, Convert.ToInt16(VoltScale), Convert.ToInt16(VoltScale), 0, Convert.ToInt16(selectedChannel), 50, 0, Convert.ToInt16(SampleRate), 0, datalength, &trigPt, 0);
                }
                if (value == -1)
                {
                    return null;
                }
            }

            List<double> output = new List<double>();
            if (selectedChannel == Channels.CH1)
            {
                for (int i = 0; i < bufferCH1.Length; ++i)
                {
                    output.Add(bufferCH1[i] / Factor);
                }
                return output;
            }
            else
            {
                for (int i = 0; i < bufferCH2.Length; ++i)
                {
                    output.Add(bufferCH2[i] / Factor);
                }
                return output;
            }
        }

    }
}
