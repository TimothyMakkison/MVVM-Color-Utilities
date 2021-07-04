using Application.Helpers.DistanceCalculator;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Application.Palette_Quantizers.Naieve
{
    public class NaieveQuantizer : IColorQuantizer
    {
        private readonly IDistanceCalculator distanceCalculator = new ManhattenDistance();

        public string Name => "Naieve Quantizer";

        public List<Color> Palette { get; set; } = new List<Color>();

        /// <summary>
        /// Generates palette from the most common colors and returns Palette.
        /// </summary>
        /// <param name="colorCount">Number of colors in Palette.</param>
        /// <param name="colorDictionary">Input colors and frequencies.</param>
        /// <returns>Palette as a list of colors.</returns>
        public List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
            => Palette = colorDictionary.OrderByDescending(x => x.Value)
                                        .Take(colorCount)
                                        .Select(x => Color.FromArgb(255, Color.FromArgb(x.Key)))
                                        .ToList();

        /// <summary>
        /// Returns index of most similar color in Palette.
        /// </summary>
        /// <param name="color">Target color.</param>
        /// <returns>Index of most </returns>
        public int GetPaletteIndex(Color color)
        {
            if (Palette.Count == 0)
            {
                throw new ArgumentNullException("Palette is empty, please use GetPalette first.", "Palette");
            }

            int bestIndex = 0;
            int bestDistance = int.MaxValue;
            for (int i = 0; i < Palette.Count; i++)
            {
                int distance = distanceCalculator.Distance(color, Palette[i]);
                if (distance < bestDistance)
                {
                    if (distance <= 12) //if color is in cell. 9 = 3 * 3
                    {
                        return i;
                    }
                    bestDistance = distance;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
    }
}