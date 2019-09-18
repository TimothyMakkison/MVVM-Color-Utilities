using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections.Concurrent;
using System.Windows;


namespace MVVM_Color_Utilities.Palette_Quantizers 
{
    class PopularityQuantizer:BaseColorQuantizer
    {
        #region Fields
        private ConcurrentDictionary<int, int> colorDictionary = new ConcurrentDictionary<int, int>();
        private List<Color> _palette = new List<Color>();
        #endregion

        #region Constructor
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

        public override int GetColorCount
        {
            get
            {
                return Palette.Count;
            }
        }
        #endregion

        #region Methods
        public override void SetColorList(ConcurrentDictionary<int, int> colorDictionary)
        {
            //this.colorDictionary = colorDictionary.Keys;
            this.colorDictionary = colorDictionary;
        }
        public override List<Color> GetPalette(int colorCount)
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
                var myList = gridIndexColorDict.ToList();

                myList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                foreach(KeyValuePair<int,int> item in myList)
                {
                    if (Palette.Count < colorCount)
                    {
                        System.Diagnostics.Debug.WriteLine(item.Key+","+item.Value);
                        Palette.Add(GridIndexToColor(item.Key));
                    }
                }
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
        public override int GetPaletteIndex(Color color)
        {
            int bestIndex=4;
            int bestDistance = int.MaxValue;
            for(int i =0; i<Palette.Count; i++)
            {
                int distance = EuclideanDistance(color, Palette[i]);
                if (distance < bestDistance)
                {
                    if (distance < 27) //if color is in an cell.
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
            int redDiffernce = Math.Abs(color1.R - color2.R);
            int greenDiffernce = Math.Abs(color1.G - color2.G);
            int blueDiffernce = Math.Abs(color1.B - color2.B);

            return redDiffernce * redDiffernce + greenDiffernce * greenDiffernce + blueDiffernce * blueDiffernce;
        }
        #endregion
    }
}
