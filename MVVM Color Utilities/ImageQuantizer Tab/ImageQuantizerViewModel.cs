using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    /// <summary>
    /// ViewModel for ImageQuantizer, converts images into lower quality forms.
    /// </summary>
    internal class ImageQuantizerViewModel : ObservableObject, IPageViewModel
    {
        private string selectedPath;
        private BaseColorQuantizer selectedQuantizer;
        private int selectedColorCount = 16;
        private readonly GeneralSettings generalSettings;

        private System.Windows.Media.Imaging.BitmapImage generatedBitmap;

        private ICommand openCommand;
        private ICommand saveCommand;

        //private readonly ImageQuantizerModel model = new ImageQuantizerModel();

        private readonly ImageBuffer imageBuffer = new ImageBuffer();

        public ImageQuantizerViewModel(GeneralSettings generalSettings)
        {
            this.generalSettings = generalSettings;
            selectedQuantizer = QuantizerList[0];
            imageBuffer.ActiveQuantizer = SelectedQuantizer;
            imageBuffer.ColorCount = SelectedColorCount;
        }

        public PackIconKind Icon => PackIconKind.PaletteAdvanced;

        /// <summary>
        /// Button displays image from this location.
        /// </summary>
        public string SelectedPath
        {
            get => selectedPath;
            set => Set(ref selectedPath, value);
        }

        /// <summary>
        /// Displayed by save bitmap button
        /// </summary>
        public System.Windows.Media.Imaging.BitmapImage GeneratedBitmap
        {
            get => generatedBitmap;
            set => Set(ref generatedBitmap, value);
        }

        public List<BaseColorQuantizer> QuantizerList => generalSettings.QuantizerList;

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

        public List<int> ColorCountList => generalSettings.ColorCountList;

        public int SelectedColorCount
        {
            get => selectedColorCount;
            set
            {
                selectedColorCount = value;
                imageBuffer.ColorCount = selectedColorCount;
                Debug.WriteLine("IQ Color count set to " + selectedColorCount.ToString());
                GenerateNewImage();
            }
        }

        public ICommand OpenCommand => PatternHandler.Singleton(ref openCommand, DialogGetImage);
        public ICommand SaveCommand => PatternHandler.Singleton(ref saveCommand, DialogSaveImage);

        /// <summary>
        /// Opens file and exectues GenerateNewImage if selected item is valid.
        /// </summary>
        private void DialogGetImage()
        {
            //Checks that the path exists and is not repeating itself.
            if (generalSettings.OpenDialogBox.ShowDialog() == true && SelectedPath != generalSettings.OpenDialogBox.FileName)
            {
                SelectedPath = generalSettings.OpenDialogBox.FileName;
                imageBuffer.OriginalBitmap = new Bitmap(Image.FromFile(SelectedPath));

                GenerateNewImage();
            }
        }

        /// <summary>
        /// Opens save dialog and saves generated image.
        /// </summary>
        private void DialogSaveImage()
        {
            if (generalSettings.SaveDialogBox.ShowDialog() == true)
            {
                string path = generalSettings.SaveDialogBox.FileName;
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
                    GeneratedBitmap = imageBuffer.GeneratedBitmap.ConvertToBitmapImage();
                }
            });
        }
    }
}