using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Concurrent;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    /// <summary>
    /// Uses an array of Colors to generate a palette of given size.
    /// </summary>
    public abstract class BaseColorQuantizer
    {
        /// <summary>
        /// Sets the colors that will be sorted through.
        /// </summary>
        /// <param name="colorDictionary"></param>
        public abstract void SetColorList(ConcurrentDictionary<int,int> colorDictionary);
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
