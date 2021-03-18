using System;
using System.Drawing;

namespace MVVM_Color_Utilities.Palette_Quantizers
{
    internal interface IImageBuilder
    {
        public Bitmap BuildBitmap(Bitmap origianl, Func<Color, Color> func);
    }
}