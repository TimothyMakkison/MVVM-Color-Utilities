using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using System.Windows;
using System.IO;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    class ImageBuffer
    {
        #region Fields
        private Bitmap currentBitmap;
        private BaseColorQuantizer activeQuantizer;
        private Int32 colorCount;
        private readonly static string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private readonly string NewImagePath = projectPath + "/Resources/Images/NewImage.bmp";

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageBuffer"/> class.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="quantizer"></param>
        /// <param name="colorCount"></param>
        public ImageBuffer(Bitmap bitmap, BaseColorQuantizer quantizer, Int32 colorCount)
        {
            currentBitmap = bitmap;
            activeQuantizer = quantizer;
            this.colorCount = colorCount;
        }
        public ImageBuffer() { }
        #endregion

        #region Properties
        /// <summary>
        /// Returns the generated palette
        /// </summary>
        public List<Color> Palette { get; set; }
        /// <summary>
        /// Stores all of the colors in the bitmap.
        /// </summary>
        public List<Int32> ColorList { get; set; } = new List<int>();
        #endregion

        //public string NewImagePath => projectPath + "/Resources/Images/NewImage.bmp";

        public Bitmap GeneratedBitmap { get; set; }
        #region Methods
        /// <summary>
        /// Forms a palette and returns true if successfull.
        /// </summary>
        /// <returns></returns>
        public bool GetPalette()
        {
            //Ensures all values will not return null.
            if (currentBitmap != null && activeQuantizer != null && colorCount > 0)
            {
                if (ColorList.Count == 0)
                    GetSourceBitmapColors();
                MessageBox.Show("soruce colors succ");
                activeQuantizer.SetColorList(ColorList);
                Palette = activeQuantizer.GetPalette(colorCount);
                return true;
            }
            return false;
        }

        private void GeneratePalette()
        {
            if (ColorList.Count == 0)
                GetSourceBitmapColors();
            MessageBox.Show("soruce colors succ");
            activeQuantizer.SetColorList(ColorList);
            Palette = activeQuantizer.GetPalette(colorCount);
        }
        /// <summary>
        /// Adds all of the bitmaps colors to the ColorList.
        /// </summary>
        private void GetSourceBitmapColors()
        {
            //Iterates through each pixel adding it to the colorList
            for (int x = 0; x < currentBitmap.Width; x++)
                for (int y = 0; y < currentBitmap.Height; y++)
                {
                    Color pixelColor = currentBitmap.GetPixel(x, y);
                    Int32 key = pixelColor.R << 16 | pixelColor.G << 8 | pixelColor.B;
                    ColorList.Add(key);
                }
        }
        public bool GenerateNewImage()
        {
            if (currentBitmap != null && activeQuantizer != null && colorCount > 0 && Palette.Count > 0)
            {
                //Task.Run(() =>ImageGenerator());
                ImageGenerator();
                return true;
            }
            else
            {
                MessageBox.Show("IB Failed generating image");
            }
            return false;
        }
        private void ImageGenerator()
        {
            Bitmap bitmap = new Bitmap(currentBitmap.Width, currentBitmap.Height);
            for (int x = 0; x < currentBitmap.Width; x++)
                for (int y = 0; y < currentBitmap.Height; y++)
                {
                    Color pixelColor = currentBitmap.GetPixel(x, y);
                    int index = activeQuantizer.GetPaletteIndex(pixelColor);
                    bitmap.SetPixel(x, y, Palette[index]);
                }
            //GeneratedBitmap = bitmap;
            MessageBox.Show("saving new image");
            bitmap.Save(NewImagePath
                , System.Drawing.Imaging.ImageFormat.Bmp);
            MessageBox.Show("save succ");
        }

        #region SetMethods
        /// <summary>
        /// Sets the bitmap to be read and clears the saved colors so the new image can be proccessed.
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetBitmap(Bitmap bitmap)
        {
            currentBitmap = bitmap;
            ColorList.Clear();
        }
        /// <summary>
        /// Sets the quantizer that will read the bitmap.
        /// </summary>
        /// <param name="quantizer"></param>
        public void SetQuantizer(BaseColorQuantizer quantizer)
        {
            activeQuantizer = quantizer;
        }
        /// <summary>
        /// Sets the color count.
        /// </summary>
        /// <param name="colorCount"></param>
        public void SetColorCount(Int32 colorCount)
        {
            this.colorCount = colorCount;
        }
        #endregion

        #endregion
    }
}
