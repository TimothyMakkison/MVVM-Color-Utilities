﻿using System.Drawing;

namespace Application.Helpers.DistanceCalculator;

public interface IDistanceCalculator
{
    /// <summary>
    /// Finds the distance between two colors.
    /// </summary>
    /// <param name="a">First color</param>
    /// <param name="b">Second color</param>
    /// <returns>Distance between colors</returns>
    int Distance(Color a, Color b);
}
