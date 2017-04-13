using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyWindowsMediaPlayer.Model
{
    [Serializable]
    class Music : Media
    {
        #region Attributes
        #endregion

        #region Properties
        public static List<string> Extensions
        {
            get
            {
                return (new List<string>() { "mp3", "wav", "wma" });
            }
        }

        public override string Name
        {
            get
            {
                try
                {
                    if (File.Exists(_path.LocalPath))
                    {
                        var file = TagLib.File.Create(_path.LocalPath);
                        if (!string.IsNullOrEmpty(file.Tag.Title))
                            return (file.Tag.Title);
                        else
                            return (base.Name);
                    }
                    else
                        return (base.Name);
                } catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to load media informations: " + e.Message);
                    return (base.Name);
                }
            }
        }

        public override BitmapImage Thumbnail
        {
            get
            {
                try
                {
                    if (_thumbnail == null)
                    {
                        if (File.Exists(_path.LocalPath))
                        {
                            var file = TagLib.File.Create(_path.LocalPath);
                            if (file.Tag.Pictures.Length >= 1)
                            {
                                var bin = (byte[])(file.Tag.Pictures[0].Data.Data);
                                System.Drawing.Image bmp = System.Drawing.Image.FromStream(new MemoryStream(bin)).GetThumbnailImage(100, 100, null, IntPtr.Zero);
                                MemoryStream ms = new MemoryStream();
                                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                ms.Position = 0;
                                BitmapImage bi = new BitmapImage();
                                bi.BeginInit();
                                bi.StreamSource = ms;
                                bi.EndInit();

                                _thumbnail = bi;
                            }
                        }
                    }

                    return (_thumbnail);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to load thumbnail image: " + e.Message);
                    return (null);
                }
            }
        }

        public string Artists
        {
            get
            {
                try
                {
                    if (File.Exists(_path.LocalPath))
                    {
                        var file = TagLib.File.Create(_path.LocalPath);
                        if (file.Tag.Performers.Length > 0)
                            return (string.Join(", ", file.Tag.Performers));
                        else if (file.Tag.Artists.Length > 0)
                            return (string.Join(", ", file.Tag.Artists));
                        else if (file.Tag.AlbumArtists.Length > 0)
                            return (string.Join(", ", file.Tag.AlbumArtists));
                        else
                            return (null);
                    }
                    return (null);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to load artist: " + e.Message);
                    return (null);
                }
            }
        }

        public string Album
        {
            get
            {
                try
                {
                    if (File.Exists(_path.LocalPath))
                    {
                        var file = TagLib.File.Create(_path.LocalPath);
                        return (file.Tag.Album);
                    }
                    else
                        return (null);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to load album: " + e.Message);
                    return (null);
                }
            }
        }

        public override string Information
        {
            get
            {
                try
                {
                    if (File.Exists(_path.LocalPath))
                    {
                        var file = TagLib.File.Create(_path.LocalPath);
                        if (file.Tag.Artists.Length > 0)
                            return (file.Tag.Album + " - " + string.Join(", ", file.Tag.Artists));
                        else
                            return (file.Tag.Album);
                    }
                    else
                        return (null);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Unable to load informations: " + e.Message);
                    return (null);
                }
            }
        }
        #endregion

        #region Ctor / Dtor
        public Music() : base()
        {
        }

        public Music(string path)
            : base(path)
        {
        }
        #endregion

        #region Methods
        #endregion
    }
}
