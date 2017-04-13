using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using NReco.VideoConverter;
using System.IO;
using System.Drawing;
using System.Windows.Media;
using System.Drawing.Imaging;

namespace MyWindowsMediaPlayer.Model
{
    [Serializable]
    class Video : Media
    {
        #region Attributes
        #endregion

        #region Properties
        public static List<string> Extensions
        {
            get
            {
                return (new List<string>() { "mp4", "mpeg", "avi", "wmv" });
            }
        }

        public override BitmapImage Thumbnail
        {
            get
            {
                if (_thumbnail == null && File.Exists(_path.LocalPath))
                {
                    FFMpegConverter ff = new FFMpegConverter();
                    MemoryStream imgStream = new MemoryStream();
                    ff.GetVideoThumbnail(_path.LocalPath, imgStream, (float)(Duration.TotalSeconds / 2.0));

                    Bitmap bmp = (Bitmap)Bitmap.FromStream(imgStream);

                    MemoryStream memory = new MemoryStream();
                    bmp.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    _thumbnail = bitmapImage;
                }

                return (_thumbnail);
            }
        }

        public override string Information
        {
            get { return (null); }
        }
        #endregion

        #region Ctor / Dtor
        public Video() : base()
        {
        }

        public Video(string path)
            : base(path)
        {
        }
        #endregion

        #region Methods
        #endregion
    }
}
