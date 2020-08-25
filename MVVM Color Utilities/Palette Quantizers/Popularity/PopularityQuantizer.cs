using MVVM_Color_Utilities.Helpers.DistanceCalculator;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers.PopularityQuantizer
{
    /// <summary>
    /// Divides each colors values by 4, compressing them and then creates a palette from the most common values
    /// and converting them back into color. This is similar to the normal popularity quantizer where each color
    /// is added to a 64x64x64 grid made up of 4x4x4 cubes. The palette is then found by finding the most
    /// populated cubes and returning the cubes average color. Memory savings are made in this implementation
    /// by not saving the individual colors when placed in the grid, instead if a color is within a cube, the
    /// cubes count increases, this does however mean an average of each cube cannot be found.
    /// </summary>
    internal class PopularityQuantizer : BaseColorQuantizer
    {
        #region Fields

        private List<Color> _palette = new List<Color>();

        private readonly IDistanceCalculator distanceCalculator = new ManhattenDistance();

        #endregion Fields

        #region Properties

        public override string Name => "PopularityQuantizer";

        public override List<Color> Palette
        {
            get => _palette;
            set => _palette = value;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Retunrs a palette by grouping similar colors and returning the most frequent colors as a palette.
        /// </summary>
        /// <param name="colorCount">Number of colors in palette.</param>
        /// <param name="colorDictionary">Input colors and frequencies.</param>
        /// <returns>Palette as a list of colors.</returns>
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

        /// <summary>
        /// Converts an integer form color into a 18 bit color.
        /// </summary>
        /// <param name="input">Color as an integer.</param>
        /// <returns>Compressed color integer.</returns>
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
        /// <returns>Index of most similar color.</returns>
        public override int GetPaletteIndex(Color color)
        {
            base.PaletteArgumentChecker();

            int bestIndex = 0;
            int bestDistance = int.MaxValue;
            for (int i = 0; i < Palette.Count; i++)
            {
                int distance = distanceCalculator.Distance(color, Palette[i]);
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

        #endregion Methods
    }
}