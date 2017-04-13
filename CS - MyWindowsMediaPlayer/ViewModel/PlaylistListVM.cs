using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Service;
using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows.Input;
using System.IO;

namespace MyWindowsMediaPlayer.ViewModel
{
    class PlaylistListVM : ViewModelBase
    {
        #region Attributes
        private ObservableCollection<Playlist> _playlists = new ObservableCollection<Playlist>();
        private string _newPlaylistName = "";

        private IPlayerService _playerService = null;
        private INavigationService _navigationService = null;
        private DelegateCommand _createPlaylistCommand = null;
        private DelegateCommand _playPlaylistCommand = null;
        private DelegateCommand _selectPlaylistCommand = null;
        #endregion

        #region Properties
        public ObservableCollection<Playlist> Playlists
        {
            get { return (new ObservableCollection<Playlist>(PlaylistsService.Instance)); }
            //set { _playlists = value; }
        }

        public string NewPlaylistName
        {
            get { return (_newPlaylistName); }
            set
            {
                _newPlaylistName = value;
                _createPlaylistCommand.RaiseCanExecuteChanged();
                NotifyPropertyChanged("NewPlaylistName");
            }
        }

        public ICommand CreatePlaylistCommand
        {
            get
            {
                if (_createPlaylistCommand == null)
                    _createPlaylistCommand = new DelegateCommand(OnCreatePlaylist, CanCreatePlaylist);

                return (_createPlaylistCommand);
            }
        }

        public ICommand PlayPlaylistCommand
        {
            get
            {
                if (_playPlaylistCommand == null)
                    _playPlaylistCommand = new DelegateCommand(OnPlayPlaylist, CanPlayPlaylist);

                return (_playPlaylistCommand);
            }
        }

        public ICommand SelectPlaylistCommand
        {
            get
            {
                if (_selectPlaylistCommand == null)
                    _selectPlaylistCommand = new DelegateCommand(OnSelectPlaylist, CanSelectPlaylist);

                return (_selectPlaylistCommand);
            }
        }
        #endregion

        #region Delegate Commands
        public void OnCreatePlaylist(object arg)
        {
            if (string.IsNullOrEmpty(NewPlaylistName))
                return;

            PlaylistsService.Instance.Add(new Playlist(NewPlaylistName.Trim()));
            NotifyPropertyChanged("Playlists");

            NewPlaylistName = "";
            SavePlaylist();
        }

        public bool CanCreatePlaylist(object arg)
        {
            return (!string.IsNullOrWhiteSpace(NewPlaylistName) && !PlaylistsService.Instance.ContainsByName(NewPlaylistName));
        }

        public void OnPlayPlaylist(object arg)
        {
            Playlist playlist = arg as Playlist;

            _playerService.SetPlaylist(playlist);
            _playerService.Play();
        }

        public bool CanPlayPlaylist(object arg)
        {
            return (true);
        }

        public void OnSelectPlaylist(object arg)
        {
            Playlist playlist = arg as Playlist;

            _navigationService.Navigate(new PlaylistVM(playlist, _playerService, _navigationService));
        }

        public bool CanSelectPlaylist(object arg)
        {
            return (true);
        }
        #endregion

        #region Saving
        public void SavePlaylist()
        {
            PlaylistsService.Instance.Export(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.PlaylistsPath));
        }
        #endregion

        #region Ctor / Dtor
        public PlaylistListVM(IPlayerService playerService, INavigationService navigationService)
        {
            _playerService = playerService;
            _navigationService = navigationService;

            PlaylistsService.Instance.ImportOnce(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.PlaylistsPath));
        }
        #endregion

        public string GetResourceTextFile(string filename)
        {
            string result = string.Empty;

            using (Stream stream = this.GetType().Assembly.
                       GetManifestResourceStream("epitech-mywindowsmediaplayer.Save." + filename))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }
            return result;
        }
    }
}
