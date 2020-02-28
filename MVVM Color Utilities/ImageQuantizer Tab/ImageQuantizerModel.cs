using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Drawing;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    internal class ImageQuantizerModel : ObservableObject
    {
        #region Fields
        private readonly ImageBuffer buffer = new ImageBuffer();
        #endregion

        #region Methods

        #region Get Methods
        public bool GetNewImage()
        {
            buffer.ScanBitmapColors();
            buffer.GetPalette();
            return buffer.GenerateNewImage();
        }
        #endregion

        #region Set Methods
        /// <summary>
        /// Sets the bitmap to be read and clears the saved colors so the new image can be proccessed.
        /// </summary>
        /// <param name="bitmap"></param>
        public void SetBitmap(Bitmap bitmap)
        {
            buffer.OriginalBitmap = bitmap;
        }
        public void SetColorCount(int colorCount)
        {
            buffer.ColorCount = colorCount;
        }
        /// <summary>
        /// Sets the quantizer that will read the bitmap.
        /// </summary>
        /// <param name="quantizer"></param>
        public void SetQuantizer(BaseColorQuantizer quantizer)
        {
            buffer.ActiveQuantizer = quantizer;
        }
        #endregion

        public Bitmap GeneratedBitmap
        {
            get
            {
                return buffer.GeneratedBitmap;
            }
        }
        /// <summary>
        /// Save generated image to location and with given type.
        /// </summary>
        /// <param name="path">Path</param>
        /// <param name="format">Image Format</param>
        public void SaveGeneratedImage(string path, System.Drawing.Imaging.ImageFormat format)
        {
            buffer.SaveGeneratedImage(path, format);
        }
        #endregion
    }
}
