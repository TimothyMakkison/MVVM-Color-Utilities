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
        private ICollection<Int32> colorList;

        /// <summary>
        /// Sets the colors that will be sorted through.
        /// </summary>
        /// <param name="colorList"></param>
        public virtual void SetColorList(ICollection<Int32> colorList)
        {
            this.colorList = colorList;
        }

        public virtual List<Color> Palette { get; set; }
        public virtual List<Color> GetPalette(Int32 colorCount)
        {
            return Palette;
        }
        public abstract string Name { get; }

        public abstract int GetPaletteIndex(Color color);
    }
}
