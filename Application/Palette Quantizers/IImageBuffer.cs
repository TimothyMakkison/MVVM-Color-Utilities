﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Application.Palette_Quantizers
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