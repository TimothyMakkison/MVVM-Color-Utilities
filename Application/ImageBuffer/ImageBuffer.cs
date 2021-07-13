using Application.Helpers;
using Application.Palette_Quantizers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Application.ImageBuffer
{
    public class ImageBuffer : IImageBuffer
    {
        //TODO Remove originalBitmap value. Might be needed to prevent error if user changes image?
        private Bitmap originalBitmap;
        private readonly Memoizer<Bitmap, ConcurrentDictionary<int, int>> _scanner;
        private IColorQuantizer _quantizer;
        private int _colorCount;
        private readonly IImageBuilder _imageBuilder;

        public ImageBuffer(IBitmapScanner bitmapScanner,
            IColorQuantizer quantizer,
            int colorCount,
            IImageBuilder imageBuilder)
        {
            _scanner = new Memoizer<Bitmap, ConcurrentDictionary<int, int>>(bitmapScanner.Scan);

            this._quantizer = quantizer;
            _colorCount = colorCount;

            _imageBuilder = imageBuilder;
        }

        public void SetQuantizer(IColorQuantizer quantizer)
        {
            this._quantizer = quantizer;
        }

        public void SetBitmap(Bitmap bitmap)
        {
            originalBitmap = bitmap;

            Debug.WriteLine($"Bitmap set, size is {originalBitmap.Size} " +
                $"with {originalBitmap.Width * originalBitmap.Height} pixels");
        }

        public void SetColorCount(int colorCount)
        {
            _colorCount = colorCount;
        }

        public ConcurrentDictionary<int, int> ScanBitmap()
        {
            return _scanner.GetValue(originalBitmap);
        }

        public IEnumerable<Color> GetPalette()
        {
            var colorFrequency = ScanBitmap();
            return _quantizer.GetPalette(_colorCount, colorFrequency);
        }

        public Bitmap GenerateNewBitmap()
        {
            var colors = GetPalette().ToArray();
            Func<Color, Color> func = c => colors[_quantizer.GetPaletteIndex(c)];

            return _imageBuilder.BuildBitmap(originalBitmap, func);
        }
    }
}