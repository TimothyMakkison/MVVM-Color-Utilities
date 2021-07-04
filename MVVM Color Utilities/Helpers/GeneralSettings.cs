using Application.Palette_Quantizers;
using Application.Palette_Quantizers.Median_Cut;
using Application.Palette_Quantizers.Naieve;
using Application.Palette_Quantizers.Octree;
using Application.Palette_Quantizers.PopularityQuantizer;
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
        private readonly List<IColorQuantizer> quantizerList = new()
        {
            new MedianCutQuantizer(),
            new PopularityQuantizer(),
            new NaieveQuantizer(),
            new OctreeQuantizer(),
        };

        private readonly List<Int32> colorCountList =
           new()
           { 1, 2, 4, 8, 16, 32, 64, 128, 256 };//{ 256,128,64,32,16,8,4,2,1};

        public List<int> ColorCountList => colorCountList.ToList();

        //TODO This should be populated using reflection.
        public List<IColorQuantizer> QuantizerList => quantizerList.ToList();
    }
}