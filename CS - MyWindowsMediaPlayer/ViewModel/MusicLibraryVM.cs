using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Service;
using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer.ViewModel
{
    class MusicLibraryVM : LibraryVM
    {

        public MusicLibraryVM(IWindowService windowService, IPlayerService playerService)
            : base(windowService, playerService)
        {
            _library = LibrariesService.Instance.FindByType(typeof(Music));
            if (_library != null)
            {
                _library.Extensions = Music.Extensions;
                _library.LoadOnce();
            }
            //_library = new Library<Music>();
            //_library.Folders = new List<Uri>() { new Uri(@"E:\Projets\CS - MyWindowsMediaPlayer\Example Medias\"), new Uri(@"E:\C# - MyWindowsMediaPlayer\Example Medias") };
        }

        public override void OnManageLibrary(object arg)
        {
            if (_windowService != null)
                _windowService.CreateWindow(new LibraryManagementVM(_library));
        }
    }
}
