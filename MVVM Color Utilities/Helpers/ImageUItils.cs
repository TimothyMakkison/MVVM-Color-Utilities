using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace MVVM_Color_Utilities.Helpers
{
    /// <summary>
    /// Contains useful converters for images.
    /// </summary>
    public static class ImageUtils
    {
        /// <summary>
        /// Converts a Bitamp into a BitmapImage.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage ConvertToBitmapImage(this Bitmap bitmap)
        {
            using var memory = new MemoryStream();
            bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();

            return bitmapImage;
        }

        public static Bitmap ToBitmap(this BitmapImage image)
        {
            using MemoryStream outStream = new();

            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(image));
            enc.Save(outStream);
            Bitmap bitmap = new(outStream);

            return new Bitmap(bitmap);
        }

        /// <summary>
        /// Save generated image to location and with given format.
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="format">Image Format</param>
        /// <returns>Bool of success of operation</returns>
        public static bool SaveImage(this Bitmap bitmap, string path, ImageFormat format)
        {
            try
            {
                bitmap.Save(path, format);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}