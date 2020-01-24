using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace MVVM_Color_Utilities.Palette_Quantizers
{
    class PopularityQuantizer : BaseColorQuantizer
    {
        #region Fields
        private List<Color> _palette = new List<Color>();
        #endregion

        #region Properties
        public override string Name => "PopularityQuantizer";

        public override List<Color> Palette
        {
            get => _palette;
            set => _palette = value;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Retunrs a palette by grouping similar colors and returning the most frequent colors as a palette.
        /// </summary>
        /// <param name="colorCount">Number of colors in palette.</param>
        /// <param name="colorDictionary">Colors that will be converted.</param>
        /// <returns>Color palette.</returns>
        public override List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
        {
            if (colorDictionary.Count > 0)
            {
                Palette.Clear();
                //Compress each color either adding to dictionary or updating frequency.
                ConcurrentDictionary<int, int> gridIndexColorDict = new ConcurrentDictionary<int, int>();
                foreach (int key in colorDictionary.Keys)
                {
                    int gridIndex = DenaryToGridIndex(key);
                    gridIndexColorDict.AddOrUpdate(gridIndex, colorDictionary[key],
                        (keyValue, frequency) => frequency + colorDictionary[key]);
                }

                //Sort by frequency and return a number of colors equal to colorCount.
                var sortedDict = gridIndexColorDict.ToList();
                sortedDict.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

                Palette = sortedDict.Take(colorCount).Select(item => GridIndexToColor(item.Key)).ToList();
            }

            return Palette;
        }

        private int DenaryToGridIndex(int input) => ((input & 0xFF0000) >> 18 << 12) | (input & 0xFF00) >> 10 << 6 | (input & 0xFF) >> 2;

        private Color GridIndexToColor(int input)
        {
            int red = (input & 0x3F000) >> 10;
            int green = (input & 0xFC0) >> 4;
            int blue = (input & 0x3F) << 2;
            return Color.FromArgb(255, red, green, blue);
        }

        /// <summary>
        /// Returns index of the most similar color in Palette.
        /// </summary>
        /// <param name="color">Target Color</param>
        /// <returns></returns>
        public override int GetPaletteIndex(Color color)
        {
            if (!Palette.Any())
            {
                throw new ArgumentException("Cannot find a similar color match as Palette is empty. Try GetPalette first.", "Palette");
            }
            int bestIndex = 0;
            int bestDistance = int.MaxValue;
            for (int i = 0; i < Palette.Count; i++)
            {
                int distance = EuclideanDistance(color, Palette[i]);
                if (distance < bestDistance)
                {
                    if (distance <= 27) //if color is in cell. 27 = 3 * 3^2
                    {
                        return i;
                    }
                    bestDistance = distance;
                    bestIndex = i;
                }
            }
            return bestIndex;
        }
        private int EuclideanDistance(Color color1, Color color2)
        {
            int redDifference = Math.Abs(color1.R - color2.R);
            int greenDifference = Math.Abs(color1.G - color2.G);
            int blueDifference = Math.Abs(color1.B - color2.B);

            return redDifference * redDifference + greenDifference * greenDifference + blueDifference * blueDifference;
        }
        private int ManhattenDistance(Color color1, Color color2)
        {
            int redDifference = Math.Abs(color1.R - color2.R);
            int greenDifference = Math.Abs(color1.G - color2.G);
            int blueDifference = Math.Abs(color1.B - color2.B);

            return redDifference + greenDifference + blueDifference;
        }
        #endregion
    }
}
