using HardWares.APIS;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpinCore.SpinAPI;

namespace HardWares.板卡.Spincore_PulseBlaster
{
    public partial class PulseBlaster : PulseBlasterBase, WinUSBOuterInterface, USBInternalInterface
    {
        public void CloseUSBPort()
        {
            (Instance as SpinAPI).StopProgramming();
            (Instance as SpinAPI).Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConnectedUSBAction()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="exc"></param>
        /// <param name="reconnect"></param>
        /// <returns></returns>
        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, false, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            List<USBDeviceInfo> infos = new List<USBDeviceInfo>();
            SpinAPI news = new SpinAPI();
            int count = news.BoardCount;
            for (int i = 0; i < count; i++)
            {
                bool exist = false;
                foreach (var item in DeviceInfos)
                {
                    if (item.Value.GetType().Name == GetType().Name)
                    {
                        if ((int)item.Value.Instance == i)
                        {
                            exist = true;
                            break;
                        }
                    }
                }
                if (!exist)
                    infos.Add(new USBDeviceInfo("Spincore PulseBlaster" + news.Version + "-" + i.ToString(), i.ToString()));
            }
            return infos;
        }

        public bool IsUSBOpen()
        {
            return true;
        }

        public object OpenUSBPort(USBDeviceInfo info)
        {
            return new SpinAPI();
        }

        public void ReceiveUSBAct()
        {
            return;
        }

        public bool TestUSBAction()
        {
            int result = (Instance as SpinAPI).Status;
            if (result < 0) return false;
            return true;
        }

        public void USBInitAction(object PortInstance)
        {
            try
            {
                ClosePort();
            }
            catch { }
           (Instance as SpinAPI).Init();
            (Instance as SpinAPI).SetClock(500);
            ChannelInds = Enumerable.Range(0, 21);
        }

        public byte[] USBPortRead()
        {
            return new byte[0];
        }

        public void USBPortWrite(byte[] value)
        {
            return;
        }
    }
}
