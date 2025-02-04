using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类.TCPIP串口
{
    internal interface TCPIPInternalInterface
    {
        /// <summary>
        /// 发送TCPIP测试信号内容
        /// </summary>
        bool TestTCPIPAction();

        /// <summary>
        /// 写TCPIP接口方法
        /// </summary>
        /// <param name="value"></param>
        void TCPIPPortWrite(byte[] value);

        /// <summary>
        /// 读TCPIP接口方法
        /// </summary>
        /// <returns></returns>
        byte[] TCPIPPortRead();

        /// <summary>
        /// 关闭TCPIP接口
        /// </summary>
        /// <returns></returns>
        void CloseTCPIPPort();

        /// <summary>
        ///  创建TCPIP接口实例
        /// </summary>
        /// <returns></returns>
        object OpenTCPIPPort(List<object> param);

        /// <summary>
        /// TCPIP接口是否打开
        /// </summary>
        /// <returns></returns>
        bool IsTCPIPOpen();

        /// <summary>
        /// 连接TCPIP成功后的处理步骤
        /// </summary>
        void ConnectedTCPIPAction();

        /// <summary>
        /// 收到TCPIP信息进行的处理
        /// </summary>
        /// <param name="port"></param>
        void ReceiveTCPIPAct();

        /// <summary>
        /// 初始化串口
        /// </summary>
        void TCPIPInitAction(object PortInstance);
    }
}
