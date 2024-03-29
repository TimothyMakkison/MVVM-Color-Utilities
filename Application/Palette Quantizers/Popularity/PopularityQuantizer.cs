﻿using Application.Helpers.DistanceCalculator;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Application.Palette_Quantizers.PopularityQuantizer;

/// <summary>
/// Divides each colors values by 4, compressing them and then creates a palette from the most common values
/// and converting them back into color. This is similar to the normal popularity quantizer where each color
/// is added to a 64x64x64 grid made up of 4x4x4 cubes. The palette is then found by finding the most
/// populated cubes and returning the cubes average color. Memory savings are made in this implementation
/// by not saving the individual colors when placed in the grid, instead if a color is within a cube, the
/// cubes count increases, this does however mean an average of each cube cannot be found.
/// </summary>
public class PopularityQuantizer : IColorQuantizer
{
    private List<Color> _palette = new();

    private readonly IDistanceCalculator distanceCalculator = new ManhattenDistance();

    public string Name => "PopularityQuantizer";

    /// <summary>
    /// Returns a palette by grouping similar colors and returning the most frequent colors as a palette.
    /// </summary>
    /// <param name="colorCount">Number of colors in palette.</param>
    /// <param name="colorDictionary">Input colors and frequencies.</param>
    /// <returns>Palette as a list of colors.</returns>
    public List<Color> GetPalette(int colorCount, ConcurrentDictionary<int, int> colorDictionary)
    {
        if (!colorDictionary.IsEmpty)
        {
            //Compress each color either adding to dictionary or updating frequency.
            ConcurrentDictionary<int, int> gridIndexColorDict = new();
            foreach (int key in colorDictionary.Keys)
            {
                int gridIndex = DenaryToGridIndex(key);
                gridIndexColorDict.AddOrUpdate(gridIndex, colorDictionary[key],
                    (_, frequency) => frequency + colorDictionary[key]);
            }

            //Sort by frequency and return a number of colors equal to colorCount.
            var sortedDict = gridIndexColorDict.ToList();
            sortedDict.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

            _palette = sortedDict.Take(colorCount).Select(item => GridIndexToColor(item.Key)).ToList();
        }

        return _palette;
    }

    /// <summary>
    /// Returns index of the most similar color in Palette.
    /// </summary>
    /// <param name="color">Target Color</param>
    /// <returns>Index of most similar color.</returns>
    public int GetPaletteIndex(Color color)
    {
        int bestIndex = 0;
        int bestDistance = int.MaxValue;
        for (int i = 0; i < _palette.Count; i++)
        {
            int distance = distanceCalculator.Distance(color, _palette[i]);
            if (distance < bestDistance)
            {
                if (distance <= 27) //if color is in cell. 27 = 3 * 3^2
                {
                    return i;
                }
                bestDistance = distance;
                bestIndex = i;
            }
        }
        return bestIndex;
    }

    //TODO Refactor to make human readable
    /// <summary>
    /// Converts an integer form color into a 18 bit color.
    /// </summary>
    /// <param name="input">Color as an integer.</param>
    /// <returns>Compressed color integer.</returns>
    private static int DenaryToGridIndex(int input) => ((input & 0xFF0000) >> 18 << 12) | (input & 0xFF00) >> 10 << 6 | (input & 0xFF) >> 2;

    private static Color GridIndexToColor(int input)
    {
        int red = (input & 0x3F000) >> 10;
        int green = (input & 0xFC0) >> 4;
        int blue = (input & 0x3F) << 2;
        return Color.FromArgb(255, red, green, blue);
    }
}
