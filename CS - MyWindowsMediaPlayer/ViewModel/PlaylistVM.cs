using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Service;
using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace MyWindowsMediaPlayer.ViewModel
{
    class PlaylistVM : ViewModelBase
    {
        #region Attributes
        private Playlist _playlist = null;

        private IPlayerService _playerService = null;
        private INavigationService _nagivationService = null;
        private ICommand _playPlaylistCommand = null;
        private ICommand _selectMediaCommand = null;
        private ICommand _removeMediaCommand = null;
        private ICommand _renamePlaylistCommand = null;
        private ICommand _deletePlaylistCommand = null;
        #endregion

        #region Properties
        public Playlist Playlist
        {
            get { return (_playlist); }
        }

        public ICollectionView Items
        {
            get
            {
                return (new ListCollectionView(_playlist));
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

        public ICommand SelectMediaCommand
        {
            get
            {
                if (_selectMediaCommand == null)
                    _selectMediaCommand = new DelegateCommand(OnSelectMedia, CanSelectMedia);

                return (_selectMediaCommand);
            }
        }

        public ICommand RemoveMediaCommand
        {
            get
            {
                if (_removeMediaCommand == null)
                    _removeMediaCommand = new DelegateCommand(OnRemoveMedia, CanRemoveMedia);

                return (_removeMediaCommand);
            }
        }

        public ICommand RenamePlaylistCommand
        {
            get
            {
                if (_renamePlaylistCommand == null)
                    _renamePlaylistCommand = new DelegateCommand(OnRenamePlaylist, CanRenamePlaylist);

                return (_renamePlaylistCommand);
            }
        }

        public ICommand DeletePlaylistCommand
        {
            get
            {
                if (_deletePlaylistCommand == null)
                    _deletePlaylistCommand = new DelegateCommand(OnDeletePlaylist, CanDeletePlaylist);

                return (_deletePlaylistCommand);
            }
        }
        #endregion

        #region Commands
        public void OnPlayPlaylist(object arg)
        {
            _playerService.SetPlaylist(_playlist);
            _playerService.Play();
        }

        public bool CanPlayPlaylist(object arg)
        {
            return (true);
        }

        public void OnSelectMedia(object arg)
        {
            _playerService.SetPlaylist(_playlist);
            _playerService.SetMedia(arg as Media);
            _playerService.Play();
        }

        public bool CanSelectMedia(object arg)
        {
            return (_playlist.Count > 0);
        }

        public void OnRemoveMedia(object arg)
        {
            _playlist.Remove(arg as Media);
            NotifyPropertyChanged("Playlist");
            NotifyPropertyChanged("Items");
        }

        public bool CanRemoveMedia(object arg)
        {
            return (true);
        }

        public void OnRenamePlaylist(object arg)
        {
            var dialogService = new DialogService();

            string newName = dialogService.InputDialog("Entrez le nouveau nom", "Renommer une playlist");
            while (newName != null && PlaylistsService.Instance.ContainsByName(newName))
            {
                newName = dialogService.InputDialog("Le nom saisi est déjà pris", "Renommer une playlist");
            }

            if (newName != null)
                _playlist.Name = newName;
            NotifyPropertyChanged("Playlist");
        }

        public bool CanRenamePlaylist(object arg)
        {
            return (true);
        }

        public void OnDeletePlaylist(object arg)
        {
            var dialogService = new DialogService();
            string confirmation = dialogService.InputDialog("Entrez \"SUPPRIMER\" pour valider", "Supprimer une playlist");

            if (confirmation == "SUPPRIMER")
            {
                if (_playlist != null)
                    PlaylistsService.Instance.Remove(_playlist);

                if (_nagivationService != null)
                    _nagivationService.GoBack();
            }
        }

        public bool CanDeletePlaylist(object arg)
        {
            return (_playlist != null);
        }
        #endregion

        #region Ctor / Dtor
        public PlaylistVM(Playlist playlist, IPlayerService playerService, INavigationService nagivationService)
        {
            _playlist = playlist;
            _playerService = playerService;
            _nagivationService = nagivationService;
        }
        #endregion

    }
}
