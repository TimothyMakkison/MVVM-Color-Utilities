﻿using MVVM_Color_Utilities.Helpers;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    public class BitmapScanner : IBitmapScanner
    {
        private int _hash;
        private ConcurrentDictionary<int, int> _dict;

        public ConcurrentDictionary<int, int> Scan(Bitmap bitmap)
        {
            var hash = bitmap.GetHashCode();
            var cond = hash == _hash;

            _hash = hash;
            return cond
                ? _dict
                : (_dict = GetColors(bitmap));
        }

        private ConcurrentDictionary<int, int> GetColors(Bitmap bitmap)
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