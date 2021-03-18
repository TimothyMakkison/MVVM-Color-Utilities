using MVVM_Color_Utilities.Helpers.DistanceCalculator;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers.Naieve
{
    internal class NaieveQuantizer : BaseColorQuantizer
    {
        private readonly IDistanceCalculator distanceCalculator = new ManhattenDistance();

        
        public override string Name => "Naieve Quantizer";

        public override List<Color> Palette { get; set; } = new List<Color>();

        /// <summary>
        /// Generates palette from the most common colors and returns Palette.
        /// </summary>
        /// <param name="colorCount">Number of colors in Palette.</param>
        /// <param name="colorDictionary">Input colors and frequencies.</param>
        /// <returns>Palette as a list of colors.</returns>
        public override List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
            => Palette = colorDictionary.OrderByDescending(x => x.Value)
                                        .Take(colorCount)
                                        .Select(x => Color.FromArgb(255, Color.FromArgb(x.Key)))
                                        .ToList();

        /// <summary>
        /// Returns index of most similar color in Palette.
        /// </summary>
        /// <param name="color">Target color.</param>
        /// <returns>Index of most </returns>
        public override int GetPaletteIndex(Color color)
        {
            PaletteArgumentChecker();

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