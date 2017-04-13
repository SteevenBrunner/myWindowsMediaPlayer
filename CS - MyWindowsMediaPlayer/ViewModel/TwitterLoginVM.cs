using MyWindowsMediaPlayer.Service;
using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyWindowsMediaPlayer.ViewModel
{
    class TwitterLoginVM : ViewModelBase
    {
        #region Attributes
        private DelegateCommand _loginCommand = null;
        private DelegateCommand _tokenCommand = null;

        private TwitterService _twitterService = null;
        #endregion

        #region Properties
        public string Token
        {
            get; set;
        }

        public ICommand TokenCommand
        {
            get
            {
                if (_tokenCommand == null)
                    _tokenCommand = new DelegateCommand(OnToken, CanToken);

                return (_tokenCommand);
            }
        }

        public ICommand LoginCommand
        {
            get
            {
                if (_loginCommand == null)
                    _loginCommand = new DelegateCommand(OnLogin, CanLogin);

                return (_loginCommand);
            }
        }
        #endregion

        #region Commands
        public void OnToken(object arg)
        {
            _twitterService.AuthorizeApplication();
        }

        public bool CanToken(object arg)
        {
            return (true);
        }

        public void OnLogin(object arg)
        {
            _twitterService.GenerateAccess(Token);
        }

        public bool CanLogin(object arg)
        {
            return (true);
        }
        #endregion

        public TwitterLoginVM()
        {
            _twitterService = new TwitterService();
            Token = Properties.Settings.Default.TwitterToken;
        }
    }
}
