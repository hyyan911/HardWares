using AForge.Video;
using AForge.Video.DirectShow;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using uc480;
using uc480.Defines;
using uc480.Defines.IO;
using uc480.Info;
using Parameter = HardWares.端口基类部分.Parameter;

namespace HardWares.相机_CCD_.摄像头
{
    /// <summary>
    /// ThorlabsDCx相机
    /// </summary>
    public partial class Camera : CameraBase
    {
        public override string ProductIdentifier { get; internal set; } = "摄像头";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        #region 参数列表

        /// <summary>
        /// 曝光时间
        /// </summary>
        public override double ExposureTime { get; set; } = double.NaN;

        /// <summary>
        /// 帧率
        /// </summary>
        public override double Framerate { get; set; } = double.NaN;

        /// <summary>
        /// 垂直镜像
        /// </summary>
        public bool VerticalMirror { get; set; } = false;

        /// <summary>
        /// 水平镜像
        /// </summary>
        public bool HorizontalMirror { get; set; } = false;

        /// <summary>
        /// 水平像素宽度
        /// </summary>
        public override int CameraPixelWidthCount
        {
            get
            {
                return (Instance as VideoCaptureDevice).VideoCapabilities[0].FrameSize.Width;
            }
        }

        /// <summary>
        /// 垂直像素宽度
        /// </summary>
        public override int CameraPixelHeightCount
        {
            get
            {
                return (Instance as VideoCaptureDevice).VideoCapabilities[0].FrameSize.Height;
            }
        }

        #endregion

        /// <summary>
        /// 获取所有可用参数
        /// </summary>
        /// <returns></returns>
        public override List<Parameter> AvailableParameterNames()
        {
            List<Parameter> result = new List<Parameter>();
            result.Add(new Parameter("FlipType", "图片变换模式", FlipType.GetType(), this, true));
            result.Add(new Parameter("CameraPixelWidthCount", "相机图片宽度", CameraPixelWidthCount.GetType(), this, true) { IsReadOnly = true });
            result.Add(new Parameter("CameraPixelHeightCount", "相机图片高度", CameraPixelWidthCount.GetType(), this, true) { IsReadOnly = true });
            result.Add(new Parameter("Lightness", "亮度", Lightness.GetType(), this, true));
            result.Add(new Parameter("Saturation", "饱和度", Saturation.GetType(), this, true) { IsReadOnly = false });
            result.Add(new Parameter("Contrast", "对比度", Contrast.GetType(), this, true) { IsReadOnly = false });
            return result;
        }

        /// 获取单帧图片
        /// <summary>
        /// <param name="waittime"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Bitmap InnerGrabFrame(uint waittime)
        {
            capture = true;
            int time = 0;
            while (capture == true && time < waittime)
            {
                Thread.Sleep(20);
                time += 20;
            }
            if (time > waittime)
            {
                capture = false;
                BrokenFrameCount += 1;
                return null;
            }
            capture = false;
            return FrameBuffer;
        }

        internal override Encoding GetCoder()
        {
            return Encoding.Default;
        }
        public override void ValidateParams()
        {
        }
    }
}
