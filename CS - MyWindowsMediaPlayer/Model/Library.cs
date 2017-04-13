using MyWindowsMediaPlayer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyWindowsMediaPlayer.Model
{
    [Serializable]
    class Library : PropertyChangedBase
    {
        #region Attributes
        protected List<Uri> _folders = new List<Uri>();
        protected List<Media> _items = null;
        protected List<string> _extensions = new List<string>();
        #endregion

        #region Properties
        public Type MediaType
        {
            get; set;
        }

        public List<Uri> Folders
        {
            get { return (_folders); }
            set
            {
                _folders = value;
                LoadItems();
            }
        }

        public List<Music> ItemsAsMusic
        {
            get
            {
                if (MediaType != typeof(Music))
                    throw new NotImplementedException();

                return (_items.Cast<Music>().ToList());
            }
        }

        public List<Video> ItemsAsVideo
        {
            get
            {
                if (MediaType != typeof(Video))
                    throw new NotImplementedException();

                return (_items.Cast<Video>().ToList());
            }
        }

        public List<Image> ItemsAsImage
        {
            get
            {
                if (MediaType != typeof(Image))
                    throw new NotImplementedException();

                return (_items.Cast<Image>().ToList());
            }
        }

        public List<Media> Items
        {
            get
            {
                return (_items);
            }
        }

        public List<string> Extensions
        {
            get { return (_extensions); }
            set
            {
                _extensions = value;
                LoadItems();
            }
        }
        #endregion

        #region Ctor / Dtor
        public Library()
        {
        }

        public Library(List<Uri> folders)
        {
            _folders.AddRange(folders);
        }
        #endregion

        #region Methods
        public void LoadOnce()
        {
            if (_items == null)
                LoadItems();
        }

        public void LoadItems()
        {
            _items = new List<Media>();
            var items = new List<string>();

            foreach (var folder in _folders)
            {
                try
                {
                    if (Directory.Exists(folder.LocalPath))
                        items.AddRange(Directory.GetFiles(folder.LocalPath, "*.*").Where(s => Extensions.Any(e => s.EndsWith(e))));
                } catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to load media from folder \"" + folder + "\": " + e.ToString());
                }
            }

            var medias = new List<Media>();
            foreach (var item in items)
            {
                Uri path = new Uri(item);

                var existingMedias = _items.Where(i => i.Path == path);
                if (existingMedias.Count() == 0)
                    medias.Add(Media.Factory.make(item));
                else
                    medias.AddRange(existingMedias);
            }
            _items = medias;
            
            OnPropertyChanged("Items");
            OnPropertyChanged("ItemsAsMusic");
            OnPropertyChanged("ItemsAsVideo");
            OnPropertyChanged("ItemsAsImage");
        }

        public void AddFolder(Uri folder)
        {
            if (!_folders.Contains(folder))
            {
                _folders.Add(folder);
                LoadItems();
                OnPropertyChanged("Folders");
            }
        }

        public void RemoveFolder(Uri folder)
        {
            if (_folders.Contains(folder))
            {
                _folders.Remove(folder);
                LoadItems();
                OnPropertyChanged("Folders");
            }
        }
        #endregion
    }
}
