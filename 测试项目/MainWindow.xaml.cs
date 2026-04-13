using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using HardWares.纳米位移台.PI;
using HardWares.仪器列表.电动翻转座;
using HardWares.源表.KEITHLEY_2450;
using HardWares.Windows;
using HardWares.温度控制器;
using HardWares.相机_CCD_;
using HardWares.端口基类;
using HardWares.源表;
using HardWares.纳米位移台;
using HardWares.纳米位移台.低温多场.MC_Newton_N;
using HardWares.仪器列表.板卡.Spincore_PulseBlaster;
//using NationalInstruments.DAQmx;
using HardWares.仪器列表.板卡;
using HardWares.射频源;
using HardWares.APD;
using HardWares.APD.Exclitas_SPCM_AQRH;
using NationalInstruments.DAQmx;
using HardWares.端口基类部分.设备信息;
using HardWares.Lock_In;
using HardWares.继电器模块;
using HardWares.电源;

namespace 测试项目
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        // 旋磁比(Mhz/Gauss)
        private static double gammaE = 2.803;

        /// <summary>
        /// 根据谱峰位置计算横向和纵向的磁场大小
        /// </summary>
        /// <param name="freq1"></param>
        /// <param name="freq2"></param>
        /// <param name="D"></param>
        /// <param name="Bp"></param>
        /// <param name="Bv"></param>
        protected void CalculateB(double freq1, double freq2, out double Bp, out double Bv, out double B)
        {
            double D = 2870;
            double detplus = (freq2 + freq1) / 2;
            double detminus = Math.Abs(freq2 - freq1);
            double det1 = 2 * D * (detplus - D) / 3;
            if (det1 < 0)
                det1 = 0;
            Bv = Math.Sqrt(det1) / gammaE;
            double det2 = Math.Pow(detminus / (2 * gammaE), 2) - Math.Pow(Bv * Bv * gammaE / (2 * D), 2);
            if (det2 < 0)
                det2 = 0;
            Bp = Math.Sqrt(det2);
            B = Math.Sqrt(Bp * Bp + Bv * Bv);
        }

        public MainWindow()
        {
            InitializeComponent();
            CalculateB(3050, 2700, out double bp, out double bv, out double b);
            count.Content = bp.ToString() + "," + bv.ToString();
        }

        SignalGeneratorBase apd = null;

        SignalGeneratorBase pb = null;

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectAPD(object sender, RoutedEventArgs e)
        {
            ConnectWindow win = new ConnectWindow(typeof(CameraBase));
            win.ShowDialog(this);
            //apd = win.ConnectedDevice as SignalGeneratorBase;
            //ParameterWindow w = new ParameterWindow(apd.Channels[0], this);
            //w.ShowDialog();
        }

        Thread t = null;
        private void RightMove(object sender, RoutedEventArgs e)
        {
        }

        private void LeftMove(object sender, RoutedEventArgs e)
        {
        }

        int channel = 5;
        private void ConnectPB(object sender, RoutedEventArgs e)
        {
            DeviceFindWindow w = new DeviceFindWindow();
            var result = w.ShowDialog();
        }
    }
}
