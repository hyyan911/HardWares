using HardWares.端口基类;
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
                if (bmap == null)
                {
                    return null;
                }
                bmap.RotateFlip(FlipType);
                BitmapSource source = CodeHelper.ImageConverter.BitmapToBitmapSource(bmap);
                source.Freeze();
                bmap.Dispose();
                return source;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public abstract Bitmap InnerGrabFrame(uint waittime);

        /// <summary>
        /// 
        /// </summary>
        public abstract double ExposureTime { get; set; }

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
