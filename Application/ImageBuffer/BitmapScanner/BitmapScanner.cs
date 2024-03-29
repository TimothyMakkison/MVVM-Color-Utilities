﻿using Serilog;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Application.ImageBuffer.BitmapScanner;

public class BitmapScanner : IBitmapScanner
{
    private readonly ILogger _logger;

    public BitmapScanner(ILogger logger)
    {
        _logger = logger;
    }

    public ConcurrentDictionary<int, int> Scan(Bitmap bitmap)
    {
        _logger.Information($"Scanning bitmap for colors, {bitmap.Size}");

        if (bitmap is null)
        {
            return new ConcurrentDictionary<int, int>();
        }

        ConcurrentDictionary<int, int> colorDict = new();

        //Gets the raw pixel data from bitmap and reads each 4 byte segment as a color.
        using (Bitmap lockableBitmap = new(bitmap))
        {
            //Get raw bitmap data
            BitmapData bitmapData = lockableBitmap.LockBits(new Rectangle(0, 0, lockableBitmap.Width, lockableBitmap.Height),
                                    ImageLockMode.ReadOnly,
                                    lockableBitmap.PixelFormat);

            IntPtr pixelBytes = bitmapData.Scan0; //Get byte array of every pixel
            byte[] Pixels = new byte[bitmap.Width * bitmap.Height * 4];
            Marshal.Copy(pixelBytes, Pixels, 0, Pixels.Length);

            //Iterate through each 4 byte group calculating the color value
            Parallel.For(0, Pixels.Length / 4, i =>
            {
                i *= 4;
                int key = Pixels[i + 2] << 16 | Pixels[i + 1] << 8 | Pixels[i];
                colorDict.AddOrUpdate(key, 1, (_, value) => value + 1);
            });
        }

        var totalPixels = bitmap.Width * bitmap.Height;
        _logger.Information($"ScanBitmap Success, Found {colorDict.Count} unique colors from {totalPixels} pixels.");
        return colorDict;
    }
}
