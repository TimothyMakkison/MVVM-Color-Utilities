using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using Microsoft.Win32;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    /// <summary>
    /// ViewModel for ImageQuantizer, converts images into lower quality forms.
    /// </summary>
    class ImageQuantizerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string selectedPath;
        private BaseColorQuantizer selectedQuantizer = QuantizerList[0];
        private int selectedColorCount = 16;

        System.Windows.Media.Imaging.BitmapImage generatedBitmap;

        private ICommand openCommand;
        private ICommand saveCommand;

        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;
        private readonly SaveFileDialog saveDialogBox = ImageBufferItems.SaveDialogBox;
        //private readonly ImageQuantizerModel model = new ImageQuantizerModel();

        private readonly ImageBuffer imageBuffer = new ImageBuffer();
        #endregion

        #region Constructor
        public ImageQuantizerViewModel()
        {
            imageBuffer.ActiveQuantizer = SelectedQuantizer;
            imageBuffer.ColorCount = SelectedColorCount;
        }
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.PaletteAdvanced;
        /// <summary>
        /// Button displays image from this location.
        /// </summary>
        public string SelectedPath
        {
            get => selectedPath;
            set => SetProperty(ref selectedPath, value);
        }
        /// <summary>
        /// Displayed by save bitmap button
        /// </summary>
        public System.Windows.Media.Imaging.BitmapImage GeneratedBitmap
        {
            get => generatedBitmap;
            set => SetProperty(ref generatedBitmap, value);
        }

        #region QuantizerList
        public static List<BaseColorQuantizer> QuantizerList => ImageBufferItems.QuantizerList;
        public BaseColorQuantizer SelectedQuantizer
        {
            get => selectedQuantizer;
            set
            {
                selectedQuantizer = value;
                imageBuffer.ActiveQuantizer = selectedQuantizer;
                Debug.WriteLine("IQ Quantizer set to " + selectedQuantizer.Name.ToString());
                GenerateNewImage();
            }
        }
        #endregion

        #region ColorCountList
        public List<int> ColorCountList => ImageBufferItems.ColorCountList;
        public int SelectedColorCount
        {
            get => selectedColorCount;
            set
            {
                selectedColorCount = value;
                imageBuffer.ColorCount  = selectedColorCount;
                Debug.WriteLine("IQ Color count set to " + selectedColorCount.ToString());
                GenerateNewImage();
            }
        }
        #endregion

        #endregion

        #region Commands
        public ICommand OpenCommand => PatternHandler.Singleton(ref openCommand, DialogGetImage);
        public ICommand SaveCommand => PatternHandler.Singleton(ref saveCommand, DialogSaveImage);
        #endregion

        #region Methods
        /// <summary>
        /// Opens file and exectues GenerateNewImage if selected item is valid.
        /// </summary>
        private void DialogGetImage()
        {
            //Checks that the path exists and is not repeating itself.
            if (dialogBox.ShowDialog()==true && SelectedPath != dialogBox.FileName)
            {
                SelectedPath = dialogBox.FileName;
                imageBuffer.OriginalBitmap = new Bitmap(Image.FromFile(SelectedPath));

                GenerateNewImage();
            }
        }

        /// <summary>
        /// Opens save dialog and saves generated image.
        /// </summary>
        private void DialogSaveImage()
        {
            if (saveDialogBox.ShowDialog() == true)
            {
                string path = saveDialogBox.FileName;
                imageBuffer.SaveGeneratedImage(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }
        /// <summary>
        /// Generates new image and then displays it.
        /// </summary>
        private void GenerateNewImage()
        {
            Task.Run(() =>
            {
                if (!imageBuffer.GeneratedBitmap.IsNull())
                {
                    GeneratedBitmap = Imageutils.ConvertToBitmapImage(imageBuffer.GeneratedBitmap);
                }
            });
        }
        #endregion
    }
}
