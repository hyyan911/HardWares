using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge;
using AForge.Video;
using AForge.Imaging;
using AForge.Controls;
using AForge.Video.DirectShow;
using System.Threading;
using HardWares.端口基类部分.设备信息;

namespace HardWares.相机_CCD_.摄像头
{
    /// <summary>
    /// ThorlabsDCx相机
    /// </summary>
    public partial class Camera : WinUSBOuterInterface, USBInternalInterface
    {
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
            List<USBDeviceInfo> names = new List<USBDeviceInfo>();
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < videoDevices.Count; i++)
            {
                names.Add(new USBDeviceInfo(videoDevices[i].Name, videoDevices[i].MonikerString));
            }
            return names;
        }

        void USBInternalInterface.CloseUSBPort()
        {
            (Instance as VideoCaptureDevice).Stop();
        }

        void USBInternalInterface.ConnectedUSBAction()
        {
            (Instance as VideoCaptureDevice).NewFrame += VideoSource_NewFrame;
        }


        bool capture = false;

        Bitmap FrameBuffer = null;

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (capture) // 检查是否需要保存图片
            {
                FrameBuffer = (Bitmap)eventArgs.Frame.Clone();
                capture = false; // 重置标志
            }
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as VideoCaptureDevice).IsRunning;
        }

        object USBInternalInterface.OpenUSBPort(USBDeviceInfo param)
        {
            VideoCaptureDevice Camera = new VideoCaptureDevice(param.USBIdentification);
            Camera.Start();
            Thread.Sleep(50);
            if (!Camera.IsRunning)
            {
                throw new Exception("设备不存在");
            }
            return Camera;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            return;
        }

        bool USBInternalInterface.TestUSBAction()
        {
            ProductName = "摄像头 " + (Instance as VideoCaptureDevice).Source;
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

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            throw new NotImplementedException();
        }
    }
}
