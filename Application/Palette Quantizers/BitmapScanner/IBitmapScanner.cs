﻿using System.Collections.Concurrent;
using System.Drawing;

namespace Application.Palette_Quantizers
{
    public interface IBitmapScanner
    {
        /// <summary>
        /// Iterate through bitmap returning a Dictionary of Colors (in int form) and frequency.
        /// </summary>
        /// <returns>Dictionary of OriginalBitmap colors.</returns>
        ConcurrentDictionary<int, int> Scan(Bitmap bitmap);
    }
}