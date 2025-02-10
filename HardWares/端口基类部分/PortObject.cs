using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CodeHelper;
using HardWares.APIS;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;

namespace HardWares.端口基类
{

    public abstract partial class PortObject
    {
        static PortObject()
        {
            ExternalDlls.LoadDlls();
        }

        /// <summary>
        /// 当被回收时释放资源
        /// </summary>
        ~PortObject()
        {
            Dispose();
        }

        /// <summary>
        /// 参数是否可用（对使用同类型的不同型号设备而言，参数列表可能有所区别。此时需要一个方法来告诉外部调用程序连接的设备哪些参数可用。需要在继承的设备类中重写）
        /// </summary>
        /// <returns></returns>
        public abstract List<Parameter> AvailableParameterNames();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="PropertyName"></param>
        public delegate void ParamsChangeEventHandler(object sender, string PropertyName);

        /// <summary>
        /// 动态参数改变事件
        /// </summary>
        public abstract event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 接口类型
        /// </summary>
        public PortType PortType { get; internal set; } = PortType.COM;

        public PortObject()
        {
        }

        /// <summary>
        /// 参数监听线程
        /// </summary>
        internal Thread ParamsListener { get; set; } = null;

        #region 接口实例
        /// <summary>
        /// COM口实例
        /// </summary>
        internal object Instance { get; set; } = null;

        /// <summary>
        /// 自定义接口实例
        /// </summary>
        internal object CustomInstance { get; set; } = null;
        #endregion


        /// <summary>
        /// 接收缓存区
        /// </summary>
        internal List<byte> ReceiveBuffer { get; set; } = new List<byte>();

        /// <summary>
        /// 断开连接事件
        /// </summary>
        public event EventHandler ConnectStateChangeEvent = null;

        internal Thread TestThread = null;

        internal void ActiveConnectChangeEvent()
        {
            ConnectStateChangeEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 更新参数
        /// </summary>
        public abstract void ValidateParams();


        internal int id = 0;
        /// <summary>
        /// 设备号
        /// </summary>
        public int ID
        {
            get
            {
                return Thread.VolatileRead(ref id);
            }
        }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; internal set; }

        /// <summary>
        /// 产品型号
        /// </summary>
        public abstract string ProductIdentifier { get; internal set; }

        /// <summary>
        /// 收发线程
        /// </summary>
        internal PortDispatcher PortArranger { get; set; } = null;

        private int is_Connected = 0;
        /// <summary>
        /// 是否连接
        /// </summary>
        internal bool Internal_Is_Connected
        {
            get
            {
                return Thread.VolatileRead(ref is_Connected) == 0 ? false : true;
            }
            set
            {
                if (value) Thread.VolatileWrite(ref is_Connected, 1);
                else Thread.VolatileWrite(ref is_Connected, 0);
            }
        }

        /// <summary>
        /// 是否定期检查连接状态
        /// </summary>
        public bool CheckConnection { get; set; } = false;

        private int isConnected = 0;
        /// <summary>
        /// 是否连接
        /// </summary>
        public bool IsConnected
        {
            get { return Thread.VolatileRead(ref isConnected) == 0 ? false : true; }
            internal set
            {
                Thread.VolatileWrite(ref isConnected, value ? 1 : 0);
            }
        }

        /// <summary>
        /// 信号状态
        /// </summary>
        internal bool QueryState { get; set; } = false;
        /// <summary>
        /// 返回数据
        /// </summary>
        internal List<string> QueryReturnedData { get; set; } = new List<string>();

        #region 接口方法

        /// <summary>
        /// 初始化串口
        /// </summary>
        internal void InitAction(object PortInstance)
        {
            if (PortType == PortType.USB)
            {
                (this as USBInternalInterface).USBInitAction(PortInstance);
            }
            if (PortType == PortType.COM)
            {
                (this as COMInternalInterface).COMInitAction(PortInstance);
            }
            if (PortType == PortType.TCPIP)
            {
                (this as TCPIPInternalInterface).TCPIPInitAction(PortInstance);
            }
        }

        /// <summary>
        /// 发送COM测试信号内容
        /// </summary>
        internal bool TestAction()
        {
            if (PortType == PortType.USB)
            {
                return (this as USBInternalInterface).TestUSBAction();
            }
            if (PortType == PortType.COM)
            {
                return (this as COMInternalInterface).TestCOMAction();
            }
            if (PortType == PortType.TCPIP)
            {
                return (this as TCPIPInternalInterface).TestTCPIPAction();
            }
            return false;
        }

        /// <summary>
        /// 写COM接口方法
        /// </summary>
        /// <param name="value"></param>
        internal void PortWrite(byte[] value)
        {
            if (PortType == PortType.USB)
            {
                (this as USBInternalInterface).USBPortWrite(value);
            }
            if (PortType == PortType.COM)
            {
                (this as COMInternalInterface).COMPortWrite(value);
            }
            if (PortType == PortType.TCPIP)
            {
                (this as TCPIPInternalInterface).TCPIPPortWrite(value);
            }
        }

        /// <summary>
        /// 读COM接口方法
        /// </summary>
        /// <returns></returns>
        internal byte[] PortRead()
        {
            if (PortType == PortType.USB)
            {
                return (this as USBInternalInterface).USBPortRead();
            }
            if (PortType == PortType.COM)
            {
                return (this as COMInternalInterface).COMPortRead();
            }
            if (PortType == PortType.TCPIP)
            {
                return (this as TCPIPInternalInterface).TCPIPPortRead();
            }
            return new byte[0];
        }

        /// <summary>
        /// 关闭COM接口
        /// </summary>
        /// <returns></returns>
        internal void ClosePort()
        {
            if (PortType == PortType.USB)
            {
                (this as USBInternalInterface).CloseUSBPort();
            }
            if (PortType == PortType.COM)
            {
                (this as COMInternalInterface).CloseCOMPort();
            }
            if (PortType == PortType.TCPIP)
            {
                (this as TCPIPInternalInterface).CloseTCPIPPort();
            }
        }

        /// <summary>
        /// 打开COM接口
        /// </summary>
        /// <returns></returns>
        internal object OpenPort(params object[] ps)
        {
            if (PortType == PortType.USB)
            {
                return (this as USBInternalInterface).OpenUSBPort(ps.ToList());
            }
            if (PortType == PortType.COM)
            {
                return (this as COMInternalInterface).OpenCOMPort(ps.ToList());
            }
            if (PortType == PortType.TCPIP)
            {
                return (this as TCPIPInternalInterface).OpenTCPIPPort(ps.ToList());
            }
            return null;
        }

        /// <summary>
        /// COM接口是否打开
        /// </summary>
        /// <returns></returns>
        internal bool IsOpen()
        {
            if (PortType == PortType.USB)
            {
                return (this as USBInternalInterface).IsUSBOpen();
            }
            if (PortType == PortType.COM)
            {
                return (this as COMInternalInterface).IsCOMOpen();
            }
            if (PortType == PortType.TCPIP)
            {
                return (this as TCPIPInternalInterface).IsTCPIPOpen();
            }
            return false;
        }

        /// <summary>
        /// 连接COM成功后的处理步骤
        /// </summary>
        internal void ConnectedAction()
        {
            if (PortType == PortType.USB)
            {
                (this as USBInternalInterface).ConnectedUSBAction();
            }
            if (PortType == PortType.COM)
            {
                (this as COMInternalInterface).ConnectedCOMAction();
            }
            if (PortType == PortType.TCPIP)
            {
                (this as TCPIPInternalInterface).ConnectedTCPIPAction();
            }
        }

        /// <summary>
        /// 收到COM信息进行的处理
        /// </summary>
        /// <param name="port"></param>
        internal void ReceiveAct()
        {
            if (PortType == PortType.USB)
            {
                (this as USBInternalInterface).ReceiveUSBAct();
            }
            if (PortType == PortType.COM)
            {
                (this as COMInternalInterface).ReceiveCOMAct();
            }
            if (PortType == PortType.TCPIP)
            {
                (this as TCPIPInternalInterface).ReceiveTCPIPAct();
            }
        }

        #endregion


        private void CreateTestThread()
        {
            //连接测试线程
            if (TestThread != null && TestThread.IsAlive == true)
            {
                TestThread.Abort();
            }
            TestThread = new Thread(() =>
            {
                while (true)
                {
                    if (CheckConnection == false)
                    {
                        Thread.Sleep(500);
                        continue;
                    }
                    Internal_Is_Connected = false;
                    TestAction();
                    if (Internal_Is_Connected == false)
                    {
                        if (IsConnected == true)
                        {
                            IsConnected = false;
                            ActiveConnectChangeEvent();
                        }
                    }
                    else
                    {
                        if (IsConnected == false)
                        {
                            IsConnected = true;
                            ActiveConnectChangeEvent();
                        }
                    }
                }
            });
            TestThread.Start();
        }


        #region 连接方式

        /// <summary>
        /// 历史设备记录
        /// </summary>
        internal static List<DeviceInfo> DeviceInfos { get; set; } = new List<DeviceInfo>();

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        internal virtual bool Connect(PortType type, out Exception exc, Encoding Coder, params object[] param)
        {
            PortType = type;
            try
            {
                //检查已有设备中是否存在此设备,如果有则关闭后重新连接
                for (int i = 0; i < DeviceInfos.Count; i++)
                {
                    if (DeviceInfos[i].CompareParam(GetType(), type, param))
                    {
                        DeviceInfos[i].Device.Dispose();
                        break;
                    }
                }
                Instance = OpenPort(param);

                //根据不同设备初始化参数
                InitAction(Instance);

                PortArranger = new PortDispatcher(this, Coder);

                //发送测试信息
                Internal_Is_Connected = TestAction();
                if (Internal_Is_Connected)
                {
                    Internal_Is_Connected = false;
                    ConnectedAction();
                    CreateTestThread();
                    //添加设备到已连接列表
                    DeviceInfos.Add(new DeviceInfo() { Device = this, PortParams = param.ToList(), PortType = type });
                    exc = null;
                    return true;
                }

                PortArranger?.Close();

                exc = new Exception("端口打开失败");
                return false;
            }
            catch (Exception ee)
            {
                exc = new Exception("接口出现异常：" + ee.Message);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 释放串口前需要进行的操作
        /// </summary>
        internal virtual void DisposeCOMAction() { }

        /// <summary>
        /// 释放串口前需要进行的操作
        /// </summary>
        internal virtual void DisposeUSBAction() { }

        /// <summary>
        /// 释放串口前需要进行的操作
        /// </summary>
        internal virtual void DisposeCustomPortAction() { }

        /// <summary>
        /// 释放掉串口，释放所正在运行的所有线程和所有资源
        /// </summary>
        public void Dispose()
        {
            //从已连接设备中删除
            for (int i = 0; i < DeviceInfos.Count; i++)
            {
                if (DeviceInfos[i].Device == this)
                {
                    DeviceInfos.RemoveAt(i);
                }
            }
            DisposeCOMAction();
            if (TestThread != null)
            {
                try
                {
                    TestThread.Abort();
                    TestThread = null;
                }
                catch { }
            }
            if (PortArranger != null)
                PortArranger.Close();
            try
            {
                if (Instance != null)
                {
                    ClosePort();
                }
            }
            catch { }
        }

        #region 获取所有同类型设备型号
        /// <summary>
        /// 获取所有同类型设备类型和名称
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, Type> GetDeviceNamesWithSameBase(Type deviceBaseType)
        {
            if (!typeof(PortObject).IsAssignableFrom(deviceBaseType))
            {
                throw new Exception("当前类型不属于设备基类PortObject");
            }
            Dictionary<string, Type> result = new Dictionary<string, Type>();
            List<Type> devices = GetSubClassTypes(deviceBaseType);
            foreach (var item in devices)
            {
                string deviceidentify = item.GetProperty("ProductIdentifier").GetValue(Activator.CreateInstance(item)).ToString();
                result.Add(deviceidentify, item);
            }
            return result;
        }

        /// <summary>
        /// C#获取一个类在其所在的程序集中的所有子类
        /// </summary>
        /// <param name="parentType">给定的类型</param>
        /// <returns>所有子类的名称</returns>
        internal static List<Type> GetSubClassTypes(Type parentType)
        {
            var subTypeList = new List<Type>();
            var assembly = parentType.Assembly;//获取当前父类所在的程序集``
            var assemblyAllTypes = assembly.GetTypes();//获取该程序集中的所有类型
            foreach (var itemType in assemblyAllTypes)//遍历所有类型进行查找
            {
                var baseType = itemType.BaseType;//获取元素类型的基类
                if (baseType != null)//如果有基类
                {
                    if (baseType.Name == parentType.Name)//如果基类就是给定的父类
                    {
                        subTypeList.Add(itemType);//加入子类表中
                    }
                }
            }
            return subTypeList;//获取所有子类类型的名称
        }
        #endregion

        #region 参数读取和保存
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public FileObject GenerateParamsFile()
        {
            List<Parameter> param = AvailableParameterNames();
            CodeHelper.FileObject file = new CodeHelper.FileObject();
            List<string> paramnames = new List<string>();
            List<string> paramvalues = new List<string>();
            foreach (var item in param)
            {
                if (item.IsReadOnly) continue;
                var value = item.ReadValue();
                if (value is Enum)
                {
                    paramnames.Add(item.ParameterName);
                    paramvalues.Add(Enum.GetName(item.ParamType, value));
                }
                else
                {
                    paramnames.Add(item.ParameterName);
                    paramvalues.Add(value.ToString());
                }
            }
            file.WriteStringData("ParamNames", paramnames);
            file.WriteStringData("ParamValues", paramvalues);
            return file;
        }

        /// <summary>
        /// 加载参数
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public void LoadParams(FileObject file)
        {
            try
            {
                List<Parameter> param = AvailableParameterNames();
                if (param.Count != file.Descriptions.Count) return;
                List<string> paramnames = file.ExtractString("ParamNames");
                List<string> paramvalues = file.ExtractString("ParamValues");
                foreach (var item in paramnames)
                {
                    foreach (var item1 in param)
                    {
                        try
                        {
                            if (item1.IsReadOnly) continue;
                            if (item == item1.ParameterName)
                            {
                                if (typeof(Enum).IsAssignableFrom(item1.ParamType))
                                {
                                    item1.WriteValue(Enum.Parse(item1.ParamType, paramvalues[paramnames.IndexOf(item)]));
                                }
                                else
                                {
                                    item1.WriteValue(Convert.ChangeType(paramvalues[paramnames.IndexOf(item)], item1.ParamType));
                                }
                            }
                        }
                        catch (Exception e) { }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        #endregion

        #region Query方法

        bool isQuereEnd = true;
        internal string ThreadSafeQuery(string messagetosend, int timeout)
        {
            while (!isQuereEnd)
            {
                Thread.Sleep(10);
            }
            isQuereEnd = false;
            string result = ThreadUnsafeQuery(messagetosend, timeout);
            isQuereEnd = true;
            return result;
        }

        internal abstract string ThreadUnsafeQuery(string messagetosend, int timeout);

        #endregion

        /// <summary>
        /// 获取所有可用端口
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAvailableCOMPorts()
        {
            List<string> result = new List<string>();
            string[] ss = SerialPort.GetPortNames();
            foreach (var item in ss)
            {
                try
                {
                    SerialPort por = new SerialPort(item);
                    por.Open();
                    por.Close();
                    result.Add(item);
                }
                catch (Exception e)
                {

                }
            }

            return result;
        }

    }

    /// <summary>
    /// 串口发送
    /// </summary>
    public abstract partial class PortObject
    {
        /// <summary>
        /// 添加信息到队列
        /// </summary>
        public void AddMessage(string message)
        {
            PortArranger.AddMessage(message);
        }

        /// <summary>
        /// 添加信息到队列
        /// </summary>
        public void AddMessage(List<byte> message)
        {
            PortArranger.AddMessage(message);
        }

        /// <summary>
        /// 添加最高优先级信息
        /// </summary>
        public void AddProierMessage(string message)
        {
            PortArranger.AddProierMessage(message);
        }

        /// <summary>
        /// 添加最高优先级信息
        /// </summary>
        public void AddProierMessage(List<byte> message)
        {
            PortArranger.AddProierMessage(message);
        }
    }
}
