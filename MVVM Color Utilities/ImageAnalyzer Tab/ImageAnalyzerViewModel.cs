using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Input;
using MVVM_Color_Utilities.Helpers.Derived_Classes;
using System.Linq;
//using PatternHelper;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    /// <summary>
    /// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
    /// </summary>
    internal class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string selectedPath;
        private BaseColorQuantizer selectedQuantizer = QuantizerList[0];
        private int selectedColorCount = ColorCountList[4];

        private List<IAColorClass> sampleColorSource = new List<IAColorClass>();

        private ICommand openCommand;

        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;
        private readonly ImageBuffer imageBuffer = new ImageBuffer();
        #endregion

        #region Constructor
        public ImageAnalyzerViewModel()
        {
            imageBuffer.ActiveQuantizer = SelectedQuantizer;
            imageBuffer.ColorCount = SelectedColorCount;
        }
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.Paint;
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
        public List<IAColorClass> SampleColorSource
        {
            get => sampleColorSource;
            set => SetProperty(ref sampleColorSource, value);
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
                Debug.WriteLine("IA Quantizer set to " + selectedQuantizer.Name.ToString());
                GetNewPalette();
            }
        }
        #endregion

        #region ColorCountList
        public static List<int> ColorCountList => ImageBufferItems.ColorCountList;
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
        #endregion

        #endregion

        #region Commands
        public ICommand OpenCommand => PatternHandler.Singleton(ref openCommand, OpenFile);
        #endregion

        #region Methods
        /// <summary>
        /// Opens a dilog box and if a selection is made, a new palette is created.
        /// </summary>
        private void OpenFile()
        {
            if (dialogBox.ShowDialog() == true && SelectedPath != dialogBox.FileName) //Checks that the path exists and is not the previous path.
            {
                SelectedPath = dialogBox.FileName;
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
        #endregion
    }
}
