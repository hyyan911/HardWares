using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Diagnostics;
using HardWares.端口基类;
using HardWares.端口基类部分;

namespace HardWares.旋转台
{
    public enum CloseLoopMode
    {
        /// <summary>
        /// 速度闭环
        /// </summary>
        Speed_CloseLoop = 0,
        /// <summary>
        /// 转矩闭环
        /// </summary>
        Torque_CloseLoop = 1,
        /// <summary>
        /// 角度闭环
        /// </summary>
        Angle_CloseLoop = 2,
        /// <summary>
        /// 无闭环
        /// </summary>
        None = 3,
    }

    /// <summary>
    /// 触发事件
    /// </summary>
    public partial class RotateMotor
    {
        /// <summary>
        /// 产品名
        /// </summary>
        public new string ProductName { get; internal set; } = "旋转台";

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "旋转台 MS4005";

        /// <summary>
        /// 警报触发事件
        /// </summary>
        public event EventHandler WarningEvent = null;
        public override event ParamsChangeEventHandler ParamsChangedEvent;

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
    /// 旋转电动机
    /// </summary>
    public partial class RotateMotor : PortObject
    {
        internal Thread MotorState_Detector { get; set; } = null;

        internal RS485Frame FrameDetector { get; set; }

        /// <summary>
        ///角度偏差
        /// </summary>
        public double AngleError { get; set; } = 0.05;

        private int isMoving = 0;

        /// <summary>
        /// 是否正在移动
        /// </summary>
        public bool Is_Moving
        {
            get
            {
                return Thread.VolatileRead(ref isMoving) == 0 ? false : true;
            }
            internal set
            {
                Thread.VolatileWrite(ref isMoving, value ? 1 : 0);
            }
        }

        //-------------------参数--------------------
        #region PID参数
        private byte angle_P = 0;
        /// <summary>
        /// 角度反馈PID中的P参数(设置后需要等待机器参数改变)
        /// </summary>
        public byte Angle_P
        {
            get
            {
                return Thread.VolatileRead(ref angle_P);
            }
            set
            {
                //发送角度改变命令
                SetPIDParams(value, Angle_I, Speed_P, Speed_I, Iq_P, Iq_I);
            }
        }

        private byte angle_I = 0;
        /// <summary>
        /// 角度反馈PID中的I参数(设置后需要等待机器参数改变)
        /// </summary>
        public byte Angle_I
        {
            get
            {
                return Thread.VolatileRead(ref angle_I);
            }
            set
            {
                //发送角度改变命令
                SetPIDParams(Angle_P, value, Speed_P, Speed_I, Iq_P, Iq_I);
            }
        }

        private byte speed_P;
        /// <summary>
        /// 速度反馈PID中的P参数(设置后需要等待机器参数改变)
        /// </summary>
        public byte Speed_P
        {
            get
            {
                return Thread.VolatileRead(ref speed_P);
            }
            set
            {
                //发送角度改变命令
                SetPIDParams(Angle_P, Angle_I, value, Speed_I, Iq_P, Iq_I);
            }
        }

        private byte speed_I;
        /// <summary>
        /// 速度反馈PID中的I参数(设置后需要等待机器参数改变)
        /// </summary>
        public byte Speed_I
        {
            get
            {
                return Thread.VolatileRead(ref speed_I);
            }
            set
            {
                //发送角度改变命令
                SetPIDParams(Angle_P, Angle_I, Speed_P, value, Iq_P, Iq_I);
            }
        }

        private byte iq_P;
        /// <summary>
        /// 转矩反馈PID中的P参数(设置后需要等待机器参数改变)
        /// </summary>
        public byte Iq_P
        {
            get
            {
                return Thread.VolatileRead(ref iq_P);
            }
            set
            {
                //发送角度改变命令
                SetPIDParams(Angle_P, Angle_I, Speed_P, Speed_I, value, Iq_I);
            }
        }

        private byte iq_I;
        /// <summary>
        /// 转矩反馈PID中的P参数(设置后需要等待机器参数改变)
        /// </summary>
        public byte Iq_I
        {
            get
            {
                return Thread.VolatileRead(ref iq_I);
            }
            set
            {
                //发送角度改变命令
                SetPIDParams(Angle_P, Angle_I, Speed_P, Speed_I, Iq_P, value);
            }
        }
        #endregion

        #region 加速度
        private int Preview_Accel = 0;
        private int accel = 0;
        /// <summary>
        /// 加速度参数(1dps/s)(设置后需要等待机器参数改变)
        /// </summary>
        public int Accel
        {
            get
            {
                return Thread.VolatileRead(ref accel);
            }
        }
        #endregion

        #region 编码器参数

        #endregion

        #region 物理参数
        private double Preview_Voltage = 0;
        private double motor_Voltage = 0;
        /// <summary>
        /// 电机电压(V)
        /// </summary>
        public double Motor_Voltage
        {
            get
            {
                return Thread.VolatileRead(ref motor_Voltage);
            }
        }

        private byte Preview_Temperature = 0;
        private byte motor_Temperature = 0;
        /// <summary>
        /// 电机温度
        /// </summary>
        public byte Motor_Temperature
        {
            get
            {
                return Thread.VolatileRead(ref motor_Temperature);
            }
        }


        private short Preview_Power = 0;
        private short power = 0;
        /// <summary>
        /// 输出功率(W)
        /// </summary>
        public short Power
        {
            get
            {
                return Thread.VolatileRead(ref power);
            }
        }
        #endregion

        #region 位置（角度）参数
        private double Preview_RealAngle = 0;
        private double motor_RealAngle = 0;
        /// <summary>
        /// 电机实际单圈角度(度)
        /// </summary>
        public double Motor_RealAngle
        {
            get
            {
                return Thread.VolatileRead(ref motor_RealAngle);
            }
        }

        private double Preview_RealMultiAngle = 0;
        private double motor_RealMultiAngle = 0;
        /// <summary>
        /// 电机实际多圈角度（度）
        /// </summary>
        public double Motor_RealMultiAngle
        {
            get
            {
                return Thread.VolatileRead(ref motor_RealMultiAngle);
            }
        }
        #endregion

        #region 速度参数 
        private short Preview_RealSpeed = 0;
        private short motor_RealSpeed = 0;
        /// <summary>
        /// 电机实际速度(单位dps)
        /// </summary>
        public short Motor_RealSpeed
        {
            get
            {
                return Thread.VolatileRead(ref motor_RealSpeed);
            }
        }
        #endregion

        #region 错误信息
        private int errorMessageID = 0;
        /// <summary>
        /// 错误信息编号
        /// </summary>
        public int ErrorMessageID
        {
            get
            {
                return errorMessageID;
            }
        }

        /// <summary>
        /// 警告信息
        /// </summary>
        public Exception Warning_Exception { get; set; } = null;
        #endregion

        #region 电机开关参数
        private int isOpen = 0;
        /// <summary>
        /// 电机是否打开
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return Thread.VolatileRead(ref isOpen) == 0 ? false : true;
            }
            set
            {
                Thread.VolatileWrite(ref isOpen, value ? 1 : 0);
            }
        }

        private int isStop = 0;
        /// <summary>
        /// 电机是否停止
        /// </summary>
        public bool IsStop
        {
            get
            {
                return Thread.VolatileRead(ref isStop) == 0 ? false : true;
            }
            set
            {
                Thread.VolatileWrite(ref isStop, value ? 1 : 0);
            }
        }
        #endregion

        #region PID模式
        private object loopMode = CloseLoopMode.None;
        /// <summary>
        /// PID反馈模式
        /// </summary>
        public CloseLoopMode LoopMode
        {
            get
            {
                return (CloseLoopMode)Thread.VolatileRead(ref loopMode);
            }
        }
        #endregion
        //-------------------------------------------
    }

    public partial class RotateMotor
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ID">电机ID</param>
        public RotateMotor(int ID)
        {
            id = ID;
            FrameDetector = new RS485Frame(ID);
        }


        /// <summary>
        /// 收到信息行为
        /// </summary>
        /// <param name="port"></param>
        public void ReceiveAct()
        {
            //接收到更新驱动信息的回复
            FrameDetector.ReadFrame(ReceiveBuffer, out List<byte> data, out byte commandType);
            if (data == null) return;
            //分析帧头
            //读取PID参数回复
            if (commandType == 0x14)
            {
                //提取参数并修改变量
                angle_P = data[2];
                angle_I = data[3];
                speed_P = data[4];
                speed_I = data[5];
                iq_P = data[6];
                iq_I = data[7];
                //触发PID参数改变事件
                ParamsChangedEvent?.Invoke(this, "Angle_P");
                ParamsChangedEvent?.Invoke(this, "Angle_I");
                ParamsChangedEvent?.Invoke(this, "Speed_P");
                ParamsChangedEvent?.Invoke(this, "Speed_I");
                ParamsChangedEvent?.Invoke(this, "Iq_P");
                ParamsChangedEvent?.Invoke(this, "Iq_I");
                return;
            }
            //写入PID参数到驱动静态内存命令（断电仍保存）
            if (commandType == 0x31)
            {
                //写入完成，读取当前PID参数
                PortArranger.AddMessage(FrameDetector.GenerateFrame(0x30, new byte[] { }));
                return;
            }
            //写入PID参数到驱动动态内存命令（断电不保存）
            if (commandType == 0x32)
            {
                //写入完成，读取当前PID参数
                PortArranger.AddMessage(FrameDetector.GenerateFrame(0x30, new byte[] { }));
                return;
            }
            //读取加速度命令
            if (commandType == 0x33)
            {
                int acce = 0;
                acce = BitConverter.ToInt32(new byte[] { data[0], data[1], data[2], data[3] }, 0);
                accel = acce;
                //触发加速度参数改变事件
                ParamsChangedEvent?.Invoke(this, "Accel");
                return;
            }
            //写入加速度到静态内存命令（断电仍保存）
            if (commandType == 0x34)
            {
                //写入完成，读取当前加速度参数
                PortArranger.AddMessage(FrameDetector.GenerateFrame(0x33, new byte[] { }));
                return;
            }
            //读取编码器命令(暂时不需要)
            if (commandType == 0x90)
            {

                return;
            }
            //写入编码器命令(暂时不需要)
            if (commandType == 0x91)
            {
                return;
            }
            //写入当前位置到静态内存作为电机零点（不使用，会损坏驱动芯片）
            if (commandType == 0x19)
            {
                return;
            }
            //读取当前多圈角度值
            if (commandType == 0x92)
            {
                long ang = 0;
                ang = BitConverter.ToInt64(data.ToArray(), 0);
                motor_RealMultiAngle = 0.01 * ang;
                return;
            }
            //读取当前单圈角度值
            if (commandType == 0x94)
            {
                ushort ang = 0;
                ang = BitConverter.ToUInt16(data.ToArray(), 0);
                motor_RealAngle = 0.01 * ang;
                return;
            }
            //清除角度数据并将当前位置设置为电机零点，断电后失效
            if (commandType == 0x95)
            {
                return;
            }
            //读取电机状态（温度，电压，错误状态标志）
            if (commandType == 0x9A)
            {
                motor_Temperature = data[0];

                motor_Voltage = 0.1 * BitConverter.ToUInt16(new byte[] { data[2], data[3] }, 0);

                errorMessageID = data[6];

                Exception ex;
                //最低位电压位
                if ((errorMessageID & 1) == 0 && (errorMessageID >> 3 & 1) == 0)
                {
                    Warning_Exception = null;
                }
                if ((errorMessageID & 1) == 1)
                {
                    ex = new Exception("电压过低");
                    Warning_Exception = ex;
                    WarningEvent?.Invoke(this, new EventArgs());
                }
                if ((errorMessageID >> 3 & 1) == 1)
                {
                    ex = new Exception("温度过高");
                    Warning_Exception = ex;
                    WarningEvent?.Invoke(this, new EventArgs());
                }
                return;
            }
            //清除电机错误标志
            if (commandType == 0x9B)
            {
                motor_Temperature = data[1];

                motor_Voltage = 0.1 * BitConverter.ToUInt16(new byte[] { data[2], data[3] }, 0);

                errorMessageID = data[6];

                Exception ex;
                //最低位电压位
                if ((errorMessageID & 1) == 0 && (errorMessageID >> 3 & 1) == 0)
                {
                    Warning_Exception = null;
                }
                if ((errorMessageID & 1) == 1)
                {
                    ex = new Exception("电压过低");
                    Warning_Exception = ex;
                    WarningEvent?.Invoke(this, new EventArgs());
                }
                if ((errorMessageID >> 3 & 1) == 1)
                {
                    ex = new Exception("温度过高");
                    Warning_Exception = ex;
                    WarningEvent?.Invoke(this, new EventArgs());
                }

                return;
            }
            //读取电机状态命令2（温度，电压，转速）
            if (commandType == 0x9C)
            {
                motor_Temperature = data[0];

                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);

                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);

                return;
            }
            //读取电机状态命令3（温度，A,B,C相电流）
            if (commandType == 0x9D)
            {
                motor_Temperature = data[0];

                return;
            }
            //电机关闭指令
            if (commandType == 0x80)
            {
                loopMode = CloseLoopMode.None;
                IsOpen = false;
                IsStop = false;
                return;
            }
            //电机停止指令
            if (commandType == 0x81)
            {
                IsOpen = true;
                IsStop = true;
                return;
            }
            //电机运行指令（从停止指令中恢复）
            if (commandType == 0x88)
            {
                IsOpen = true;
                IsStop = false;
                return;
            }
            //启动转矩闭环PID控制指令(当前电机上未实现此指令)
            if (commandType == 0xA1)
            {
            }
            //启动速度闭环PID控制指令
            if (commandType == 0xA2)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Speed_CloseLoop;

                return;
            }
            //启动位置闭环PID控制指令1
            if (commandType == 0xA3)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Angle_CloseLoop;
                return;
            }
            //启动位置闭环PID控制指令2
            if (commandType == 0xA4)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Angle_CloseLoop;
                return;
            }
            //启动位置闭环PID控制指令3
            if (commandType == 0xA5)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Angle_CloseLoop;
                return;
            }
            //启动位置闭环PID控制指令4
            if (commandType == 0xA6)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Angle_CloseLoop;
                return;
            }
            //增量位置闭环控制指令
            if (commandType == 0xA7)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Angle_CloseLoop;
                return;
            }
            //启动位置闭环PID控制指令6
            if (commandType == 0xA8)
            {
                motor_Temperature = data[0];
                power = BitConverter.ToInt16(new byte[] { data[1], data[2] }, 0);
                motor_RealSpeed = BitConverter.ToInt16(new byte[] { data[3], data[4] }, 0);
                loopMode = CloseLoopMode.Angle_CloseLoop;
                return;
            }

            //读取设备信息
            if (commandType == 0x12)
            {
                Internal_Is_Connected = true;
            }
        }


        /// <summary>
        /// 连接成功后行为
        /// </summary>
        public void ConnectedAction()
        {
            ValidateParams();
            if (MotorState_Detector != null && MotorState_Detector.IsAlive == true)
            {
                MotorState_Detector.Abort();
            }

            ParamsChangedEvent += AngleListen;

            AngleListener = new Thread(() =>
              {
                  while (true)
                  {
                      //读取多圈角度信息
                      PortArranger.AddMessage(FrameDetector.GenerateFrame(0x92, new byte[] { }));

                      //读取单圈角度信息
                      PortArranger.AddMessage(FrameDetector.GenerateFrame(0x94, new byte[] { }));

                      Thread.Sleep(100);
                  }
              });
            AngleListener.Start();

            //状态监测线程
            MotorState_Detector = new Thread(() =>
              {
                  while (true)
                  {
                      //读取加速度信息
                      PortArranger.AddMessage(FrameDetector.GenerateFrame(0x33, new byte[] { }));

                      //读取电机状态1和错误状态
                      PortArranger.AddMessage(FrameDetector.GenerateFrame(0x9A, new byte[] { }));

                      //读取电机状态2
                      PortArranger.AddMessage(FrameDetector.GenerateFrame(0x9C, new byte[] { }));

                      Thread.Sleep(300);
                  }
              });
            MotorState_Detector.Start();

            ParamsListener = new Thread(() =>
              {
                  while (true)
                  {
                      if (Preview_Accel != accel)
                      {
                          Preview_Accel = accel;
                          ParamsChangedEvent?.Invoke(this, "Accel");
                      }
                      if (Preview_Power != power)
                      {
                          Preview_Power = power;
                          ParamsChangedEvent?.Invoke(this, "Power");
                      }
                      if (Preview_RealAngle != motor_RealAngle)
                      {
                          Preview_RealAngle = motor_RealAngle;
                          ParamsChangedEvent?.Invoke(this, "Motor_RealAngle");
                      }
                      if (Preview_RealMultiAngle != motor_RealMultiAngle)
                      {
                          Preview_RealMultiAngle = motor_RealMultiAngle;
                          ParamsChangedEvent?.Invoke(this, "Motor_RealMultiAngle");
                      }
                      if (Preview_RealSpeed != motor_RealSpeed)
                      {
                          Preview_RealSpeed = motor_RealSpeed;
                          ParamsChangedEvent?.Invoke(this, "Motor_RealSpeed");
                      }
                      if (Preview_Temperature != motor_Temperature)
                      {
                          Preview_Temperature = motor_Temperature;
                          ParamsChangedEvent?.Invoke(this, "Motor_Temperature");
                      }
                      if (Preview_Voltage != motor_Voltage)
                      {
                          Preview_Voltage = motor_Voltage;
                          ParamsChangedEvent?.Invoke(this, "Motor_Voltage");
                      }
                      Thread.Sleep(20);
                  }
              });
            ParamsListener.Start();
        }

        /// <summary>
        /// 角度监听事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="name"></param>
        private void AngleListen(object sender, string name)
        {
            if (name == "Motor_RealMultiAngle")
            {
                if (Math.Abs(TargetAngle - Motor_RealMultiAngle) < AngleError)
                {
                    ++inRangeCount;
                }
                else
                {
                    inRangeCount = 0;
                }
                if (inRangeCount == 10)
                {
                    Is_Moving = false;
                    inRangeCount = 0;
                }
            }
        }

        /// <summary>
        /// 初始化参数
        /// </summary>
        internal void InitAction()
        {
        }

        /// <summary>
        /// 发送测试信息
        /// </summary>
        internal void TestAction()
        {
            //测试指令
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x12, new byte[] { }));
        }

        /// <summary>
        /// 设置PID参数
        /// </summary>
        private void SetPIDParams(byte anglep, byte anglei, byte speedp, byte speedi, byte iqp, byte iqi)
        {
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x31, new byte[] { anglep, anglei, speedp, speedi, iqp, iqi }));
        }

        /// <summary>
        /// 更新电机静态参数
        /// </summary>
        public override void ValidateParams()
        {
            //读取PID参数
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x14, new byte[] { }));

            //读取多圈角度信息
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x92, new byte[] { }));

            Thread.Sleep(100);

            targetAngle = Motor_RealMultiAngle;
        }

        private double targetAngle = 0;
        /// <summary>
        /// 目标角度 
        /// </summary>
        public double TargetAngle { get { return Thread.VolatileRead(ref targetAngle); } }


        private Thread ParamsListener = null;

        private Thread AngleListener = null;
        private int inRangeCount = 0;

        /// <summary>
        /// 设置多圈角度,精度为0.01度（电机转动方向由目标和当前角度差决定）
        /// </summary>
        public void SetAngle(double angle, double Maxspeed)
        {
            //设置目标角度
            targetAngle = angle;
            ParamsChangedEvent?.Invoke(this, "TargetAngle");

            long inputangle = (long)Math.Truncate(angle * 100);
            uint inputspeed = (uint)(100 * Maxspeed);
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0xA4, BitConverter.GetBytes(inputangle).Concat(BitConverter.GetBytes(inputspeed)).ToArray()));
        }

        /// <summary>
        /// 设置多圈角度,精度为0.01度（电机转动方向由目标和当前角度差决定）
        /// </summary>
        public bool SetAngleAndWait(double angle, double Maxspeed, int maxWaittime)
        {
            if (Is_Moving == true) return false;
            Is_Moving = true;
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //设置目标角度
            targetAngle = angle;
            ParamsChangedEvent?.Invoke(this, "TargetAngle");

            long inputangle = (long)Math.Truncate(angle * 100);
            uint inputspeed = (uint)(100 * Maxspeed);
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0xA4, BitConverter.GetBytes(inputangle).Concat(BitConverter.GetBytes(inputspeed)).ToArray()));

            while (watch.ElapsedMilliseconds < maxWaittime && Is_Moving == true) { Thread.Sleep(20); }
            if (Is_Moving == false) return true;
            return false;
        }

        /// <summary>
        /// 设置角度，在现有基础上旋转一个增量，增量正负号决定旋转方向，,如果电机正在旋转则忽略此命令
        /// </summary>
        /// <param name="isClockWise"></param>
        /// <param name="angle"></param>
        public void SetAngleIncrement(double angleIncrement, double Maxspeed)
        {
            //设置目标角度
            targetAngle += angleIncrement;
            ParamsChangedEvent?.Invoke(this, "TargetAngle");

            int angleIn = (int)(100 * angleIncrement);
            uint speed = (uint)(100 * Maxspeed);
            List<byte> anglebytes = BitConverter.GetBytes(angleIn).ToList();
            List<byte> speedbytes = BitConverter.GetBytes(speed).ToList();

            //anglebytes.AddRange(speedbytes);
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0xA7, anglebytes.ToArray()));
        }

        /// <summary>
        /// 设置角度并等待旋转完成，在现有基础上旋转一个增量，增量正负号决定旋转方向，,如果电机正在旋转则忽略此命令
        /// </summary>
        /// <param name="isClockWise"></param>
        /// <param name="angle"></param>
        public bool SetAngleIncrementAndWait(double angleIncrement, int maxWaittime)
        {
            if (Is_Moving == true) return false;
            Is_Moving = true;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //设置目标角度
            targetAngle += angleIncrement;
            ParamsChangedEvent?.Invoke(this, "TargetAngle");

            Is_Moving = true;
            int angleIn = (int)(100 * angleIncrement);
            List<byte> anglebytes = BitConverter.GetBytes(angleIn).ToList();
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0xA7, anglebytes.ToArray()));

            while (watch.ElapsedMilliseconds < maxWaittime && Is_Moving == true) { Thread.Sleep(20); }
            if (Is_Moving == false) return true;
            return false;
        }

        /// <summary>
        /// 设置加速度(单位：1dps/s)
        /// </summary>
        public void SetAccelerate(uint accelerate)
        {
            byte[] bytes = BitConverter.GetBytes(accelerate);
            //写入加速度
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x34, bytes));
        }

        /// <summary>
        /// 设置速度，同时进入速度闭环模式,正负号表示旋转方向
        /// </summary>
        public void SetSpeed(double speed)
        {
            int realspeed = (int)(100 * speed);
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0xA2, BitConverter.GetBytes(realspeed)));
        }

        /// <summary>
        /// 启动电机
        /// </summary>
        public void Open()
        {
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x88, new byte[] { }));
        }

        /// <summary>
        /// 停止电机
        /// </summary>
        public void Stop()
        {
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x81, new byte[] { }));
        }

        /// <summary>
        /// 关闭电机
        /// </summary>
        public void Close()
        {
            PortArranger.AddMessage(FrameDetector.GenerateFrame(0x80, new byte[] { }));
        }

        /// <summary>
        /// 释放资源时进行的操作
        /// </summary>
        internal void DisposeAction()
        {
            if (MotorState_Detector != null)
            {
                try
                {
                    MotorState_Detector.Abort();
                }
                catch { }
            }
            if (AngleListener != null)
            {
                try
                {
                    AngleListener.Abort();
                }
                catch { }
            }
            if (ParamsListener != null)
            {
                try
                {
                    ParamsListener.Abort();
                }
                catch { }
            }
        }
    }
}
