using Application.Palette_Quantizers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Application.ImageBuffer
{
    public interface IImageBuffer
    {
        Bitmap GenerateNewBitmap();

        IEnumerable<Color> GetPalette();

        ConcurrentDictionary<int, int> ScanBitmap();

        void SetBitmap(Bitmap bitmap);

        void SetColorCount(int colorCount);

        void SetQuantizer(IColorQuantizer quantizer);
    }
}