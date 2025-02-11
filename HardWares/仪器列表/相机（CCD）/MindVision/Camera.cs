using HardWares.端口基类部分;
using MVSDK;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;
using CameraHandle = System.Int32;

namespace HardWares.相机.MindVision
{
    /// <summary>
    /// MindVision工业相机
    /// </summary>
    public partial class Camera
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll", SetLastError = true)]
        private static extern bool DeleteObject(IntPtr hObject);

        internal tSdkCameraDevInfo CameraInfo = new tSdkCameraDevInfo();

        /// <summary>
        /// 图像保存地址
        /// </summary>
        protected IntPtr m_ImageBuffer;

        /// <summary>
        /// 
        /// </summary>
        public override int CameraPixelWidthCount { get; }


        #region 参数列表
        /// <summary>
        /// 曝光时间
        /// </summary>
        public override double ExposureTime { get; set; } = double.NaN;

        /// <summary>
        /// 帧率
        /// </summary>
        public override double Framerate { get; set; } = double.NaN;
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override int CameraPixelHeightCount { get; }

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        /// <summary>
        /// 获取帧图像
        /// </summary>
        /// <param name="waittime"></param>
        /// <returns></returns>
        public override Bitmap InnerGrabFrame(uint waittime)
        {
            int width = 0;
            int height = 0;
            IntPtr outimage = IntPtr.Zero;
            //500毫秒超时,图像没捕获到前，线程会被挂起,释放CPU，所以该线程中无需调用sleep
            outimage = MvApi.CameraGetImageBufferEx((CameraHandle)Instance, ref width, ref height, 500);

            if (outimage.ToInt32() != 0 && outimage != null)//如果是触发模式，则有可能超时
            {
                Bitmap bit = new Bitmap(width, height, 3 * width, System.Drawing.Imaging.PixelFormat.Format24bppRgb, outimage);
                if (bit == null)
                {
                    bit.Dispose();
                    bit = null;
                    return InnerGrabFrame(500);
                }
                return bit;
            }
            return null;
        }

        internal override Encoding GetCoder()
        {
            return Encoding.Default;
        }

        public override List<Parameter> AvailableParameterNames()
        {
            return new List<Parameter>();
        }

        public override void ValidateParams()
        {
        }
    }
}
