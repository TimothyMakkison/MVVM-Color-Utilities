using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Windows.Input;
using System.Windows;
using System.Drawing;
using System.Security;
using Microsoft.Win32;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using MVVM_Color_Utilities.Palette_Quantizers;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    class ImageAnalyzerModel : ObservableObject
    {
        private readonly ImageBuffer buffer = new ImageBuffer();

        /// <summary>
        /// Sets the bitmap to be read and clears the saved colors so the new image can be proccessed.
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetBitmap(Bitmap bitmap)
        {
            buffer.OriginalBitmap=bitmap;
        }
        /// <summary>
        /// Sets the quantizer that will read the bitmap.
        /// </summary>
        /// <param name="quantizer"></param>
        public void SetQuantizer(BaseColorQuantizer quantizer)
        {
            buffer.ActiveQuantizer= quantizer;
        }
        /// <summary>
        /// Sets the color count.
        /// </summary>
        /// <param name="colorCount">Number of colors in final palette.</param>
        public void SetColorCount(Int32 colorCount)
        {
            buffer.ColorCount= colorCount;
        }
        /// <summary>
        /// Returns <see cref="ImageBuffer"/> GetPalette.
        /// </summary>
        /// <returns></returns>
        public List<Color> GetPalette()
        {
            buffer.ScanBitmapColors();
            buffer.GetPalette();
            return buffer.Palette;
        }
    }
}
