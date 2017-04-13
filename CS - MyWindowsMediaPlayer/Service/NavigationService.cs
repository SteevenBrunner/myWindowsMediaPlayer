using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyWindowsMediaPlayer.Service
{
    sealed class NavigationService : INavigationService
    {
        private Frame _frame;

        public void Navigate(string view)
        {
            if (_frame != null)
                _frame.Navigate(new Uri(view, UriKind.Relative));
        }

        public void Navigate(object viewmodel)
        {
            _frame.Navigate(viewmodel);
        }

        public void Navigate(string view, object parameter)
        {
            if (_frame != null)
                _frame.Navigate(new Uri(view, UriKind.Relative), parameter);
        }

        public void GoBack()
        {
            _frame.GoBack();
        }

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

    }
}
