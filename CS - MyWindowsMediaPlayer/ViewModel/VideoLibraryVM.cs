using MyWindowsMediaPlayer.Model;
using MyWindowsMediaPlayer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer.ViewModel
{
    class VideoLibraryVM : LibraryVM
    {
        public VideoLibraryVM(IWindowService windowService, IPlayerService playerService)
            : base(windowService, playerService)
        {
            _library = LibrariesService.Instance.FindByType(typeof(Video));
            if (_library != null)
            {
                _library.Extensions = Video.Extensions;
                _library.LoadOnce();
            }
        }

        public override void OnManageLibrary(object arg)
        {
            if (_windowService != null)
                _windowService.CreateWindow(new LibraryManagementVM(_library));
        }
    }
}
