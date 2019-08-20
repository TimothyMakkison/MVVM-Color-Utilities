using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;


namespace MVVM_Color_Utilities.Helpers
{
    public static class ImageBufferItems
    {
        public static List<Int32> ColorCountOptions { get; } =
            new List<int> { 1, 2, 4, 8, 16, 32, 64, 128, 256 };//{ 256,128,64,32,16,8,4,2,1};

        public static List<BaseColorQuantizer> QuantizerOptions { get; } = new List<BaseColorQuantizer>
        {
            new MedianCutQuantizer()
        };
    }
}
