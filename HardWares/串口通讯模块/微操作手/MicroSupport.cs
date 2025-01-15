using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using HardWares.端口基类;
using HardWares.端口基类部分;
using Controls;

namespace HardWares.微操作手
{
    /// <summary>
    /// 微操作手
    /// </summary>
    public partial class MicroSupport : PortObject
    {

        /// <summary>
        /// 产品名
        /// </summary>
        public new string ProductName { get; internal set; } = "微操作手";

        /// <summary>
        /// 产品型号
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "Microsupport微操作手";


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
            set
            {
                Thread.VolatileWrite(ref isMoving, value ? 1 : 0);
            }
        }

        private int isReseting = 0;
        /// <summary>
        /// 是否正在还原
        /// </summary>
        public bool IsReseting
        {
            get
            {
                return Thread.VolatileRead(ref isReseting) == 0 ? false : true;
            }
            set
            {
                Thread.VolatileWrite(ref isReseting, value ? 1 : 0);
            }
        }

        /// <summary>
        /// X上限
        /// </summary>
        private int UpperX = 10000;
        /// <summary>
        /// Y上限
        /// </summary>
        private int UpperY = 10000;
        /// <summary>
        /// Z上限
        /// </summary>
        private int UpperZ = 10000;

        /// <summary>
        /// X下限
        /// </summary>
        private int LowerX = 0;
        /// <summary>
        /// Y下限
        /// </summary>
        private int LowerY = 0;
        /// <summary>
        /// Z下限
        /// </summary>
        private int LowerZ = 0;


        private int xloc = 5000;
        /// <summary>
        /// X位置
        /// </summary>
        public int XLocation
        {
            get
            {
                return Thread.VolatileRead(ref xloc);
            }
        }

        private int yloc = 5000;
        /// <summary>
        /// Y位置
        /// </summary>
        public int YLocation
        {
            get
            {
                return Thread.VolatileRead(ref yloc);
            }
        }

        private int zloc = 0;
        /// <summary>
        /// Z位置
        /// </summary>
        public int ZLocation
        {
            get
            {
                return Thread.VolatileRead(ref zloc);
            }
        }

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 错误信息事件
        /// </summary>
        public event EventHandler WrongDataEvent = null;

        public void ConnectedAction()
        {
            ResetMouse();
        }

        /// <summary>
        /// 是否接收到信息
        /// </summary>
        private bool Isreceived = false;

        /// <summary>
        /// 指令码
        /// </summary>
        private byte messageCommand = 0;


        public void ReceiveAct()
        {
            GetFrame(out byte head, out List<byte> data);
            if (data == null) return;

            //错误码
            if (head == (byte)(messageCommand | 0xC0))
            {
                WrongDataEvent?.Invoke(this, new EventArgs());
            }

            ///正确回复
            if (head == (byte)(messageCommand | 0x80))
            {
                if (messageCommand == 0x05)
                {
                }
                if (messageCommand == 0x01)
                {
                    Internal_Is_Connected = true;
                }
            }

            Isreceived = true;
        }

        public override void ValidateParams()
        {
        }

        internal void DisposeAction()
        {
            if (mousebtn != null) UnregisterMouseButton();
        }

        internal void InitAction()
        {
        }

        internal void TestAction()
        {
            SendFrame(0x01, new List<byte>() { }, 50);
        }

        /// <summary>
        /// 复位初始化(耗时操作)
        /// </summary>
        /// <param name="MaxSec"></param>
        public void Init()
        {
            LowerX = -10000;
            LowerY = -10000;
            LowerZ = -10000;

            Move(-10000, -10000);
            Move(1000);
            xloc = 0;
            yloc = 0;
            zloc = 0;
            ParamsChangedEvent?.Invoke(this, "XLocation");
            ParamsChangedEvent?.Invoke(this, "YLocation");
            ParamsChangedEvent?.Invoke(this, "ZLocation");
            LowerX = 0;
            LowerY = 0;
            LowerZ = 0;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        private void SendFrame(byte head, List<byte> data, long waittime)
        {
            Isreceived = false;

            messageCommand = head;

            List<byte> mes = new List<byte>();
            //帧头
            mes.Add(0x57);
            mes.Add(0xAB);
            //地址码
            mes.Add(0x00);
            //命令码
            mes.Add(head);
            //数据长度
            mes.Add((byte)data.Count);
            //数据体
            mes.AddRange(data);

            byte sum = (byte)mes.Sum(x => x);

            //校验和
            mes.Add(sum);

            PortArranger.AddMessage(mes);

            Stopwatch watch = new Stopwatch();
            watch.Start();
            while (Isreceived == false && watch.ElapsedMilliseconds < waittime)
            {
                Thread.Sleep(5);
            };
        }

        /// <summary>
        /// 获取数据并且进行分析
        /// </summary>
        private void GetFrame(out byte head, out List<byte> data)
        {
            try
            {
                for (int i = 0; i < ReceiveBuffer.Count - 4; ++i)
                {
                    if (ReceiveBuffer[i] == 0x57 && ReceiveBuffer[i + 1] == 0xAB)
                    {
                        int Length = ReceiveBuffer[i + 4];
                        if ((byte)ReceiveBuffer.GetRange(i, Length + 5).Sum(x => x) == ReceiveBuffer[i + Length + 5])
                        {
                            data = ReceiveBuffer.GetRange(i + 5, Length);
                            head = ReceiveBuffer[i + 3];
                            ReceiveBuffer.RemoveRange(i, Length + 6);
                            return;
                        }
                    }
                }

                data = null;
                head = 0;
                return;
            }
            catch
            {
                data = null;
                head = 0;
                return;
            };
        }


        private static int maxScreenWidth = 500;

        private static int maxScreenHeight = 500;

        private static int current_x = maxScreenWidth / 2;

        private static int current_y = maxScreenHeight / 2;

        private int innerMovingSpeed = 20;

        /// <summary>
        /// 在xy方向移动指定距离(鼠标每次最多只能移动127个像素),正负值表示移动方向
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void move(int x, int y)
        {
            if (x == 0 && y == 0) return;

            int tempx = 0, tempy = 0;

            double ratio = Math.Abs(x) / (double)Math.Abs(y);
            int facx = Math.Abs(x) / innerMovingSpeed;
            int signx = x >= 0 ? 1 : -1;
            int facy = Math.Abs(y) / innerMovingSpeed;
            int signy = y >= 0 ? 1 : -1;

            int ox = 0, oy = 0;

            //x方向移动的位置更多
            if (ratio >= 1)
            {
                int dety = (int)(innerMovingSpeed / ratio);
                int detx = innerMovingSpeed;
                for (int i = 0; i < facx; i++)
                {

                    JudgeRangeXY(signx * detx, signy * dety, out ox, out oy);
                    SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);
                    Thread.Sleep(5);
                    xloc += ox;
                    yloc += oy;
                    ParamsChangedEvent?.Invoke(this, "XLocation");
                    ParamsChangedEvent?.Invoke(this, "YLocation");

                    tempy += dety;
                    tempx += detx;
                }
                int resx = Math.Abs(x) - tempx;
                int resy = Math.Abs(y) - tempy;

                JudgeRangeXY(signx * resx, signy * resy, out ox, out oy);
                SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);
                Thread.Sleep(5);

                xloc += ox;
                yloc += oy;
                ParamsChangedEvent?.Invoke(this, "XLocation");
                ParamsChangedEvent?.Invoke(this, "YLocation");

            }
            else
            {
                int detx = (int)(innerMovingSpeed * ratio);
                int dety = innerMovingSpeed;
                for (int i = 0; i < facy; i++)
                {
                    JudgeRangeXY(signx * detx, signy * dety, out ox, out oy);
                    SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);
                    Thread.Sleep(5);

                    xloc += ox;
                    yloc += oy;
                    ParamsChangedEvent?.Invoke(this, "XLocation");
                    ParamsChangedEvent?.Invoke(this, "YLocation");

                    tempy += dety;
                    tempx += detx;
                }
                int resx = Math.Abs(x) - tempx;
                int resy = Math.Abs(y) - tempy;

                JudgeRangeXY(signx * resx, signy * resy, out ox, out oy);
                SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);
                Thread.Sleep(5);
                xloc += ox;
                yloc += oy;
                ParamsChangedEvent?.Invoke(this, "XLocation");
                ParamsChangedEvent?.Invoke(this, "YLocation");
            }
            return;
        }

        /// <summary>
        /// 在xy方向移动指定距离(鼠标每次最多只能移动127个像素),正负值表示移动方向
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        private void direcmove(int x, int y)
        {
            if (x == 0 && y == 0) return;

            int tempx = 0, tempy = 0;

            double ratio = Math.Abs(x) / (double)Math.Abs(y);
            int facx = Math.Abs(x) / 127;
            int signx = x >= 0 ? 1 : -1;
            int facy = Math.Abs(y) / 127;
            int signy = y >= 0 ? 1 : -1;

            int ox = 0, oy = 0;

            //x方向移动的位置更多
            if (ratio >= 1)
            {
                int dety = (int)(127 / ratio);
                int detx = 127;
                for (int i = 0; i < facx; i++)
                {

                    JudgeRangeXY(signx * detx, signy * dety, out ox, out oy);
                    SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);

                    tempy += dety;
                    tempx += detx;
                }
                int resx = Math.Abs(x) - tempx;
                int resy = Math.Abs(y) - tempy;

                JudgeRangeXY(signx * resx, signy * resy, out ox, out oy);
                SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);

            }
            else
            {
                int detx = (int)(127 * ratio);
                int dety = 127;
                for (int i = 0; i < facy; i++)
                {
                    JudgeRangeXY(signx * detx, signy * dety, out ox, out oy);
                    SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);

                    tempy += dety;
                    tempx += detx;
                }
                int resx = Math.Abs(x) - tempx;
                int resy = Math.Abs(y) - tempy;

                JudgeRangeXY(signx * resx, signy * resy, out ox, out oy);
                SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, ConvertStepX(ox), ConvertStepYZ(oy), 0 }, 50);
            }
            return;
        }

        List<bool> movingBuffer = new List<bool>();

        public void Move(int x, int y)
        {
            lock (movingBuffer)
            {
                if (Is_Moving && movingBuffer.Count == 0) return;
                if (movingBuffer.Count == 0)
                {
                    Is_Moving = true;
                }

                movingBuffer.Add(true);

                if (Math.Abs(x) > (maxScreenWidth - 1) / 2 && Math.Abs(y) > (maxScreenHeight - 1) / 2)
                {
                    int mx = x < 0 ? -(maxScreenWidth - 1) / 2 : (maxScreenWidth - 1) / 2;
                    int my = y < 0 ? -(maxScreenHeight - 1) / 2 : (maxScreenHeight - 1) / 2;
                    Move(mx, my);
                    x -= mx;
                    y -= my;
                    Move(x, y);
                    movingBuffer.RemoveAt(movingBuffer.Count - 1);
                    if (movingBuffer.Count == 0) Is_Moving = false;
                    return;
                }

                if (Math.Abs(x) > (maxScreenWidth - 1) / 2)
                {
                    int mx = x < 0 ? -(maxScreenWidth - 1) / 2 : (maxScreenWidth - 1) / 2;
                    Move(mx, 0);
                    x -= mx;
                    Move(x, y);
                    movingBuffer.RemoveAt(movingBuffer.Count - 1);
                    if (movingBuffer.Count == 0) Is_Moving = false;
                    return;
                }
                if (Math.Abs(y) > (maxScreenHeight - 1) / 2)
                {
                    int my = y < 0 ? -(maxScreenHeight - 1) / 2 : (maxScreenHeight - 1) / 2;
                    Move(0, my);
                    y -= my;
                    Move(x, y);
                    movingBuffer.RemoveAt(movingBuffer.Count - 1);
                    if (movingBuffer.Count == 0) Is_Moving = false;
                    return;
                }

                bool isOutRange = JudgeScreenRangeXY(x, y, out int detx, out int dety);

                move(detx, dety);
                current_x += detx;
                current_y += dety;

                if (isOutRange)
                {
                    ResetMouse();
                    int resx = x - detx;
                    int resy = y - dety;
                    current_x = maxScreenWidth / 2 + resx;
                    current_y = maxScreenHeight / 2 + resy;
                    move(resx, resy);

                    Is_Moving = false;
                    movingBuffer.RemoveAt(movingBuffer.Count - 1);
                    return;
                }

                move(x - detx, y - dety);
                current_x += x - detx;
                current_y += y - dety;

                movingBuffer.RemoveAt(movingBuffer.Count - 1);
                if (movingBuffer.Count == 0) Is_Moving = false;
            }
        }

        /// <summary>
        ///800*560
        /// </summary>
        private void ResetMouse()
        {
            IsReseting = true;
            SendFrame(0x05, new List<byte>() { 0x01, 0b00000000, 0, 0, 0 }, 50);
            Thread.Sleep(30);
            SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, 0, 0, 0 }, 50);
            Thread.Sleep(30);
            IsReseting = false;
        }

        /// <summary>
        /// 在Z方向移动
        /// </summary>
        /// <param name="z"></param>
        public void Move(int z)
        {
            SendFrame(0x05, new List<byte>() { 0x01, 0b00000000, 0, 0, ConvertStepYZ(z) }, 50);
            zloc += z;
            ParamsChangedEvent?.Invoke(this, "ZLocation");
        }

        private byte ConvertStepX(long step)
        {
            if (step == 0) return 0;
            if (step < 0)
            {
                return (byte)Math.Abs(step);
            }
            else
            {
                return (byte)(256 - Math.Abs(step));
            }
        }

        private byte ConvertStepYZ(long step)
        {
            if (step == 0) return 0;
            if (step > 0)
            {
                return (byte)step;
            }
            else
            {
                return (byte)(256 - Math.Abs(step));
            }
        }

        /// <summary>
        /// 判断是否超范围,如果超过范围那么X，Y均返回0，两个方向是独立的
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private void JudgeRangeXY(int x, int y, out int ox, out int oy)
        {
            ox = x;
            oy = y;
            if (xloc + x < LowerX)
            {
                ox = -xloc;
            }
            if (xloc + x > UpperX)
            {
                ox = UpperX - xloc;
            }

            if (yloc + y < LowerY)
            {
                oy = -yloc;
            }
            if (yloc + y > UpperY)
            {
                oy = UpperY - yloc;
            }
        }

        /// <summary>
        /// 判断是否超屏幕范围,如果超过范围那么X，Y均返回0，两个方向是独立的
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private bool JudgeScreenRangeXY(int x, int y, out int ox, out int oy)
        {
            bool outrange = false;
            int x1 = x + current_x, y1 = y + current_y;

            ox = x;
            oy = y;

            if (x1 < 0 || x1 > maxScreenWidth)
            {
                outrange = true;
                ox = x1 < 0 ? -current_x : maxScreenWidth - current_x;
            }
            if (y1 < 0 || y1 > maxScreenHeight)
            {
                outrange = true;
                oy = y1 < 0 ? -current_y : maxScreenHeight - current_y;
            }

            return outrange;
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
    /// 鼠标部分
    /// </summary>
    public partial class MicroSupport
    {

        /// <summary>
        /// 鼠标移动灵敏度，值在0到1之间
        /// </summary>
        public double MouseMovingMagnification { get; set; } = 0.5;

        private DecoratedButton mousebtn = null;

        /// <summary>
        /// 鼠标监听窗口
        /// </summary>
        private Window ListeningWindow = null;

        /// <summary>
        /// 鼠标跟踪点
        /// </summary>
        private Point MousePoint = new Point(double.NaN, double.NaN);

        /// <summary>
        /// 按钮范围
        /// </summary>
        private Rect box;

        /// <summary>
        /// 鼠标按下事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseDownEvent(object sender, MouseButtonEventArgs e)
        {
            MousePoint = e.GetPosition(ListeningWindow);

            SendFrame(0x05, new List<byte>() { 0x01, 0b00000001, 0, 0, 0 }, 50);
        }

        /// <summary>
        /// 鼠标抬起事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseUpEvent(object sender, MouseButtonEventArgs e)
        {
            MousePoint = new Point(double.NaN, double.NaN);
            //重置
            SendFrame(0x05, new List<byte>() { 0x01, 0b00000000, 0, 0, 0 }, 50);
            current_x = maxScreenWidth / 2;
            current_y = maxScreenHeight / 2;

        }

        /// <summary>
        /// 滚轮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (mousebtn.KeepPressed)
            {
                int value = e.Delta;
                MoveDirection direz = value > 0 ? MoveDirection.Forward : MoveDirection.Backward;
                Move(value);
                mousebtn.CaptureMouse();

                zloc += value;
                ParamsChangedEvent?.Invoke(this, "ZLocation");
            }
        }

        /// <summary>
        /// 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MouseClick(object sender, RoutedEventArgs e)
        {
            Point initpoint = mousebtn.TranslatePoint(new Point(0, 0), ListeningWindow);
            box = new Rect(initpoint.X, initpoint.Y, mousebtn.Width, mousebtn.Height);

            Point loc = Mouse.GetPosition(ListeningWindow);

            if (!box.Contains(loc)) return;

            mousebtn.KeepPressed = !mousebtn.KeepPressed;
            if (mousebtn.KeepPressed)
            {
                mousebtn.CaptureMouse();
            }
            else
            {
                mousebtn.ReleaseMouseCapture();
            }
        }

        /// <summary>
        /// 鼠标移动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveEvent(object sender, MouseEventArgs e)
        {
            if (double.IsNaN(MousePoint.X)) return;
            //处在鼠标模式
            if (mousebtn.KeepPressed && e.LeftButton == MouseButtonState.Pressed)
            {
                if (double.IsNaN(MousePoint.X) == false)
                {
                    //计算位移量
                    Point np = e.GetPosition(ListeningWindow);
                    Vector del = MouseMovingMagnification * (np - MousePoint);
                    JudgeScreenRangeXY((int)-del.X, (int)del.Y, out int ox, out int oy);
                    JudgeRangeXY(ox, oy, out ox, out oy);

                    direcmove(ox, oy);

                    current_x += ox;
                    current_y += oy;

                    xloc += ox;
                    yloc += oy;

                    ParamsChangedEvent?.Invoke(this, "XLocation");
                    ParamsChangedEvent?.Invoke(this, "YLocation");
                }
            }
        }



        /// <summary>
        ///  注册鼠标按钮
        /// </summary>
        /// <param name="btn">按钮</param>
        /// <param name="listenwindow">监听窗口</param>
        public void RegisterMouseButton(DecoratedButton btn, Window listenwindow)
        {
            if (ListeningWindow != null || mousebtn != null) throw new Exception("不能重复注册鼠标按钮");
            ListeningWindow = listenwindow;
            mousebtn = btn;
            mousebtn.MouseLeftButtonDown += MouseDownEvent;
            mousebtn.MouseLeftButtonUp += MouseUpEvent;
            mousebtn.Click += MouseClick;
            mousebtn.MouseWheel += MouseWheel;
            mousebtn.MouseMove += MoveEvent;
        }

        /// <summary>
        /// 解注册鼠标按钮
        /// </summary>
        public void UnregisterMouseButton()
        {
            if (mousebtn == null)
            {
                ListeningWindow = null;
                return;
            }
            mousebtn.MouseLeftButtonDown -= MouseDownEvent;
            mousebtn.MouseLeftButtonUp -= MouseUpEvent;
            mousebtn.Click -= MouseClick;
            mousebtn.MouseWheel -= MouseWheel;
            mousebtn.MouseMove -= MoveEvent;

            ListeningWindow = null;
            mousebtn = null;
            return;
        }
    }
}
