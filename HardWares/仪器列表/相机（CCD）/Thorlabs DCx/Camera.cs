using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using uc480;
using uc480.Defines;
using uc480.Defines.IO;
using uc480.Info;
using Parameter = HardWares.端口基类部分.Parameter;

namespace HardWares.相机_CCD_.Thorlabs_DCx
{
    /// <summary>
    /// ThorlabsDCx相机
    /// </summary>
    public partial class Camera : CameraBase
    {

        public override string ProductIdentifier { get; internal set; } = "Thorlabs DCx";

        public override event ParamsChangeEventHandler ParamsChangedEvent;

        #region 参数列表

        /// <summary>
        /// 曝光时间
        /// </summary>
        public override double ExposureTime
        {
            get
            {
                Status status = (Instance as uc480.Camera).Timing.Exposure.Get(out double value);
                if (status != Status.SUCCESS)
                    return double.NaN;
                return value;
            }
            set
            {
                (Instance as uc480.Camera).Timing.Exposure.Set(value);
            }
        }

        /// <summary>
        /// 帧率
        /// </summary>
        public override double Framerate
        {
            get
            {
                Status status = (Instance as uc480.Camera).Timing.Framerate.Get(out double value);
                if (status != Status.SUCCESS)
                    return double.NaN;
                return value;
            }
            set
            {
                (Instance as uc480.Camera).Timing.Framerate.Set(value);
            }
        }

        /// <summary>
        /// 水平像素宽度
        /// </summary>
        public override int CameraPixelWidthCount
        {
            get
            {
                (Instance as uc480.Camera).Size.AOI.Get(out int px, out int py, out int width, out int height);
                return width;
            }
        }

        /// <summary>
        /// 垂直像素宽度
        /// </summary>
        public override int CameraPixelHeightCount
        {
            get
            {
                (Instance as uc480.Camera).Size.AOI.Get(out int px, out int py, out int width, out int height);
                return height;
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
            result.Add(new Parameter("Framerate", "帧率(fps)", Framerate.GetType(), this, true));
            result.Add(new Parameter("ExposureTime", "曝光时间(ms)", ExposureTime.GetType(), this, true));
            result.Add(new Parameter("FlipType", "图片变换模式", FlipType.GetType(), this, true));
            result.Add(new Parameter("CameraPixelWidthCount", "水平像素数", CameraPixelWidthCount.GetType(), this, true) { IsReadOnly = true });
            result.Add(new Parameter("CameraPixelHeightCount", "垂直像素数", CameraPixelHeightCount.GetType(), this, true) { IsReadOnly = true });
            return result;
        }

        private bool capture;

        private object obj = new object();

        public Bitmap FrameBuffer = null;

        private void NewFrame(object sender, EventArgs e)
        {
            if (capture) // 检查是否需要保存图片
            {
                (Instance as uc480.Camera).Memory.GetActive(out Int32 mid);
                (Instance as uc480.Camera).Memory.Lock(mid);
                lock (obj)
                {
                    (Instance as uc480.Camera).Memory.CopyToBitmap(mid, out FrameBuffer);
                }
                (Instance as uc480.Camera).Memory.Unlock(mid);
                capture = false; // 重置标志
            }
        }

        internal override Encoding GetCoder()
        {
            return Encoding.Default;
        }

        /// <summary>
        /// 获取单帧图片
        /// </summary>
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
                return null;
            return FrameBuffer;
        }

        public override void ValidateParams()
        {
        }

    }
}
