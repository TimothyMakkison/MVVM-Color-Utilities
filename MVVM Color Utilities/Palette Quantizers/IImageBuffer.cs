using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    internal interface IImageBuffer
    {
        IColorQuantizer ActiveQuantizer { get; set; }
        ConcurrentDictionary<int, int> BitmapColors { get; set; }
        int ColorCount { get; set; }
        Bitmap GeneratedBitmap { get; set; }
        Bitmap OriginalBitmap { get; set; }

        Bitmap GenerateNewBitmap();
        List<Color> GetPalette();
    }
}