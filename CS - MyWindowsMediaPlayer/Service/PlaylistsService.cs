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
    class PlaylistsService : List<Playlist>
    {
        #region Singleton
        static PlaylistsService _instance = new PlaylistsService();
        static List<string> _importedFiles = new List<string>();

        private PlaylistsService(){}

        static public PlaylistsService Instance
        {
            get { return (_instance); }
        }
        #endregion

        #region Properties
        public List<string> Names
        {
            get
            {
                return (this.Select(x => x.Name).ToList());
            }
        }
        #endregion

        #region Methods
        public Playlist FindByName(string name)
        {
            var matches = this.Where(p => p.Name == name);

            return (matches.Count() > 0 ? matches.First() : null);
        }

        public bool ContainsByName(string name)
        {
            string tmpName = name.Trim().ToLower();

            var matches = this.Where(p => p.Name.ToLower() == tmpName);

            return (matches.Count() > 0);
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

            var names = from i in xdoc.Descendants("playlist")
                        select new
                        {
                            Path = (string)i.Attribute("name")
                        };

            var paths1 = xdoc.Descendants("playlist")
                            .SelectMany(x => x.Descendants("media"), (pl, media) => Tuple.Create(pl.Attribute("name").Value, media.Attribute("path").Value))
                            .GroupBy(x => x.Item1)
                            .ToList();

            int inc = 0;

            foreach (var name1 in names)
            {
                this.Add(new Playlist(name1.Path));
                foreach (var name in paths1.Where(x => x.Key == name1.Path))
                {
                    foreach (var tuple in name)
                    {
                        if (tuple.Item2.Length > 0)
                        {
                            this[inc].Add(Media.Factory.make(tuple.Item2));
                        }
                    }
                }
                inc++;
            }
        }

        public void Export(string file)
        {
            System.IO.StreamWriter outFile = new System.IO.StreamWriter(file);

            XElement save = new XElement("playlists");
            String media = "";
            String path = "";
            foreach (var item in this)
            {
                XElement playlistName = new XElement("playlist");
                media = item.Name;
                playlistName.SetAttributeValue("name", media);
                foreach (var elem in item)
                {
                    XElement xml = new XElement("media");
                    path = elem.Path.LocalPath;
                    xml.SetAttributeValue("path", path);
                    playlistName.Add(xml);
                }
                save.Add(playlistName);
            }
            outFile.WriteLine(save);
            outFile.Close();
        }

        private void LoadDefault(string file)
        {
            if (!Directory.Exists(Path.GetDirectoryName(file)))
                Directory.CreateDirectory(Path.GetDirectoryName(file));

            Export(file);
        }
        #endregion
    }
}
