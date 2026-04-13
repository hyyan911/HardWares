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
using HardWares.电源;
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

    /// <summary>
    /// 包含通道的设备类
    /// </summary>
    public abstract partial class ElementPortObject : PortObject
    {
        static ElementPortObject()
        {
            ExternalDlls.LoadDlls();
        }

        /// <summary>
        /// 当被回收时释放资源
        /// </summary>
        ~ElementPortObject()
        {
            if (AutoDispose)
            {
                Dispose();
            }
        }

        public List<PortElement> Channels { get; set; } = new List<PortElement>();

    }
}
