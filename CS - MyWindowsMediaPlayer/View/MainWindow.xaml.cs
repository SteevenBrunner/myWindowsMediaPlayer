using MyWindowsMediaPlayer.Service;
using MyWindowsMediaPlayer.Utils;
using MyWindowsMediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Threading;

namespace MyWindowsMediaPlayer.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _fullscreen = false;
        private DispatcherTimer _doubleClickTimer = new DispatcherTimer();
        private GridLength[] _columnsWidth = new GridLength[4];

        public MainWindow()
        {
            InitializeComponent();

            var viewModel = new MainWindowVM(new NavigationService(this.LeftContentFrame), new PopupService(this.TwitterResult, this.PopupMessage));
            this.DataContext = viewModel;

            Closing += viewModel.OnWindowClosing;

            _doubleClickTimer.Interval = TimeSpan.FromMilliseconds(GetDoubleClickTime());
            _doubleClickTimer.Tick += (s, e) => _doubleClickTimer.Stop();
        }

        private void MediaPlayer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_doubleClickTimer.IsEnabled)
                _doubleClickTimer.Start();
            else
            {
                if (!_fullscreen)
                {
                    this.WindowStyle = WindowStyle.None;
                    this.WindowState = WindowState.Maximized;

                    this.ContentSplitter.Visibility = Visibility.Collapsed;
                    this.LeftContentFrame.Visibility = Visibility.Collapsed;
                    this.LeftMenu.Visibility = Visibility.Collapsed;
                    _columnsWidth[0] = this.ContentGrid.ColumnDefinitions[0].Width;
                    _columnsWidth[1] = this.ContentGrid.ColumnDefinitions[1].Width;
                    _columnsWidth[2] = this.ContentGrid.ColumnDefinitions[2].Width;
                    this.ContentGrid.ColumnDefinitions[0].Width = new GridLength(0, GridUnitType.Auto);
                    this.ContentGrid.ColumnDefinitions[1].Width = new GridLength(0, GridUnitType.Auto);
                    this.ContentGrid.ColumnDefinitions[2].Width = new GridLength(0, GridUnitType.Auto);
                }
                else
                {
                    this.WindowStyle = WindowStyle.SingleBorderWindow;
                    this.WindowState = WindowState.Normal;

                    this.ContentGrid.ColumnDefinitions[0].Width = _columnsWidth[0];
                    this.ContentGrid.ColumnDefinitions[1].Width = _columnsWidth[1];
                    this.ContentGrid.ColumnDefinitions[2].Width = _columnsWidth[2];
                    this.ContentSplitter.Visibility = Visibility.Visible;
                    this.LeftContentFrame.Visibility = Visibility.Visible;
                    this.LeftMenu.Visibility = Visibility.Visible;
                }

                _fullscreen = !_fullscreen;
            }
        }

        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();
    }
}
