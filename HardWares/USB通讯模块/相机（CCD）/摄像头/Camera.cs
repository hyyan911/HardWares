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
        public bool ConnectUSB(string usbName, out Exception exc)
        {
            return Connect(PortType.USB, out exc, Encoding.Default, false, usbName);
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public List<string> GetUsbDeviceNames()
        {
            List<string> names = new List<string>();
            FilterInfoCollection videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            for (int i = 0; i < videoDevices.Count; i++)
            {
                names.Add(videoDevices[i].MonikerString);
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

        object USBInternalInterface.CreateUSBInstance(List<object> param)
        {
            return new VideoCaptureDevice(param[0] as string);
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            return (Instance as VideoCaptureDevice).IsRunning;
        }

        void USBInternalInterface.OpenUSBPort()
        {
            (Instance as VideoCaptureDevice).Start();
            Thread.Sleep(50);
            if (!(Instance as VideoCaptureDevice).IsRunning)
            {
                throw new Exception("设备不存在");
            }
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
