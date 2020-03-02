﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;
using MVVM_Color_Utilities.Helpers;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    class ImageBuffer : ObservableObject
    {
        #region Fields
        private Bitmap originalBitmap;
        private Bitmap generatedBitmap;

        private ConcurrentDictionary<int, int> bitmapColors;
        private BaseColorQuantizer activeQuantizer;
        private int colorCount;
        #endregion

        #region Properties
        /// <summary>
        /// Bitmap that will be analayzed.
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get => originalBitmap;
            set
            {
                if (SetProperty(ref originalBitmap,value))
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
                if (SetProperty(ref activeQuantizer,value))
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
                if (SetProperty(ref colorCount,value))
                {
                    GeneratedBitmap = null;
                    Palette = new List<Color>();
                }
            }
        }
        #region Dependent Properties
        /// <summary>
        /// Generated bitmap
        /// </summary>
        public Bitmap GeneratedBitmap
        {
            get => Singleton(ref generatedBitmap, GenerateNewBitmap);
            set => SetProperty(ref generatedBitmap,value);
        }

        /// <summary>
        /// Stores all bitmap colors.
        /// </summary>
        public ConcurrentDictionary<int, int> BitmapColors
        {
            get =>  Singleton(ref bitmapColors, bitmapColors.IsNullOrEmpty(), GetBitmapColors);
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
        #endregion

        #endregion

        #region Methods

        #region Scanning bitmap
        /// <summary>
        /// Iterates through OriginalBitmap, adding each color to the ColorList.
        /// </summary>
        /// <returns>Dictionary of OriginalBitmap colors.</returns>
        public ConcurrentDictionary<int, int> GetBitmapColors()
        {
            Debug.WriteLine($"Scanning bitmap for colors");

            if (OriginalBitmap.IsNull())
            {
                return new ConcurrentDictionary<int, int>();
            }

            ConcurrentDictionary<int, int> colorDict = new ConcurrentDictionary<int, int>();

            //Gets the raw pixel data from bitmap and reads each 4 byte segment as a color.
            using (Bitmap lockableBitmap = new Bitmap(OriginalBitmap))
            {
                //Get raw bitmap data 
                BitmapData bitmapData = lockableBitmap.LockBits(new Rectangle(0, 0, lockableBitmap.Width, lockableBitmap.Height),
                                        ImageLockMode.ReadOnly,
                                        lockableBitmap.PixelFormat);

                IntPtr pixelBytes = bitmapData.Scan0; //Get byte array of every pixel
                byte[] Pixels = new byte[OriginalBitmap.Width * OriginalBitmap.Height * 4];
                Marshal.Copy(pixelBytes, Pixels, 0, Pixels.Length);

                //Iterate through each 4 byte group calculating the color value
                Parallel.For(0, Pixels.Length / 4, i =>
                {
                    i *= 4;
                    int key = Pixels[i + 2] << 16 | Pixels[i + 1] << 8 | Pixels[i];
                    colorDict.AddOrUpdate(key, 1, (keyValue, value) => value + 1);
                });
            }

            Debug.WriteLine($"ScanBitmap Success, Found {colorDict.Count.ToString()} unique colors");
            return colorDict;
        }
        #endregion

        #region Generate palette
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
        #endregion

        #region Generate bitmap
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
            var newBitmap = new Bitmap(OriginalBitmap.Width, OriginalBitmap.Height);
            var refresh = Palette; //Call palette (quanitzer will generate palette if current is null or empty)

            for (int x = 0; x < newBitmap.Width; x++)
                for (int y = 0; y < newBitmap.Height; y++)
                {
                    Color pixelColor = OriginalBitmap.GetPixel(x, y);
                    int index = ActiveQuantizer.GetPaletteIndex(pixelColor);
                    newBitmap.SetPixel(x, y, Palette[index]);
                }
            Debug.WriteLine("Success, generated image");

            #region Pixelator
            //int division = 10;

            //Bitmap temp = new Bitmap((GeneratedBitmap.Width / division) - 1, (GeneratedBitmap.Height / division) - 1);

            //for (int x = 0; x < temp.Width; x++)
            //{
            //    for (int y = 0; y < temp.Height; y++)
            //    {
            //        temp.SetPixel(x, y, Sample(x * division, y * division));
            //    }
            //    Debug.WriteLine(x);
            //}

            //GeneratedBitmap = temp;

            //Debug.WriteLine("done");

            //Color Sample(int x, int y)
            //{
            //    ConcurrentDictionary<Color, int> color = new ConcurrentDictionary<Color, int>();

            //    for (int dx = x; dx < x + division; dx++)
            //    {
            //        for (int dy = y; dy < y + division; dy++)
            //        {
            //            color.AddOrUpdate(GeneratedBitmap.GetPixel(dx, dy), 1, (keyValue, value) => value + 1);
            //        }
            //    }
            //    return color.Aggregate((z, m) => z.Value > m.Value ? z : m).Key;
            //}
            #endregion

            return newBitmap;
        }
        #endregion

        #region Save bitmap
        /// <summary>
        /// Save generated image to location and with given type.
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="format">Image Format</param>
        /// <returns>Bool of success of operation</returns>
        public bool SaveGeneratedImage(string path, ImageFormat format)
        {
            try
            {
                GeneratedBitmap.Save(path, format);
                return true;
            }
            catch
            {
                Debug.WriteLine("Failed saving image to "+path);
                return false;
            }
        }
        #endregion

        #endregion
    }
}
