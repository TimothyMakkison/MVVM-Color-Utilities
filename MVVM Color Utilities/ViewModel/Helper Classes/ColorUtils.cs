using System;
using System.Windows.Media;
using System.Runtime.InteropServices;

namespace MVVM_Color_Utilities.ViewModel.Helper_Classes
{
    public class ColorUtils
    {
        #region Fields + Static Extern
        private System.Drawing.Point _cursor = new System.Drawing.Point();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref System.Drawing.Point pt);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
        #endregion

        #region Methods
        /// <summary>
        /// Returns the color of the cursor position
        /// </summary>
        /// <returns></returns>
        public Color GetCursorColor()
        {
            GetCursorPos(ref _cursor);
            return GetPixelColor(_cursor.X, _cursor.Y);
        }
        /// <summary>
        /// Gets the color of the inputted location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static public Color GetPixelColor(int x, int y)
        {
            IntPtr hdc = GetDC(IntPtr.Zero);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                         (byte)((pixel & 0x0000FF00) >> 8),
                         (byte)((pixel & 0x00FF0000) >> 16));
            return color;
        }
        #endregion
    }
}
//Color color = Color.FromArgb((int)(pixel & 0x000000FF),
//                        (int)(pixel & 0x0000FF00) >> 8,
//                        (int)(pixel & 0x00FF0000) >> 16);