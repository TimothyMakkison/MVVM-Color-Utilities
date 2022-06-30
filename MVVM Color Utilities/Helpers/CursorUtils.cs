using System;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace MVVM_Color_Utilities.Helpers;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetCursorPos(ref System.Drawing.Point pt);

    [DllImport("user32.dll")]
    internal static extern IntPtr GetDC(IntPtr hwnd);

    [DllImport("user32.dll")]
    internal static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

    [DllImport("gdi32.dll")]
    internal static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);
}

/// <summary>
/// Contains color converters and pixel color finder.
/// </summary>
public static class CursorUtils
{
    private static System.Drawing.Point _cursor = new();

    /// <summary>
    /// Returns the color of the cursor position
    /// </summary>
    /// <returns></returns>
    public static Color GetCursorColor()
    {
        NativeMethods.GetCursorPos(ref _cursor);
        return GetPixelColor(_cursor.X, _cursor.Y);
    }

    /// <summary>
    /// Gets the color of the inputted location
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static Color GetPixelColor(int x, int y)
    {
        IntPtr hdc = NativeMethods.GetDC(IntPtr.Zero);
        uint pixel = NativeMethods.GetPixel(hdc, x, y);
        NativeMethods.ReleaseDC(IntPtr.Zero, hdc);
        Color color = Color.FromRgb((byte)(pixel & 0x000000FF),
                     (byte)((pixel & 0x0000FF00) >> 8),
                     (byte)((pixel & 0x00FF0000) >> 16));
        return color;
    }
}
