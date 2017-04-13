using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MyWindowsMediaPlayer.Model
{
    abstract class Media
    {
        #region Factory
        public static class Factory
        {
            private static Dictionary<string, Func<string, Media>> _extensions = new Dictionary<string, Func<string, Media>>
            {
                { "mp4", ((string path) => new Video(path)) },
                { "mpeg", ((string path) => new Video(path)) },
                { "avi", ((string path) => new Video(path)) },
                { "wmv", ((string path) => new Video(path)) },
                { "mp3", ((string path) => new Music(path)) },
                { "wav", ((string path) => new Music(path)) },
                { "wma", ((string path) => new Music(path)) },
                { "jpg", ((string path) => new Image(path)) },
                { "png", ((string path) => new Image(path)) },
            };

            public static Media make(string path)
            {
                if (string.IsNullOrEmpty(path))
                    return (null);

                string extension = System.IO.Path.GetExtension(path).Substring(1);

                if (!_extensions.ContainsKey(extension))
                    return (null);

                return (_extensions[extension].Invoke(path));
            }
        }
        #endregion

        #region Attributes
        protected Uri _path;
        protected string _name;
        protected MediaState _state;
        protected BitmapImage _thumbnail = null;
        #endregion

        #region Properties
        public TimeSpan Position
        {
            get; set;
        }
        public TimeSpan Duration
        {
            get; set;
        }

        public Uri Path
        {
            get { return (_path); }
            set
            {
                _path = value;
                this.parseName();
            }
        }

        public virtual string Name
        {
            get { return (_name); }
        }

        public MediaState State
        {
            get { return (_state); }
            set { _state = value; }
        }

        public virtual BitmapImage Thumbnail
        {
            get { throw new NotImplementedException(); }
        }

        public virtual String Information
        {
            get { throw new NotImplementedException(); }
        }

        public bool FileExists
        {
            get
            {
                return (File.Exists(_path.LocalPath));
            }
        }

        public bool Tweeted
        {
            get; set;
        }
        #endregion

        #region Ctor / Dtor
        public Media()
        {
            _path = null;
            _state = MediaState.Stop;

            Tweeted = false;
        }

        public Media(string path)
        {
            _path = new Uri(path);
            _state = MediaState.Stop;

            this.parseName();

            Tweeted = false;
        }
        #endregion

        #region Methods
        private void parseName()
        {
            _name = System.IO.Path.GetFileNameWithoutExtension(_path.ToString());
        }
        #endregion
    }
}
