using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    /// <summary>
    /// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
    /// </summary>
    internal class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string _selectedPath;
        private BaseColorQuantizer _selectedQuantizer = QuantizerList[0];
        private int _selectedColorCount = 16;

        private ObservableCollection<IAColorClass> _sampleColorSource;

        private ICommand _openCommand;

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
            get => _selectedPath;
            set => SetProperty(ref _selectedPath, value);
        }
        /// <summary>
        /// Contains image palette
        /// </summary>
        public ObservableCollection<IAColorClass> SampleColorSource
        {
            get => _sampleColorSource;
            set => SetProperty(ref _sampleColorSource, value);
        }
        #region QuantizerList
        public static List<BaseColorQuantizer> QuantizerList => ImageBufferItems.QuantizerList;
        public BaseColorQuantizer SelectedQuantizer
        {
            get => _selectedQuantizer;
            set
            {
                _selectedQuantizer = value;
                imageBuffer.ActiveQuantizer = _selectedQuantizer;
                Debug.WriteLine("IA Quantizer set to " + _selectedQuantizer.Name.ToString());
                GetNewPalette();
            }
        }
        #endregion

        #region ColorCountList
        public List<int> ColorCountList => ImageBufferItems.ColorCountList;
        public int SelectedColorCount
        {
            get => _selectedColorCount;
            set
            {
                _selectedColorCount = value;
                imageBuffer.ColorCount = _selectedColorCount;
                Debug.WriteLine("IA Color count set to " + _selectedColorCount.ToString());
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
                if (_openCommand == null)
                {
                    _openCommand = new RelayCommand(param => OpenFile());
                }
                return _openCommand;
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
            ObservableCollection<IAColorClass> newColorSource = new ObservableCollection<IAColorClass>();
            foreach (Color color in imageBuffer.Palette)
            {
                newColorSource.Add(new IAColorClass(color));
            }

            SampleColorSource = newColorSource;
        }
        #endregion
    }
}
