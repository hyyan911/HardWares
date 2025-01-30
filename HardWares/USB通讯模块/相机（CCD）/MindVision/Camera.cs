using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVSDK;
using CameraHandle = System.Int32;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;
using System.Threading;
using System.Windows;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows.Interop;
using HardWares.端口基类;
using HardWares.相机_CCD_;

namespace HardWares.相机.MindVision
{
    /// <summary>
    /// MindVision工业相机
    /// </summary>
    public partial class Camera : CameraBase, WinUSBOuterInterface, USBInternalInterface
    {

        /// <summary>
        /// 
        /// </summary>
        public override string ProductIdentifier { get; internal set; } = "MindVision相机";

        /// <summary>
        /// 连接USB
        /// </summary>
        /// <param name="usbName"></param>
        /// <param name="exc"></param>
        /// <returns></returns>
        public bool ConnectUSB(string usbName, out Exception exc)
        {
            return Connect(PortType.USB, out exc, Encoding.Default, false, usbName);
        }

        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetUsbDeviceNames()
        {
            return new List<string>();
            MvApi.CameraEnumerateDevice(out tSdkCameraDevInfo[] infos);
            List<string> result = new List<string>();
            if (infos == null) return new List<string>();
            foreach (var item in infos)
            {
                result.Add((Encoding.UTF8.GetString(item.acProductName)).Replace("\0", ""));
            }
            return result;
        }

        internal override string ThreadUnsafeQuery(string messagetosend, int timeout)
        {
            throw new NotImplementedException();
        }


        #region 内部方法
        void USBInternalInterface.CloseUSBPort()
        {
            if ((CameraHandle)Instance != 0)
            {
                MvApi.CameraUnInit((CameraHandle)Instance);
                Instance = 0;
            }
        }

        /// <summary>
        /// 初始化相机
        /// </summary>
        void USBInternalInterface.ConnectedUSBAction()
        {
            tSdkCameraCapbility tCameraCapability;
            //获取相机特征
            MvApi.CameraGetCapability((CameraHandle)Instance, out tCameraCapability);
            //设置抓拍通道的分辨率。
            tSdkImageResolution tResolution;
            tResolution.uSkipMode = 0;
            tResolution.uBinAverageMode = 0;
            tResolution.uBinSumMode = 0;
            tResolution.uResampleMask = 0;
            tResolution.iVOffsetFOV = 0;
            tResolution.iHOffsetFOV = 0;
            tResolution.iWidth = 0;
            tResolution.iHeight = 0;
            //tResolution.iIndex = 0xff;表示自定义分辨率,如果tResolution.iWidth和tResolution.iHeight
            //定义为0，则表示跟随预览通道的分辨率进行抓拍。抓拍通道的分辨率可以动态更改。
            //本例中将抓拍分辨率固定为最大分辨率。
            tResolution.iIndex = 0xff;
            tResolution.acDescription = new byte[32];//描述信息可以不设置
            tResolution.iWidthZoomHd = 0;
            tResolution.iHeightZoomHd = 0;
            tResolution.iWidthZoomSw = 0;
            tResolution.iHeightZoomSw = 0;

            MvApi.CameraPlay((CameraHandle)Instance);
        }

        bool USBInternalInterface.IsUSBOpen()
        {
            MvApi.CameraIsOpened(ref CameraInfo, out bool result);
            return result;
        }

        object USBInternalInterface.OpenUSBPort(List<object> param)
        {
            MvApi.CameraEnumerateDevice(out tSdkCameraDevInfo[] infos);
            for (int i = 0; i < infos.Length; ++i)
            {
                if ((Encoding.UTF8.GetString(infos[i].acProductName)).Replace("\0", "") == param[0].ToString())
                {
                    CameraHandle handle = 0;
                    CameraSdkStatus status = MvApi.CameraInit(ref infos[i], -1, -1, ref handle);
                    if (status == CameraSdkStatus.CAMERA_STATUS_SUCCESS)
                    {
                        return handle;
                    }
                    return null;
                }
            }
            return null;
        }

        void USBInternalInterface.ReceiveUSBAct()
        {
            return;
        }

        bool USBInternalInterface.TestUSBAction()
        {
            return true;
        }

        void USBInternalInterface.USBInitAction(object PortInstance)
        {
        }

        byte[] USBInternalInterface.USBPortRead()
        {
            return new byte[0];
        }

        void USBInternalInterface.USBPortWrite(byte[] value)
        {
            return;
        }
        #endregion
    }
}
