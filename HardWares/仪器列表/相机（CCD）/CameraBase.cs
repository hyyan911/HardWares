using HardWares.端口基类;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using uc480.Defines;

namespace HardWares.相机_CCD_
{
    public abstract class CameraBase : PortObject
    {
        protected object cameralockobj = new object();

        WriteableBitmap source = null;

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
                    bmap = ProcessImage(bmap, lightness, (float)contrast, (float)saturation);
                    bmap.RotateFlip(FlipType);
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
        private double contrast = 1;
        /// <summary>
        /// 对比度
        /// </summary>
        public double Contrast
        {
            get { return contrast; }
            set
            {
                double v = value;
                if (v > 2)
                {
                    v = 2;
                }
                if (v < 0)
                {
                    v = 0;
                }
                contrast = v;
            }
        }

        private int lightness = 0;
        /// <summary>
        /// 对比度
        /// </summary>
        public double Lightness
        {
            get { return lightness; }
            set
            {
                double v = value;
                if (v > 255)
                {
                    v = 255;
                }
                if (v < -255)
                {
                    v = -255;
                }
                lightness = (int)v;
            }
        }

        private double saturation = 1;
        /// <summary>
        /// 对比度
        /// </summary>
        public double Saturation
        {
            get { return saturation; }
            set
            {
                double v = value;
                if (v > 2)
                {
                    v = 2;
                }
                if (v < 0)
                {
                    v = 0;
                }
                saturation = v;
            }
        }

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

        /// <summary>
        /// /亮度：参数范围：-255（完全变暗）到 255（完全变亮），默认0，一般需要调大
        //对比度：参数范围：0.0f（完全低对比度）到 2.0f（完全高对比度），默认1.0，一般需要调大
        //饱和度：参数范围：0.0f（灰度图）到 2.0f（完全饱和），默认1.0，一般需要调小
        /// </summary>
        /// <param name="input"></param>
        /// <param name="brightness"></param>
        /// <param name="contrast"></param>
        /// <param name="saturation"></param>
        /// <returns></returns>
        private Bitmap ProcessImage(Bitmap input, int brightness, float contrast, float saturation)
        {
            Mat mat = BitmapConverter.ToMat(input);//bitmap转为mat
            input.Dispose();                            //亮度，对比度
            Mat adjustedImage = new Mat();
            mat.ConvertTo(adjustedImage, -1, contrast, brightness); //alpha=contrast, beta=brightness，输出adjustedImage

            //饱和度
            // 转换到HSV颜色空间
            Mat hsvImage = new Mat();
            Cv2.CvtColor(adjustedImage, hsvImage, ColorConversionCodes.BGR2HSV);//adjustedImage → hsvImage
            Mat[] hsvChannels = hsvImage.Split();// 分离 HSV 通道
            hsvChannels[1] *= saturation;// 调整饱和度通道
            Cv2.Merge(hsvChannels, hsvImage);// 合并 HSV 通道→ hsvImage
                                             // 转换回 BGR 颜色空间
            Mat adjustedSat = new Mat();
            Cv2.CvtColor(hsvImage, adjustedSat, ColorConversionCodes.HSV2BGR);//hsvImage → adjustedSat
            var map = BitmapConverter.ToBitmap(adjustedSat);
            adjustedImage.Dispose();
            return map;
        }
    }
}
