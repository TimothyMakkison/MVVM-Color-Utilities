using System;
using System.Collections.Generic;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using Microsoft.Win32;

namespace MVVM_Color_Utilities.Helpers
{
    /// <summary>
    /// Contains shared dialog boxes and avaliable image buffer options.
    /// </summary>
    public static class ImageBufferItems
    {
        public static  readonly OpenFileDialog OpenDialogBox = new OpenFileDialog()
        { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title = "Browse Images" };

        public static readonly SaveFileDialog SaveDialogBox = new SaveFileDialog()
        { Filter = "JPG (*.jpg;*.jpeg)|(*.jpg;*.jpeg)", Title = "Save Image" };
        //|PNG(*.png)| (*.png)|BMP(*.bmp)| (*.bmp)"

        public static List<Int32> ColorCountList { get; } =
            new List<int> { 1, 2, 4, 8, 16, 32, 64, 128, 256 };//{ 256,128,64,32,16,8,4,2,1};

        public static List<BaseColorQuantizer> QuantizerList { get; } = new List<BaseColorQuantizer>
        {
            new MedianCutQuantizer()
        };
    }
}
