namespace MVVM_Color_Utilities.Helpers
{
    /// <summary>
    /// Contains useful math functions.
    /// </summary>
    public static class MathUtils
    {
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