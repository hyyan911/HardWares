using HardWares.源表;
using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.PortHelper;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HardWares.电源.Rigol_DP811
{
    /// <summary>
    /// 电源基类
    /// </summary>
    public partial class Power : PowerBase, USBInternalInterface, WinUSBOuterInterface
    {
        VISAResourceUSBHelper VISAHelper = new VISAResourceUSBHelper(GetName, '\n');

        internal int ChannelCounts = 0;

        List<KeyValuePair<string, int>> ChannelList = new List<KeyValuePair<string, int>>()
        {
            new KeyValuePair<string, int>("DP811",1),
            new KeyValuePair<string, int>("DP813",1),
            new KeyValuePair<string, int>("DP821",2),
            new KeyValuePair<string, int>("DP822",2),
            new KeyValuePair<string, int>("DP831",3),
            new KeyValuePair<string, int>("DP832",3),
        };

        private static string GetName(string idnstring)
        {
            try
            {
                if (idnstring.ToUpper().Contains("RIGOL TECHNOLOGIES"))
                {
                    string[] ss = idnstring.Split(',');
                    if (ss[1].Contains("DP811"))
                    {
                        return "DP811 " + ss[2];
                    }
                    if (ss[1].Contains("DP812"))
                    {
                        return "DP812 " + ss[2];
                    }
                    if (ss[1].Contains("DP821"))
                    {
                        return "DP821 " + ss[2];
                    }
                    if (ss[1].Contains("DP822"))
                    {
                        return "DP822 " + ss[2];
                    }
                    if (ss[1].Contains("DP831"))
                    {
                        return "DP831 " + ss[2];
                    }
                    if (ss[1].Contains("DP832"))
                    {
                        return "DP832 " + ss[2];
                    }
                    if (ss[1].Contains("DP811"))
                    {
                        return "DP811 " + ss[2];
                    }
                }
                return "";
            }
            catch (Exception) { return ""; }
        }

        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return VISAHelper.ConnectUSB(this, info, out exc, reconnect);
        }

        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            return VISAHelper.EnumerateUSBDevices(this);
        }

        void USBInternalInterface.CloseUSBPort()
        {
            VISAHelper.CloseUSBPort(this);
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            //添加射频和低频通道
            Channels.Clear();
            string model = ProductName.Split(' ')[0];
            ChannelCounts = ChannelList.Where(x => x.Key == model).ToList()[0].Value;
            for (int i = 0; i < ChannelCounts; i++)
            {
                Channels.Add(new PowerChannel() { ParentDevice = this, ChannelInd = i + 1, ChannelName = "电源通道" + (i + 1).ToString() });
            }
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return VISAHelper.IsUSBOpen(this);
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo param)
        {
            return VISAHelper.OpenUSBPort(this, param);
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            VISAHelper.ReceiveUSBAct(this);
        }

        bool USBInternalInterface.TestUSBAction()
        {
            return VISAHelper.TestUSBPort(this);
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            VISAHelper.USBInitAction(this, PortInstance);
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return VISAHelper.USBPortRead(this);
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            VISAHelper.USBPortWrite(this, value);
        }
    }
}
