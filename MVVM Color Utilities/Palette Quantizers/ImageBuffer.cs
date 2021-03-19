using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    internal class ImageBuffer : ObservableObject, IImageBuffer
    {
        private Bitmap originalBitmap;
        private Memoizer<Bitmap, ConcurrentDictionary<int, int>> _scanner;
        private IColorQuantizer _quantizer;
        private int _colorCount;
        private Memoizer<int, ConcurrentDictionary<int, int>, List<Color>> _paletteBuilder;
        private IImageBuilder _imageBuilder;

        public ImageBuffer(IBitmapScanner bitmapScanner,
            IColorQuantizer quantizer,
            int colorCount,
            IImageBuilder builder)
        {
            _scanner = new Memoizer<Bitmap, ConcurrentDictionary<int, int>>(bitmapScanner.Scan);

            this._quantizer = quantizer;
            _paletteBuilder = new Memoizer<int, ConcurrentDictionary<int, int>, List<Color>>(_quantizer.GetPalette);
            _colorCount = colorCount;

            _imageBuilder = builder;
        }

        public ImageBuffer() : this(new BitmapScanner(), new Median_Cut.MedianCutQuantizer(), 16, new ImageBuilder())
        {
        }

        public void SetQuantizer(IColorQuantizer quantizer)
        {
            this._quantizer = quantizer;
            this._paletteBuilder = new Memoizer<int, ConcurrentDictionary<int, int>, List<Color>>(quantizer.GetPalette);
        }

        public void SetBitmap(Bitmap bitmap)
        {
            originalBitmap = bitmap;

            Debug.WriteLine($"Bitmap set, size is {originalBitmap.Size} " +
                $"with {originalBitmap.Width * originalBitmap.Height} pixels");
        }

        public void SetColorCount(int colorCount)
        {
            this._colorCount = colorCount;
        }

        public ConcurrentDictionary<int, int> ScanBitmap()
        {
            return _scanner.GetValue(originalBitmap);
        }

        public IEnumerable<Color> GetPalette()
        {
            var colorFrequency = ScanBitmap();
            return _paletteBuilder.GetValue(_colorCount, colorFrequency);
        }

        /// <summary>
        /// Uses the CurrentBitmap and Palette to generate an approximate image.
        /// </summary>
        /// <returns>Generated bitmap.</returns>
        public Bitmap GenerateNewBitmap()
        {
            var colors = GetPalette().ToArray();
            Func<Color, Color> func = c => colors[_quantizer.GetPaletteIndex(c)];

            return _imageBuilder.BuildBitmap(originalBitmap, func);
        }
    }
}