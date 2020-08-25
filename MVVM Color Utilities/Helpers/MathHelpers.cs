namespace MVVM_Color_Utilities.Helpers
{
    /// <summary>
    /// Contains useful math functions.
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Returns a boolean value if value is within given range.
        /// </summary>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static bool TestRange(int lower, int upper, int value)
        {
            return value > lower && value < upper ? true : false;
        }

        //TODO implement IComparable TestRange

        //TODO refactor clamp

        /// <summary>
        /// Clamps value between upper and lower bounds.
        /// </summary>
        /// <param name="lowerBound">LowerBound</param>
        /// <param name="upperBound">UpperBound</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
        public static int Clamp(int lowerBound, int upperBound, int value)
        {
            if (upperBound < lowerBound)
                upperBound = lowerBound;
            if (value < lowerBound)
                value = lowerBound;
            else if (value > upperBound)
                value = upperBound;
            return value;
        }
    }
}