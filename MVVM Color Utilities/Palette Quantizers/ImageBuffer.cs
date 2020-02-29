using System;
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

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    class ImageBuffer 
    {
        #region Fields
        private Bitmap originalBitmap;
        private ConcurrentDictionary<int, int> bitmapColors;
        private BaseColorQuantizer activeQuantizer;
        private int colorCount;
        private List<Color> palette;
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
                Debug.WriteLine("Bitmap set, size is " + value.Width + "x" + value.Height +
                    " with " + value.Width * value.Height + " pixels");
                if (originalBitmap != value)
                {
                    originalBitmap = value;
                    BitmapColors.Clear();
                    Palette.Clear();
                    GeneratedBitmap = null;
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
                if (activeQuantizer != value)
                {
                    activeQuantizer = value;
                    Palette.Clear();
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
                if (colorCount != value)
                {
                    colorCount = value;
                    GeneratedBitmap = null;
                    Palette.Clear();
                }
            }
        }
        #region Dependant Properties
        private Bitmap generatedBitmap;
        /// <summary>
        /// Generated bitmap
        /// </summary>
        public Bitmap GeneratedBitmap
        {
            get
            {
                if (generatedBitmap.IsNull())
                {
                    generatedBitmap = GenerateNewImage();
                }
                return generatedBitmap;
            }
            set => generatedBitmap = value;
        }
        ///// <summary>
        ///// Stores all of the colors in the bitmap.
        ///// </summary>
        //public ConcurrentDictionary<int, int> ColorDictionary { get; private set; } = new ConcurrentDictionary<int, int>();
        public ConcurrentDictionary<int, int> BitmapColors
        {
            get
            {
                if (bitmapColors == null || bitmapColors.IsEmpty)
                {
                    bitmapColors = GetBitmapColors();
                }
                return bitmapColors;
            }
        }
        /// <summary>
        /// Returns the generated palette
        /// </summary>
        public List<Color> Palette
        {
            get
            {
                if (palette.IsNullOrEmpty())
                {
                    palette = GetPalette();
                }
                return palette;
            }
        }
       
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Iterates through OriginalBitmap, adding each color to the ColorList.
        /// </summary>
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
        /// <summary>
        /// Generates a new Palette.
        /// </summary>
        /// <returns>Returns success of operation</returns>
        public List<Color> GetPalette()
        {
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
        /// <returns>Returns success of operation.</returns>
        public Bitmap GenerateNewImage()
        {
            Debug.WriteLine("Generating new image");
            if (OriginalBitmap.IsNull())
            {
                return null;
            }
            var newBitmap = new Bitmap(OriginalBitmap.Width, OriginalBitmap.Height);
            var n= Palette;

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
        protected T Singleton<T>(ref T storage, Func<T> func)
        {
            if (storage.IsNull())
            {
                storage = func.Invoke();
            }
            return storage;
        }
        #endregion
    }
}
