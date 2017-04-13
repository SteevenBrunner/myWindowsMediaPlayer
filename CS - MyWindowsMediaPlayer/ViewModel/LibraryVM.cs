using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Service;
using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;

namespace MyWindowsMediaPlayer.ViewModel
{
    abstract class LibraryVM : ViewModelBase
    {
        #region Attributes
        protected IWindowService _windowService = null;
        protected IPlayerService _playerService = null;
        private DispatcherTimer _mediaTimer = new DispatcherTimer();

        protected Library _library = null;

        private DelegateCommand _manageLibraryCommand = null;
        private DelegateCommand _addToPlaylistCommand = null;
        private DelegateCommand _selectMediaCommand = null;
        #endregion

        #region Properties
        public Library Library
        {
            get { return (_library); }
        }

        public ICommand ManageLibraryCommand
        {
            get
            {
                if (_manageLibraryCommand == null)
                    _manageLibraryCommand = new DelegateCommand(OnManageLibrary, CanManageLibrary);

                return (_manageLibraryCommand);
            }
        }

        public ICommand SelectMediaCommand
        {
            get
            {
                if (_selectMediaCommand == null)
                    _selectMediaCommand = new DelegateCommand(OnSelectMedia, CanSelectMedia);

                return (_selectMediaCommand);
            }
        }

        public ICommand AddToPlaylistCommand
        {
            get
            {
                if (_addToPlaylistCommand == null)
                    _addToPlaylistCommand = new DelegateCommand(OnAddToPlaylist, CanAddToPlaylist);

                return (_addToPlaylistCommand);
            }
        }
        #endregion

        #region Commands
        public abstract void OnManageLibrary(object arg);

        public bool CanManageLibrary(object arg)
        {
            return (_library != null);
        }

        public void OnSelectMedia(object arg)
        {
            _playerService.SetMedia(arg as Media);
            _playerService.Play();
        }

        public bool CanSelectMedia(object arg)
        {
            return (_library.Items.Count > 0);
        }

        public void OnAddToPlaylist(object arg)
        {
            if (arg != null)
            {
                var dialogService = new DialogService();

                string playlistName = dialogService.SelectDialog("Sélectionnez la playlist", "Ajout à une playlist", PlaylistsService.Instance.Names);

                if (!string.IsNullOrWhiteSpace(playlistName))
                {
                    Playlist playlist = PlaylistsService.Instance.FindByName(playlistName);

                    if (playlist != null)
                    {
                        playlist.Add(arg as Media);
                    }
                }
            }
        }

        public bool CanAddToPlaylist(object arg)
        {
            return (true);
        }
        #endregion

        #region Methods
        private void UpdateLibrary(Object sender, EventArgs e)
        {
            if (_library != null)
            {
                _library.LoadItems();
            }
        }
        #endregion

        public LibraryVM(IWindowService windowService, IPlayerService playerService)
        {
            LibrariesService.Instance.ImportOnce(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Properties.Settings.Default.LibrariesPath));
            
            _windowService = windowService;
            _playerService = playerService;

            _mediaTimer.Interval = TimeSpan.FromMilliseconds(20000);
            _mediaTimer.Tick += new EventHandler(UpdateLibrary);
            _mediaTimer.Start();
        }
    }
}
