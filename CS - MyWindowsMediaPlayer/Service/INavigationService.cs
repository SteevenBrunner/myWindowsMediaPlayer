using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer.Service
{
    interface INavigationService
    {
        void Navigate(string page);
        void Navigate(object viewModel);
        void Navigate(string page, object parameter);
        void GoBack();
    }
}
