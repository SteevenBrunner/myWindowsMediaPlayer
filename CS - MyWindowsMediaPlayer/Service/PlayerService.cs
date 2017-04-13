using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyWindowsMediaPlayer.Service
{
    class PlayerService : IPlayerService
    {
        private MainWindowVM _mainWindow;

        public void SetMedia(Media media)
        {
            _mainWindow.CurrentMedia = media;
        }

        public void SetPlaylist(Playlist playlist)
        {
            _mainWindow.CurrentPlaylist = playlist;
        }

        public void Play()
        {
            _mainWindow.OnPlay(null);
        }

        public PlayerService(MainWindowVM mainWindow)
        {
            _mainWindow = mainWindow;
        }
    }
}
