using System;
using System.Drawing;

namespace Application.ImageBuffer.ImageBuilder
{
    public interface IImageBuilder
    {
        public Bitmap BuildBitmap(Bitmap origianl, Func<Color, Color> func);
    }
}