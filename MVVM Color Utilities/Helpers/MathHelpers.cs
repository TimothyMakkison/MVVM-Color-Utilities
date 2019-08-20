using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.Helpers
{
    public static class MathHelpers
    {
        /// <summary>
        /// Returns a boolean value if value is within given range.
        /// </summary>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static bool TestRange(int lower, int upper,int value)
        {
            return value>lower && value<upper ?true:false;
        }
    }
}
