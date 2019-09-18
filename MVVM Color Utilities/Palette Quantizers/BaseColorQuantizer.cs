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
        #region Fields
        private ICollection<Int32> colorList = new List<Int32>();
        private List<Color> _palette = new List<Color>();
        #endregion
        #region Properties
        /// <summary>
        /// Sets the display name of derived quantizer.
        /// </summary>
        public virtual string Name { get; } = "BaseColorQuantizer";
        /// <summary>
        /// Generated Color Palette.
        /// </summary>
        public virtual List<Color> Palette
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
        /// <summary>
        /// Generates a new palette
        /// </summary>
        /// <param name="colorCount"></param>
        /// <returns></returns>
        public abstract List<Color> GetPalette(Int32 colorCount);
        /// <summary>
        /// Returns the most similar  <see cref="Palette"/> index to the input color.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public abstract int GetPaletteIndex(Color color);
        #endregion
        /// <summary>
        /// Sets the colors that will be sorted through.
        /// </summary>
        /// <param name="colorDictionary"></param>
        public abstract void SetColorList(ConcurrentDictionary<int,int> colorDictionary);
      

        public virtual int GetColorCount
        {
            get
            {
                return Palette.Count;
            }
        }
    }
}
