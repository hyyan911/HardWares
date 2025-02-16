using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.端口基类.Custom串口
{
    internal interface CustomInternalInterface
    {
        /// <summary>
        /// 发送Custom测试信号内容
        /// </summary>
        bool TestCustomAction();

        /// <summary>
        /// 写Custom接口方法
        /// </summary>
        /// <param name="value"></param>
        void CustomPortWrite(byte[] value);

        /// <summary>
        /// 读Custom接口方法
        /// </summary>
        /// <returns></returns>
        byte[] CustomPortRead();

        /// <summary>
        /// 关闭Custom接口
        /// </summary>
        /// <returns></returns>
        void CloseCustomPort();

        /// <summary>
        ///  创建Custom接口实例
        /// </summary>
        /// <returns></returns>
        object OpenCustomPort(CustomDeviceInfo info);

        /// <summary>
        /// Custom接口是否打开
        /// </summary>
        /// <returns></returns>
        bool IsCustomOpen();

        /// <summary>
        /// 连接Custom成功后的处理步骤
        /// </summary>
        void ConnectedCustomAction();

        /// <summary>
        /// 收到Custom信息进行的处理
        /// </summary>
        /// <param name="port"></param>
        void ReceiveCustomAct();

        /// <summary>
        /// 初始化串口
        /// </summary>
        void CustomInitAction(object PortInstance);
    }
}
