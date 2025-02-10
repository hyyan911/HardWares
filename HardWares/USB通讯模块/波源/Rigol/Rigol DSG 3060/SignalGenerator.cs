using HardWares.端口基类;
using HardWares.端口基类.TCPIP串口;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardWares.波源.Rigol_DSG_3060
{
    public partial class RFSignalGenerator : USBInternalInterface, WinUSBOuterInterface
    {
        public bool ConnectUSB(string usbName, out Exception exc)
        {
            return VISA.ConnectUSB(this, usbName, out exc);
        }

        public List<string> GetUsbDeviceNames()
        {
            return VISA.EnumerateUSBDevices(this);
        }

        void USBInternalInterface.CloseUSBPort()
        {
            VISA.CloseUSBPort(this);
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return VISA.IsUSBOpen(this);
        }

        object USBInternalInterface.OpenUSBPort(List<object> param)
        {
            return VISA.OpenUSBPort(this, param);
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            VISA.ReceiveUSBAct(this);
        }

        bool USBInternalInterface.TestUSBAction()
        {
            return VISA.TestUSBPort(this);
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            VISA.USBInitAction(this, PortInstance);
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return VISA.USBPortRead(this);
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            VISA.USBPortWrite(this, value);
        }
    }
}
