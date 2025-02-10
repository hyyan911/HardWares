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
using HardWares.波源.Rigol_DSG_3060;

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
            Port.ItemsSource = new PIController().GetTCPIPDeviceNames();
        }

        PIController cc = null;
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, RoutedEventArgs e)
        {
            cc = new PIController();
            State.Content = cc.ConnectTCPIP(Port.SelectedItem.ToString(), out Exception ex).ToString();
            if (State.Content.ToString() == "False") { cc.Dispose(); return; }
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
        }

        private void LeftMove(object sender, RoutedEventArgs e)
        {
        }
    }
}
