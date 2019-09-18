using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using System.Windows;
using System.IO;
using System.Collections.Concurrent;
using System.Diagnostics;

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
            get
            {
                return _originalBitmap;
            }
            set
            {
                if(_originalBitmap != value)
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
            get
            {
                return _activeQuantizer;
            }
            set
            {
                if(_activeQuantizer != value)
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
            get
            {
                return _colorCount;
            }
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
        public void ScanBitmapColors()
        {
            #region WriteToDebug
            Debug.Write("Getting Bitmap Colors -> ");
            if(OriginalBitmap == null)
            {
                Debug.WriteLine("Fail, Original Bitmap is null");
            }
            else if(ColorDictionary.Count> 0)
            {
                Debug.WriteLine("Success, ColorList already generated");
            }
            #endregion
            else
            {
                //Iterates through each pixel adding it to the colorList
                ConcurrentDictionary<int, int> newColorList = new ConcurrentDictionary<int, int>();

                for (int x = 0; x < OriginalBitmap.Width; x++)
                    for (int y = 0; y < OriginalBitmap.Height; y++)
                    {
                        Color pixelColor = OriginalBitmap.GetPixel(x, y);
                        Int32 key = pixelColor.R << 16 | pixelColor.G << 8 | pixelColor.B;
                        newColorList.AddOrUpdate(key, 1, (keyValue, value) => value + 1);
                    }
                ColorDictionary = newColorList;
                Debug.WriteLine("Success, Found "+newColorList.Count.ToString()+" colors");
            }
        }
        /// <summary>
        /// Generates a new Palette.
        /// </summary>
        /// <returns>Returns success of operation</returns>
        public bool GetPalette()
        {
            Debug.Write("Getting palette -> ");
            if(Palette.Count > 0)
            {
                Debug.WriteLine("palette " + Palette[0].ToString());
                Debug.WriteLine("Success, Palette of size "+ Palette.Count+ " already generated");

                return true;
            }
            else if (ColorDictionary.Count > 0  && ActiveQuantizer!= null && ColorCount>0)
            {
                _activeQuantizer.SetColorList(ColorDictionary);
                Palette = _activeQuantizer.GetPalette(ColorCount);
                Debug.WriteLine("Success, Generated palette of "+ Palette.Count+" colors");
                return true;
            }
            else
            {
                Debug.WriteLine("Fail, Either ColorList, ColorCount or ActiveQuantizer were null");
                return false;
            }
        }
        /// <summary>
        /// Uses the currentBitmap and Palette to generate a approximate image.
        /// </summary>
        public bool GenerateNewImage()
        {
            Debug.Write("Generating new image -> ");
            if(GeneratedBitmap != _defaultBitmap)
            {
                Debug.WriteLine("Success, bitmap already generated");
                return true;
            }
            else if (OriginalBitmap != null && Palette.Count > 0 && ActiveQuantizer !=null)
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
        public void SaveGeneratedImage(string path, System.Drawing.Imaging.ImageFormat format)
        {
            try
            {
                GeneratedBitmap.Save(path, format);
            }
            catch { Debug.WriteLine("Failed saving image to "+path); }
        }
        #endregion
    }
}
