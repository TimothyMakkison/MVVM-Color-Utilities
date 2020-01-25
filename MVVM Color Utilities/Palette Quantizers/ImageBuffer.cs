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

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    class ImageBuffer 
    {
        #region Fields
        private Bitmap _originalBitmap;
        private readonly Bitmap _defaultBitmap = new Bitmap(1, 1);
        private BaseColorQuantizer _activeQuantizer;
        private Int32 _colorCount;
        #endregion

        #region Properties
        /// <summary>
        /// Bitmap that will be analayzed.
        /// </summary>
        public Bitmap OriginalBitmap
        {
            get => _originalBitmap;
            set
            {
                Debug.WriteLine("Bitmap set, size is " + value.Width + "x" + value.Height +
                    " with " + value.Width * value.Height + " pixels");
                if (_originalBitmap != value)
                {
                    _originalBitmap = value;
                    ColorDictionary.Clear();
                    Palette = new List<Color>();
                    GeneratedBitmap = _defaultBitmap;
                }
            }
        }
        /// <summary>
        /// Currently selected quantizer.
        /// </summary>
        public BaseColorQuantizer ActiveQuantizer
        {
            get => _activeQuantizer;
            set
            {
                if (_activeQuantizer != value)
                {
                    _activeQuantizer = value;
                    Palette = new List<Color>();
                    GeneratedBitmap = _defaultBitmap;
                }
            }
        }
        /// <summary>
        /// Number of colors in generated palette.
        /// </summary>
        public int ColorCount
        {
            get => _colorCount;
            set
            {
                if (_colorCount != value)
                {
                    _colorCount = value;
                    Palette = new List<Color>();
                    GeneratedBitmap = _defaultBitmap;
                }
            }
        }
        #region Dependant Properties
        /// <summary>
        /// Stores all of the colors in the bitmap.
        /// </summary>
        public ConcurrentDictionary<int, int> ColorDictionary { get; private set; } = new ConcurrentDictionary<int, int>();
        /// <summary>
        /// Returns the generated palette
        /// </summary>
        public List<Color> Palette { get; private set; } = new List<Color>();
        /// <summary>
        /// Generated bitmap
        /// </summary>
        public Bitmap GeneratedBitmap { get; private set; }
        #endregion

        #endregion

        #region Methods
        /// <summary>
        /// Iterates through OriginalBitmap, adding each color to the ColorList.
        /// </summary>
        public bool ScanBitmapColors()
        {
            if (!ColorDictionary.IsNullOrEmpty())//Checks to see it hasn't been scanned.
            {
                Debug.WriteLine("Success, ColorDictionary already generated");
                return true;
            }
            else if(!OriginalBitmap.IsNull("OriginalBitmap"))
            {
                //Iterates through each pixel adding it to the colorList
                ConcurrentDictionary<int, int> newColorDict = new ConcurrentDictionary<int, int>();

                for (int x = 0; x < OriginalBitmap.Width; x++)
                    for (int y = 0; y < OriginalBitmap.Height; y++)
                    {
                        Color pixelColor = OriginalBitmap.GetPixel(x, y);
                        Int32 key = pixelColor.R << 16 | pixelColor.G << 8 | pixelColor.B;
                        newColorDict.AddOrUpdate(key, 1, (keyValue, value) => value + 1);
                    }
                ColorDictionary = newColorDict;
                Debug.WriteLine("ScanBitmap Success, Found " + newColorDict.Count.ToString() + " colors");
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Generates a new Palette.
        /// </summary>
        /// <returns>Returns success of operation</returns>
        public bool GetPalette()
        {
            if(!Palette.IsNullOrEmpty())//Checks that palette hasn't been made alreay.
            {
                Debug.WriteLine("Success, Palette of size " + Palette.Count+ " already generated");
                return true;
            }
            else if (!ColorDictionary.IsNullOrEmpty("ColorDictionary") && !ActiveQuantizer.IsNull("ActiveQuantizer")
                && !ColorCount.IsEmpty("ColorCount"))
            {
                Palette = _activeQuantizer.GetPalette(ColorCount,ColorDictionary);
                Debug.WriteLine("Success, Generated palette of " + Palette.Count+" unique colors");
                return true;
            }
            else
            {
                Debug.WriteLine("Fail, cannot generate palette");
                return false;
            }
        }
        /// <summary>
        /// Uses the CurrentBitmap and Palette to generate an approximate image.
        /// </summary>
        /// <returns>Returns success of operation.</returns>
        public bool GenerateNewImage()
        {
            if(GeneratedBitmap != _defaultBitmap)//Checks that GeneratedBitmap hasn't been formed yet.
            {
                Debug.WriteLine("Success, bitmap already generated");
                return true;
            }
            else if (!OriginalBitmap.IsNull("OrigninalBitmap") && !Palette.IsNullOrEmpty("Palette") 
                && !ActiveQuantizer.IsNull("ActiveQuantizer"))
            {
                GeneratedBitmap = new Bitmap(OriginalBitmap.Width, OriginalBitmap.Height);

                for (int x = 0; x < OriginalBitmap.Width; x++)
                    for (int y = 0; y < OriginalBitmap.Height; y++)
                    {
                        Color pixelColor = OriginalBitmap.GetPixel(x, y);
                        int index = ActiveQuantizer.GetPaletteIndex(pixelColor);
                        GeneratedBitmap.SetPixel(x, y, Palette[index]);
                    }
                Debug.WriteLine("Success, generated image");
                return true;
            }
            else
            {
                Debug.WriteLine("Fail, Either OriginalBitmap or Palette is null");
                return false;
            }
        }
        /// <summary>
        /// Save generated image to location and with given type.
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="format">Image Format</param>
        /// <returns>Bool of success of operation</returns>
        public bool SaveGeneratedImage(string path, System.Drawing.Imaging.ImageFormat format)
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
    }
}
