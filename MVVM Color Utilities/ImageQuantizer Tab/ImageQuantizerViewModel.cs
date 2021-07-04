using Application.ImageBuffer;
using Application.Palette_Quantizers;
using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using MVVM_Color_Utilities.ViewModel;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    /// <summary>
    /// ViewModel for ImageQuantizer, converts images into lower quality forms.
    /// </summary>
    internal class ImageQuantizerViewModel : BindableBase, IPageViewModel
    {
        private string selectedPath;
        private IColorQuantizer selectedQuantizer;
        private int selectedColorCount;

        private readonly GeneralSettings _generalSettings;
        private readonly IFileDialog _fileDialog;
        private readonly IImageBuffer _imageBuffer;

        private System.Windows.Media.Imaging.BitmapImage generatedBitmap;

        public ImageQuantizerViewModel(GeneralSettings generalSettings, IFileDialog fileDialog, IImageBuffer imageBuffer)
        {
            _generalSettings = generalSettings;
            _imageBuffer = imageBuffer;
            _fileDialog = fileDialog;

            selectedQuantizer = QuantizerList[0];
            selectedColorCount = ColorCountList[4];

            _imageBuffer.SetQuantizer(SelectedQuantizer);
            _imageBuffer.SetColorCount(SelectedColorCount);

            OpenCommand = new DelegateCommand(LoadImageAndQuatize);
            SaveCommand = new DelegateCommand(DialogSaveImage);
        }

        public PackIconKind Icon => PackIconKind.PaletteAdvanced;

        public DelegateCommand OpenCommand { get; }
        public DelegateCommand SaveCommand { get; }

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

        /// <summary>
        /// Opens file and exectues GenerateNewImage if selected item is valid.
        /// </summary>
        private void LoadImageAndQuatize()
        {
            if (_fileDialog.OpenImageDialogBox(out string path) && path != SelectedPath)
            {
                SelectedPath = path;

                var bitmap = new Bitmap(Image.FromFile(SelectedPath));
                _imageBuffer.SetBitmap(bitmap);
                GenerateNewImage();
            }
        }

        private void GenerateNewImage()
        {
            Task.Run(() =>
            {
                GeneratedBitmap = _imageBuffer.GenerateNewBitmap()
                                             .ConvertToBitmapImage();
            });
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
    }
}