using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using System.Windows;
using System.IO;
using System.Diagnostics;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    class ImageBuffer
    {
        #region Fields
        private Bitmap _originalBitmap, _generatedBitmap;
        private BaseColorQuantizer _activeQuantizer;
        private Int32 _colorCount;
        #endregion

        #region Properties
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
                    ColorList = null;
                    Palette = null;
                    GeneratedBitmap = null;
                }
            }
        }
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
                    Palette = null;
                    GeneratedBitmap = null;
                }
            }
        }
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
                    Palette = null;
                    GeneratedBitmap = null;
                }
            }
        }
        /// <summary>
        /// Stores all of the colors in the bitmap.
        /// </summary>
        public List<Int32> ColorList { get; set; }
        /// <summary>
        /// Returns the generated palette
        /// </summary>
        public List<Color> Palette { get; set; }
        /// <summary>
        /// Generated bitmap
        /// </summary>
        public Bitmap GeneratedBitmap
        {
            get
            {
                return _generatedBitmap;
            }
            set
            {
                _generatedBitmap = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Iterates through OriginalBitmap, adding each color to the ColorList.
        /// </summary>
        public void GetSourceBitmapColors()
        {
            Debug.Write("Getting Bitmap Colors -> ");
            if(OriginalBitmap == null)
            {
                Debug.WriteLine("Fail, Original Bitmap is null");
            }
            else if(ColorList != null)
            {
                Debug.WriteLine("Success, ColorList already generated");
            }
            else
            {
                //Iterates through each pixel adding it to the colorList
                List<int> newColorList = new List<int>();

                for (int x = 0; x < OriginalBitmap.Width; x++)
                    for (int y = 0; y < OriginalBitmap.Height; y++)
                    {
                        Color pixelColor = OriginalBitmap.GetPixel(x, y);
                        Int32 key = pixelColor.R << 16 | pixelColor.G << 8 | pixelColor.B;
                        newColorList.Add(key);
                    }
                ColorList = newColorList;

                Debug.WriteLine("Success, Found "+newColorList.Count.ToString()+" colors");
            }

        }
        public bool GetPalette()
        {
            Debug.Write("Getting palette -> ");
            if (ColorList != null && ActiveQuantizer!= null && ColorCount>0)
            {
                _activeQuantizer.SetColorList(ColorList);
                Palette = _activeQuantizer.GetPalette(ColorCount);
                Debug.WriteLine("Success, Generated palette of "+ Palette.Count.ToString()+" colors");
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
            if (OriginalBitmap != null && Palette != null)
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
            catch { MessageBox.Show("fail save"); }
        }
        #endregion
    }
}
