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

        PortObject obj = null;
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, RoutedEventArgs e)
        {
            ConnectWindow win = new ConnectWindow(typeof(NanoControllerBase));
            State.Content = win.ShowDialog(this);
            obj = win.ConnectedDevice;
            //Thread t = new Thread(() =>
            //{
            //    while (true)
            //    {
            //        MeasureGroup ta = cc.Measure();
            //        Dispatcher.Invoke(() =>
            //        {
            //            tar.Content = ta.Voltage.ToString();
            //            pos.Content = ta.Current.ToString();
            //        });
            //        Thread.Sleep(20);
            //    }
            //});
            //t.Start();

        }

        private void RightMove(object sender, RoutedEventArgs e)
        {
            ParameterWindow win = new ParameterWindow(obj, this);
            win.ShowDialog();
            var p = (obj as NanoControllerBase).Stages[0].Position;

            var t = (obj as NanoControllerBase).Stages[0].Target;

            //PowerSourceBase s = obj as PowerSourceBase;
            //var re = s.Output;
        }

        private void LeftMove(object sender, RoutedEventArgs e)
        {
        }
    }
}
