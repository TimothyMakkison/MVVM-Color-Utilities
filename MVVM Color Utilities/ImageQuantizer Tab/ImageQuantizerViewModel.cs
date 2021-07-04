using Application.ImageBuffer;
using Application.Palette_Quantizers;
using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
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
        private IColorQuantizer selectedQuantizer;
        private int selectedColorCount;

        private readonly GeneralSettings _generalSettings;
        private readonly IFileDialog _fileDialog;
        private readonly IImageBuffer _imageBuffer;

        private System.Windows.Media.Imaging.BitmapImage generatedBitmap;

        private ICommand openCommand;
        private ICommand saveCommand;

        public ImageQuantizerViewModel(GeneralSettings generalSettings, IFileDialog fileDialog, IImageBuffer imageBuffer)
        {
            _generalSettings = generalSettings;
            _imageBuffer = imageBuffer;
            _fileDialog = fileDialog;

            selectedQuantizer = QuantizerList[0];
            selectedColorCount = ColorCountList[4];

            _imageBuffer.SetQuantizer(SelectedQuantizer);
            _imageBuffer.SetColorCount(SelectedColorCount);
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

        public List<IColorQuantizer> QuantizerList => _generalSettings.QuantizerList;

        public IColorQuantizer SelectedQuantizer
        {
            get => selectedQuantizer;
            set
            {
                selectedQuantizer = value;
                _imageBuffer.SetQuantizer(selectedQuantizer);
                Debug.WriteLine("IQ Quantizer set to " + selectedQuantizer.Name);
                GenerateNewImage();
            }
        }

        public List<int> ColorCountList => _generalSettings.ColorCountList;

        public int SelectedColorCount
        {
            get => selectedColorCount;
            set
            {
                selectedColorCount = value;
                _imageBuffer.SetColorCount(selectedColorCount);
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
            if (_fileDialog.OpenImageDialogBox(out string path) && path != SelectedPath)
            {
                SelectedPath = path;

                var bitmap = new Bitmap(Image.FromFile(SelectedPath));
                _imageBuffer.SetBitmap(bitmap);
                GenerateNewImage();
            }
        }

        /// <summary>
        /// Opens save dialog and saves generated image.
        /// </summary>
        private void DialogSaveImage()
        {
            if (_fileDialog.SaveImageDialogBox(out string path))
            {
                GeneratedBitmap.ToBitmap().SaveImage(path, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        /// <summary>
        /// Generates new image and then displays it.
        /// </summary>
        private void GenerateNewImage()
        {
            Task.Run(() =>
            {
                GeneratedBitmap = _imageBuffer.GenerateNewBitmap()
                                             .ConvertToBitmapImage();
            });
        }
    }
}