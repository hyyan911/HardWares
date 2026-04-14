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
using HardWares.端口基类.Custom串口;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;
using HardWares.纳米位移台;
using LibUsbDotNet;
using LibUsbDotNet.LibUsb;
using LibUsbDotNet.Main;
using Thorlabs.MotionControl.DeviceManagerCLI;

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
            if (AutoDispose)
            {
                Dispose();
            }
        }

        /// <summary>
        /// 是否在销毁对象时调用Dispose方法
        /// </summary>
        internal virtual bool AutoDispose { get; set; } = true;

        /// <summary>
        /// 获取所有可用参数
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

        internal abstract Encoding GetCoder();

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
        /// 产品种类
        /// </summary>
        public abstract string ProductType { get; internal set; }

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
            if (PortType == PortType.CUSTOM)
            {
                (this as CustomInternalInterface).CustomInitAction(PortInstance);
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
            if (PortType == PortType.CUSTOM)
            {
                return (this as CustomInternalInterface).TestCustomAction();
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
            if (PortType == PortType.CUSTOM)
            {
                (this as CustomInternalInterface).CustomPortWrite(value);
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
            if (PortType == PortType.CUSTOM)
            {
                return (this as CustomInternalInterface).CustomPortRead();
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
            if (PortType == PortType.CUSTOM)
            {
                (this as CustomInternalInterface).CloseCustomPort();
            }
        }

        /// <summary>
        /// 打开COM接口
        /// </summary>
        /// <returns></returns>
        internal object OpenPort(DeviceInfoBase info)
        {
            if (PortType == PortType.USB)
            {
                return (this as USBInternalInterface).OpenUSBPort(info as USBDeviceInfo);
            }
            if (PortType == PortType.COM)
            {
                return (this as COMInternalInterface).OpenCOMPort(info as COMDeviceInfo);
            }
            if (PortType == PortType.TCPIP)
            {
                return (this as TCPIPInternalInterface).OpenTCPIPPort(info as TCPIPDeviceInfo);
            }
            if (PortType == PortType.CUSTOM)
            {
                return (this as CustomInternalInterface).OpenCustomPort(info as CustomDeviceInfo);
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
            if (PortType == PortType.CUSTOM)
            {
                return (this as CustomInternalInterface).IsCustomOpen();
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
            if (PortType == PortType.CUSTOM)
            {
                (this as CustomInternalInterface).ConnectedCustomAction();
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
            if (PortType == PortType.CUSTOM)
            {
                (this as CustomInternalInterface).ReceiveCustomAct();
            }
        }

        #endregion


        #region 设备获取和判断连接状态
        /// <summary>
        /// 检测设备是否连接
        /// </summary>
        /// <returns></returns>
        public static bool IsDeviceConnected(PortObject dev)
        {
            if (dev == null) return false;
            if (DeviceInfos.Where((x) => x.Value == dev).Count() == 0) return false;
            return true;
        }

        /// <summary>
        /// 寻找已连接设备,找不到返回null
        /// </summary>
        /// <returns></returns>
        public static PortObject FindDevice(string productIdentifier, string productName)
        {
            var result = DeviceInfos.Where((x) => x.Value.ProductName == productName && x.Value.ProductIdentifier == productIdentifier);
            if (result.Count() == 0) return null;
            else
            {
                return result.ElementAt(0).Value;
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
        internal static List<KeyValuePair<DeviceInfoBase, PortObject>> DeviceInfos { get; set; } = new List<KeyValuePair<DeviceInfoBase, PortObject>>();

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        internal virtual bool Connect(DeviceInfoBase info, out Exception exc, bool reconnect = false, bool PortArrangerAvailable = true, bool createtestthread = false)
        {
            PortType = info.PortType;
            try
            {
                if (reconnect)
                {
                    //检查已有设备中是否存在此设备,如果有则关闭后重新连接
                    foreach (var item in DeviceInfos)
                    {
                        if (item.Key.CompareParam(info))
                        {
                            item.Value.Dispose();
                        }
                    }
                }
                else
                {
                    foreach (var item in DeviceInfos)
                    {
                        if (item.Key.CompareParam(info))
                        {
                            exc = new Exception("设备已存在,无法连接诶");
                            return false;
                        }
                    }
                }

                Instance = OpenPort(info);

                //根据不同设备初始化参数
                InitAction(Instance);

                if (PortArrangerAvailable)
                    PortArranger = new PortDispatcher(this, GetCoder());

                //发送测试信息
                Internal_Is_Connected = TestAction();
                if (Internal_Is_Connected)
                {
                    Internal_Is_Connected = false;
                    ConnectedAction();
                    if (createtestthread)
                        CreateTestThread();
                    ProductName = info.DeviceName;
                    //添加设备到已连接列表
                    DeviceInfos.Add(new KeyValuePair<DeviceInfoBase, PortObject>(info.Copy(), this));
                    exc = null;
                    return true;
                }

                PortArranger?.Close();
                try
                {
                    ClosePort();
                }
                catch (Exception)
                {
                }
                exc = new Exception("端口打开失败");
                return false;
            }
            catch (Exception ee)
            {
                exc = new Exception("接口出现异常：" + ee.Message);
                PortArranger?.Close();
                try
                {
                    ClosePort();
                }
                catch (Exception)
                {
                }
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
                if (DeviceInfos[i].Value == this)
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
            FileObject file = new FileObject();

            List<string> paramnames = new List<string>();
            List<string> paramvalues = new List<string>();

            //如果有子设备
            if (this is ElementPortObject)
            {
                var channels = (this as ElementPortObject).Channels;
                foreach (var st in channels)
                {
                    GetParamsData(out List<string> elenames, out List<string> elevs, st.AvailableParameterNames());
                    file.WriteStringData(st.ChannelName + "@" + "ParamNames", elenames);
                    file.WriteStringData(st.ChannelName + "@" + "ParamValues", elevs);
                }
            }

            GetParamsData(out List<string> devpnames, out List<string> devpvalues, AvailableParameterNames());

            file.WriteStringData("ParamNames", devpnames);
            file.WriteStringData("ParamValues", devpvalues);
            return file;
        }

        private void GetParamsData(out List<string> Pnames, out List<string> Pvalues, List<Parameter> Ps)
        {
            List<string> paramnames = new List<string>();
            List<string> paramvalues = new List<string>();
            foreach (var item in Ps)
            {
                if (item.IsReadOnly) continue;
                var value = item.ReadValue();
                if (value is Enum)
                {
                    paramnames.Add(item.ParameterName);
                    paramvalues.Add(Enum.GetName(item.ParamType, value));
                    continue;
                }
                if (value is KeyValuePair<string, List<string>>)
                {
                    paramnames.Add(item.ParameterName);
                    paramvalues.Add(((KeyValuePair<string, List<string>>)value).Key);
                    continue;
                }
                else
                {
                    paramnames.Add(item.ParameterName);
                    paramvalues.Add(value.ToString());
                }
            }
            Pnames = paramnames;
            Pvalues = paramvalues;
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
                List<string> paramnames = file.ExtractString("ParamNames");
                List<string> paramvalues = file.ExtractString("ParamValues");
                if (param.Where((x) => x.IsReadOnly == false).Count() != paramnames.Count) return;
                SetParamsData(paramnames, paramvalues, param);
                if (this is ElementPortObject)
                {
                    var stages = (this as ElementPortObject).Channels;
                    foreach (var st in stages)
                    {
                        var pnames = file.ExtractString(st.ChannelName + "@" + "ParamNames");
                        var pvalues = file.ExtractString(st.ChannelName + "@" + "ParamValues");
                        SetParamsData(pnames, pvalues, st.AvailableParameterNames());
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void SetParamsData(List<string> Pnames, List<string> Pvalues, List<Parameter> Ps)
        {
            foreach (var item in Pnames)
            {
                foreach (var item1 in Ps)
                {
                    try
                    {
                        if (item1.IsReadOnly) continue;
                        if (item == item1.ParameterName)
                        {
                            if (typeof(Enum).IsAssignableFrom(item1.ParamType))
                            {
                                item1.WriteValue(Enum.Parse(item1.ParamType, Pvalues[Pnames.IndexOf(item)]));
                                continue;
                            }
                            if (item1.ParamType.IsAssignableFrom(typeof(KeyValuePair<string, List<string>>)))
                            {
                                item1.WriteValue(Pvalues[Pnames.IndexOf(item)]);
                                continue;
                            }
                            else
                            {
                                item1.WriteValue(Convert.ChangeType(Pvalues[Pnames.IndexOf(item)], item1.ParamType));
                                continue;
                            }
                        }
                    }
                    catch (Exception e) { }
                }
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
            string result = "";
            try
            {
                result = ThreadUnsafeQuery(messagetosend, timeout);
            }
            catch (Exception ex) { }
            finally
            {
                isQuereEnd = true;
            }
            return result;
        }
        internal string ThreadSafeQuery(List<byte> messagetosend, int timeout)
        {
            while (!isQuereEnd)
            {
                Thread.Sleep(10);
            }
            isQuereEnd = false;
            string result = "";
            try
            {
                result = ThreadUnsafeQuery(messagetosend, timeout);
            }
            catch (Exception ex) { }
            finally
            {
                isQuereEnd = true;
            }
            return result;
        }

        internal abstract string ThreadUnsafeQuery(string messagetosend, int timeout);

        internal virtual string ThreadUnsafeQuery(List<byte> messagetosend, int timeout)
        {
            return ThreadUnsafeQuery(GetCoder().GetString(messagetosend.ToArray()), timeout);
        }

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
