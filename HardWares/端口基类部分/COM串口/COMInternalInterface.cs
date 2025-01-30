using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类.COM串口
{
    internal interface COMInternalInterface
    {
        /// <summary>
        /// 发送COM测试信号内容
        /// </summary>
        bool TestCOMAction();

        /// <summary>
        /// 写COM接口方法
        /// </summary>
        /// <param name="value"></param>
        void COMPortWrite(byte[] value);

        /// <summary>
        /// 读COM接口方法
        /// </summary>
        /// <returns></returns>
        byte[] COMPortRead();

        /// <summary>
        /// 关闭COM接口
        /// </summary>
        /// <returns></returns>
        void CloseCOMPort();

        /// <summary>
        ///  创建COM接口实例
        /// </summary>
        /// <returns></returns>
        object OpenCOMPort(List<object> param);

        /// <summary>
        /// COM接口是否打开
        /// </summary>
        /// <returns></returns>
        bool IsCOMOpen();

        /// <summary>
        /// 连接COM成功后的处理步骤
        /// </summary>
        void ConnectedCOMAction();

        /// <summary>
        /// 收到COM信息进行的处理
        /// </summary>
        /// <param name="port"></param>
        void ReceiveCOMAct();

        /// <summary>
        /// 初始化串口
        /// </summary>
        void COMInitAction(object PortInstance);
    }
}
