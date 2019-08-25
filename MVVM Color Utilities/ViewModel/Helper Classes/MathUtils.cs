using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ViewModel.Helper_Classes
{
    public static class MathUtils
    {
        /// <summary>
        /// Clamps value between upper and lower bounds.
        /// </summary>
        /// <param name="lowerBound">LowerBound</param>
        /// <param name="upperBound">UpperBound</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static int Clamp(int lowerBound, int upperBound, int value)
        {
            if (value < lowerBound)
                value = lowerBound;
            else if (value > upperBound)
                value = upperBound;
            return value;
        }
    }
}
