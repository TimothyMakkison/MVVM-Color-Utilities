using Application.ImageBuffer;
using Application.Palette_Quantizers;
using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using MVVM_Color_Utilities.Models;
using MVVM_Color_Utilities.ViewModel;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    /// <summary>
    /// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
    /// </summary>
    public class ImageAnalyzerViewModel : BindableBase, IPageViewModel
    {
        private string selectedPath;
        private IColorQuantizer selectedQuantizer;
        private int selectedColorCount;
        private readonly IFileDialog _fileDialog;

        private List<ColorModel> sampleColorSource = new();

        private readonly GeneralSettings _generalSettings;
        private readonly IImageBuffer _imageBuffer;

        private readonly IDataContext<ColorModel> _dataContext;

        public ImageAnalyzerViewModel(
            GeneralSettings generalSettings,
            IDataContext<ColorModel> colorDataContext,
            IFileDialog fileDialog,
            IImageBuffer imageBuffer,
            IEnumerable<IColorQuantizer> quantizerList)
        {
            _generalSettings = generalSettings;
            _fileDialog = fileDialog;
            _imageBuffer = imageBuffer;

            _dataContext = colorDataContext;

            QuantizerList = quantizerList.ToList();
            selectedColorCount = ColorCountList[4];
            selectedQuantizer = QuantizerList[0];

            _imageBuffer.SetQuantizer(SelectedQuantizer);
            _imageBuffer.SetColorCount(SelectedColorCount);

            SaveCommand = new DelegateCommand<ColorModel>(SaveColor);
            OpenCommand = new DelegateCommand(OpenFile);
        }

        public DelegateCommand<ColorModel> SaveCommand { get; }
        public DelegateCommand OpenCommand { get; }

        public PackIconKind Icon => PackIconKind.Paint;
        public List<IColorQuantizer> QuantizerList { get; }

        /// <summary>
        /// Button displays image from this location.
        /// </summary>
        public string SelectedPath
        {
            get => selectedPath;
            set => SetProperty(ref selectedPath, value);
        }

        /// <summary>
        /// Contains image palette
        /// </summary>
        public List<ColorModel> SampleColorSource
        {
            get => sampleColorSource;
            set => SetProperty(ref sampleColorSource, value);
        }

        private void SaveColor(ColorModel item)
        {
            //TODO fix id.
            _dataContext.Add(new ColorModel(item.Color)).Save();
        }

        public IColorQuantizer SelectedQuantizer
        {
            get => selectedQuantizer;
            set
            {
                selectedQuantizer = value;
                _imageBuffer.SetQuantizer(selectedQuantizer);
                Debug.WriteLine("IA Quantizer set to " + selectedQuantizer.Name);
                GetNewPalette();
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
                Debug.WriteLine("IA Color count set to " + selectedColorCount.ToString());
                GetNewPalette();
            }
        }

        /// <summary>
        /// Opens a dilog box and if a selection is made, a new palette is created.
        /// </summary>
        private void OpenFile()
        {
            //Checks that the path exists and is not the previous path.
            if (_fileDialog.OpenImageDialogBox(out string path) && SelectedPath != path)
            {
                SelectedPath = path;
                var bitmap = new Bitmap(Image.FromFile(SelectedPath));

                _imageBuffer.SetBitmap(bitmap);
                GetNewPalette();
            }
        }

        /// <summary>
        /// Clears previous palette and gets new colors.
        /// </summary>
        private async void GetNewPalette()
        {
            var result = await Task.Run(_imageBuffer.GetPalette);
            SampleColorSource = result.Select(color => new ColorModel(color))
                                               .ToList();
        }
    }
}