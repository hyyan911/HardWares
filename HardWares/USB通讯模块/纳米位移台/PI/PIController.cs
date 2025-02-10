using HardWares.数据处理;
using HardWares.纳米位移台;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using HardWares.APIS.PI;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.IO.Ports;
using System.Windows;
using HardWares.端口基类;
using HardWares.端口基类.COM串口;

namespace HardWares.纳米位移台.PI
{
    /// <summary>
    /// PI位移台
    /// </summary>
    public partial class PIController : NanoControllerBase, USBInternalInterface, WinUSBOuterInterface
    {

        /// <summary>
        /// 枚举所有此类型USB设备
        /// </summary>
        public static List<string> EnumerateUSBDevices()
        {

            return PIAPI2.EnumerateUSB();
        }

        void USBInternalInterface.CloseUSBPort()
        {
            if (Instance == null) return;
            if ((int)Instance == -1) return;
            if (IsGCS2)
            {
                PIAPI2.PI_CloseConnection((int)Instance);
            }
            else
            {
                return;
            }
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            //获取函数列表
            CommandSet = GetSupportedCommands();
            //判断是否是GCS2版本
            IsGCS2 = !IsGCS3();
            //获取轴列表
            var retun = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("SAI?", "", ""), 1000));
            Stages.Clear();
            foreach (var item in retun)
            {
                //添加位移台
                Stages.Add(new PIStage(item.Trim(), this));
            }
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return PIAPI2.PI_IsConnected((int)Instance);
        }

        object USBInternalInterface.OpenUSBPort(List<object> param)
        {
            if (IsGCS2)
            {
                int id = PIAPI2.PI_ConnectUSB((param[0] as string).Split(' ').Last());
                ProductName = param[0] as string;
                return id;
            }
            return -1;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            List<List<byte>> retu = DataProcess.ProcessReceivedSerialData(LF, ReceiveBuffer, out List<byte> result);
            ReceiveBuffer = result;
            if (retu.Count != 0)
            {
                QueryState = true;
                QueryReturnedData = new List<string>();
                foreach (var item in retu)
                {
                    QueryReturnedData.Add(Encoding.ASCII.GetString(item.ToArray()));
                }
            }
        }

        bool USBInternalInterface.TestUSBAction()
        {
            List<string> result = ProcessQueryResult(ThreadSafeQuery(ProcessCmd("*IDN?", "", ""), 300));
            if (result.Count == 0) return false;
            if (result[0].Contains("Physik Instrumente"))
            {
                return true;
            }
            return false;
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            return;
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            if (IsGCS2)
            {
                return PIAPI2.GetAnswer((int)Instance, out bool state).ToArray();
            }
            else
            {
                return new byte[0];
            }
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            if (IsGCS2)
            {
                PIAPI2.PI_GcsCommandset((int)Instance, Encoding.ASCII.GetString(value));
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 连接USB，只有第一个参数有效，值为USB口的标识字符串
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public bool ConnectUSB(string usbname, out Exception exc)
        {
            return Connect(PortType.USB, out exc, Encoding.ASCII, usbname);
        }

        /// <summary>
        /// 获取USB设备列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<string> GetUsbDeviceNames()
        {
            return PIAPI2.EnumerateUSB();
        }
    }
}
