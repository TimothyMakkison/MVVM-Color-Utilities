using System.Drawing;

namespace MVVM_Color_Utilities.Helpers.DistanceCalculator
{
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
}