using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HardWares.数据处理;
using HardWares.端口基类;
using HardWares.端口基类部分;

namespace HardWares
{
    /// <summary>
    /// 三轴角度调整台
    /// </summary>
    public partial class ClampAndAngleRotater : PortObject
    {
        /// <summary>
        /// 产品名
        /// </summary>
        public new string ProductName { get; internal set; } = "Aurduino控制夹具";

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Aurduino控制夹具";

        #region 线程区域

        private int isCharge = 0;
        /// <summary>
        /// X轴是否正在移动
        /// </summary>
        public bool IsCharge
        {
            get
            {
                return Thread.VolatileRead(ref isCharge) == 0 ? false : true;
            }
            internal set
            {
                Thread.VolatileWrite(ref isCharge, value ? 1 : 0);
            }
        }

        private int isClampOn = 0;
        /// <summary>
        /// X轴是否正在移动
        /// </summary>
        public bool IsClampOn
        {
            get
            {
                return Thread.VolatileRead(ref isClampOn) == 0 ? false : true;
            }
            internal set
            {
                Thread.VolatileWrite(ref isClampOn, value ? 1 : 0);
            }
        }

        private int isInit = 0;
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool IsInit
        {
            get
            {
                return Thread.VolatileRead(ref isInit) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref isInit, 1);
                else Thread.VolatileWrite(ref isInit, 0);
            }
        }

        private int x_Moving = 0;
        /// <summary>
        /// X轴是否正在移动
        /// </summary>
        public bool X_Moving
        {
            get
            {
                return Thread.VolatileRead(ref x_Moving) == 0 ? false : true;
            }
            internal set
            {
                if (value) Thread.VolatileWrite(ref x_Moving, 1);
                else Thread.VolatileWrite(ref x_Moving, 0);
            }
        }

        private int y_Moving = 0;
        /// <summary>
        /// Y轴是否正在移动
        /// </summary>
        public bool Y_Moving
        {
            get
            {
                return Thread.VolatileRead(ref y_Moving) == 0 ? false : true;
            }
            internal set
            {
                if (value) Thread.VolatileWrite(ref y_Moving, 1);
                else Thread.VolatileWrite(ref y_Moving, 0);
            }
        }

        private int z_Moving = 0;
        /// <summary>
        /// Z轴是否正在移动
        /// </summary>
        public bool Z_Moving
        {
            get
            {
                return Thread.VolatileRead(ref z_Moving) == 0 ? false : true;
            }
            internal set
            {
                if (value) Thread.VolatileWrite(ref z_Moving, 1);
                else Thread.VolatileWrite(ref z_Moving, 0);
            }
        }

        private int is_X_Aborted = 0;
        private bool Is_X_Aborted
        {
            get
            {
                return Thread.VolatileRead(ref is_X_Aborted) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref is_X_Aborted, 1);
                else Thread.VolatileWrite(ref is_X_Aborted, 0);
            }
        }

        private int is_Y_Aborted = 0;
        private bool Is_Y_Aborted
        {
            get
            {
                return Thread.VolatileRead(ref is_Y_Aborted) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref is_Y_Aborted, 1);
                else Thread.VolatileWrite(ref is_Y_Aborted, 0);
            }
        }

        private int is_Z_Aborted = 0;
        private bool Is_Z_Aborted
        {
            get
            {
                return Thread.VolatileRead(ref is_Z_Aborted) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref is_Z_Aborted, 1);
                else Thread.VolatileWrite(ref is_Z_Aborted, 0);
            }
        }

        private int is_All_Aborted = 0;
        private bool Is_All_Aborted
        {
            get
            {
                return Thread.VolatileRead(ref is_All_Aborted) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref is_All_Aborted, 1);
                else Thread.VolatileWrite(ref is_All_Aborted, 0);
            }
        }
        #endregion

        #region 设备参数区域
        private long x_Loc;
        /// <summary>
        /// X轴位置
        /// </summary>
        public long X_Loc
        {
            get { return Thread.VolatileRead(ref x_Loc); }
        }

        private long y_Loc;
        /// <summary>
        /// Y轴位置
        /// </summary>
        public long Y_Loc
        {
            get { return Thread.VolatileRead(ref y_Loc); }
        }

        private long z_Loc;

        /// <summary>
        /// Z轴位置
        /// </summary>
        public long Z_Loc
        {
            get { return Thread.VolatileRead(ref z_Loc); }
        }
        #endregion


        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public void ConnectedAction()
        {
            //松开夹具
            ClampOff();
            Initial(500);
            StopChargeSensor();
        }


        public void ReceiveAct()
        {
            List<List<byte>> datas = DataProcess.ProcessReceivedSerialData('#', ReceiveBuffer, out List<byte> result);
            foreach (var data in datas)
            {
                string msg = System.Text.Encoding.ASCII.GetString(data.ToArray());
                if (msg == "") return;
                msg += "#";

                //测试信号
                if (msg.Contains("B") && msg.Contains("00TEST#"))
                {
                    if (Is_ID_Necessary == true)
                    {
                        if (msg[1] - '0' == id)
                            Internal_Is_Connected = true;
                    }
                    else
                    {
                        Internal_Is_Connected = true;
                        id = msg[1] - '0';
                    }
                }

                //夹紧夹具
                if (msg == "BCLAMPON#")
                {
                    IsClampOn = true;
                    ParamsChangedEvent?.Invoke(this, "IsClampOn");
                }

                //夹紧夹具
                if (msg == "BCLAMPOFF#")
                {
                    IsClampOn = false;
                    ParamsChangedEvent?.Invoke(this, "IsClampOn");
                }


                //充电
                if (msg == "BSENCON0#")
                {
                    IsCharge = true;
                    ParamsChangedEvent?.Invoke(this, "IsCharge");
                }

                //停止充电
                if (msg == "BSENCOF0#")
                {
                    IsCharge = false;
                    ParamsChangedEvent?.Invoke(this, "IsCharge");
                }

                //移动结束指令
                if (msg[0] == 'B' && msg[2] == 'B' && msg[msg.Length - 1] == '#')
                {
                    if (msg[1] == 'X')
                    {
                        X_Moving = false;
                        string str = msg.Substring(3, 5);
                        Thread.VolatileWrite(ref x_Loc, long.Parse(str));
                        ParamsChangedEvent?.Invoke(this, "X_Loc");
                    }
                    if (msg[1] == 'Y')
                    {
                        Y_Moving = false;
                        string str = msg.Substring(3, 5);
                        Thread.VolatileWrite(ref y_Loc, long.Parse(str));
                        ParamsChangedEvent?.Invoke(this, "Y_Loc");
                    }
                    if (msg[1] == 'Z')
                    {
                        Z_Moving = false;
                        string str = msg.Substring(3, 5);
                        Thread.VolatileWrite(ref z_Loc, long.Parse(str));
                        ParamsChangedEvent?.Invoke(this, "Z_Loc");
                    }
                }

                //急停指令
                if (msg == "BXAFINIS#")
                {
                    Is_X_Aborted = true;
                }
                if (msg == "BYAFINIS#")
                {
                    Is_Y_Aborted = true;
                }
                if (msg == "BZAFINIS#")
                {
                    Is_Z_Aborted = true;
                }
                if (msg == "BAAFINIS#")
                {
                    Is_All_Aborted = true;
                }

                //初始化指令
                if (msg == "BIFINISH#")
                {
                    IsInit = true;
                }
            }
        }

        public override void ValidateParams()
        {
        }

        internal void DisposeAction()
        {
        }

        internal void InitAction()
        {

        }

        internal void TestAction()
        {
            PortArranger.AddMessage("ET00TEST#ET00TEST#ET00TEST#");
        }


        /// <summary>
        /// 位移台移动(同一时间一个位移台只能存在一个指令,多余指令被忽略)
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool Move(RotateAxis axis, RotateDirection direction, long step)
        {
            try
            {
                if (axis == RotateAxis.X)
                {
                    if (X_Moving == true) return false;
                    PortArranger.AddMessage("WX" + (direction == RotateDirection.AntiClockWise ? "B" : "F") + step.ToString("D5") + "#");
                    X_Moving = true;
                    return true;
                }
                if (axis == RotateAxis.Y)
                {
                    if (Y_Moving == true) return false;
                    PortArranger.AddMessage("WY" + (direction == RotateDirection.AntiClockWise ? "B" : "F") + step.ToString("D5") + "#");
                    Y_Moving = true;
                    return true;
                }
                if (axis == RotateAxis.Z)
                {
                    if (Z_Moving == true) return false;
                    PortArranger.AddMessage("WZ" + (direction == RotateDirection.AntiClockWise ? "B" : "F") + step.ToString("D5") + "#");
                    Z_Moving = true;
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        ///位移台移动并阻塞等待其移动完成
        /// </summary>
        /// <param name="axis">运动轴</param>
        /// <param name="direction">运动方向</param>
        /// <param name="step">运动步数</param>
        /// <param name="waitTime">等待时间，超过等待时间直接返回</param>
        /// <returns></returns>
        /// <returns></returns>
        public bool MoveAndWait(RotateAxis axis, RotateDirection direction, long step, long waitTime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Move(axis, direction, step);
            while (true)
            {
                if (axis == RotateAxis.X)
                {
                    if (X_Moving == false) return true;
                }
                if (axis == RotateAxis.Y)
                {
                    if (Y_Moving == false) return true;
                }
                if (axis == RotateAxis.Z)
                {
                    if (Z_Moving == false) return true;
                }
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    Terminate(axis, 100);
                    return false;
                }
            }
        }

        /// <summary>
        /// 移动所有轴
        /// </summary>
        /// <param name="dirsx">X方向</param>
        /// <param name="stepx">Y方向</param>
        /// <param name="dirsy">Z方向</param>
        /// <param name="stepy">X步数</param>
        /// <param name="dirsz">Y步数</param>
        /// <param name="stepz">Z步数</param>
        /// <returns></returns>
        public bool MoveAllAxis(RotateDirection dirsx, long stepx, RotateDirection dirsy, long stepy, RotateDirection dirsz, long stepz)
        {
            try
            {

                if (X_Moving || Y_Moving || Z_Moving) return false;
                X_Moving = true;
                Y_Moving = true;
                Z_Moving = true;
                PortArranger.AddMessage("WX" + (dirsx == RotateDirection.AntiClockWise ? "B" : "F") + stepx.ToString("D5") + "#" +
                    "WY" + (dirsy == RotateDirection.AntiClockWise ? "B" : "F") + stepy.ToString("D5") + "#" +
                    "WZ" + (dirsz == RotateDirection.AntiClockWise ? "B" : "F") + stepz.ToString("D5") + "#");
                return true;
            }

            catch (Exception) { return false; }
        }

        /// <summary>
        /// 急停指定轴（耗时操作）
        /// </summary>
        /// <returns></returns>
        public bool Terminate(RotateAxis axis, long waittime)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                if (axis == RotateAxis.X)
                {
                    Is_X_Aborted = false;
                    PortArranger.AddProierMessage("E" + ID.ToString() + "X" + "ABORT#");
                    while (Is_X_Aborted == false && watch.ElapsedMilliseconds <= waittime)
                    {
                        Thread.Sleep(10);
                    }
                    if (Is_X_Aborted == false) return false;
                    return true;
                }
                if (axis == RotateAxis.Y)
                {
                    Is_Y_Aborted = false;
                    PortArranger.AddProierMessage("E" + ID.ToString() + "Y" + "ABORT#");
                    while (Is_Y_Aborted == false && watch.ElapsedMilliseconds <= waittime)
                    {
                        Thread.Sleep(10);
                    }
                    if (Is_Y_Aborted == false) return false;
                    return true;
                }
                if (axis == RotateAxis.Z)
                {
                    Is_Z_Aborted = false;
                    PortArranger.AddProierMessage("E" + ID.ToString() + "Z" + "ABORT#");
                    while (Is_Z_Aborted == false && watch.ElapsedMilliseconds <= waittime)
                    {
                        Thread.Sleep(10);
                    }
                    if (Is_Z_Aborted == false) return false;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 给传感器充电
        /// </summary>
        public void ChargeSensor()
        {
            PortArranger.AddMessage("ESENCON0#");
        }

        /// <summary>
        /// 停止充电
        /// </summary>
        public void StopChargeSensor()
        {
            PortArranger.AddMessage("ESENCOF0#");
        }

        /// <summary>
        /// 夹紧夹具
        /// </summary>
        public void ClampOn()
        {
            PortArranger.AddMessage("ECLAMPON#");
        }

        /// <summary>
        /// 松开夹具
        /// </summary>
        public void ClampOff()
        {
            PortArranger.AddMessage("ECLAMPOF#");
        }

        /// <summary>
        /// 急停所有轴（耗时操作）
        /// </summary>
        /// <returns></returns>
        public bool TerminateAll(long maxWaitTime)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Is_All_Aborted = false;
                PortArranger.AddProierMessage("EA" + "ABORT0#");
                while (Is_All_Aborted == false && watch.ElapsedMilliseconds <= maxWaitTime)
                {
                    Thread.Sleep(10);
                }
                if (Is_All_Aborted == false) return false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initial(int maxWaittime)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            IsInit = false;
            PortArranger.AddMessage("E" + ID.ToString() + "I000000#");
            while (IsInit == false && watch.ElapsedMilliseconds < maxWaittime)
            {
                Thread.Sleep(10);
            }
            if (IsInit == false) return false;
            else
            {
                return true;
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
    }

    /// <summary>
    /// 旋转轴
    /// </summary>
    public enum RotateAxis
    {
        X = 0,
        Y = 1,
        Z = 2
    }
    /// <summary>
    /// 旋转方向
    /// </summary>
    public enum RotateDirection
    {
        ClockWise = 0,
        AntiClockWise = 1
    }
}
