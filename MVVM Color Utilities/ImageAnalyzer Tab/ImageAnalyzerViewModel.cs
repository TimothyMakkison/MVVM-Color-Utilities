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

        private AsyncObservableCollection<IAColorClass> sampleColorSource 
            = new AsyncObservableCollection<IAColorClass>();

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
        public AsyncObservableCollection<IAColorClass> SampleColorSource
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
        public ICommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                {
                    openCommand = new RelayCommand(param => OpenFile());
                }
                return openCommand;
            }
        }
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
            //var t = new List<Color>();
            //Task.Run(() =>
            //{
            //    SampleColorSource.Clear();
            //    t = imageBuffer.Palette;
            //    Debug.WriteLine(t.Count);

            //});
            //Debug.WriteLine(t.Count);

            SampleColorSource.Clear();
            foreach (Color color in imageBuffer.Palette)
            {
                SampleColorSource.Add(new IAColorClass(color));
            }
        }
        #endregion
    }
}
