using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Utils;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.IO;
using System.Windows.Media.Imaging;
using MyWindowsMediaPlayer.Service;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using System.Web;
using System.Threading;

namespace MyWindowsMediaPlayer.ViewModel
{
    class MainWindowVM : ViewModelBase
    {
        #region Attributes
        private INavigationService _navigationService = null;
        private IWindowService _windowService = null;
        private PopupService _twitterPopup = null;

        private Media _currentMedia = null;
        private Playlist _currentPlaylist = null;
        private MediaElement _mediaElement = null;
        private DispatcherTimer _mediaTimer = new DispatcherTimer();
        private int _volume = 0;

        private DelegateCommand _loadCommand = null;
        private DelegateCommand _playCommand = null;
        private DelegateCommand _pauseCommand = null;
        private DelegateCommand _stopCommand = null;
        private DelegateCommand _navigateLibraryCommand = null;
        private DelegateCommand _navigatePlaylistsCommand = null;
        private DelegateCommand _twitterCommand = null;
        private DelegateCommand _youtubeCommand = null;
        private DelegateCommand _soundcloudCommand = null;
        #endregion

        #region Properties
        public MediaElement MediaElementCtrl
        {
            get { return (_mediaElement); }
        }
        public Media CurrentMedia
        {
            get { return (_currentMedia); }
            set
            {
                if (value != null && value.FileExists)
                {
                    if (_currentMedia != value)
                        _mediaElement.Source = null;
                    if (_currentPlaylist != null && !_currentPlaylist.Contains(value))
                        CurrentPlaylist = null;
                    else if (_currentPlaylist != null && _currentPlaylist.Contains(value))
                        _currentPlaylist.SetCurrentMedia(value);
                    _currentMedia = value;
                }
                else
                    _currentMedia = null;
                _twitterCommand.RaiseCanExecuteChanged();
                _playCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("CurrentPlaylist");
                NotifyPropertyChanged("CurrentMedia");
            }
        }
        public Playlist CurrentPlaylist
        {
            get { return (_currentPlaylist); }
            set
            {
                if (value != _currentPlaylist)
                {
                    if (_currentPlaylist != null)
                        _currentPlaylist.ResetCurrentMedia();
                    OnStop(null);
                    _currentMedia = null;
                    _currentPlaylist = value;
                    NotifyPropertyChanged("CurrentMedia");
                    NotifyPropertyChanged("CurrentPlaylist");
                    _twitterCommand.RaiseCanExecuteChanged();
                    _playCommand.RaiseCanExecuteChanged();
                }
            }
        }
        public int Volume
        {
            get { return (_volume); }
            set
            {
                _volume = value;
                if (_mediaElement != null)
                    _mediaElement.Volume = _volume / 2 / 100.0;
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                    _loadCommand = new DelegateCommand(OnLoad, CanLoad);

                return (_loadCommand);
            }
        }
        public ICommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                    _playCommand = new DelegateCommand(OnPlay, CanPlay);

                return (_playCommand);
            }
        }
        public ICommand PauseCommand
        {
            get
            {
                if (_pauseCommand == null)
                    _pauseCommand = new DelegateCommand(OnPause, CanPause);

                return (_pauseCommand);
            }
        }
        public ICommand StopCommand
        {
            get
            {
                if (_stopCommand == null)
                    _stopCommand = new DelegateCommand(OnStop, CanStop);

                return (_stopCommand);
            }
        }
        public ICommand NavigateLibraryCommand
        {
            get
            {
                if (_navigateLibraryCommand == null)
                    _navigateLibraryCommand = new DelegateCommand(OnNavigateLibrary, CanNavigateLibrary);

                return (_navigateLibraryCommand);
            }
        }
        public ICommand NavigatePlaylistsCommand
        {
            get
            {
                if (_navigatePlaylistsCommand == null)
                    _navigatePlaylistsCommand = new DelegateCommand(OnNavigatePlaylists, CanNavigatePlaylists);

                return (_navigatePlaylistsCommand);
            }
        }
        public ICommand TwitterCommand
        {
            get
            {
                if (_twitterCommand == null)
                    _twitterCommand = new DelegateCommand(OnTwitterCommand, CanTwitterCommand);

                return (_twitterCommand);
            }
        }

        public ICommand YoutubeCommand
        {
            get
            {
                if (_youtubeCommand == null)
                    _youtubeCommand = new DelegateCommand(OnYoutubeCommand, CanYoutubeCommand);

                return (_youtubeCommand);
            }
        }

        public ICommand SoundcloudCommand
        {
            get
            {
                if (_soundcloudCommand == null)
                    _soundcloudCommand = new DelegateCommand(OnSoundcloudCommand, CanSoundcloudCommand);

                return (_soundcloudCommand);
            }
        }
        #endregion

        #region Delegate Commands
        public void OnLoad(object arg)
        {
            var dialog = new System.Windows.Forms.OpenFileDialog();
            var result = dialog.ShowDialog();

            if (!string.IsNullOrEmpty(dialog.FileName))
            {
                _currentMedia = Media.Factory.make(dialog.FileName);
                if (_currentMedia != null)
                    _currentMedia.State = MediaState.Stop;
                if (_mediaElement != null)
                    _mediaElement.Stop();

                if (_currentPlaylist != null)
                {
                    _currentPlaylist = null;
                    NotifyPropertyChanged("CurrentPlaylist");
                }
                _playCommand.RaiseCanExecuteChanged();
                _pauseCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
                _twitterCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanLoad(object arg)
        {
            return (true);
        }

        public void OnPlay(object arg)
        {
            if (_currentPlaylist != null && _currentMedia == null)
            {
                CurrentMedia = _currentPlaylist.Next();
            }

            if (_currentMedia != null)
            {
                _mediaTimer.Start();
                if (_mediaElement.Source != _currentMedia.Path)
                    _mediaElement.Source = null;
                _mediaElement.Source = _currentMedia.Path;
                _mediaElement.Volume = Volume / 2 / 100.0;
                _mediaElement.Play();
                _currentMedia.State = MediaState.Play;

                _playCommand.RaiseCanExecuteChanged();
                _pauseCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
                _twitterCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanPlay(object arg)
        {
            return ((_currentMedia != null && _currentMedia.State != MediaState.Play)
                    || (_currentPlaylist != null && _currentMedia == null));
        }

        public void OnPause(object arg)
        {
            if (_currentMedia != null)
            {
                _mediaTimer.Stop();
                _mediaElement.Pause();
                _currentMedia.State = MediaState.Pause;

                _playCommand.RaiseCanExecuteChanged();
                _pauseCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
            }
        }

        public bool CanPause(object arg)
        {
            return (_currentMedia != null && _currentMedia.State == MediaState.Play);
        }

        public void OnStop(object arg)
        {
            if (_currentMedia != null)
            {
                _mediaTimer.Stop();
                _mediaElement.Stop();
                _currentMedia.State = MediaState.Stop;

                _playCommand.RaiseCanExecuteChanged();
                _pauseCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
            }
        }
        public bool CanStop(object arg)
        {
            return (_currentMedia != null && _currentMedia.State != MediaState.Stop);
        }

        public void OnNavigateLibrary(object arg)
        {
            string destination = (string)arg;

            switch (destination)
            {
                case "musics":
                    _navigationService.Navigate(new MusicLibraryVM(new WindowService(), new PlayerService(this)));
                    break;
                case "videos":
                    _navigationService.Navigate(new VideoLibraryVM(new WindowService(), new PlayerService(this)));
                    break;
                case "images":
                    _navigationService.Navigate(new ImageLibraryVM(new WindowService(), new PlayerService(this)));
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("User tried to load an invalid page.");
                    break;
            }
        }

        public bool CanNavigateLibrary(object arg)
        {
            return (true);
        }

        public void OnNavigatePlaylists(object arg)
        {
            _navigationService.Navigate(new PlaylistListVM(new PlayerService(this), _navigationService));
        }

        public bool CanNavigatePlaylists(object arg)
        {
            return (true);
        }

        public void OnTwitterCommand(object arg)
        {
            TwitterService twitterService = new TwitterService();

            if (twitterService.AuthenticationRequired())
            {
                if (_windowService == null)
                    _windowService = new WindowService();

                _windowService.CreateWindow(new TwitterLoginVM());
            }
            else
            {
                string message;

                if (_currentMedia.GetType() == typeof(Music))
                {
                    if ((_currentMedia as Music).Artists != null)
                        message = "Entrain d'écouter " + _currentMedia.Name + " de " + (_currentMedia as Music).Artists + " sur #MyWindowsMediaPlayer";
                    else
                        message = "Entrain d'écouter " + _currentMedia.Name + " sur #MyWindowsMediaPlayer";
                }
                else if (_currentMedia.GetType() == typeof(Video))
                    message = "Entrain de regarder " + _currentMedia.Name + " sur #MyWindowsMediaPlayer";
                else
                    message = "Entrain de lire le média " + _currentMedia.Name + " sur #MyWindowsMediaPlayer";

                if (twitterService.SendTweet(message) == false)
                {
                    _twitterCommand.RaiseCanExecuteChanged();
                    _twitterPopup.Message = "Erreur : " + twitterService.LastError;
                    _twitterPopup.Show(5);
                    System.Diagnostics.Debug.WriteLine("Unable to send tweet: " + twitterService.LastError);
                }
                else
                {
                    _currentMedia.Tweeted = true;
                    _twitterCommand.RaiseCanExecuteChanged();
                    _twitterPopup.Message = "Votre tweet a bien été envoyé !";
                    _twitterPopup.Show(3);
                }
            }
        }

        public bool CanTwitterCommand(object arg)
        {
            return (_currentMedia != null && !_currentMedia.Tweeted);
        }

        public void OnYoutubeCommand(object arg)
        {
            var dialogService = new DialogService();
            string url = dialogService.InputDialog("Adresse de la vidéo Youtube", "Youtube");

            if (string.IsNullOrWhiteSpace(url))
                return;

            Regex rgx = new Regex(@"^(?:https?\:\/\/)?(?:www\.)?(?:youtu\.be\/|youtube\.com\/(?:embed\/|v\/|watch\?v\=))([\w-]{10,12})(?:$|\&|\?\#).*");
            if (!string.IsNullOrEmpty(url) && rgx.IsMatch(url))
            {
                string URL = @"http://francois.kiene.fr/music/download.php?download="+ url;

                WebRequest MyRequest = HttpWebRequest.Create(URL);
                WebResponse MyResponse = MyRequest.GetResponse();

                string RealURL = MyResponse.ResponseUri.ToString();

                HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(RealURL);

                HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

                Stream receiveStream = myHttpWebResponse.GetResponseStream();
                StreamReader reader = new StreamReader(receiveStream, Encoding.UTF8);
                String responseString = reader.ReadToEnd();

                url = System.Web.HttpUtility.UrlPathEncode(responseString);

                Uri uri = new Uri(url);
                string filename = System.IO.Path.GetFileName(uri.LocalPath);

                if (filename != ".mp4")
                {
                    CurrentMedia = Media.Factory.make(url);
                    OnPlay(null);
                }
                else
                    dialogService.InformationDialog("Le lien fourni est invalide", "Youtube");
            }
            else
                dialogService.InformationDialog("Le lien fourni est invalide", "Youtube");
        }

        public bool CanYoutubeCommand(object arg)
        {
            return (true);
        }

        public void download(string stream, string name)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(stream, name);
            }
        }

        public void OnSoundcloudCommand(object arg)
        {
            var dialogService = new DialogService();
            string url = dialogService.InputDialog("Adresse de la musique sur Soundcloud", "Soundcloud");

            if (string.IsNullOrWhiteSpace(url))
                return;

            Regex rgx = new Regex(@"^(?:https?\:\/\/)?(?:www\.)?(?:soundcloud\.com\/).*");
            string data = null;
            if (!string.IsNullOrWhiteSpace(url) && rgx.IsMatch(url))
            {
                try
                {
                    string URL = @"http://api.soundcloud.com/resolve?url=" + url + "&client_id=df3f321110a6cf9290a08ba6dbd501fa";
                    WebClient c = new WebClient();
                    data = c.DownloadString(URL);
                }
                catch (WebException wex)
                {
                    if (((HttpWebResponse)wex.Response).StatusCode == HttpStatusCode.NotFound)
                        return;
                }
                if (!string.IsNullOrWhiteSpace(data))
                {
                    Newtonsoft.Json.Linq.JObject results = Newtonsoft.Json.Linq.JObject.Parse(data);
                    string stream = results["stream_url"].ToString() + @"?client_id=df3f321110a6cf9290a08ba6dbd501fa";
                    string name = results["title"].ToString() + ".mp3";

                    if (stream.Length > 0)
                    {
                        string tempfile = Path.Combine(Path.GetTempPath(), name);
                        download(stream, tempfile);
                        CurrentMedia = Media.Factory.make(tempfile);
                        OnPlay(null);
                    }
                }
            }
            else
                dialogService.InformationDialog("Le lien fourni est invalide", "Soundcloud");
        }
        public bool CanSoundcloudCommand(object arg)
        {
            return (true);
        }

        public void OnWindowClosing(object sender, EventArgs e)
        {
            PlaylistsService.Instance.Export(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.PlaylistsPath));
            LibrariesService.Instance.Export(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.LibrariesPath));
        }
        #endregion

        public MainWindowVM(INavigationService navigationService, PopupService twitterPopup)
        {
            _navigationService = navigationService;
            _twitterPopup = twitterPopup;

            _mediaElement = new MediaElement();
            _mediaElement.LoadedBehavior = MediaState.Manual;
            _mediaElement.UnloadedBehavior = MediaState.Stop;
            _mediaElement.MediaOpened += new RoutedEventHandler(MediaOpened);
            _mediaElement.MediaEnded += new RoutedEventHandler(MediaEnded);

            _mediaTimer.Interval = TimeSpan.FromMilliseconds(100);
            _mediaTimer.Tick += new EventHandler(UpdateMediaPosition);
            _mediaTimer.Start();

            OnNavigatePlaylists(null);
        }

        #region Methods
        private void UpdateMediaPosition(Object sender, EventArgs e)
        {
            if (_currentMedia != null)
            {
                _currentMedia.Position = _mediaElement.Position;
                NotifyPropertyChanged("CurrentMedia");
            }
        }

        private void MediaOpened(object sender, RoutedEventArgs e)
        {
            if (_mediaElement.NaturalDuration.HasTimeSpan)
                _currentMedia.Duration = _mediaElement.NaturalDuration.TimeSpan;
            else
                _currentMedia.Duration = new TimeSpan(0);
        }

        private void MediaEnded(object sender, RoutedEventArgs e)
        {
            if (_currentMedia != null)
                _currentMedia.State = MediaState.Stop;
            if (_mediaElement != null)
                _mediaElement.Stop();

            _playCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _stopCommand.RaiseCanExecuteChanged();

            if (_currentPlaylist != null)
            {
                _currentMedia = null;
                OnPlay(null);
            }
        }
        #endregion
    }
}
