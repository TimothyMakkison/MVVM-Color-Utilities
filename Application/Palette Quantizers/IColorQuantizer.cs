﻿using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Application.Palette_Quantizers;

public interface IColorQuantizer
{
    string Name { get; }

    List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary);

    int GetPaletteIndex(Color color);
}
