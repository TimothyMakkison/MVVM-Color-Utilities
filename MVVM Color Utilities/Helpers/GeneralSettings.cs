using Application.Palette_Quantizers;
using Application.Palette_Quantizers.Median_Cut;
using Application.Palette_Quantizers.Octree;
using Application.Palette_Quantizers.PopularityQuantizer;
using Application.Palette_Quantizers.Naieve;
using Microsoft.Win32;
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
        //TODO use reflections to intitialize
        private readonly List<IColorQuantizer> quantizerList = new List<IColorQuantizer>
        {
            new MedianCutQuantizer(),
            new PopularityQuantizer(),
            new NaieveQuantizer(),
            new OctreeQuantizer(),
        };

        private readonly List<Int32> colorCountList =
           new List<int> { 1, 2, 4, 8, 16, 32, 64, 128, 256 };//{ 256,128,64,32,16,8,4,2,1};

        //TODO Should be injected with a transient lifetime.
        public readonly OpenFileDialog OpenDialogBox = new OpenFileDialog()
        { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title = "Browse Images" };

        public readonly SaveFileDialog SaveDialogBox = new SaveFileDialog()
        { Filter = "JPG (*.jpg;*.jpeg)|(*.jpg;*.jpeg)", Title = "Save Image" };

        public List<int> ColorCountList => colorCountList.ToList();

        //TODO This should be populated using reflection.
        public List<IColorQuantizer> QuantizerList => quantizerList.ToList();
    }
}