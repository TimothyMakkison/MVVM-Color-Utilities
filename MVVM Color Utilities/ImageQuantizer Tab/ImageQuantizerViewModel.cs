using Application.ImageBuffer;
using Application.Palette_Quantizers;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    /// <summary>
    /// ViewModel for ImageQuantizer, converts images into lower quality forms.
    /// </summary>
    public class ImageQuantizerViewModel : BindableBase
    {
        private Bitmap _bitmap;
        private string selectedPath;
        private IColorQuantizer selectedQuantizer;
        private int selectedColorCount;

        private readonly GeneralSettings _generalSettings;
        private readonly IFileDialog _fileDialog;
        private readonly IImageBuffer _imageBuffer;
        private readonly ILogger _logger;

        private System.Windows.Media.Imaging.BitmapImage generatedBitmap;

        public ImageQuantizerViewModel(GeneralSettings generalSettings, IFileDialog fileDialog, IImageBuffer imageBuffer, IEnumerable<IColorQuantizer> quantizerList, ILogger logger)
        {
            _generalSettings = generalSettings;
            _imageBuffer = imageBuffer;
            _fileDialog = fileDialog;

            QuantizerList = quantizerList.ToList();
            selectedQuantizer = QuantizerList[0];
            selectedColorCount = ColorCountList[4];

            OpenCommand = new DelegateCommand(LoadImageAndQuantize);
            SaveCommand = new DelegateCommand(DialogSaveImage);
            _logger = logger;
        }

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

        public List<IColorQuantizer> QuantizerList { get; }

        public IColorQuantizer SelectedQuantizer
        {
            get => selectedQuantizer;
            set
            {
                selectedQuantizer = value;
                _logger.Information($"IQ Quantizer set to {selectedQuantizer.Name}");
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
                _logger.Information($"IQ Color count set to {selectedColorCount}");
                GenerateNewImage();
            }
        }

        /// <summary>
        /// Opens file and exectues GenerateNewImage if selected item is valid.
        /// </summary>
        private void LoadImageAndQuantize()
        {
            if (_fileDialog.OpenImageDialogBox(out string path) && path != SelectedPath)
            {
                SelectedPath = path;

                _bitmap = new Bitmap(Image.FromFile(SelectedPath));
                GenerateNewImage();
            }
        }

        private void GenerateNewImage()
        {
            Task.Run(() =>
            {
                GeneratedBitmap = _imageBuffer.GenerateNewBitmap(_bitmap, SelectedQuantizer, SelectedColorCount)
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