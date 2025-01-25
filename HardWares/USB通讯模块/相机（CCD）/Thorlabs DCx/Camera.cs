using HardWares.端口基类;
using HardWares.端口基类部分;
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
        internal CameraInformation SelectedCamera { get; set; }

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
        public bool ConnectUSB(string usbName, out Exception exc)
        {
            uc480.Types.CameraInformation[] cameraList;
            uc480.Info.Camera.GetCameraList(out cameraList);
            foreach (var item in cameraList)
            {
                if (usbName.Split('_')[0] == item.DeviceID.ToString())
                {
                    SelectedCamera = item;
                    return Connect(PortType.USB, out exc, Encoding.Default);
                }
            }
            exc = new Exception("未找到设备");
            return false;
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<string> GetUsbDeviceNames()
        {
            List<string> res = new List<string>();
            uc480.Types.CameraInformation[] cameraList;
            uc480.Info.Camera.GetCameraList(out cameraList);
            foreach (var item in cameraList)
            {
                if (item.InUse) continue;
                res.Add(item.CameraID.ToString() + "_" + item.SerialNumber);
            }
            return res;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            throw new NotImplementedException();
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

        object USBInternalInterface.CreateUSBInstance(List<object> param)
        {
            return new uc480.Camera();
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as uc480.Camera).IsOpened;
        }

        void USBInternalInterface.OpenUSBPort()
        {
            ConnectStatus = (Instance as uc480.Camera).Init((Int32)SelectedCamera.DeviceID | (Int32)uc480.Defines.DeviceEnumeration.UseDeviceID);
            if (ConnectStatus != uc480.Defines.Status.SUCCESS) return;
            ConnectStatus = (Instance as uc480.Camera).Memory.Allocate(out Int32 m, true);
            if (ConnectStatus != uc480.Defines.Status.SUCCESS) return;
            MemoryIntptr = m;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            return;
        }

        bool USBInternalInterface.TestUSBAction()
        {
            if (ConnectStatus != uc480.Defines.Status.SUCCESS) return false;
            ProductName = "Thorlabs DCx " + SelectedCamera.Model.ToString() + " " + SelectedCamera.SerialNumber;
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
