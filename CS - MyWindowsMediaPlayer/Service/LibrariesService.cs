using MyWindowsMediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MyWindowsMediaPlayer.Service
{
    class LibrariesService : List<Library>
    {
        #region Singleton
        static LibrariesService _instance = new LibrariesService();
        static List<string> _importedFiles = new List<string>();

        private LibrariesService() { }

        static public LibrariesService Instance
        {
            get { return (_instance); }
        }
        #endregion

        #region Methods
        public Library FindByType(Type type)
        {
            var matches = this.Where(p => p.MediaType == type);

            return (matches.Count() > 0 ? matches.First() : null);
        }

        public void ImportOnce(string file)
        {
            if (!_importedFiles.Contains(file))
            {
                _importedFiles.Add(file);
                this.Import(file);
            }
        }

        public void Import(string file)
        {
            if (!File.Exists(file))
            {
                LoadDefault(file);
                return;
            }

            var xdoc = XDocument.Load(file);

            var names = from i in xdoc.Descendants("library")
                        select new
                        {
                            Type = (string)i.Attribute("type")
                        };

            var paths1 = xdoc.Descendants("library")
                            .SelectMany(x => x.Descendants("folder"), (pl, media) => Tuple.Create(pl.Attribute("type").Value, media.Attribute("path").Value))
                            .GroupBy(x => x.Item1)
                            .ToList();

            foreach (var libtype in names)
            {
                List<Uri> pathList = new List<Uri>();
                foreach (var name in paths1.Where(x => x.Key == libtype.Type))
                {
                    foreach (var tuple in name)
                    {
                        if (tuple.Item2.Length > 0)
                        {
                            Uri myUri = new Uri(tuple.Item2);
                            pathList.Add(myUri);
                        }
                    }
                }

                Library library = new Library();
                library.Folders = pathList;
                library.MediaType = Type.GetType(libtype.Type);
                this.Add(library);
            }
        }

        public void Export(string file)
        {
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(file);

            XElement libSave = new XElement("libraries");
            String type = "";
            String dir = "";
            foreach (var item in this)
            {
                XElement libType = new XElement("library");
                type = item.MediaType.ToString();
                libType.SetAttributeValue("type", type);
                foreach (var elem in item.Folders)
                {
                    XElement xml = new XElement("folder");
                    dir = elem.LocalPath;
                    xml.SetAttributeValue("path", dir);
                    libType.Add(xml);
                }
                libSave.Add(libType);
            }
            outFile.WriteLine(libSave);
            outFile.Close();
        }

        private void LoadDefault(string file)
        {
            Library musics = new Library()
            {
                MediaType = typeof(Music),
                Folders = new List<Uri>() {
                    new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic))
                }
            };
            Library videos = new Library()
            {
                MediaType = typeof(Video),
                Folders = new List<Uri>() {
                    new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))
                }
            };
            Library images = new Library()
            {
                MediaType = typeof(Image),
                Folders = new List<Uri>() {
                    new Uri(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
                }
            };

            this.Add(musics);
            this.Add(videos);
            this.Add(images);

            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));

            Export(file);
        }
        #endregion
    }
}
