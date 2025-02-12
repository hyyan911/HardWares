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
using HardWares.端口基类部分.设备信息;


namespace HardWares.纳米位移台.PI
{
    /// <summary>
    /// PI位移台
    /// </summary>
    public partial class PIController : NanoControllerBase, USBInternalInterface, WinUSBOuterInterface
    {
        private static string GetUSBProductName(string arg)
        {
            return arg;
        }

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
            var retun = ProcessQueryResult(PIAPI2.GetAxisNames((int)Instance));
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

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo info)
        {
            if (IsGCS2)
            {
                int id = PIAPI2.PI_ConnectUSB(info.USBIdentification);
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
                PIAPI2.PI_GcsCommandset((int)Instance, value);
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectUSB(USBDeviceInfo info, out Exception exc)
        {
            return Connect(info, out exc);
        }

        /// <summary>
        /// 获取USB设备列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            var r = PIAPI2.EnumerateUSB();

            r = r.Where((x) =>
            {
                foreach (var item in DevTypes)
                {
                    if (x.Contains(item)) return true;
                }
                return false;
            }).ToList();

            return r.Select(x =>
            {
                return new USBDeviceInfo(x, x.Split(' ').Last());
            }).ToList();
        }
    }
}
