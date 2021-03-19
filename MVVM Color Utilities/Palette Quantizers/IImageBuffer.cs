using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    internal interface IImageBuffer
    {
        Bitmap GeneratedBitmap { get; set; }
        Bitmap GenerateNewBitmap();
        IEnumerable<Color> GetPalette();
    }
}