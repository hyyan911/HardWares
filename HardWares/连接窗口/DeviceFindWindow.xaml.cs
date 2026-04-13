using CodeHelper;
using Controls;
using Controls.Windows;
using HardWares.端口基类;
using HardWares.端口基类部分;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ComboBox = Controls.ComboBox;

namespace HardWares.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class DeviceFindWindow : Window
    {
        PortObject SelectedDevice = null;

        PortElement SelectedChannel = null;

        bool IsExit = true;


        public DeviceFindWindow()
        {
            InitializeComponent();
            WindowResizeHelper helper = new WindowResizeHelper();
            helper.RegisterCloseWindow(this, null, null, CloseBtn);
            helper.BeforeClose += new RoutedEventHandler((s, e) => { if (IsExit) { SelectedChannel = null; SelectedDevice = null; } });
            //获取设备列表
            var list = PortObject.DeviceInfos;
            DevicePanel.ClearItems();
            foreach (var item in list)
            {
                DevicePanel.AddItem(item.Value, item.Value.ProductIdentifier, item.Value.ProductName);
            }
        }

        /// <summary>
        /// 显示窗口，返回选中的设备，如果未选择返回（null,null）,如果选中不带通道的设备则返回（设备，null）,如果选中通道则返回（设备，通道）
        /// </summary>
        /// <returns></returns>
        public new KeyValuePair<PortObject, PortElement> ShowDialog()
        {
            base.ShowDialog();
            return new KeyValuePair<PortObject, PortElement>(SelectedDevice, SelectedChannel);
        }

        private static void CloneTextBoxStyle(TextBox source, TextBox target)
        {
            target.FontSize = source.FontSize;
            target.FontWeight = source.FontWeight;
            target.TextAlignment = source.TextAlignment;
            target.HorizontalAlignment = source.HorizontalAlignment;
            target.VerticalAlignment = source.VerticalAlignment;
            target.HorizontalContentAlignment = source.HorizontalContentAlignment;
            target.VerticalContentAlignment = source.VerticalContentAlignment;
            target.BorderThickness = source.BorderThickness;
            target.BorderBrush = source.BorderBrush;
            target.Background = source.Background;
            target.CaretBrush = source.CaretBrush;
            target.Foreground = source.Foreground;
            target.SelectionBrush = source.SelectionBrush;
        }

        /// <summary>
        /// 刷新通道
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        private void DevicePanel_ItemSelected(int arg1, object arg2)
        {
            SelectedDevice = arg2 as PortObject;
            UpdateChannelPanel();
        }

        private void UpdateChannelPanel()
        {
            SelectedChannel = null;
            DeviceChannelPanel.ClearItems();
            if (SelectedDevice is ElementPortObject)
            {
                var channels = (SelectedDevice as ElementPortObject).Channels;
                foreach (var item in channels)
                {
                    DeviceChannelPanel.AddItem(item, item.ChannelName);
                }
            }
        }

        private void DeviceChannelPanel_ItemSelected(int arg1, object arg2)
        {
            SelectedChannel = arg2 as PortElement;
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            IsExit = false;
            Close();
        }
    }
}
