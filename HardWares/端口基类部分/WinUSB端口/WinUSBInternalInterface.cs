using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace HardWares.端口基类
{
    /// <summary>
    /// WinUSB类
    /// </summary>
    internal interface USBInternalInterface
    {
        /// <summary>
        /// 收到USB信息进行的处理
        /// </summary>
        /// <param name="port"></param>
        void ReceiveUSBAct();

        /// <summary>
        /// 连接USB成功后的处理步骤
        /// </summary>
        void ConnectedUSBAction();

        /// <summary>
        /// 发送USB测试信号内容
        /// </summary>
        bool TestUSBAction();

        byte[] USBPortRead();

        /// <summary>
        /// USB接口是否打开
        /// </summary>
        /// <returns></returns>
        bool IsUSBOpen();

        /// <summary>
        /// 创建USB接口实例
        /// </summary>
        /// <returns></returns>
        object CreateUSBInstance(List<object> param);

        /// <summary>
        /// 打开USB接口
        /// </summary>
        /// <returns></returns>
        void OpenUSBPort();

        /// <summary>
        /// 关闭USB接口
        /// </summary>
        /// <returns></returns>
        void CloseUSBPort();

        /// <summary>
        /// 写USB接口方法
        /// </summary>
        /// <param name="value"></param>
        void USBPortWrite(byte[] value);

        /// <summary>
        /// 初始化串口
        /// </summary>
        void USBInitAction(object PortInstance);
    }
}
