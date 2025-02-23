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
using HardWares.板卡;
using HardWares.APD;
using HardWares.APD.Exclitas_SPCM_AQRH;
using NationalInstruments.DAQmx;
using HardWares.端口基类部分.设备信息;
using HardWares.Lock_In;

namespace 测试项目
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        APDBase apd = null;

        LockInBase pb = null;

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConnectAPD(object sender, RoutedEventArgs e)
        {
            ConnectWindow win = new ConnectWindow(typeof(APDBase));
            win.ShowDialog(this);
            apd = win.ConnectedDevice as APDBase;
            ParameterWindow w = new ParameterWindow(apd, this);
            w.ShowDialog();
        }

        Thread t = null;
        private void RightMove(object sender, RoutedEventArgs e)
        {
            pb.PIDOutput = !pb.PIDOutput;
            count.Content = pb.DemodR;
        }

        private void LeftMove(object sender, RoutedEventArgs e)
        {
        }

        int channel = 5;
        private void ConnectPB(object sender, RoutedEventArgs e)
        {
            ConnectWindow win = new ConnectWindow(typeof(LockInBase));
            win.ShowDialog(this);
            pb = win.ConnectedDevice as LockInBase;
        }
    }
}
