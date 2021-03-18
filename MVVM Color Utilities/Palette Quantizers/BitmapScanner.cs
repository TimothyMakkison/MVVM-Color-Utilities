using System.Collections.Concurrent;
using System.Drawing;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    public class BitmapScanner : IBitmapScanner
    {
        public ConcurrentDictionary<int, int> Scan(Bitmap bitmap)
        {
            Debug.WriteLine($"Scanning bitmap for colors");

            if (bitmap.IsNull())
            {
                return new ConcurrentDictionary<int, int>();
            }

            ConcurrentDictionary<int, int> colorDict = new ConcurrentDictionary<int, int>();

            //Gets the raw pixel data from bitmap and reads each 4 byte segment as a color.
            using (Bitmap lockableBitmap = new Bitmap(bitmap))
            {
                //Get raw bitmap data
                BitmapData bitmapData = lockableBitmap.LockBits(new Rectangle(0, 0, lockableBitmap.Width, lockableBitmap.Height),
                                        ImageLockMode.ReadOnly,
                                        lockableBitmap.PixelFormat);

                IntPtr pixelBytes = bitmapData.Scan0; //Get byte array of every pixel
                byte[] Pixels = new byte[bitmap.Width * bitmap.Height * 4];
                Marshal.Copy(pixelBytes, Pixels, 0, Pixels.Length);

                //Iterate through each 4 byte group calculating the color value
                Parallel.For(0, Pixels.Length / 4, i =>
                {
                    i *= 4;
                    int key = Pixels[i + 2] << 16 | Pixels[i + 1] << 8 | Pixels[i];
                    colorDict.AddOrUpdate(key, 1, (keyValue, value) => value + 1);
                });
            }

            Debug.WriteLine($"ScanBitmap Success, Found {colorDict.Count} unique colors");
            return colorDict;
        }
    }
}
