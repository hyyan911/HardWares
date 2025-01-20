using HardWares.数据处理;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NationalInstruments.Visa;
using Ivi.Visa;

namespace HardWares.源表.KEITHLEY_2450
{
    public partial class PowerSource : PowerSourceBase, USBInternalInterface, WinUSBOuterInterface
    {
        public static List<UsbSession> USBNameBuffer = new List<UsbSession>();
        public static List<string> USBNames = new List<string>();

        private string name = "";

        void USBInternalInterface.CloseUSBPort()
        {
            if (Instance == null) return;
            (Instance as UsbSession).Clear();
            (Instance as UsbSession).UnlockResource();
            USBNameBuffer.Remove(Instance as UsbSession);
            USBNames.Remove(name);
            (Instance as UsbSession).Dispose();
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            try
            {
                (Instance as UsbSession).Clear();
            }
            catch (Exception ex) { }
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return new byte[0];
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            (Instance as UsbSession).RawIO.Write(value);
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
        }

        object USBInternalInterface.CreateUSBInstance(List<object> param)
        {
            using (ResourceManager m = new ResourceManager())
            {
                UsbSession res = (UsbSession)m.Open(param[0] as string, AccessModes.None, 1000, out ResourceOpenStatus stat);
                foreach (var item in USBNames)
                {
                    if (item == param[0] as string)
                    {
                        try
                        {
                            USBNameBuffer[USBNames.IndexOf(item)].UnlockResource();
                        }
                        catch (Exception ex) { }
                        USBNameBuffer.RemoveAt(USBNames.IndexOf(item));
                        USBNames.Remove(item);
                        break;
                    }
                }
                try
                {
                    res.LockResource(2000);
                    USBNameBuffer.Add(res);
                    USBNames.Add(param[0] as string);
                }
                catch (Exception ex) { }
                res.TerminationCharacterEnabled = false;
                name = param[0] as string;
                return res;
            }
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return true;
        }

        void USBInternalInterface.OpenUSBPort()
        {
            return;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            if (ReceiveBuffer.Count != 0)
            {
                List<List<byte>> retu = DataProcess.ProcessReceivedSerialData('\n', ReceiveBuffer, out List<byte> result);
                ReceiveBuffer = result;
                if (retu.Count != 0)
                {
                    QueryState = true;
                    //只取第一条指令
                    QueryReturnedData = new List<string>() { PortArranger.Coder.GetString(retu[0].ToArray()) };
                }
            }
        }

        bool USBInternalInterface.TestUSBAction()
        {
            string res = ThreadSafeQuery("*IDN?\n", 1000);
            if (res.Contains("KEITHLEY") && res.Contains("MODEL 2450"))
            {
                //获取序列号
                string strsegs = res.Split(',')[2];
                ProductName = "KEITHLEY 2450 " + strsegs;
                return true;
            }
            return false;
        }

        public bool ConnectUSB(string description, out Exception exc)
        {
            foreach (var item in DevNames)
            {
                if (item.Key == description || item.Value == description)
                {
                    return Connect(PortType.USB, out exc, Encoding.ASCII, false, item.Value);
                }
            }
            exc = new Exception("未找到设备");
            return false;
        }

        private static Dictionary<string, string> DevNames = new Dictionary<string, string>();

        public List<string> GetUsbDeviceNames()
        {
            using (ResourceManager m = new ResourceManager())
            {
                DevNames.Clear();
                try
                {
                    List<string> strs = m.Find("USB?*").ToList();
                    List<string> result = new List<string>();
                    foreach (var item in strs)
                    {
                        using (var ss = m.Open(item))
                        {
                            try
                            {
                                string res = ThreadSafeQuery("*IDN?\n", 200);
                                if (res.Contains("KEITHLEY") && res.Contains("MODEL 2450"))
                                {
                                    //获取序列号
                                    string strsegs = res.Split(',')[2];
                                    string pro = "KEITHLEY 2450 " + strsegs;
                                    result.Add(pro);
                                    DevNames.Add(pro, item);
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return new List<string>();
                }
            }
        }
    }
}
