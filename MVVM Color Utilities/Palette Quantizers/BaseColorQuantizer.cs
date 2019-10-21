using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections.Concurrent;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    /// <summary>
    /// Uses an array of Colors to generate a palette of given size.
    /// </summary>
    public abstract class BaseColorQuantizer
    {
        #region Fields
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
        public abstract List<Color> GetPalette(Int32 colorCount, ConcurrentDictionary<int, int> colorDictionary);
        /// <summary>
        /// Returns index of the most similar color in Palette.
        /// </summary>
        /// <param name="color">Original Color</param>
        /// <returns></returns>
        public virtual int GetPaletteIndex(Color color)
        {
            if (!Palette.Any())
            {
                throw new ArgumentException("Cannot access Palette as it is empty. Try GetPalette first.","Palette");
            }
            return 0;
        }
        #endregion
    }
}
