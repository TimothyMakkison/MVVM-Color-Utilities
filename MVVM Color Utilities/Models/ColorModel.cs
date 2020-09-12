using System;
using System.Drawing;

namespace MVVM_Color_Utilities.Models
{
    public class ColorModel
    {
        public ColorModel(Color color)
        {
            Id = new Guid();
            Color = color;
        }

        public string Name { get; set; }
        public Color Color { get; }
        public Guid Id { get; }
    }
}