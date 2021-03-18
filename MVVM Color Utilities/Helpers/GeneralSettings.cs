using Microsoft.Win32;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using MVVM_Color_Utilities.Palette_Quantizers.Naieve;
using MVVM_Color_Utilities.Palette_Quantizers.Octree;
using MVVM_Color_Utilities.Palette_Quantizers.PopularityQuantizer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVVM_Color_Utilities.Helpers
{
    /// <summary>
    /// Contains shared dialog boxes and avaliable image buffer options.
    /// </summary>
    public class GeneralSettings
    {
        private readonly List<IColorQuantizer> quantizerList = new List<IColorQuantizer>
        {
            new MedianCutQuantizer(),
            new PopularityQuantizer(),
            new NaieveQuantizer(),
            new OctreeQuantizer(),
        };

        private readonly List<Int32> colorCountList =
           new List<int> { 1, 2, 4, 8, 16, 32, 64, 128, 256 };//{ 256,128,64,32,16,8,4,2,1};

        public readonly OpenFileDialog OpenDialogBox = new OpenFileDialog()
        { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title = "Browse Images" };

        public readonly SaveFileDialog SaveDialogBox = new SaveFileDialog()
        { Filter = "JPG (*.jpg;*.jpeg)|(*.jpg;*.jpeg)", Title = "Save Image" };

        public List<Int32> ColorCountList => colorCountList.ToList();

        public List<IColorQuantizer> QuantizerList => quantizerList.ToList();
    }
}