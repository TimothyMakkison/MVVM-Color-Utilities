using System.Runtime.CompilerServices;

namespace MVVM_Color_Utilities.Helpers.Extensions;

public static class ColorExtension
{
    /// <summary>
    /// Convert Media Color (WPF) to Drawing Color (WinForm)
    /// </summary>
    /// <param name="mediaColor"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static System.Drawing.Color ToDrawingColor(this System.Windows.Media.Color mediaColor)
    {
        return System.Drawing.Color.FromArgb(mediaColor.A, mediaColor.R, mediaColor.G, mediaColor.B);
    }

    /// <summary>
    /// Convert Drawing Color (WPF) to Media Color (WinForm)
    /// </summary>
    /// <param name="drawingColor"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static System.Windows.Media.Color ToMediaColor(this System.Drawing.Color drawingColor)
    {
        return System.Windows.Media.Color.FromArgb(drawingColor.A, drawingColor.R, drawingColor.G, drawingColor.B);
    }

    public static System.Windows.Media.SolidColorBrush ToBrush(this System.Drawing.Color color)
    {
        return new System.Windows.Media.SolidColorBrush(color.ToMediaColor());
    }

    public static string ToHex(this System.Drawing.Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
