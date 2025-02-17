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
using System.Windows.Shapes;
using HardWares.温度控制器;
using HardWares.温度控制器.SRS_PTC10;
using Controls;
using HardWares.端口基类;
using HardWares;
using System.Threading;
using Controls.Windows;
using HardWares.端口基类部分.设备信息;
using System.Windows.Media.Animation;
using HardWares.端口基类.COM串口;
using HardWares.端口基类部分.PortHelper;

namespace HardWares.Windows
{
    /// <summary>
    /// ConnectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ConnectWindow : Window
    {

        bool IsConnected = false;

        /// <summary>
        /// 
        /// </summary>
        public PortObject ConnectedDevice { get; private set; } = null;

        /// <summary>
        /// 连接信息
        /// </summary>
        public DeviceInfoBase ConnectInfo { get; private set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetType"></param>
        /// <param name="parentwindow"></param>
        public ConnectWindow(Type targetType)
        {
            InitializeComponent();

            Dictionary<string, Type> types = PortObject.GetDeviceNamesWithSameBase(targetType);

            DeviceTypeList.TemplateButton = DeviceTypeList;

            foreach (var item in types)
            {
                DeviceTypeList.Items.Add(new DecoratedButton() { Text = item.Key, Tag = item.Value });
            }

            DoubleAnimation ani = new DoubleAnimation(0, 360, new Duration(TimeSpan.FromSeconds(2))) { RepeatBehavior = RepeatBehavior.Forever };
            Rot.BeginAnimation(RotateTransform.AngleProperty, ani);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowDialog(Window parentwindow)
        {
            if (parentwindow != null)
            {
                Owner = parentwindow;
                WindowStartupLocation = WindowStartupLocation.CenterOwner;
            }
            ShowDialog();
            return IsConnected;
        }

        /// <summary>
        /// 连接USB
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Connect(object sender, RoutedEventArgs e)
        {
            if (DeviceList.GetSelectedTag() == null || DeviceTypeList.SelectedItem == null) return;
            Thread t = new Thread(() =>
            {
                PortObject obj = null;
                Dispatcher.Invoke(() =>
                {
                    Connecting.Visibility = Visibility.Visible;
                    Connected.Visibility = Visibility.Hidden;
                    obj = Activator.CreateInstance(DeviceTypeList.SelectedItem.Tag as Type) as PortObject;
                });
                DeviceInfoBase info = DeviceList.GetSelectedTag() as DeviceInfoBase;
                IsConnected = obj.Connect(info, out Exception ex);
                if (IsConnected)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Connecting.Visibility = Visibility.Hidden;
                        Connected.Visibility = Visibility.Visible;
                        CloseBtn.IsEnabled = true;
                    });
                    ConnectInfo = info;
                    ConnectedDevice = obj;
                    Thread.Sleep(2000);
                    Dispatcher.Invoke(() =>
                    {
                        if (IsActive)
                            Close();
                    });
                }
                else
                {
                    MessageWindow.ShowTipWindow(ex.Message, this);
                    Dispatcher.Invoke(() =>
                    {
                        Connecting.Visibility = Visibility.Hidden;
                        Connected.Visibility = Visibility.Hidden;
                    });
                    IsConnected = false;
                    ConnectedDevice = null;
                }
            });
            t.Start();


        }



        Queue<Thread> RefreshTasks = new Queue<Thread>();
        bool RefreshThreadEndTag = false;
        /// <summary>
        /// 刷新串口列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshPort(object sender, RoutedEventArgs e)
        {
            if (DeviceTypeList.SelectedItem == null) return;
            RefreshThreadEndTag = true;
            Thread RefreshThread = new Thread(() =>
            {
                while (RefreshTasks.Count > 1) Thread.Sleep(100);
                RefreshThreadEndTag = false;

                PortObject obj = null;
                Dispatcher.Invoke(() =>
                {
                    DeviceList.ClearItems();
                    DeviceList.Visibility = Visibility.Collapsed;
                    LoadingImage.Visibility = Visibility.Visible;
                    CloseBtn.IsEnabled = false;
                    obj = Activator.CreateInstance(DeviceTypeList.SelectedItem.Tag as Type) as PortObject;
                });
                List<DeviceInfoBase> connectinfos = new List<DeviceInfoBase>();
                if (obj is COMOuterInterface)
                {
                    PortHelper.EndJudgeFunc = new Func<bool>(() => { return RefreshThreadEndTag; });
                    connectinfos.AddRange((obj as COMOuterInterface).GetCOMDeviceInfos());
                    PortHelper.EndJudgeFunc = null;
                }
                if (RefreshThreadEndTag)
                {
                    EndRefreshBehaviour();
                    RefreshThreadEndTag = false;
                    RefreshTasks.Dequeue();
                    return;
                }
                if (obj is WinUSBOuterInterface)
                {
                    PortHelper.EndJudgeFunc = new Func<bool>(() => { return RefreshThreadEndTag; });
                    connectinfos.AddRange((obj as WinUSBOuterInterface).GetUsbDeviceInfos());
                    PortHelper.EndJudgeFunc = null;
                }
                if (RefreshThreadEndTag)
                {
                    EndRefreshBehaviour();
                    RefreshThreadEndTag = false;
                    RefreshTasks.Dequeue();
                    return;
                }
                if (obj is TCPIPOuterInterface)
                {
                    PortHelper.EndJudgeFunc = new Func<bool>(() => { return RefreshThreadEndTag; });
                    connectinfos.AddRange((obj as TCPIPOuterInterface).GetTCPIPDeviceInfos());
                    PortHelper.EndJudgeFunc = null;
                }
                if (RefreshThreadEndTag)
                {
                    EndRefreshBehaviour();
                    RefreshThreadEndTag = false;
                    RefreshTasks.Dequeue();
                    return;
                }
                if (obj is CustomOuterInterface)
                {
                    PortHelper.EndJudgeFunc = new Func<bool>(() => { return RefreshThreadEndTag; });
                    connectinfos.AddRange((obj as CustomOuterInterface).GetCustomDeviceInfos());
                    PortHelper.EndJudgeFunc = null;
                }
                if (RefreshThreadEndTag)
                {
                    EndRefreshBehaviour();
                    RefreshThreadEndTag = false;
                    RefreshTasks.Dequeue();
                    return;
                }
                Dispatcher.Invoke(() =>
                {
                    foreach (var item in connectinfos)
                    {
                        DeviceList.AddItem(item, item.DeviceName);
                    }
                });
                EndRefreshBehaviour();
                RefreshThreadEndTag = false;
                RefreshTasks.Dequeue();
            });
            RefreshThread.Start();
            RefreshTasks.Enqueue(RefreshThread);
        }

        private void EndRefreshBehaviour()
        {
            Dispatcher.Invoke(() =>
            {
                DeviceList.Visibility = Visibility.Visible;
                LoadingImage.Visibility = Visibility.Collapsed;
                CloseBtn.IsEnabled = true;
            });
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.GetPosition(this).Y < 30)
            {
                DragMove();
            }
        }
    }
}
