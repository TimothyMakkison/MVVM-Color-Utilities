using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Application.Palette_Quantizers
{
    public class CachingColorQuantizer : IColorQuantizer
    {
        private readonly IColorQuantizer _baseQuantizer;
        private readonly Dictionary<(int, ConcurrentDictionary<int, int>), List<Color>> _paletteCache = new();

        public CachingColorQuantizer(IColorQuantizer baseQuantizer)
        {
            _baseQuantizer = baseQuantizer;
        }

        public string Name => _baseQuantizer.Name;

        public List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
        {
            var tuple = (colorCount, colorDictionary);
            if (_paletteCache.TryGetValue(tuple, out List<Color> palette))
            {
                return palette;
            }

            palette = _baseQuantizer.GetPalette(colorCount, colorDictionary);
            _paletteCache.Add(tuple, palette);
            return palette;
        }

        public int GetPaletteIndex(Color color)
        {
            return _baseQuantizer.GetPaletteIndex(color);
        }
    }
}