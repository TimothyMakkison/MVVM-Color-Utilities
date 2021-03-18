﻿using MVVM_Color_Utilities.Helpers;
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
    internal class ImageBuffer : ObservableObject, IImageBuffer
    {
        private Bitmap originalBitmap;
        private Bitmap generatedBitmap;

        private ConcurrentDictionary<int, int> bitmapColors;
        private BaseColorQuantizer activeQuantizer;
        private int colorCount;
        private readonly IBitmapScanner _bitmapScanner;

        public ImageBuffer(IBitmapScanner bitmapScanner)
        {
            this._bitmapScanner = bitmapScanner;
        }

        public ImageBuffer() : this(new BitmapScanner())
        {
        }

        /// <summary>
        /// Bitmap that will be analayzed.
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get => originalBitmap;
            set
            {
                if (Set(ref originalBitmap, value))
                {
                    BitmapColors = new ConcurrentDictionary<int, int>();
                    Palette = new List<Color>();
                    GeneratedBitmap = null;
                    Debug.WriteLine($"Bitmap set, size is {value.Width}x{value.Height} " +
                        $"with {value.Width * value.Height} pixels");
                }
            }
        }

        /// <summary>
        /// Currently selected quantizer.
        /// </summary>
        public BaseColorQuantizer ActiveQuantizer
        {
            get => activeQuantizer;
            set
            {
                if (Set(ref activeQuantizer, value))
                {
                    Palette = new List<Color>();
                    GeneratedBitmap = null;
                }
            }
        }

        /// <summary>
        /// Number of colors in generated palette.
        /// </summary>
        public int ColorCount
        {
            get => colorCount;
            set
            {
                if (Set(ref colorCount, value))
                {
                    GeneratedBitmap = null;
                    Palette = new List<Color>();
                }
            }
        }

        /// <summary>
        /// Generated bitmap
        /// </summary>
        public Bitmap GeneratedBitmap
        {
            get => PatternHandler.Singleton(ref generatedBitmap, GenerateNewBitmap);
            set => Set(ref generatedBitmap, value);
        }

        /// <summary>
        /// Stores all bitmap colors.
        /// </summary>
        public ConcurrentDictionary<int, int> BitmapColors
        {
            get
            {
                return PatternHandler.Singleton(ref bitmapColors, bitmapColors.IsNullOrEmpty(), () => _bitmapScanner.Scan(OriginalBitmap));
            }

            set => bitmapColors = value;
        }

        /// <summary>
        /// Returns the generated palette
        /// </summary>
        public List<Color> Palette
        {
            get
            {
                if (ActiveQuantizer.Palette.IsNullOrEmpty())
                {
                    GetPalette();
                }
                return ActiveQuantizer.Palette;
            }
            set => ActiveQuantizer.Palette = value;
        }

        /// <summary>
        /// Generates a new Palette.
        /// </summary>
        /// <returns>Color palette.</returns>
        public List<Color> GetPalette()
        {
            Debug.WriteLine("Getting palette");
            if (BitmapColors.IsNullOrEmpty())
            {
                Debug.WriteLine("Get palette returning null values");

                return new List<Color>();
            }
            var palette = activeQuantizer.GetPalette(ColorCount, BitmapColors);
            Debug.WriteLine("Success, Generated palette of " + palette.Count + " unique colors");
            return palette;
        }

        /// <summary>
        /// Uses the CurrentBitmap and Palette to generate an approximate image.
        /// </summary>
        /// <returns>Generated bitmap.</returns>
        public Bitmap GenerateNewBitmap()
        {
            Debug.WriteLine("Generating new image");
            if (OriginalBitmap.IsNull())
            {
                return null;
            }

            if (ActiveQuantizer.Palette.IsNullOrEmpty())
            {
                Debug.WriteLine("\n getting pal \n");
                Palette = GetPalette();
            }

            Bitmap lockableBitmap = new Bitmap(OriginalBitmap);
            //Get raw bitmap data
            BitmapData bitmapData = lockableBitmap.LockBits(new Rectangle(0, 0, lockableBitmap.Width, lockableBitmap.Height),
                                    ImageLockMode.ReadWrite,
                                    lockableBitmap.PixelFormat);

            IntPtr ptr = bitmapData.Scan0; //Get address of first line
            byte[] rgbBytes = new byte[OriginalBitmap.Width * OriginalBitmap.Height * 4];
            Marshal.Copy(ptr, rgbBytes, 0, rgbBytes.Length);

            //Iterate through each 4 byte group calculating the color value
            Parallel.For(0, rgbBytes.Length / 4, i =>
            {
                i *= 4;
                var color = Color.FromArgb(rgbBytes[i + 2], rgbBytes[i + 1], rgbBytes[i]);
                int index = ActiveQuantizer.GetPaletteIndex(color);
                var newColor = Palette[index];
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