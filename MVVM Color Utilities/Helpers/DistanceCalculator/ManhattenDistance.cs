using System;
using System.Drawing;

namespace MVVM_Color_Utilities.Helpers.DistanceCalculator
{
    /// <summary>
    /// Finds the manhatten distance between two objects.
    /// </summary>
    public class ManhattenDistance : IDistanceCalculator
    {
        public int Distance(Color a, Color b)
        {
            int redDifference = Math.Abs(a.R - b.R);
            int greenDifference = Math.Abs(a.G - b.G);
            int blueDifference = Math.Abs(a.B - b.B);

            return redDifference + greenDifference + blueDifference;
        }
    }
}