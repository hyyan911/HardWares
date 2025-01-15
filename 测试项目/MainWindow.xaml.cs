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
using HardWares.源表.KEITHLEY_2450;
using HardWares.源表;

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
            Port.ItemsSource = new PowerSource().GetUsbDeviceNames();
        }


        PowerSource cc = null;
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, RoutedEventArgs e)
        {
            cc = new PowerSource();
            State.Content = cc.ConnectUSB(Port.SelectedItem.ToString(), out Exception ex).ToString();
            if (State.Content.ToString() == "False") { cc.Dispose(); return; }
            cc.Output = true;
            tar.Content = cc.Measure().Voltage;
            pos.Content = cc.Measure().Current;
            cc.Dispose();
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
            //cc.Stages[0].MoveToAndWait(cc.Stages[0].Target + 0.1, 2000);
        }

        private void LeftMove(object sender, RoutedEventArgs e)
        {
            // cc.Stages[0].MoveToAndWait(cc.Stages[0].Target - 0.1, 2000);
        }
    }
}
