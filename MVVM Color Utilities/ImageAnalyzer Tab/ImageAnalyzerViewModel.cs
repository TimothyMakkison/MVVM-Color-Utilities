using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    /// <summary>
    /// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
    /// </summary>
    internal class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        private string selectedPath;
        private BaseColorQuantizer selectedQuantizer;
        private int selectedColorCount;

        private List<IAColorClass> sampleColorSource = new List<IAColorClass>();

        private ICommand openCommand;

        private readonly GeneralSettings generalSettings;
        private readonly ImageBuffer imageBuffer = new ImageBuffer();

        private readonly ColorDataContext dataContext;
        #region Constructor

        public ImageAnalyzerViewModel(GeneralSettings generalSettings, ColorDataContext colorDataContext)
        {
            this.generalSettings = generalSettings;
            this.dataContext = colorDataContext;
            SaveCommand = new RelayCommand(x => Save(x));

            selectedColorCount = ColorCountList[4];
            selectedQuantizer = QuantizerList[0];

            imageBuffer.ActiveQuantizer = SelectedQuantizer;
            imageBuffer.ColorCount = SelectedColorCount;
        }

        #endregion Constructor

        #region Properties

        public PackIconKind Icon => PackIconKind.Paint;

        /// <summary>
        /// Button displays image from this location.
        /// </summary>
        public string SelectedPath
        {
            get => selectedPath;
            set => Set(ref selectedPath, value);
        }

        /// <summary>
        /// Contains image palette
        /// </summary>
        public List<IAColorClass> SampleColorSource
        {
            get => sampleColorSource;
            set => Set(ref sampleColorSource, value);
        }

        public ICommand SaveCommand { get; }

        private void Save(object item)
        {
            var a = item as IAColorClass;
            //TODO fix id.
            dataContext.Add(new ColorModel(4,a.ColorHex,""));
        }

        #region QuantizerList

        public List<BaseColorQuantizer> QuantizerList => generalSettings.QuantizerList;

        public BaseColorQuantizer SelectedQuantizer
        {
            get => selectedQuantizer;
            set
            {
                selectedQuantizer = value;
                imageBuffer.ActiveQuantizer = selectedQuantizer;
                Debug.WriteLine("IA Quantizer set to " + selectedQuantizer.Name.ToString());
                GetNewPalette();
            }
        }

        #endregion QuantizerList

        #region ColorCountList

        public List<int> ColorCountList => generalSettings.ColorCountList;

        public int SelectedColorCount
        {
            get => selectedColorCount;
            set
            {
                selectedColorCount = value;
                imageBuffer.ColorCount = selectedColorCount;
                Debug.WriteLine("IA Color count set to " + selectedColorCount.ToString());
                GetNewPalette();
            }
        }

        #endregion ColorCountList

        #endregion Properties

        #region Commands

        public ICommand OpenCommand => PatternHandler.Singleton(ref openCommand, OpenFile);

        #endregion Commands

        #region Methods

        /// <summary>
        /// Opens a dilog box and if a selection is made, a new palette is created.
        /// </summary>
        private void OpenFile()
        {
            if (generalSettings.OpenDialogBox.ShowDialog() == true && SelectedPath != generalSettings.OpenDialogBox.FileName) //Checks that the path exists and is not the previous path.
            {
                SelectedPath = generalSettings.OpenDialogBox.FileName;
                imageBuffer.OriginalBitmap = new Bitmap(Image.FromFile(SelectedPath));
                GetNewPalette();
            }
        }

        /// <summary>
        /// Clears previous palette and gets new colors.
        /// </summary>
        private void GetNewPalette()
        {
            Task.Run(() =>
            {
                SampleColorSource.Clear();
                SampleColorSource = imageBuffer.Palette.Select(x => new IAColorClass(x)).ToList();
            });
        }

        #endregion Methods
    }
}