using HardWares.端口基类;
using HardWares.端口基类部分;
using HardWares.端口基类部分.设备信息;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uc480;
using uc480.Types;

namespace HardWares.相机_CCD_.Thorlabs_DCx
{
    /// <summary>
    /// ThorlabsDCx相机
    /// </summary>
    public partial class Camera : WinUSBOuterInterface, USBInternalInterface
    {

        internal uc480.Defines.Status ConnectStatus { get; set; } = uc480.Defines.Status.NO_SUCCESS;

        /// <summary>
        /// 内存指针
        /// </summary>
        internal Int32 MemoryIntptr { get; set; }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="usbName"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ConnectUSB(USBDeviceInfo info, out Exception exc, bool reconnect = false)
        {
            return Connect(info, out exc, reconnect);
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<USBDeviceInfo> GetUsbDeviceInfos()
        {
            List<USBDeviceInfo> res = new List<USBDeviceInfo>();
            uc480.Types.CameraInformation[] cameraList;
            uc480.Info.Camera.GetCameraList(out cameraList);
            foreach (var item in cameraList)
            {
                if (item.InUse) continue;
                res.Add(new USBDeviceInfo("Thorlabs DCx " + item.Model.ToString() + " " + item.SerialNumber, item.SerialNumber));
            }
            return res;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            return "";
        }

        void USBInternalInterface.CloseUSBPort()
        {
            (Instance as uc480.Camera).Acquisition.Stop();

            (Instance as uc480.Camera).Exit();
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            ConnectStatus = (Instance as uc480.Camera).Acquisition.Capture();
            (Instance as uc480.Camera).EventFrame += NewFrame;
            if (ConnectStatus != uc480.Defines.Status.SUCCESS) throw new Exception("相机未正常开启");
            (Instance as uc480.Camera).Display.Mode.Set(uc480.Defines.DisplayMode.DiB);
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as uc480.Camera).IsOpened;
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo param)
        {
            uc480.Types.CameraInformation[] cameraList;
            uc480.Info.Camera.GetCameraList(out cameraList);
            foreach (var item in cameraList)
            {
                if (param.USBIdentification == item.SerialNumber.ToString())
                {
                    uc480.Camera C = new uc480.Camera();
                    ConnectStatus = C.Init((Int32)item.DeviceID | (Int32)uc480.Defines.DeviceEnumeration.UseDeviceID);
                    if (ConnectStatus != uc480.Defines.Status.SUCCESS) return null;
                    ConnectStatus = C.Memory.Allocate(out Int32 m, true);
                    if (ConnectStatus != uc480.Defines.Status.SUCCESS) return null;
                    MemoryIntptr = m;
                    return C;
                }
            }
            throw new Exception("未找到设备");
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            return;
        }

        bool USBInternalInterface.TestUSBAction()
        {
            if (ConnectStatus != uc480.Defines.Status.SUCCESS) return false;
            return true;
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
            return;
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return new byte[0];
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            return;
        }
    }
}
