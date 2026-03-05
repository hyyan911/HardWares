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
using Size = OpenCvSharp.Size;

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

            if (EnhancedContrastAlgorithm)
            {
                mat = ApplySelectiveContrastEnhancement(mat, ContrastTreshold);
            }

            return BitmapConverter.ToBitmap(mat);

        }

        #region 对比度增强算法
        /// <summary>
        /// 选择性局部对比度增强（修复版）
        /// </summary>
        public Mat ApplySelectiveContrastEnhancement(Mat image, double contrastThreshold = 30)
        {
            Mat gray = EnsureGray(image);

            try
            {
                // 计算局部对比度
                Mat localContrast = CalculateLocalContrast(gray, blockSize: 8);

                // 创建掩码：仅选择低对比度区域
                Mat mask = new Mat();
                Cv2.Threshold(localContrast, mask, contrastThreshold, 255, ThresholdTypes.BinaryInv);

                // 确保掩码类型正确
                mask = EnsureMaskCompatibility(mask, gray);

                // 创建掩码的逆
                Mat maskInv = new Mat();
                Cv2.BitwiseNot(mask, maskInv);

                // 对低对比度区域应用 CLAHE
                Mat enhanced = new Mat();
                using (CLAHE clahe = Cv2.CreateCLAHE(EnhancedContrastIntensity,
                                                   tileGridSize: new Size(4, 4)))
                {
                    clahe.Apply(gray, enhanced);
                }

                // 提取低对比度区域（增强后）
                Mat enhancedLow = new Mat();
                Cv2.BitwiseAnd(enhanced, enhanced, enhancedLow, mask);

                // 提取高对比度区域（保持原样）
                Mat originalHigh = new Mat();
                Cv2.BitwiseAnd(gray, gray, originalHigh, maskInv);

                // 合并结果
                Mat result = new Mat();
                Cv2.Add(enhancedLow, originalHigh, result);

                Cv2.EqualizeHist(result, result);
                Cv2.FastNlMeansDenoising(result, result, h: NoiseFilterIntensity, templateWindowSize: 7, searchWindowSize: 21);

                // 清理
                localContrast.Dispose();
                mask.Dispose();
                maskInv.Dispose();
                enhanced.Dispose();
                enhancedLow.Dispose();
                originalHigh.Dispose();

                return result;
            }
            catch (Exception ex)
            {
                return gray.Clone(); // 返回原图
            }
            finally
            {
                if (gray != image) gray.Dispose();
            }
        }

        #region 辅助方法

        /// <summary>
        /// 确保掩码与源图像兼容
        /// </summary>
        private static Mat EnsureMaskCompatibility(Mat mask, Mat sourceImage)
        {
            Mat compatibleMask = mask.Clone();

            try
            {
                // 1. 检查并转换类型
                if (compatibleMask.Type() != MatType.CV_8UC1)
                {
                    if (compatibleMask.Channels() != 1)
                    {
                        // 多通道转单通道
                        Mat temp = new Mat();
                        Cv2.CvtColor(compatibleMask, temp, ColorConversionCodes.BGR2GRAY);
                        temp.ConvertTo(compatibleMask, MatType.CV_8UC1);
                        temp.Dispose();
                    }
                    else
                    {
                        // 单通道类型转换
                        Mat temp = new Mat();
                        compatibleMask.ConvertTo(temp, MatType.CV_8UC1);
                        compatibleMask.Dispose();
                        compatibleMask = temp;
                    }
                }

                // 2. 检查并调整尺寸
                if (compatibleMask.Size() != sourceImage.Size())
                {
                    Mat resized = new Mat();
                    Cv2.Resize(compatibleMask, resized,
                              new Size(sourceImage.Width, sourceImage.Height),
                              interpolation: InterpolationFlags.Nearest);
                    compatibleMask.Dispose();
                    compatibleMask = resized;
                }

                // 3. 确保值是二值化（0或255）
                double minVal, maxVal;
                Cv2.MinMaxLoc(compatibleMask, out minVal, out maxVal);

                if (maxVal > 1.0) // 已经是0-255范围
                {
                    // 已经是二值化
                    return compatibleMask;
                }
                else
                {
                    // 需要二值化
                    Mat binaryMask = new Mat();
                    Cv2.Threshold(compatibleMask, binaryMask, 0.5, 255, ThresholdTypes.Binary);
                    compatibleMask.Dispose();
                    return binaryMask;
                }
            }
            catch
            {
                // 如果出错，创建默认掩码
                compatibleMask.Dispose();
                return Mat.Ones(sourceImage.Size(), MatType.CV_8UC1) * 255;
            }
        }

        private static Mat EnsureGray(Mat image)
        {
            if (image.Channels() == 1)
                return image.Clone();

            Mat gray = new Mat();
            Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);
            return gray;
        }

        /// <summary>
        /// 计算局部对比度
        /// </summary>
        private Mat CalculateLocalContrast(Mat image, int blockSize = 8)
        {
            Mat gray = EnsureGray(image);
            Mat localContrast = new Mat(gray.Size(), MatType.CV_32F);

            // 将图像分割为块
            for (int y = 0; y < gray.Rows; y += blockSize)
            {
                for (int x = 0; x < gray.Cols; x += blockSize)
                {
                    int width = Math.Min(blockSize, gray.Cols - x);
                    int height = Math.Min(blockSize, gray.Rows - y);

                    Mat block = new Mat(gray, new Rect(x, y, width, height));

                    // 计算块的对比度（标准差）
                    Scalar mean, stdDev;
                    Cv2.MeanStdDev(block, out mean, out stdDev);

                    // 填充结果
                    Mat contrastBlock = new Mat(height, width, MatType.CV_32F,
                                               new Scalar(stdDev.Val0));
                    contrastBlock.CopyTo(localContrast.SubMat(new Rect(x, y, width, height)));

                    block.Dispose();
                    contrastBlock.Dispose();
                }
            }

            if (gray != image) gray.Dispose();
            return localContrast;
        }

        private static double GetSumFromIntegral(Mat integral, int x1, int y1, int x2, int y2)
        {
            float A = integral.At<float>(y1, x1);
            float B = integral.At<float>(y1, x2);
            float C = integral.At<float>(y2, x1);
            float D = integral.At<float>(y2, x2);

            return D - B - C + A;
        }

        #endregion
        #endregion

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



        /// <summary>
        /// 选择性局部对比度增强算法
        /// </summary>
        public bool EnhancedContrastAlgorithm { get; set; } = false;

        /// <summary>
        /// 增强系数
        /// </summary>
        public int EnhancedContrastIntensity { get; set; } = 12;

        /// <summary>
        /// 对比度增强阈值
        /// </summary>
        public int ContrastTreshold { get; set; } = 10;

        /// <summary>
        /// 去噪强度系数
        /// </summary>
        public int NoiseFilterIntensity { get; set; } = 3;
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
