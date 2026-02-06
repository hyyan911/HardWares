using HardWares.端口基类;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using uc480.Defines;
using OpenCvSharp.Extensions;
using OpenCvSharp;

namespace HardWares.相机_CCD_
{
    public abstract class CameraBase : PortObject
    {
        protected object cameralockobj = new object();

        WriteableBitmap source = null;

        private Bitmap SetParameter(Bitmap bmp)
        {
            var mat = BitmapConverter.ToMat(bmp);

            // 转换到 HSV 色彩空间
            Cv2.CvtColor(mat, mat, ColorConversionCodes.BGR2HSV);

            // 分离 HSV 通道
            Mat[] hsvChannels = new Mat[3];
            Cv2.Split(mat, out hsvChannels);

            // 调整饱和度（S 通道）
            Mat saturationChannel = hsvChannels[1];
            Cv2.Multiply(saturationChannel, Saturation, saturationChannel);
            Cv2.Threshold(saturationChannel, saturationChannel, 255, 255, ThresholdTypes.Trunc);

            // 调整亮度（V 通道）
            Mat valueChannel = hsvChannels[2];
            Cv2.Add(valueChannel, Lightness, valueChannel);
            Cv2.Threshold(valueChannel, valueChannel, 255, 255, ThresholdTypes.Trunc);

            // 合并 HSV 通道
            Cv2.Merge(hsvChannels, mat);

            // 转换回 BGR 色彩空间
            Cv2.CvtColor(mat, mat, ColorConversionCodes.HSV2BGR);

            // 调整对比度
            mat.ConvertTo(mat, mat.Type(), Contrast, 0);

            return BitmapConverter.ToBitmap(mat);

        }

        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="waittime"></param>
        /// <returns></returns>
        public BitmapSource GrabFrame(uint waittime)
        {
            try
            {
                Bitmap bmap = InnerGrabFrame(waittime);
                if (bmap == null) source = null;
                else
                {
                    bmap.RotateFlip(FlipType);
                    bmap = SetParameter(bmap);
                    source = CodeHelper.ImageConverter.UpdateWritableBitmap(bmap, source);
                }
                if (bmap == null)
                {
                    ++BrokenFrameCount;
                    return null;
                }
                return source;
            }
            catch (Exception ex)
            {
                source = null;
                ++BrokenFrameCount;
                return null;
            }
        }

        public abstract Bitmap InnerGrabFrame(uint waittime);

        /// <summary>
        /// 
        /// </summary>
        public abstract double ExposureTime { get; set; }

        /// <summary>
        /// 未加载帧数
        /// </summary>
        public int BrokenFrameCount { get; set; } = 0;

        /// <summary>
        /// 
        /// </summary>
        public abstract double Framerate { get; set; }

        /// <summary>
        /// 水平像素宽度
        /// </summary>
        public abstract int CameraPixelWidthCount { get; }

        /// <summary>
        /// 垂直像素宽度
        /// </summary>
        public abstract int CameraPixelHeightCount { get; }

        #region 图像处理参数
        /// <summary>
        /// 对比度
        /// </summary>
        public double Contrast { get; set; } = 1;

        /// <summary>
        /// 亮度
        /// </summary>
        public double Lightness { get; set; } = 0;

        /// <summary>
        /// 饱和度
        /// </summary>
        public double Saturation { get; set; } = 0;
        #endregion


        private RotateFlipType fliptype = RotateFlipType.RotateNoneFlipNone;
        /// <summary>
        /// 翻转方式
        /// </summary>
        public RotateFlipType FlipType
        {
            get { return fliptype; }
            set { fliptype = value; }
        }
    }
}
