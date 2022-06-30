using Application.Palette_Quantizers;
using System.Collections.Generic;
using System.Drawing;

namespace Application.ImageBuffer;

public interface IImageBuffer
{
    Bitmap GenerateNewBitmap(Bitmap bitmap, IColorQuantizer quantizer, int colorCount);

    IEnumerable<Color> GetPalette(Bitmap bitmap, IColorQuantizer quantizer, int colorCount);
}
