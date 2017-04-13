using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyWindowsMediaPlayer.Service
{
    class WindowService : IWindowService
    {
        public void CreateWindow(object viewModel)
        {
            var window = new Window();
            window.Content = viewModel;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.ResizeMode = ResizeMode.NoResize;
            window.Show();
        }
    }
}
