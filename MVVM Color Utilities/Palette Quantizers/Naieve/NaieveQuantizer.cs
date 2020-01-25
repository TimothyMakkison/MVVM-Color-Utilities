using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers.Naieve
{
    class NaieveQuantizer : BaseColorQuantizer
    {
        #region Properties
        public override string Name => "Naieve Quantizer";

        public override List<Color> Palette { get; set; } = new List<Color>();

        /// <summary>
        /// Generates palette from the most common colors and returns Palette.
        /// </summary>
        /// <param name="colorCount">Number of colors in Palette.</param>
        /// <param name="colorDictionary">Input colors and frequencies.</param>
        /// <returns>Palette as a list of colors.</returns>
        public override List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary) 
            => Palette = colorDictionary.OrderBy(x => x.Value)
                                         .Take(colorCount)
                                         .Select(x => Color.FromArgb((x.Key & 0xFF0000) >> 16,
                                                                     (x.Key & 0xFF00) >> 8,
                                                                     x.Key & 0xFF))
                                         .ToList();

        /// <summary>
        /// Returns index of most similar color in Palette.
        /// </summary>
        /// <param name="color">Target color.</param>
        /// <returns>Index of most </returns>
        public override int GetPaletteIndex(Color color)
        {
            base.PaletteArgumentChecker();

            int bestIndex = 0;
            int bestDistance = int.MaxValue;
            for (int i = 0; i < Palette.Count; i++)
            {
                int distance = EuclideanDistance(color, Palette[i]);
                if (distance < bestDistance)
                {
                    if (distance <= 12) //if color is in cell. 27 = 3 * 3^2
                    {
                        return i;
                    }
                    bestDistance = distance;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Finds the euclidean distance between two colors.
        /// </summary>
        /// <param name="color1">First color.</param>
        /// <param name="color2">Second color.</param>
        /// <returns>Distance between colors.</returns>
        private int EuclideanDistance(Color color1, Color color2)
        {
            int redDifference = Math.Abs(color1.R - color2.R);
            int greenDifference = Math.Abs(color1.G - color2.G);
            int blueDifference = Math.Abs(color1.B - color2.B);

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }
        #endregion
    }
}
