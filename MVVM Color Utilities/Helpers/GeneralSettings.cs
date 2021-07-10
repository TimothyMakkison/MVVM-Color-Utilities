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
        private readonly List<Int32> colorCountList =
           new()
           { 1, 2, 4, 8, 16, 32, 64, 128, 256 };//{ 256,128,64,32,16,8,4,2,1};

        public List<int> ColorCountList => colorCountList.ToList();
    }
}