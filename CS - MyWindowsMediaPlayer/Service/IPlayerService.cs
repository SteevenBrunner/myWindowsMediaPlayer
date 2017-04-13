using MyWindowsMediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer.Service
{
    interface IPlayerService
    {
        void SetMedia(Media media);
        void SetPlaylist(Playlist playlist);
        void Play();
    }
}
