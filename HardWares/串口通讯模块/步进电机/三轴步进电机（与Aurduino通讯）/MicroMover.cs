using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using System.Windows;
using System.Diagnostics;
using HardWares.数据处理;
using System.Collections;
using HardWares.端口基类;
using HardWares.端口基类部分;

namespace HardWares.步进电机.Aurduino
{
    /// <summary>
    /// 位移台
    /// </summary>
    public class MicroMover : PortObject
    {
        /// <summary>
        /// 产品名
        /// </summary>
        public new string ProductName { get; internal set; } = "";

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "三轴步进电机运动系统（Aurduino控制）";


        private bool IsChangeMoving = true;

        private double angleDiltX = 0;
        /// <summary>
        /// X轴倾角
        /// </summary>
        public double AngleDiltX
        {
            get
            {
                return angleDiltX;
            }
            set
            {
                angleDiltX = value;
                Origin_Z = Z_Loc;
                Origin_X = X_Loc;
                Origin_Y = Y_Loc;
            }
        }

        private double angleDiltY = 0;
        /// <summary>
        /// Y轴倾角
        /// </summary>
        public double AngleDiltY
        {
            get
            {
                return angleDiltY;
            }
            set
            {
                angleDiltY = value;
                Origin_Z = Z_Loc;
                Origin_X = X_Loc;
                Origin_Y = Y_Loc;
            }
        }

        /// <summary>
        /// 初始X轴偏移值
        /// </summary>
        private double Origin_X { get; set; } = 0;

        /// <summary>
        /// 初始Y轴偏移值
        /// </summary>
        private double Origin_Y { get; set; } = 0;

        #region 线程区域

        private int isUVOpen = 0;
        /// <summary>
        /// 紫外灯状态
        /// </summary>
        public bool IsUVOpen
        {
            get
            {
                return Thread.VolatileRead(ref isUVOpen) == 0 ? false : true;
            }
            set
            {
                if (value == true)
                {
                    PortArranger.AddMessage("EUVON000#");
                }
                else
                {
                    PortArranger.AddMessage("EUVOFF00#");
                }
            }
        }

        /// <summary>
        /// 不加偏移的Z轴值
        /// </summary>
        private int Origin_Z = 0;

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

        private int is_Initialed = 0;
        /// <summary>
        /// 是否初始化
        /// </summary>
        private bool Is_Initialed
        {
            get
            {
                return Thread.VolatileRead(ref is_Initialed) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref is_Initialed, 1);
                else Thread.VolatileWrite(ref is_Initialed, 0);
            }
        }



        /// <summary>
        /// 设置接收到信息的操作
        /// </summary>
        public void ReceiveAct()
        {
            List<List<byte>> datas = DataProcess.ProcessReceivedSerialData('#', ReceiveBuffer, out List<byte> result);
            foreach (var data in datas)
            {
                string msg = System.Text.Encoding.ASCII.GetString(data.ToArray());
                if (msg == "") return;
                msg += "#";
                //移动结束指令
                if (msg[0] == 'B' && msg[2] == 'B' && msg[msg.Length - 1] == '#')
                {
                    if (msg[1] == 'X')
                    {
                        X_Moving = false;
                        string str = msg.Substring(3, 5);
                        Thread.VolatileWrite(ref x_Loc, int.Parse(str));
                    }
                    if (msg[1] == 'Y')
                    {
                        Y_Moving = false;
                        string str = msg.Substring(3, 5);
                        Thread.VolatileWrite(ref y_Loc, int.Parse(str));
                    }
                    if (msg[1] == 'Z')
                    {
                        if (IsChangeMoving == true)
                        {
                            Z_Moving = false;
                        }
                        string str = msg.Substring(3, 5);
                        Thread.VolatileWrite(ref z_Loc, int.Parse(str));
                    }
                }
                //急停指令
                if (msg == "BXAFINIS#")
                {
                    Is_X_Aborted = true;
                    if (IsChangeMoving == true)
                    {
                        X_Moving = false;
                    }
                }
                if (msg == "BYAFINIS#")
                {
                    Is_Y_Aborted = true;
                    if (IsChangeMoving == true)
                    {
                        Y_Moving = false;
                    }
                }
                if (msg == "BZAFINIS#")
                {
                    Is_Z_Aborted = true;
                    if (IsChangeMoving == true)
                    {
                        Z_Moving = false;
                    }
                }
                if (msg == "BAAFINIS#")
                {
                    Is_All_Aborted = true;
                    X_Moving = false;
                    Y_Moving = false;
                    Z_Moving = false;
                }

                if (msg == "BIFINISH#")
                {
                    Is_Initialed = true;
                }

                //开启紫外灯
                if (msg == "BUVON000#")
                {
                    Thread.VolatileWrite(ref isUVOpen, 1);
                    ParamsChangedEvent?.Invoke(this, "IsUVOpen");
                }

                //关闭紫外灯
                if (msg == "BUVOFF00#")
                {
                    Thread.VolatileWrite(ref isUVOpen, 0);
                    ParamsChangedEvent?.Invoke(this, "IsUVOpen");
                }

                if (msg.Contains("B") && msg.Contains("00TEST#"))
                {
                    Internal_Is_Connected = true;
                    id = msg[1] - '0';
                }

                #region 参数设置指令
                if (msg.Contains("BXL"))
                {
                    string str = msg.Substring(3, 5);
                    Thread.VolatileWrite(ref x_LowerRange, int.Parse(str));
                    ParamsChangedEvent?.Invoke(this, "X_LowerRange");
                }
                if (msg.Contains("BYL"))
                {
                    string str = msg.Substring(3, 5);
                    Thread.VolatileWrite(ref y_LowerRange, int.Parse(str));
                    ParamsChangedEvent?.Invoke(this, "Y_LowerRange");
                }
                if (msg.Contains("BZL"))
                {
                    string str = msg.Substring(3, 5);
                    Thread.VolatileWrite(ref z_LowerRange, int.Parse(str));
                    ParamsChangedEvent?.Invoke(this, "Z_LowerRange");
                }
                if (msg.Contains("BXU"))
                {
                    string str = msg.Substring(3, 5);
                    Thread.VolatileWrite(ref x_UpperRange, int.Parse(str));
                    ParamsChangedEvent?.Invoke(this, "X_UpperRange");
                }
                if (msg.Contains("BYU"))
                {
                    string str = msg.Substring(3, 5);
                    Thread.VolatileWrite(ref y_UpperRange, int.Parse(str));
                    ParamsChangedEvent?.Invoke(this, "Y_UpperRange");
                }
                if (msg.Contains("BZU"))
                {
                    string str = msg.Substring(3, 5);
                    Thread.VolatileWrite(ref z_UpperRange, int.Parse(str));
                    ParamsChangedEvent?.Invoke(this, "Z_UpperRange");
                }
                #endregion
            }

        }
        #endregion


        private int Preview_xloc = 0;
        #region 设备参数区域
        private int x_Loc;
        /// <summary>
        /// X轴位置
        /// </summary>
        public int X_Loc
        {
            get { return Thread.VolatileRead(ref x_Loc); }
        }

        private int Preview_yloc = 0;
        private int y_Loc;
        /// <summary>
        /// Y轴位置
        /// </summary>
        public int Y_Loc
        {
            get { return Thread.VolatileRead(ref y_Loc); }
        }

        private int Preview_zloc = 0;
        private int z_Loc;
        /// <summary>
        /// Z轴位置
        /// </summary>
        public int Z_Loc
        {
            get { return Thread.VolatileRead(ref z_Loc); }
        }

        private int x_LowerRange = 0;
        /// <summary>
        /// X轴最小值(注意设置值时相当于向设备发送一个指令，因此属性值的改变不是即时的，写程序时应该注意到这个问题。)
        /// </summary>
        public int X_LowerRange
        {
            get { return Thread.VolatileRead(ref x_LowerRange); }
            set
            {
                if (value < 0 || value >= x_UpperRange) throw new Exception();
                PortArranger.AddMessage("WXL" + value.ToString("D5") + "#");
            }
        }

        private int x_UpperRange = 0;
        /// <summary>
        /// X轴最大值(注意设置值时相当于向设备发送一个指令，因此属性值的改变不是即时的，写程序时应该注意到这个问题。)
        /// </summary>
        public int X_UpperRange
        {
            get { return Thread.VolatileRead(ref x_UpperRange); }
            set
            {
                if (value < 0 || value <= x_LowerRange) throw new Exception();
                PortArranger.AddMessage("WXU" + value.ToString("D5") + "#");
            }
        }

        private int y_LowerRange = 0;
        /// <summary>
        /// Y轴最小值(注意设置值时相当于向设备发送一个指令，因此属性值的改变不是即时的，写程序时应该注意到这个问题。)
        /// </summary>
        public int Y_LowerRange
        {
            get { return Thread.VolatileRead(ref y_LowerRange); }
            set
            {
                if (value < 0 || value >= y_UpperRange) throw new Exception();
                PortArranger.AddMessage("WYL" + value.ToString("D5") + "#");
            }
        }

        private int y_UpperRange = 0;
        /// <summary>
        /// Y轴最大值(注意设置值时相当于向设备发送一个指令，因此属性值的改变不是即时的，写程序时应该注意到这个问题。)
        /// </summary>
        public int Y_UpperRange
        {
            get { return Thread.VolatileRead(ref y_UpperRange); }
            set
            {
                if (value < 0 || value <= y_LowerRange) throw new Exception();
                PortArranger.AddMessage("WYU" + value.ToString("D5") + "#");
            }
        }

        private int z_LowerRange = 0;
        /// <summary>
        /// Z轴最小值(注意设置值时相当于向设备发送一个指令，因此属性值的改变不是即时的，写程序时应该注意到这个问题。)
        /// </summary>
        public int Z_LowerRange
        {
            get { return Thread.VolatileRead(ref z_LowerRange); }
            set
            {
                if (value < 0 || value >= z_UpperRange) throw new Exception();
                PortArranger.AddMessage("WZL" + value.ToString("D5") + "#");
            }
        }

        private int z_UpperRange = 0;
        /// <summary>
        /// Z轴最大值(注意设置值时相当于向设备发送一个指令，因此属性值的改变不是即时的，写程序时应该注意到这个问题。)
        /// </summary>
        public int Z_UpperRange
        {
            get { return Thread.VolatileRead(ref z_UpperRange); }
            set
            {
                if (value < 0 || value <= z_LowerRange) throw new Exception();
                PortArranger.AddMessage("WZU" + value.ToString("D5") + "#");
            }
        }

        private int time_Gap = 1;
        /// <summary>
        /// 相邻两次脉冲之间的最小时间间隔，单位：（ms），反映了电机的移动速度
        /// </summary>
        public int Time_Gap
        {
            get { return Thread.VolatileRead(ref time_Gap); }
            set
            {
                if (value < 0) throw new Exception();
                PortArranger.AddMessage("WXS" + value.ToString("D5") + "#");
            }
        }
        #endregion

        /// <summary>
        /// X轴位置改变事件
        /// </summary>
        public event EventHandler XLocationChanged = null;

        /// <summary>
        /// Y轴位置改变事件
        /// </summary>
        public event EventHandler YLocationChanged = null;

        /// <summary>
        /// Z轴位置改变事件
        /// </summary>
        public event EventHandler ZLocationChanged = null;

        private Thread LocationListener { get; set; } = null;

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        public MicroMover(int number, double diltX, double diltY)
        {
            id = number;
            AngleDiltX = diltX;
            AngleDiltY = diltY;
        }

        /// <summary>
        /// 更新参数（耗时操作,至少100ms）
        /// </summary>
        public override void ValidateParams()
        {
            PortArranger.AddMessage("RXMAX000#");
            PortArranger.AddMessage("RYMAX000#");
            PortArranger.AddMessage("RZMAX000#");
            Thread.Sleep(100);

            PortArranger.AddMessage("RXMIN000#");
            PortArranger.AddMessage("RYMIN000#");
            PortArranger.AddMessage("RZMIN000#");
            Thread.Sleep(100);

            PortArranger.AddMessage("RXSPEED0#");

            //位置信息
            PortArranger.AddMessage("RX000000#");
            PortArranger.AddMessage("RY000000#");
            PortArranger.AddMessage("RZ000000#");
            Thread.Sleep(100);
        }

        /// <summary>
        /// 发送测试信息
        /// </summary>
        internal void TestAction()
        {
            PortArranger.AddMessage("ET00TEST#");
        }

        /// <summary>
        /// 连接成功后的操作
        /// </summary>
        public void ConnectedAction()
        {
            ValidateParams();
            //关闭紫外灯
            IsUVOpen = false;

            //刷新监听线程
            LocationListener = new Thread(() =>
              {
                  while (true)
                  {
                      if (Preview_xloc != x_Loc)
                      {
                          Preview_xloc = x_Loc;
                          XLocationChanged?.Invoke(this, new EventArgs());
                      }
                      if (Preview_yloc != y_Loc)
                      {
                          Preview_yloc = y_Loc;
                          YLocationChanged?.Invoke(this, new EventArgs());
                      }
                      if (Preview_zloc != z_Loc)
                      {
                          Preview_zloc = z_Loc;
                          ZLocationChanged?.Invoke(this, new EventArgs());
                      }
                      Thread.Sleep(10);
                  }
              });
            LocationListener.Start();

            //更新初始偏移值
            Origin_X = X_Loc;
            Origin_Y = Y_Loc;
            Origin_Z = Z_Loc;
        }

        /// <summary>
        /// 位移台移动(同一时间一个位移台只能存在一个指令,多余指令被忽略)
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool Move(MoverAxis axis, MoveDirection direction, int step, int maxSpeed)
        {
            int histZ = 0;
            try
            {
                if (axis == MoverAxis.X)
                {
                    if (X_Moving == true || Z_Moving == true) return false;
                    X_Moving = true;
                    Z_Moving = true;
                    IsChangeMoving = false;
                    //将X和Z按照速度进行分割
                    string xdire = direction == MoveDirection.Backward ? "B" : "F";

                    step = Math.Abs(step);

                    int count = step / maxSpeed;
                    int resx = step - maxSpeed * count;

                    double Zdelt = 0;

                    for (int i = 0; i < count; i++)
                    {
                        if (histZ != Z_Loc)
                        {
                            //存在倾角,计算Z轴偏移值
                            Zdelt = Origin_Z + (X_Loc + ConverttoNumber(direction) * maxSpeed - Origin_X) * Math.Tan(AngleDiltX) + (Y_Loc - Origin_Y) * Math.Tan(AngleDiltY) - Z_Loc;
                            PortArranger.AddMessage("WZ" + (Zdelt < 0 ? "B" : "F") + ((int)(Math.Abs(Zdelt))).ToString("D5") + "#");
                            if (((int)(Math.Abs(Zdelt))) != 0)
                            {
                                histZ = Z_Loc;
                            }
                        }
                        PortArranger.AddMessage("WX" + xdire + maxSpeed.ToString("D5") + "#");
                        Thread.Sleep(40);
                    }
                    IsChangeMoving = true;
                    //存在倾角,计算Z轴偏移值
                    Zdelt = Origin_Z + (X_Loc + ConverttoNumber(direction) * resx - Origin_X) * Math.Tan(AngleDiltX) + (Y_Loc - Origin_Y) * Math.Tan(AngleDiltY) - Z_Loc;
                    PortArranger.AddMessage("WX" + xdire + resx.ToString("D5") + "#");
                    PortArranger.AddMessage("WZ" + (Zdelt < 0 ? "B" : "F") + ((int)(Math.Abs(Zdelt))).ToString("D5") + "#");
                    Thread.Sleep(40);
                    return true;
                }
                if (axis == MoverAxis.Y)
                {
                    if (Y_Moving == true || Z_Moving == true) return false;
                    Y_Moving = true;
                    Z_Moving = true;

                    IsChangeMoving = false;

                    //将X和Z按照速度进行分割
                    string ydire = direction == MoveDirection.Backward ? "B" : "F";

                    step = Math.Abs(step);

                    int count = step / maxSpeed;
                    int resy = step - maxSpeed * count;
                    double Zdelt = 0;

                    for (int i = 0; i < count; i++)
                    {
                        if (histZ != Z_Loc)
                        {
                            Zdelt = Origin_Z + (Y_Loc + ConverttoNumber(direction) * maxSpeed - Origin_Y) * Math.Tan(AngleDiltY) + (X_Loc - Origin_X) * Math.Tan(AngleDiltX) - Z_Loc;
                            PortArranger.AddMessage("WZ" + (Zdelt < 0 ? "B" : "F") + ((int)Math.Abs(Zdelt)).ToString("D5") + "#");
                            if (((int)(Math.Abs(Zdelt))) != 0)
                            {
                                histZ = Z_Loc;
                            }
                        }
                        PortArranger.AddMessage("WY" + ydire + maxSpeed.ToString("D5") + "#");
                        Thread.Sleep(40);
                    }

                    IsChangeMoving = true;
                    Zdelt = Origin_Z + (Y_Loc + ConverttoNumber(direction) * resy - Origin_Y) * Math.Tan(AngleDiltY) + (X_Loc - Origin_X) * Math.Tan(AngleDiltX) - Z_Loc;
                    PortArranger.AddMessage("WY" + ydire + resy.ToString("D5") + "#");
                    PortArranger.AddMessage("WZ" + (Zdelt < 0 ? "B" : "F") + ((int)Math.Abs(Zdelt)).ToString("D5") + "#");
                    Thread.Sleep(40);
                    return true;
                }
                if (axis == MoverAxis.Z)
                {
                    if (Z_Moving == true) return false;

                    Z_Moving = true;
                    IsChangeMoving = false;

                    step = Math.Abs(step);

                    int count = step / maxSpeed;
                    int resz = step - maxSpeed * count;

                    for (int i = 0; i < count; i++)
                    {
                        PortArranger.AddMessage("WZ" + (direction == MoveDirection.Backward ? "B" : "F") + Math.Abs(maxSpeed).ToString("D5") + "#");
                        Origin_Z += (direction == MoveDirection.Backward ? -1 : 1) * Math.Abs(maxSpeed);
                        Thread.Sleep(40);
                    }
                    IsChangeMoving = true;
                    PortArranger.AddMessage("WZ" + (direction == MoveDirection.Backward ? "B" : "F") + Math.Abs(resz).ToString("D5") + "#");
                    Origin_Z += (direction == MoveDirection.Backward ? -1 : 1) * Math.Abs(resz);
                    Thread.Sleep(40);
                    return true;
                }
                return false;
            }
            catch (Exception) { return false; }
        }

        /// <summary>
        /// 位移台移动(同一时间一个位移台只能存在一个指令,多余指令被忽略)
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="direction"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public bool Move(MoverAxis axis, MoveDirection direction, int step)
        {
            try
            {
                if (axis == MoverAxis.X)
                {
                    if (X_Moving == true || Z_Moving == true) return false;
                    X_Moving = true;
                    Z_Moving = true;
                    //存在倾角,计算Z轴偏移值
                    double Zdelt = Origin_Z + (X_Loc + ConverttoNumber(direction) * step - Origin_X) * Math.Tan(AngleDiltX) + (Y_Loc - Origin_Y) * Math.Tan(AngleDiltY) - Z_Loc;
                    PortArranger.AddMessage("WX" + (direction == MoveDirection.Backward ? "B" : "F") + (Math.Abs(step)).ToString("D5") + "#");
                    PortArranger.AddMessage("WZ" + (Zdelt < 0 ? "B" : "F") + ((int)Math.Abs(Zdelt)).ToString("D5") + "#");

                    return true;
                }
                if (axis == MoverAxis.Y)
                {
                    if (Y_Moving == true || Z_Moving == true) return false;
                    Y_Moving = true;
                    Z_Moving = true;
                    double Zdelt = Origin_Z + (Y_Loc + ConverttoNumber(direction) * step - Origin_Y) * Math.Tan(AngleDiltY) + (X_Loc - Origin_X) * Math.Tan(AngleDiltX) - Z_Loc;
                    PortArranger.AddMessage("WY" + (direction == MoveDirection.Backward ? "B" : "F") + (Math.Abs(step)).ToString("D5") + "#");
                    PortArranger.AddMessage("WZ" + (Zdelt < 0 ? "B" : "F") + ((int)Math.Abs(Zdelt)).ToString("D5") + "#");
                    return true;
                }
                if (axis == MoverAxis.Z)
                {
                    if (Z_Moving == true) return false;
                    Z_Moving = true;
                    PortArranger.AddMessage("WZ" + (direction == MoveDirection.Backward ? "B" : "F") + (Math.Abs(step)).ToString("D5") + "#");
                    Origin_Z += (direction == MoveDirection.Backward ? -1 : 1) * Math.Abs(step);
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
        public bool MoveAndWait(MoverAxis axis, MoveDirection direction, int step, int waitTime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Move(axis, direction, step);
            while (true)
            {
                if (axis == MoverAxis.X)
                {
                    if (X_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Y)
                {
                    if (Y_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Z)
                {
                    if (Z_Moving == false) return true;
                }
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    if (axis == MoverAxis.X || axis == MoverAxis.Y)
                    {
                        Terminate(axis, 1000);
                        Terminate(MoverAxis.Z, 1000);
                    }
                    else
                    {
                        Terminate(axis, 1000);
                    }
                    return false;
                }
            }
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
        public bool MoveAndWait(MoverAxis axis, MoveDirection direction, int step, int waitTime, int maxSpeed)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Move(axis, direction, step, maxSpeed);
            while (true)
            {
                if (axis == MoverAxis.X)
                {
                    if (X_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Y)
                {
                    if (Y_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Z)
                {
                    if (Z_Moving == false) return true;
                }
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    if (axis == MoverAxis.X || axis == MoverAxis.Y)
                    {
                        Terminate(axis, 1000);
                        Terminate(MoverAxis.Z, 1000);
                    }
                    else
                    {
                        Terminate(axis, 1000);
                    }
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
        public bool MoveAllAxis(MoveDirection dirsx, int stepx, MoveDirection dirsy, int stepy, MoveDirection dirsz, int stepz)
        {
            try
            {

                if (X_Moving || Y_Moving || Z_Moving) return false;
                //计算偏移值
                X_Moving = true;
                Y_Moving = true;
                Z_Moving = true;
                double Zdelt = Origin_Z + (X_Loc + stepx * ConverttoNumber(dirsx) - Origin_X) * Math.Tan(AngleDiltX) + (Y_Loc + ConverttoNumber(dirsy) * stepy - Origin_Y) * Math.Tan(AngleDiltY) - Z_Loc;
                PortArranger.AddMessage("WX" + (dirsx == MoveDirection.Backward ? "B" : "F") + stepx.ToString("D5") + "#");
                PortArranger.AddMessage("WY" + (dirsy == MoveDirection.Backward ? "B" : "F") + stepy.ToString("D5") + "#");
                int z = (int)Zdelt + ConverttoNumber(dirsz) * stepz;
                PortArranger.AddMessage("WZ" + (z < 0 ? "B" : "F") + Math.Abs(z).ToString("D5") + "#");
                Origin_Z += ConverttoNumber(dirsz) * stepz;
                return true;
            }

            catch (Exception) { return false; }
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
        public bool MoveAllAxis(MoveDirection dirsx, int stepx, MoveDirection dirsy, int stepy, MoveDirection dirsz, int stepz, int maxSpeed)
        {
            try
            {

                if (X_Moving || Y_Moving || Z_Moving) return false;
                //计算偏移值
                X_Moving = true;
                Y_Moving = true;
                double Zdelt = Origin_Z + (X_Loc + stepx * ConverttoNumber(dirsx) - Origin_X) * Math.Tan(AngleDiltX) + (Y_Loc + ConverttoNumber(dirsy) * stepy - Origin_Y) * Math.Tan(AngleDiltY) - Z_Loc;
                PortArranger.AddMessage("WX" + (dirsx == MoveDirection.Backward ? "B" : "F") + stepx.ToString("D5") + "#");
                PortArranger.AddMessage("WY" + (dirsy == MoveDirection.Backward ? "B" : "F") + stepy.ToString("D5") + "#");
                int z = (int)Zdelt + ConverttoNumber(dirsz) * stepz;
                PortArranger.AddMessage("WZ" + (z < 0 ? "B" : "F") + Math.Abs(z).ToString("D5") + "#");
                Origin_Z += ConverttoNumber(dirsz) * stepz;
                return true;
            }

            catch (Exception) { return false; }
        }

        private int ConverttoNumber(MoveDirection dir)
        {
            return dir == MoveDirection.Backward ? -1 : 1;
        }

        private MoveDirection ConvertToDirection(int number)
        {
            return number < 0 ? MoveDirection.Backward : MoveDirection.Forward;
        }

        /// <summary>
        /// 移动所有轴并且等待
        /// </summary>
        /// <param name="dirsx"></param>
        /// <param name="stepx"></param>
        /// <param name="dirsy"></param>
        /// <param name="stepy"></param>
        /// <param name="dirsz"></param>
        /// <param name="stepz"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public bool MoveAllAxisAndWait(MoveDirection dirsx, int stepx, MoveDirection dirsy, int stepy, MoveDirection dirsz, int stepz, int waitTime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool result = MoveAllAxis(dirsx, stepx, dirsy, stepy, dirsz, stepz);
            if (result == false) return false;
            while (true)
            {
                if (!X_Moving && !Y_Moving && !Z_Moving)
                    return true;
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    TerminateAll(100);
                    return false;
                }
            }
        }


        /// <summary>
        /// 移动所有轴并且等待
        /// </summary>
        /// <param name="dirsx"></param>
        /// <param name="stepx"></param>
        /// <param name="dirsy"></param>
        /// <param name="stepy"></param>
        /// <param name="dirsz"></param>
        /// <param name="stepz"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public bool MoveAllAxisAndWait(MoveDirection dirsx, int stepx, MoveDirection dirsy, int stepy, MoveDirection dirsz, int stepz, int waitTime, int maxSpeed)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool result = MoveAllAxis(dirsx, stepx, dirsy, stepy, dirsz, stepz, maxSpeed);
            if (result == false) return false;
            while (true)
            {
                if (!X_Moving && !Y_Moving && !Z_Moving) return true;
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    TerminateAll(100);
                    return false;
                }
            }
        }

        /// <summary>
        /// 移动所有轴到指定位置
        /// </summary>
        /// <param name="dirsx">X方向</param>
        /// <param name="stepx">Y方向</param>
        /// <param name="dirsy">Z方向</param>
        /// <param name="stepy">X步数</param>
        /// <param name="dirsz">Y步数</param>
        /// <param name="stepz">Z步数</param>
        /// <returns></returns>
        public bool MoveAllAxisTo(int targetLocationx, int targetLocationy, int targetLocationz)
        {
            try
            {
                int stepx = 0, stepy = 0, stepz = 0;
                if (X_Moving || Y_Moving || Z_Moving) return false;
                stepx = targetLocationx - X_Loc;
                stepy = targetLocationy - Y_Loc;
                stepz = targetLocationz - Z_Loc;

                return MoveAllAxis(ConvertToDirection(Math.Sign(stepx)), Math.Abs(stepx), ConvertToDirection(Math.Sign(stepy)), Math.Abs(stepy), ConvertToDirection(Math.Sign(stepz)), Math.Abs(stepz));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 移动所有轴到指定位置
        /// </summary>
        /// <param name="dirsx">X方向</param>
        /// <param name="stepx">Y方向</param>
        /// <param name="dirsy">Z方向</param>
        /// <param name="stepy">X步数</param>
        /// <param name="dirsz">Y步数</param>
        /// <param name="stepz">Z步数</param>
        /// <returns></returns>
        public bool MoveAllAxisTo(int targetLocationx, int targetLocationy, int targetLocationz, int maxSpeed)
        {
            try
            {
                int stepx = 0, stepy = 0, stepz = 0;
                if (X_Moving || Y_Moving || Z_Moving) return false;
                stepx = targetLocationx - X_Loc;
                stepy = targetLocationy - Y_Loc;
                stepz = targetLocationz - Z_Loc;

                return MoveAllAxis(ConvertToDirection(Math.Sign(stepx)), Math.Abs(stepx), ConvertToDirection(Math.Sign(stepy)), Math.Abs(stepy), ConvertToDirection(Math.Sign(stepz)), Math.Abs(stepz), maxSpeed);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 移动所有轴到指定位置并且等待
        /// </summary>
        /// <param name="dirsx"></param>
        /// <param name="stepx"></param>
        /// <param name="dirsy"></param>
        /// <param name="stepy"></param>
        /// <param name="dirsz"></param>
        /// <param name="stepz"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public bool MoveAllAxisToAndWait(int targetLocationx, int targetLocationy, int targetLocationz, int waitTime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool result = MoveAllAxisTo(targetLocationx, targetLocationy, targetLocationz);
            if (result == false) return false;
            while (true)
            {
                if (!X_Moving && !Y_Moving && !Z_Moving) return true;
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    TerminateAll(100);
                    return false;
                }
            }
        }

        /// <summary>
        /// 移动所有轴到指定位置并且等待
        /// </summary>
        /// <param name="dirsx"></param>
        /// <param name="stepx"></param>
        /// <param name="dirsy"></param>
        /// <param name="stepy"></param>
        /// <param name="dirsz"></param>
        /// <param name="stepz"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public bool MoveAllAxisToAndWait(int targetLocationx, int targetLocationy, int targetLocationz, int waitTime, int maxSpeed)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            bool result = MoveAllAxisTo(targetLocationx, targetLocationy, targetLocationz, maxSpeed);
            if (result == false) return false;
            while (true)
            {
                if (!X_Moving && !Y_Moving && !Z_Moving) return true;
                if (timer.ElapsedMilliseconds > waitTime)
                {
                    TerminateAll(100);
                    return false;
                }
            }
        }

        /// <summary>
        /// 位移台移动到(同一时间一个位移台只能存在一个指令,多余指令被忽略)
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="targetLocation">目标位置</param>
        /// <returns></returns>
        public bool MoveTo(MoverAxis axis, int targetLocation)
        {
            if (targetLocation < 0) return false;
            int step = 0;
            if (axis == MoverAxis.X)
            {
                if (X_Moving == true || Z_Moving == true) return false;
                step = targetLocation - X_Loc;
                return Move(MoverAxis.X, step < 0 ? MoveDirection.Backward : MoveDirection.Forward, Math.Abs(step));
            }
            if (axis == MoverAxis.Y)
            {
                if (Y_Moving == true || Z_Moving == true) return false;
                step = targetLocation - Y_Loc;
                return Move(MoverAxis.Y, step < 0 ? MoveDirection.Backward : MoveDirection.Forward, Math.Abs(step));
            }
            if (axis == MoverAxis.Z)
            {
                if (Z_Moving == true) return false;
                step = targetLocation - Z_Loc;
                return Move(MoverAxis.Z, step < 0 ? MoveDirection.Backward : MoveDirection.Forward, Math.Abs(step));
            }
            return false;
        }

        /// <summary>
        /// 位移台移动到(同一时间一个位移台只能存在一个指令,多余指令被忽略)
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="targetLocation">目标位置</param>
        /// <returns></returns>
        public bool MoveTo(MoverAxis axis, int targetLocation, int maxSpeed)
        {
            if (targetLocation < 0) return false;
            int step = 0;
            if (axis == MoverAxis.X)
            {
                if (X_Moving == true || Z_Moving == true) return false;
                step = targetLocation - X_Loc;
                return Move(MoverAxis.X, step < 0 ? MoveDirection.Backward : MoveDirection.Forward, Math.Abs(step), maxSpeed);
            }
            if (axis == MoverAxis.Y)
            {
                if (Y_Moving == true || Z_Moving == true) return false;
                step = targetLocation - Y_Loc;
                return Move(MoverAxis.Y, step < 0 ? MoveDirection.Backward : MoveDirection.Forward, Math.Abs(step), maxSpeed);
            }
            if (axis == MoverAxis.Z)
            {
                if (Z_Moving == true) return false;
                step = targetLocation - Z_Loc;
                return Move(MoverAxis.Z, step < 0 ? MoveDirection.Backward : MoveDirection.Forward, Math.Abs(step), maxSpeed);
            }
            return false;
        }

        /// <summary>
        ///位移台移动到并阻塞等待其移动完成
        /// </summary>
        /// <param name="axis">运动轴</param>
        /// <param name="direction">运动方向</param>
        /// <param name="targetLocation">目标位置</param>
        /// <param name="waitTime">等待时间，超过等待时间直接返回</param>
        /// <returns></returns>
        /// <returns></returns>
        public bool MoveToAndWait(MoverAxis axis, int targetLocation, int waitTime)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            MoveTo(axis, targetLocation);
            while (true)
            {
                if (axis == MoverAxis.X)
                {
                    if (X_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Y)
                {
                    if (Y_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Z)
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
        ///位移台移动到并阻塞等待其移动完成
        /// </summary>
        /// <param name="axis">运动轴</param>
        /// <param name="direction">运动方向</param>
        /// <param name="targetLocation">目标位置</param>
        /// <param name="waitTime">等待时间，超过等待时间直接返回</param>
        /// <returns></returns>
        /// <returns></returns>
        public bool MoveToAndWait(MoverAxis axis, int targetLocation, int waitTime, int maxSpeed)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            MoveTo(axis, targetLocation, maxSpeed);
            while (true)
            {
                if (axis == MoverAxis.X)
                {
                    if (X_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Y)
                {
                    if (Y_Moving == false && Z_Moving == false) return true;
                }
                if (axis == MoverAxis.Z)
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
        /// 急停指定轴（耗时操作）
        /// </summary>
        /// <returns></returns>
        public bool Terminate(MoverAxis axis, int waittime)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                if (axis == MoverAxis.X)
                {
                    Is_X_Aborted = false;
                    PortArranger.AddProierMessage("E" + ID.ToString() + "X" + "ABORT#");
                    while (Is_X_Aborted == false && watch.ElapsedTicks <= waittime)
                    {
                        Thread.Sleep(10);
                    }
                    if (Is_X_Aborted == false) return false;
                    return true;
                }
                if (axis == MoverAxis.Y)
                {
                    Is_Y_Aborted = false;
                    PortArranger.AddProierMessage("E" + ID.ToString() + "Y" + "ABORT#");
                    while (Is_Y_Aborted == false && watch.ElapsedTicks <= waittime)
                    {
                        Thread.Sleep(10);
                    }
                    if (Is_Y_Aborted == false) return false;
                    return true;
                }
                if (axis == MoverAxis.Z)
                {
                    Is_Z_Aborted = false;
                    PortArranger.AddProierMessage("E" + ID.ToString() + "Z" + "ABORT#");
                    while (Is_Z_Aborted == false && watch.ElapsedTicks <= waittime)
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
        /// 急停所有轴（耗时操作）
        /// </summary>
        /// <returns></returns>
        public bool TerminateAll(int maxWaitTime)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                Is_All_Aborted = false;
                PortArranger.AddProierMessage("E" + id.ToString() + "A" + "ABORT#");
                while (Is_All_Aborted == false && watch.ElapsedTicks <= maxWaitTime)
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
        /// 位移台初始化(耗时操作)
        /// </summary>
        /// <returns></returns>
        public bool Initial(int MaxTime)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //先停止所有操作
            PortArranger.ClearQueue();
            TerminateAll(100);
            PortArranger.AddMessage("E" + ID.ToString() + "I00000#");
            while (Is_Initialed == false && watch.ElapsedMilliseconds < MaxTime)
            {
                Thread.Sleep(10);
            }
            if (Is_Initialed == false)
            {
                Is_Initialed = true;
                return false;
            }
            Origin_X = 1;
            Origin_Y = 1;
            Origin_Z = 1;
            return true;
        }

        /// <summary>
        /// 释放资源进行的操作
        /// </summary>
        internal void DisposeAction()
        {
            LocationListener?.Abort();
            if (LocationListener == null) return;
            while (LocationListener.ThreadState == System.Threading.ThreadState.Running)
            {
                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        internal void InitAction()
        {
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

/// <summary>
/// 位移轴
/// </summary>
public enum MoverAxis
{
    X = 0,
    Y = 1,
    Z = 2
}

/// <summary>
/// 移动方向
/// </summary>
public enum MoveDirection
{
    Forward = 0,
    Backward = 1
}

