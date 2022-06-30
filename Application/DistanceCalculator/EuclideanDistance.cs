using System;
using System.Drawing;

namespace Application.Helpers.DistanceCalculator;

/// <summary>
/// Finds the euclidean distance between two objects.
/// </summary>
public class EuclideanDistance : IDistanceCalculator
{
    int IDistanceCalculator.Distance(Color a, Color b)
    {
        int redDifference = Math.Abs(a.R - b.R);
        int greenDifference = Math.Abs(a.G - b.G);
        int blueDifference = Math.Abs(a.B - b.B);

        return (redDifference * redDifference)
            + (greenDifference * greenDifference)
            + (blueDifference * blueDifference);
    }
}
