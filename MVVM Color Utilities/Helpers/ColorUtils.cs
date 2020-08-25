using System;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace MVVM_Color_Utilities.Helpers
{
    internal static class NativeMethods
    {
        #region Fields + Static Extern

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref System.Drawing.Point pt);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        internal static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        #endregion Fields + Static Extern
    }

    /// <summary>
    /// Contains color converters and pixel color finder.
    /// </summary>
    public static class ColorUtils
    {
        #region Fields

        private static System.Drawing.Point _cursor = new System.Drawing.Point();

        #endregion Fields

        #region Methods

        #region Get cursor color

        /// <summary>
        /// Returns the color of the cursor position
        /// </summary>
        /// <returns></returns>
        public static Color GetCursorColor()
        {
            NativeMethods.GetCursorPos(ref _cursor);
            return GetPixelColor(_cursor.X, _cursor.Y);
        }

        #endregion Get cursor color

        #region Get color on screen

        /// <summary>
        /// Gets the color of the inputted location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static public Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
            uint pixel = NativeMethods.GetPixel(hdc, x, y);
            NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                         (byte)((pixel & 0x0000FF00) >> 8),
                         (byte)((pixel & 0x00FF0000) >> 16));
            return color;
        }

        #endregion Get color on screen

        #region ColorToHex

        /// <summary>
        /// Converts System.Drawing.Color to hex code.
        /// </summary>
        /// <param name="color">Drawing Color</param>
        /// <returns>Hex Code</returns>
        static public string ColorToHex(System.Drawing.Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        /// <summary>
        /// System.Windows.Media.Color to hex code.
        /// </summary>
        /// <param name="color">Media color</param>
        /// <returns>Hex code</returns>
        static public string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        #endregion ColorToHex

        #region Media Color to Drawing Color

        /// <summary>
        /// Converts System.Windows.Media.Color to System.Drawing.Color.
        /// </summary>
        /// <param name="color">Drawing Color</param>
        /// <returns>Media Color</returns>
        static public Color DrawingToMediaColor(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        #endregion Media Color to Drawing Color

        #region Color to SolidColorBrush

        /// <summary>
        /// Converts System.Windows.Media.Color to a SolidColorBrush.
        /// </summary>
        /// <param name="color">System.Windows.Media Color</param>
        /// <returns>SolidColorBrush</returns>
        static public SolidColorBrush ColorToBrush(Color color)
        {
            return new SolidColorBrush(color);
        }

        /// <summary>
        /// Converts System.Drawing.Color to a SolidColorBrush.
        /// </summary>
        /// <param name="color">System.Drawing Color</param>
        /// <returns>SolidColorBrush</returns>
        static public SolidColorBrush ColorToBrush(System.Drawing.Color color)
        {
            return new SolidColorBrush(DrawingToMediaColor(color));
        }

        #endregion Color to SolidColorBrush

        #endregion Methods
    }
}