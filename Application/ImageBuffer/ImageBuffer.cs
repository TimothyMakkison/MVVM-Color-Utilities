using Application.ImageBuffer.BitmapScanner;
using Application.ImageBuffer.ImageBuilder;
using Application.Palette_Quantizers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Application.ImageBuffer;

public class ImageBuffer : IImageBuffer
{
    private readonly Func<Bitmap, ConcurrentDictionary<int, int>> _scanner;
    private readonly IImageBuilder _imageBuilder;

    public ImageBuffer(IBitmapScanner bitmapScanner,
        IImageBuilder imageBuilder)
    {
        _scanner = image => bitmapScanner.Scan(image);
        _imageBuilder = imageBuilder;
    }

    private ConcurrentDictionary<int, int> ScanBitmap(Bitmap bitmap)
    {
        return _scanner(bitmap);
    }

    //TODO fix crash if bitmap is not assigned
    public IEnumerable<Color> GetPalette(Bitmap bitmap, IColorQuantizer quantizer, int colorCount)
    {
        var colorFrequency = ScanBitmap(bitmap);
        return quantizer.GetPalette(colorCount, colorFrequency);
    }

    public Bitmap GenerateNewBitmap(Bitmap bitmap, IColorQuantizer quantizer, int colorCount)
    {
        var colors = GetPalette(bitmap, quantizer, colorCount).ToArray();
        Color func(Color c) => colors[quantizer.GetPaletteIndex(c)];

        return _imageBuilder.BuildBitmap(bitmap, func);
    }
}
