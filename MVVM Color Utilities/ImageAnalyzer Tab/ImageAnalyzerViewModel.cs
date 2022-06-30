using Application.ImageBuffer;
using Application.Palette_Quantizers;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using MVVM_Color_Utilities.Models;
using Prism.Commands;
using Prism.Mvvm;
using Serilog;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab;

/// <summary>
/// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
/// </summary>
public class ImageAnalyzerViewModel : BindableBase
{
    private Bitmap _bitmap;
    private string selectedPath;
    private IColorQuantizer selectedQuantizer;
    private int selectedColorCount;
    private readonly IFileDialog _fileDialog;

    private List<ColorModel> sampleColorSource = new();

    private readonly GeneralSettings _generalSettings;
    private readonly IImageBuffer _imageBuffer;

    private readonly IDataContext<ColorModel> _dataContext;
    private readonly ILogger _logger;

    public ImageAnalyzerViewModel(
        GeneralSettings generalSettings,
        IDataContext<ColorModel> colorDataContext,
        IFileDialog fileDialog,
        IImageBuffer imageBuffer,
        IEnumerable<IColorQuantizer> quantizerList,
        ILogger logger)
    {
        _generalSettings = generalSettings;
        _fileDialog = fileDialog;
        _imageBuffer = imageBuffer;

        _dataContext = colorDataContext;

        QuantizerList = quantizerList.ToList();
        selectedColorCount = ColorCountList[4];
        selectedQuantizer = QuantizerList[0];

        SaveCommand = new DelegateCommand<ColorModel>(SaveColor);
        OpenCommand = new DelegateCommand(OpenFile);
        _logger = logger;
    }

    public DelegateCommand<ColorModel> SaveCommand { get; }
    public DelegateCommand OpenCommand { get; }

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
            _logger.Information($"IA Quantizer set to {selectedQuantizer.Name}");
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
            _logger.Information($"IA Color count set to {selectedColorCount}");
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
            _bitmap = new Bitmap(Image.FromFile(SelectedPath));

            GetNewPalette();
        }
    }

    /// <summary>
    /// Clears previous palette and gets new colors.
    /// </summary>
    private async void GetNewPalette()
    {
        var result = await Task.Run(() => _imageBuffer.GetPalette(_bitmap, SelectedQuantizer, SelectedColorCount));
        SampleColorSource = result.Select(color => new ColorModel(color))
                                           .ToList();
    }
}
