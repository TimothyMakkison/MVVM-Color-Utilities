using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Concurrent;
using System.Windows;
using System.Diagnostics;


namespace MVVM_Color_Utilities.Palette_Quantizers 
{
    class PopularityQuantizer:BaseColorQuantizer
    {
        #region Fields
        private List<Color> _palette = new List<Color>();
        #endregion

        #region Properties
        public override string Name { get; } = "PopularityQuantizer";

        public override List<Color> Palette
        {
            get
            {
                return _palette;
            }
            set
            {
                _palette = value;
            }
        }
        #endregion

        #region Methods
        public override List<Color> GetPalette(int colorCount,ConcurrentDictionary<int, int> colorDictionary)
        { 
            if (colorDictionary.Count > 0)
            {
                Palette.Clear();
                ConcurrentDictionary<int, int> gridIndexColorDict = new ConcurrentDictionary<int, int>();
                foreach (int key in colorDictionary.Keys)
                {
                    int gridIndex = DenaryToGridIndex(key);
                    gridIndexColorDict.AddOrUpdate(gridIndex, colorDictionary[key], 
                        (keyValue, value) => value + colorDictionary[key]);
                }

                var sortedDict = gridIndexColorDict.ToList();
                sortedDict.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

                foreach (KeyValuePair<int,int> item in sortedDict.Take(colorCount))
                {
                    Palette.Add(GridIndexToColor(item.Key));
                }

                //var temp = sortedDict.Take(colorCount).ForEach();
                //var ree = sortedDict.ForEach(temp => GridIndexToColor(temp.Key));
            }

            return Palette;
        }
        private int DenaryToGridIndex(int input)
        {
             return (input & 0xFF0000) >> 18 << 12 | (input & 0xFF00) >> 10 << 6
                 | (input & 0xFF) >> 2;
        }
        private Color GridIndexToColor(int input)
        {
            int red = (input & 0x3F000 )>> 10;
            int green = (input &0xFC0) >> 4;
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
            //base.GetPaletteIndex();
            if (!Palette.Any())
            {
                throw new ArgumentException("Cannot find a similar color match as Palette is empty. Try GetPalette first.", "Palette");
            }
            int bestIndex =0;
            int bestDistance = int.MaxValue;
            for(int i =0; i<Palette.Count; i++)
            {
                int distance = EuclideanDistance(color, Palette[i]);
                if (distance < bestDistance)
                {
                    if (distance <= 27) //if color is in an cell.
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
