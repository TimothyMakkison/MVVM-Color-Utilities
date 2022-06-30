using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Application.Palette_Quantizers.Median_Cut;

internal class MedianCutCube
{
    private int redLowBound = 255, redUpperBound = 0;
    private int greenLowBound = 255, greenUpperBound = 0;
    private int blueLowBound = 255, blueUpperBound = 0;

    private Color averageColor;
    private readonly ICollection<int> colorList;

    public MedianCutCube(ICollection<int> colors)
    {
        colorList = colors;
        Shrink();
    }

    public int PaletteIndex { get; set; }

    public Color AverageColor
    {
        get
        {
            if (averageColor == default)
            {
                averageColor = GetAverageColor();
            }
            return averageColor;
        }
    }

    //TODO add switch statement

    /// <summary>
    /// Gets the index of the greatest length axis in the cube. 0 = Red, 1 = Green, 2 = Blue.
    /// </summary>
    public sbyte ChannelIndex
    {
        get
        {
            int redSize = redUpperBound - redLowBound;
            int greenSize = greenUpperBound - greenLowBound;
            int blueSize = blueUpperBound - blueLowBound;

            if (redSize >= greenSize && redSize >= blueSize)
            {
                return 0;
            }
            if (greenSize >= redSize && greenSize >= blueSize)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }
    }

    /// <summary>
    /// Calculcates the cubes average color by summing and averaging every color item.
    /// </summary>
    /// <returns>Cubes average color</returns>
    private Color GetAverageColor()
    {
        int red = 0, green = 0, blue = 0;
        foreach (int argb in colorList)
        {
            Color color = Color.FromArgb(argb);
            red += color.R;
            green += color.G;
            blue += color.B;
        }

        red = colorList.Count == 0 ? 0 : red / colorList.Count;
        green = colorList.Count == 0 ? 0 : green / colorList.Count;
        blue = colorList.Count == 0 ? 0 : blue / colorList.Count;
        return Color.FromArgb(255, red, green, blue);
    }

    /// <summary>
    /// Reduce bounds to range of rgb values.
    /// </summary>
    private void Shrink()
    {
        foreach (int argb in colorList)
        {
            Color color = Color.FromArgb(argb);

            int red = color.R;
            int green = color.G;
            int blue = color.B;

            if (red < redLowBound)
            {
                redLowBound = red;
            }

            if (red > redUpperBound)
            {
                redUpperBound = red;
            }

            if (green < greenLowBound)
            {
                greenLowBound = green;
            }

            if (green > greenUpperBound)
            {
                greenUpperBound = green;
            }

            if (blue < blueLowBound)
            {
                blueLowBound = blue;
            }

            if (blue > blueUpperBound)
            {
                blueUpperBound = blue;
            }
        }
    }

    /// <summary>
    /// Splits this cube's color list at median index, and returns two newly created cubes.
    /// </summary>
    /// <param name="componentIndex">Index of the component (red = 0, green = 1, blue = 2).</param>
    /// <param name="firstMedianCutCube">The first created cube.</param>
    /// <param name="secondMedianCutCube">The second created cube.</param>
    public void SplitAtMedian(sbyte componentIndex, out MedianCutCube firstMedianCutCube, out MedianCutCube secondMedianCutCube)
    {
        List<int> colors = componentIndex switch
        {
            // red colors
            0 => colorList.OrderBy(argb => Color.FromArgb(argb).R).ToList(),
            // green colors
            1 => colorList.OrderBy(argb => Color.FromArgb(argb).G).ToList(),
            // blue colors
            2 => colorList.OrderBy(argb => Color.FromArgb(argb).B).ToList(),
            _ => throw new NotSupportedException("Only three color components are supported (R, G and B)."),
        };

        // retrieves the median index (a half point)
        int medianIndex = colorList.Count >> 1;

        // creates the two half-cubes
        firstMedianCutCube = new MedianCutCube(colors.GetRange(0, medianIndex));
        secondMedianCutCube = new MedianCutCube(colors.GetRange(medianIndex, colors.Count - medianIndex));
    }

    /// <summary>
    /// Determines whether the color is within the cube.
    /// </summary>
    /// <param name="color">Target color</param>
    /// <returns>Whether cube contains target color.</returns>
    public bool IsColorIn(Color color)
    {
        int red = color.R;
        int green = color.G;
        int blue = color.B;
        return red >= redLowBound && red <= redUpperBound &&
               green >= greenLowBound && green <= greenUpperBound &&
               blue >= blueLowBound && blue <= blueUpperBound;
    }
}
