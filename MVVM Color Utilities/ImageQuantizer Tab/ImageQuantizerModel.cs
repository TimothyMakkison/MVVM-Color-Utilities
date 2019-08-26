using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using MVVM_Color_Utilities.Helpers;
using System.Windows.Input;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    class ImageQuantizerModel : ObservableObject
    {
        #region Fields
        private readonly ImageBuffer buffer = new ImageBuffer();
        #endregion

        #region Methods
        #region Get Methods
        /// <summary>
        /// Returns <see cref="ImageBuffer"/> GetPalette.
        /// </summary>
        /// <returns></returns>
        public List<Color> GetPalette()
        {
            if (buffer.GetPalette())
            {
                return buffer.Palette;
            }
            else
            {
                return new List<Color>();
            }
        }
        public void GetNewImage()
        {
            buffer.GetPalette();
            buffer.GenerateNewImage();
        }
        #endregion
        #region Set Methods
        /// <summary>
        /// Sets the bitmap to be read and clears the saved colors so the new image can be proccessed.
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetBitmap(Bitmap bitmap)
        {
            MessageBox.Show("IQ Bitmap set");

            buffer.SetBitmap(bitmap);
        }
        /// <summary>
        /// Sets the quantizer that will read the bitmap.
        /// </summary>
        /// <param name="quantizer"></param>
        public void SetQuantizer(BaseColorQuantizer quantizer)
        {
            buffer.SetQuantizer(quantizer);
        }
        /// <summary>
        /// Sets the color count.
        /// </summary>
        /// <param name="colorCount">Number of colors in final palette.</param>
        public void SetColorCount(Int32 colorCount)
        {
            buffer.SetColorCount(colorCount);
        }
        #endregion
        public Bitmap GeneratedBitmap
        {
            get
            {
                return buffer.GeneratedBitmap;
            }
        }
        /// <summary>
        /// Save generated image to location and with given type.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="format"></param>
        public void SaveGeneratedImage(string path, System.Drawing.Imaging.ImageFormat format)
        {
            buffer.SaveGeneratedImage(path, format);
        }
        #endregion
    }
}
