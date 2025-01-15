using HardWares.数据处理;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares
{
    //物理参数（波形，频率，占空比等）
    public partial class FrequencyGenerator : PortObject
    {

        /// <summary>
        /// 产品名
        /// </summary>
        public new string ProductName { get; internal set; } = "波源";

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "信号发生器FY系列";

        /// <summary>
        /// 波形
        /// </summary>
        public class WaveFormTypes
        {
            /// <summary>
            /// 正弦
            /// </summary>
            public const int Sine = 0;
            /// <summary>
            /// 矩形波
            /// </summary>
            public const int Square = 1;
            /// <summary>
            /// CMOS波
            /// </summary>
            public static int CMOS = 2;
            /// <summary>
            /// 直流波
            /// </summary>
            public static int DC = 4;
            /// <summary>
            /// 三角波
            /// </summary>
            public static int Triangle = 5;
            /// <summary>
            /// 正锯齿波
            /// </summary>
            public static int P_SawTooth = 6;
            /// <summary>
            /// 反锯齿波
            /// </summary>
            public static int N_SawTooth = 7;
            /// <summary>
            /// 正阶梯波
            /// </summary>
            public static int P_Step = 9;
            /// <summary>
            /// 反阶梯波
            /// </summary>
            public static int N_Step = 10;
            /// <summary>
            /// 正指数波
            /// </summary>
            public static int P_Exp = 11;
            /// <summary>
            /// 反指数波
            /// </summary>
            public static int N_Exp = 12
;

        }

        #region 主波参数设置
        private int preview_WaveForm = 0;
        private int waveForm = 0;
        /// <summary>
        /// 波形
        /// </summary>
        public int WaveForm
        {
            get
            {
                //获取设置波形
                SendFrame("RMW", "", 100);
                return waveForm;
            }
            set
            {
                //发送设置波形
                SendFrame("WMW", value.ToString(), 100);
                SendFrame("RMW", "", 100);
            }
        }

        private double preview_WaveFreq = 0;
        private double waveFreq = 0;
        /// <summary>
        /// 波形频率
        /// </summary>
        public double WaveFreq
        {
            get
            {
                //获取波形频率
                SendFrame("RMF", "", 100);
                return waveFreq;
            }
            set
            {
                //设置波形频率
                SendFrame("WMF", value.ToString(), 100);
                SendFrame("RMF", "", 100);
            }
        }

        private double preview_WaveAmplitude = 0;
        private double waveAmplitude = 0;
        /// <summary>
        /// 波形幅度
        /// </summary>
        public double WaveAmplitude
        {
            get
            {
                SendFrame("RMA", "", 100);
                return waveAmplitude;
            }
            set
            {
                SendFrame("WMA", value.ToString(), 100);
                SendFrame("RMA", "", 100);
            }
        }

        private double preview_WaveBias = 0;
        private double waveBias = 0;
        /// <summary>
        /// 波形幅度
        /// </summary>
        public double WaveBias
        {
            get
            {
                SendFrame("RMO", "", 100);
                return waveBias;
            }
            set
            {
                SendFrame("WMO", value.ToString(), 100);
                SendFrame("RMO", "", 100);
            }
        }

        private double preview_WaveCycRate = 0;
        private double waveCycRate = 0;
        /// <summary>
        /// 波形占空比
        /// </summary>
        public double WaveCycRate
        {
            get
            {
                SendFrame("RMD", "", 100);
                return waveCycRate;
            }
            set
            {
                SendFrame("WMD", value.ToString(), 100);
                SendFrame("RMD", "", 100);
            }
        }


        private double preview_WavePhase = 0;
        private double wavePhase = 0;
        /// <summary>
        /// 波形占空比
        /// </summary>
        public double WavePhase
        {
            get
            {
                SendFrame("RMP", "", 100);
                return wavePhase;
            }
            set
            {
                SendFrame("WMP", value.ToString(), 100);
                SendFrame("RMP", "", 100);
            }
        }

        private int preview_IsWaveOpen;
        private int iswaveOpen = 0;
        /// <summary>
        /// 波形开关
        /// </summary>
        public bool IsWaveOpen
        {
            get
            {
                SendFrame("RMN", "", 100);
                return iswaveOpen == 0 ? false : true;
            }
            set
            {
                SendFrame("WMN", value == true ? "1" : "0", 100);
                SendFrame("RMN", "", 100);
            }
        }
        #endregion

        #region 副波参数设置
        private int preview_SubWaveForm = 0;
        private int subwaveForm = 0;
        /// <summary>
        /// 波形
        /// </summary>
        public int SubWaveForm
        {
            get
            {
                //获取设置波形
                SendFrame("RFW", "", 100);
                return subwaveForm;
            }
            set
            {
                //发送设置波形
                SendFrame("WFW", value.ToString(), 100);
                SendFrame("RFW", "", 100);
            }
        }

        private double preview_SubWaveFreq = 0;
        private double subwaveFreq = 0;
        /// <summary>
        /// 波形频率
        /// </summary>
        public double SubWaveFreq
        {
            get
            {
                //获取波形频率
                SendFrame("RFF", "", 100);
                return subwaveFreq;
            }
            set
            {
                //设置波形频率
                SendFrame("WFF", value.ToString(), 100);
                SendFrame("RFF", "", 100);
            }
        }

        private double preview_SubWaveAmplitude = 0;
        private double subwaveAmplitude = 0;
        /// <summary>
        /// 波形幅度
        /// </summary>
        public double SubWaveAmplitude
        {
            get
            {
                SendFrame("RFA", "", 100);
                return subwaveAmplitude;
            }
            set
            {
                SendFrame("WFA", value.ToString(), 100);
                SendFrame("RFA", "", 100);
            }
        }

        private double preview_SubWaveBias = 0;
        private double subwaveBias = 0;
        /// <summary>
        /// 波形幅度
        /// </summary>
        public double SubWaveBias
        {
            get
            {
                SendFrame("RFO", "", 100);
                return subwaveBias;
            }
            set
            {
                SendFrame("WFO", value.ToString(), 100);
                SendFrame("RFO", "", 100);
            }
        }

        private double preview_SubWaveCycRate = 0;
        private double subwaveCycRate = 0;
        /// <summary>
        /// 波形占空比
        /// </summary>
        public double SubWaveCycRate
        {
            get
            {
                SendFrame("RFD", "", 100);
                return subwaveCycRate;
            }
            set
            {
                SendFrame("WFD", value.ToString(), 100);
                SendFrame("RFD", "", 100);
            }
        }

        private double preview_SubWavePhase = 0;
        private double subwavePhase = 0;
        /// <summary>
        /// 波形占空比
        /// </summary>
        public double SubWavePhase
        {
            get
            {
                SendFrame("RFP", "", 100);
                return subwavePhase;
            }
            set
            {
                SendFrame("WFP", value.ToString(), 100);
                SendFrame("RFP", "", 100);
            }
        }


        private int preview_SubIsWaveOpen = 0;
        private int subiswaveOpen = 0;

        /// <summary>
        /// 波形开关
        /// </summary>
        public bool SubIsWaveOpen
        {
            get
            {
                SendFrame("RFN", "", 100);
                return subiswaveOpen == 0 ? false : true;
            }
            set
            {
                SendFrame("WFN", value == true ? "1" : "0", 100);
                SendFrame("RFN", "", 100);
            }
        }

        public override List<Parameter> AvailableParameterNames()
        {
            throw new NotImplementedException();
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

    /// <summary>
    /// 频率发生器(DDS)
    /// </summary>
    public partial class FrequencyGenerator : PortObject
    {
        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 参数监听线程
        /// </summary>
        private Thread ParamsListener = null;

        /// <summary>
        /// 换行符
        /// </summary>
        private static string LF = "\n";

        /// <summary>
        /// 是否收到机器返回的数据
        /// </summary >
        private int isReceived = 0;
        private bool IsReceived
        {
            get
            {
                return Thread.VolatileRead(ref isReceived) == 0 ? false : true;
            }
            set
            {
                Thread.VolatileWrite(ref isReceived, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 指令名称
        /// </summary>
        private object commandName = "";
        private object CommandName
        {
            get { return Thread.VolatileRead(ref commandName); }
            set
            {
                Thread.VolatileWrite(ref commandName, value);
            }
        }

        /// <summary>
        ///连接成功行为
        /// </summary>
        public void ConnectedAction()
        {
            SendFrame("UBZ", "0", 200);
            SendFrame("USA", "1", 200);
            ParamsListener = new Thread(() =>
              {
                  while (true)
                  {
                      if (preview_IsWaveOpen != iswaveOpen)
                      {
                          preview_IsWaveOpen = iswaveOpen;
                          ParamsChangedEvent?.Invoke(this, "IsWaveOpen");
                      }
                      if (preview_SubIsWaveOpen != subiswaveOpen)
                      {
                          preview_SubIsWaveOpen = subiswaveOpen;
                          ParamsChangedEvent?.Invoke(this, "SubIsWaveOpen");
                      }
                      if (preview_SubWaveAmplitude != subwaveAmplitude)
                      {
                          preview_SubWaveAmplitude = subwaveAmplitude;
                          ParamsChangedEvent?.Invoke(this, "SubWaveAmplitude");
                      }
                      if (preview_SubWaveBias != subwaveBias)
                      {
                          preview_SubWaveBias = subwaveBias;
                          ParamsChangedEvent?.Invoke(this, "SubWaveBias");
                      }
                      if (preview_SubWaveCycRate != subwaveCycRate)
                      {
                          preview_SubWaveCycRate = subwaveCycRate;
                          ParamsChangedEvent?.Invoke(this, "SubWaveCycRate");
                      }
                      if (preview_SubWaveForm != subwaveForm)
                      {
                          preview_SubWaveForm = subwaveForm;
                          ParamsChangedEvent?.Invoke(this, "SubWaveForm");
                      }
                      if (preview_SubWaveFreq != subwaveFreq)
                      {
                          preview_SubWaveFreq = subwaveFreq;
                          ParamsChangedEvent?.Invoke(this, "SubWaveFreq");
                      }
                      if (preview_SubWavePhase != subwavePhase)
                      {
                          preview_SubWavePhase = subwavePhase;
                          ParamsChangedEvent?.Invoke(this, "SubWavePhase");
                      }
                      if (preview_WaveAmplitude != waveAmplitude)
                      {
                          preview_WaveAmplitude = waveAmplitude;
                          ParamsChangedEvent?.Invoke(this, "WaveAmplitude");
                      }
                      if (preview_WaveBias != waveBias)
                      {
                          preview_WaveBias = waveBias;
                          ParamsChangedEvent?.Invoke(this, "WaveBias");
                      }
                      if (preview_WaveCycRate != waveCycRate)
                      {
                          preview_WaveCycRate = waveCycRate;
                          ParamsChangedEvent?.Invoke(this, " WaveCycRate");
                      }
                      if (preview_WaveForm != waveForm)
                      {
                          preview_WaveForm = waveForm;
                          ParamsChangedEvent?.Invoke(this, "WaveForm");
                      }
                      if (preview_WaveFreq != waveFreq)
                      {
                          preview_WaveFreq = waveFreq;
                          ParamsChangedEvent?.Invoke(this, "WaveFreq");
                      }
                      if (preview_WavePhase != wavePhase)
                      {
                          preview_WavePhase = wavePhase;
                          ParamsChangedEvent?.Invoke(this, "WavePhase");
                      }
                      Thread.Sleep(30);
                  }
              });
            ParamsListener.Start();
            return;
        }

        /// <summary>
        /// 接收到数据行为
        /// </summary>
        /// <param name="port"></param>
        public void ReceiveAct()
        {
            if (IsReceived == true) return;

            string command = CommandName as string;
            List<List<byte>> datas = DataProcess.ProcessReceivedSerialData('#', ReceiveBuffer, out List<byte> result);
            foreach (var data in datas)
            {
                string con = System.Text.Encoding.ASCII.GetString(data.ToArray());
                if (command == "UID" && con == "1644399483")
                {
                    Internal_Is_Connected = true;
                    IsReceived = true;
                    return;
                }

                if (con == "1644399483") continue;

                if (command == "RMW")
                {
                    Thread.VolatileWrite(ref waveForm, int.Parse(con));
                    IsReceived = true;
                    return;
                }

                if (command == "RMF")
                {
                    Thread.VolatileWrite(ref waveFreq, double.Parse(con));
                    IsReceived = true;
                    return;
                }

                if (command == "RMA")
                {
                    Thread.VolatileWrite(ref waveAmplitude, int.Parse(con) * 0.001);
                    IsReceived = true;
                    return;
                }

                if (command == "RMO")
                {
                    long v = long.Parse(con);
                    if (v >= 2147483648)
                    {
                        Thread.VolatileWrite(ref waveBias, (v - 4294967296) * 0.001);
                    }
                    else
                    {
                        Thread.VolatileWrite(ref waveBias, v * 0.001);
                    }
                    IsReceived = true;
                    return;
                }

                if (command == "RMD")
                {
                    Thread.VolatileWrite(ref waveCycRate, int.Parse(con) * 0.1);
                    IsReceived = true;
                    return;
                }

                if (command == "RMP")
                {
                    Thread.VolatileWrite(ref wavePhase, int.Parse(con) * 0.001);
                    IsReceived = true;
                    return;
                }
                if (command == "RMN")
                {
                    Thread.VolatileWrite(ref iswaveOpen, int.Parse(con) == 0 ? 0 : 1);
                    IsReceived = true;
                    return;
                }


                if (command == "RFW")
                {
                    Thread.VolatileWrite(ref subwaveForm, int.Parse(con));
                    IsReceived = true;
                    return;
                }

                if (command == "RFF")
                {
                    Thread.VolatileWrite(ref subwaveFreq, int.Parse(con));
                    IsReceived = true;
                    return;
                }

                if (command == "RFA")
                {
                    Thread.VolatileWrite(ref subwaveAmplitude, int.Parse(con) * 0.001);
                    IsReceived = true;
                    return;
                }

                if (command == "RFO")
                {
                    long v = long.Parse(con);
                    if (v >= 2147483648)
                    {
                        Thread.VolatileWrite(ref waveBias, (v - 4294967296) * 0.001);
                    }
                    else
                    {
                        Thread.VolatileWrite(ref waveBias, v * 0.001);
                    }
                    IsReceived = true;
                    return;
                }

                if (command == "RFD")
                {
                    Thread.VolatileWrite(ref subwaveCycRate, int.Parse(con) * 0.1);
                    IsReceived = true;
                    return;
                }

                if (command == "RFP")
                {
                    Thread.VolatileWrite(ref subwavePhase, int.Parse(con) * 0.001);
                    IsReceived = true;
                }
                if (command == "RFN")
                {
                    Thread.VolatileWrite(ref subiswaveOpen, int.Parse(con) == 0 ? 0 : 1);
                    IsReceived = true;
                    return;
                }

            }
            //读取ID号
            return;

        }

        /// <summary>
        /// 刷新参数
        /// </summary>
        public override void ValidateParams()
        {
            return;
        }

        /// <summary>
        /// 销毁行为
        /// </summary>
        internal void DisposeAction()
        {
            if (ParamsListener != null)
            {
                ParamsListener.Abort();
                while (ParamsListener.ThreadState == ThreadState.Running) Thread.Sleep(10);
            }
            return;
        }

        /// <summary>
        /// 初始化行为
        /// </summary>
        internal void InitAction()
        {
        }

        /// <summary>
        /// 数据机返回的全是数据，无法区分测试帧
        /// </summary>
        internal void TestAction()
        {
            SendFrame("UID", "", 100);
            SendFrame("UID", "", 100);
            SendFrame("UID", "", 100);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private void SendFrame(string name, string data, long maxtime)
        {
            IsReceived = false;
            //添加要发送信息到队列，之后等待收到信息
            PortArranger.AddMessage(name + data + LF);
            CommandName = name;

            int time = 0;
            while (IsReceived == false && time < maxtime)
            {
                Thread.Sleep(5);
                time += 5;
            }
            //收到信息后进入下一帧
            CommandName = "";
            return;
        }

    }
}