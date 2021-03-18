using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    /// <summary>
    /// Uses an array of Colors to generate a palette of given size.
    /// </summary>
    public abstract class BaseColorQuantizer : IColorQuantizer
    {
        private List<Color> _palette = new List<Color>();

        /// <summary>
        /// Sets the display name of derived quantizer.
        /// </summary>
        public virtual string Name => "Base Color Quantizer";

        /// <summary>
        /// Generated Color Palette.
        /// </summary>
        public virtual List<Color> Palette
        {
            get => _palette;
            set => _palette = value;
        }

        /// <summary>
        /// Generates a new palette
        /// </summary>
        /// <param name="colorCount"></param>
        /// <returns></returns>
        public abstract List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary);

        /// <summary>
        /// Returns index of the most similar color in Palette.
        /// </summary>
        /// <param name="color">Target Color.</param>
        /// <returns>Index of most similar color.</returns>
        public virtual int GetPaletteIndex(Color color)
        {
            if (!Palette.Any())
            {
                throw new ArgumentException("Cannot access Palette as it is empty. Try GetPalette first.", "Palette");
            }
            return 0;
        }

        /// <summary>
        /// Throws exception if Palette is empty.
        /// </summary>
        protected void PaletteArgumentChecker()
        {
            if (!Palette.Any())
            {
                throw new ArgumentNullException("Palette is empty, please use GetPalette first.", "Palette");
            }
        }
    }
}