using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using MVVM_Color_Utilities.Helpers;
using System.Windows.Input;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Reflection;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    /// <summary>
    /// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
    /// </summary>
    class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string _selectedPath;
        private BaseColorQuantizer _selectedQuantizer = QuantizerList[0];
        private int _selectedColorCount =16;

        private ObservableCollection<IAColorClass> _sampleColorSource;

        private ICommand _openCommand;

        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;
        private readonly ImageAnalyzerModel model = new ImageAnalyzerModel();
        #endregion

        #region Constructor
        public ImageAnalyzerViewModel()
        {
            model.SetQuantizer(SelectedQuantizer);
            model.SetColorCount(SelectedColorCount );
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
                model.SetQuantizer(_selectedQuantizer);
                Debug.WriteLine("IA Quantizer set to " + _selectedQuantizer.Name.ToString());
                //Dispatcher.CurrentDispatcher.Invoke(() => SampleColorSource = GetNewPalette());
                //Task.Run(() => GetNewPalette());
                GetNewPalette();
            }
        }
        #endregion

        #region ColorCountList
        public List<Int32> ColorCountList => ImageBufferItems.ColorCountList;
        public int SelectedColorCount
        {
            get => _selectedColorCount;

            set
            {
                _selectedColorCount = value;
                model.SetColorCount(_selectedColorCount);
                Debug.WriteLine("IA Color count set to " + _selectedColorCount.ToString());
                //Dispatcher.CurrentDispatcher.Invoke(() => GetNewPalette());
                //Task.Run(() => GetNewPalette());
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
                model.SetBitmap(new Bitmap(Image.FromFile(SelectedPath)));
                //Dispatcher.CurrentDispatcher.Invoke(() => GetNewPalette());
                //Task.Run(() => GetNewPalette());
                GetNewPalette();
            }
        }
        /// <summary>
        /// Clears previous palette and gets new colors.
        /// </summary>
        private void GetNewPalette()
        {
            ObservableCollection<IAColorClass> newColorSource = new ObservableCollection<IAColorClass>();
            foreach (Color color in model.GetPalette())
            {
                newColorSource.Add(new IAColorClass(color));
            }

            SampleColorSource = newColorSource;
        }
        #endregion
    }
}
