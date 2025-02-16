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
            // Port.ItemsSource = DaqSystem.Local.GetTerminals(TerminalTypes.All);
        }

        PortObject obj = null;
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, RoutedEventArgs e)
        {
            ConnectWindow win = new ConnectWindow(typeof(PulseBlasterBase));
            State.Content = win.ShowDialog(this);
            obj = win.ConnectedDevice;
            Thread t = new Thread(() =>
            {
                while (true)
                {
                    var ta = (obj as NanoControllerBase).Stages[0].Position;
                    Dispatcher.Invoke(() =>
                    {
                        tar.Content = ta.ToString();
                        pos.Content = ta.ToString();
                    });
                    Thread.Sleep(50);
                }
            });
            t.Start();
        }

        private void RightMove(object sender, RoutedEventArgs e)
        {
            string v = (obj as NanoControllerBase).Stages[0].PositionUnit;
            (obj as NanoControllerBase).Stages[0].MoveStepAndWait(0.01, 500, true);
        }

        private void LeftMove(object sender, RoutedEventArgs e)
        {
            (obj as NanoControllerBase).Stages[0].MoveStepAndWait(-0.01, 100, true);
        }
    }
}
