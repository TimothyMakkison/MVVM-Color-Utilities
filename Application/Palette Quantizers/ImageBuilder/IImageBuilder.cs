using System;
using System.Drawing;

namespace Application.Palette_Quantizers
{
    public interface IImageBuilder
    {
        public Bitmap BuildBitmap(Bitmap origianl, Func<Color, Color> func);
    }
}