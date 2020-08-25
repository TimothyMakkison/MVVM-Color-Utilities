using System;
using System.Drawing;
using System.Linq;

namespace MVVM_Color_Utilities.Helpers.DistanceCalculator
{
    /// <summary>
    /// Calculates the Chebyshev distance between two points.
    /// This is done by finding the largest distance between each norm.
    /// </summary>
    public class ChebyshevDistance : IDistanceCalculator
    {
        int IDistanceCalculator.Distance(Color a, Color b)
        {
            int[] distances = new int[3]
            {
                Math.Abs(a.R-b.R),
                Math.Abs(a.G-b.G),
                Math.Abs(a.B-b.B),
            };
            return distances.Max();
        }
    }
}