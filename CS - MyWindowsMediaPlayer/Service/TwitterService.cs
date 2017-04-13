using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetSharp;

namespace MyWindowsMediaPlayer.Service
{
    class TwitterService
    {
        #region Attributes
        private readonly string APP_KEY = "yfb6Sy9Pd3TazdJNztK8kLWNd";
        private readonly string APP_SECRET = "K0ECP5cQKH5mJo9bYzOMR986wKR9w1WVJBLbjl424d5w3pgrVa";

        TweetSharp.TwitterService _service = null;
        OAuthRequestToken _requestToken = null;
        #endregion

        #region Properties
        private string TwitterToken
        {
            get
            {
                return (Properties.Settings.Default.TwitterToken);
            }
            set
            {
                Properties.Settings.Default.TwitterToken = value;
                Properties.Settings.Default.Save();
            }
        }

        private string TwitterAccess
        {
            get
            {
                return (Properties.Settings.Default.TwitterAccess);
            }
            set
            {
                Properties.Settings.Default.TwitterAccess = value;
                Properties.Settings.Default.Save();
            }
        }

        private string TwitterAccessSecret
        {
            get
            {
                return (Properties.Settings.Default.TwitterAccessSecret);
            }
            set
            {
                Properties.Settings.Default.TwitterAccessSecret = value;
                Properties.Settings.Default.Save();
            }
        }

        public String LastError
        {
            get
            {
                return (_service.Response.Error != null ? _service.Response.Error.Message : null);
            }
        }
        #endregion

        #region Methods
        public void AuthorizeApplication()
        {
            _requestToken = _service.GetRequestToken();

            Uri uri = _service.GetAuthorizationUri(_requestToken);
            Process.Start(uri.ToString());
        }

        public bool GenerateAccess(string token)
        {
            TwitterToken = token;

            if (string.IsNullOrEmpty(TwitterAccess) || string.IsNullOrEmpty(TwitterAccessSecret))
            {
                OAuthAccessToken access = _service.GetAccessToken(_requestToken, TwitterToken);
                TwitterAccess = access.Token;
                TwitterAccessSecret = access.TokenSecret;
            }

            _service.AuthenticateWith(TwitterAccess, TwitterAccessSecret);
            return (_service.Response.Error == null);
        }

        public bool SendTweet(string message)
        {
            _service.AuthenticateWith(TwitterAccess, TwitterAccessSecret);
            TwitterStatus tweet = _service.SendTweet(new SendTweetOptions { Status = message });

            return (_service.Response.Error == null);
        }

        public bool AuthenticationRequired()
        {
            return (string.IsNullOrEmpty(TwitterAccess) || string.IsNullOrEmpty(TwitterAccessSecret));
        }
        #endregion

        public TwitterService()
        {
            _service = new TweetSharp.TwitterService(APP_KEY, APP_SECRET);
        }
    }
}
