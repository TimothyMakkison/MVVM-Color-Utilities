using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    public abstract class BaseColorQuantizer
    {
        /// <summary>
        /// Sets the colors that will be sorted through.
        /// </summary>
        /// <param name="colorList"></param>
        public abstract void SetColorList(ICollection<Int32> colorList);
        /// <summary>
        /// Generated Color Palette.
        /// </summary>
        public abstract List<Color> Palette { get; set; }
        /// <summary>
        /// Generates a new palette
        /// </summary>
        /// <param name="colorCount"></param>
        /// <returns></returns>
        public abstract List<Color> GetPalette(Int32 colorCount);
        /// <summary>
        /// Sets the display name of derived quantizer.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Returns the most similar  <see cref="Palette"/> index to the input color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public abstract int GetPaletteIndex(Color color);
    }
}
