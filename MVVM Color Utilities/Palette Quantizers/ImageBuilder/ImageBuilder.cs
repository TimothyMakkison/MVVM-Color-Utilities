using MVVM_Color_Utilities.Helpers;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    public class ImageBuilder : IImageBuilder
    {
        public Bitmap BuildBitmap(Bitmap original, Func<Color, Color> func)
        {
            Debug.WriteLine("Generating new image");
            if (original.IsNull())
            {
                return null;
            }

            Bitmap lockableBitmap = new Bitmap(original);
            //Get raw bitmap data
            BitmapData bitmapData = lockableBitmap.LockBits(new Rectangle(0, 0, lockableBitmap.Width, lockableBitmap.Height),
                                    ImageLockMode.ReadWrite,
                                    lockableBitmap.PixelFormat);

            IntPtr ptr = bitmapData.Scan0; //Get address of first line
            byte[] rgbBytes = new byte[original.Width * original.Height * 4];
            Marshal.Copy(ptr, rgbBytes, 0, rgbBytes.Length);

            //Iterate through each 4 byte group calculating the color value
            Parallel.For(0, rgbBytes.Length / 4, i =>
            {
                i *= 4;
                var color = Color.FromArgb(rgbBytes[i + 2], rgbBytes[i + 1], rgbBytes[i]);
                var newColor = func(color);
                rgbBytes[i + 2] = newColor.R;
                rgbBytes[i + 1] = newColor.G;
                rgbBytes[i] = newColor.B;
            });

            Marshal.Copy(rgbBytes, 0, ptr, rgbBytes.Length);

            // Unlock the bits.
            lockableBitmap.UnlockBits(bitmapData);

            return lockableBitmap;
        }
    }
}