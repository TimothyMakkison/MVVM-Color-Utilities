using Application.Helpers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;

namespace Application.Palette_Quantizers
{
    [ScrutorIgnore]
    public class CachingColorQuantizer : IColorQuantizer
    {
        private readonly IColorQuantizer _baseQuantizer;
        private readonly Dictionary<int, List<Color>> _paletteCache = new();

        public CachingColorQuantizer(IColorQuantizer baseQuantizer)
        {
            _baseQuantizer = baseQuantizer;
        }

        public string Name => _baseQuantizer.Name;

        public List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
        {
            var hash = colorCount.GetHashCode() ^ colorDictionary.GetSequenceHash();
            if (_paletteCache.TryGetValue(hash, out List<Color> palette))
            {
                return palette;
            }

            palette = _baseQuantizer.GetPalette(colorCount, colorDictionary);
            _paletteCache.Add(hash, new(palette));
            return palette;
        }

        public int GetPaletteIndex(Color color)
        {
            return _baseQuantizer.GetPaletteIndex(color);
        }
    }
}