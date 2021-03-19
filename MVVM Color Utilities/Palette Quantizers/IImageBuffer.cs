using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    internal interface IImageBuffer
    {
        Bitmap GenerateNewBitmap();

        IEnumerable<Color> GetPalette();

        ConcurrentDictionary<int, int> ScanBitmap();

        void SetBitmap(Bitmap bitmap);

        void SetColorCount(int colorCount);

        void SetQuantizer(IColorQuantizer quantizer);
    }
}