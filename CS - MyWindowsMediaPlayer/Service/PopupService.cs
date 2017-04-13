using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace MyWindowsMediaPlayer.Service
{
    class PopupService
    {
        #region Attributes
        Popup _popup = null;
        TextBlock _message = null;

        private DispatcherTimer _displayTimer = new DispatcherTimer();
        #endregion

        #region Properties
        public string Message
        {
            get
            {
                return (_message.Text);
            }
            set
            {
                _message.Text = value;
            }
        }
        #endregion

        #region Methods
        public void Show(int seconds = 0)
        {
            _popup.IsOpen = true;

            if (seconds > 0)
            {
                _displayTimer.Interval = TimeSpan.FromMilliseconds(seconds * 1000);
                _displayTimer.Tick += new EventHandler(Hide);
                _displayTimer.Start();
            }
        }

        public void Hide()
        {
            _displayTimer.Stop();
            _popup.IsOpen = false;
        }

        public void Hide(Object sender, EventArgs e)
        {
            Hide();
        }
        #endregion

        public PopupService(Popup popup, TextBlock message)
        {
            _popup = popup;
            _message = message;
        }
    }
}
