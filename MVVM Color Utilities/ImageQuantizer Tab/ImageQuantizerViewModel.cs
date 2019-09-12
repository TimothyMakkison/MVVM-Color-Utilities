using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using Microsoft.Win32;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    /// <summary>
    /// ViewModel for ImageQuantizer, converts images into lower quality forms.
    /// </summary>
    class ImageQuantizerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string _selectedPath;
        private BaseColorQuantizer _selectedQuantizer;
        private int _selectedColorCount;

        private ICommand _openCommand;
        private ICommand _saveCommand;

        private readonly ImageQuantizerModel model = new ImageQuantizerModel();
        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;
        private readonly SaveFileDialog saveDialogBox = ImageBufferItems.SaveDialogBox;
        System.Windows.Media.Imaging.BitmapImage _generatedBitmap;
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.PaletteAdvanced;

        public string SelectedPath
        {
            get
            {
                return _selectedPath;
            }
            set
            {
                _selectedPath = value;
                OnPropertyChanged("SelectedPath");
            }
        }
        public System.Windows.Media.Imaging.BitmapImage GeneratedBitmap
        {
            get
            {
                return _generatedBitmap;
            }
            set
            {
                _generatedBitmap = value;
                OnPropertyChanged("GeneratedBitmap");
            }
        }
        #region QuantizerList
        public List<BaseColorQuantizer> QuantizerList { get; } = ImageBufferItems.QuantizerList;
        public int QuantizerComboIndex => 0;
        public BaseColorQuantizer SelectedQuantizer
        {
            set
            {
                _selectedQuantizer = value;
                Debug.WriteLine("IQ Quantizer set to " + _selectedQuantizer.Name.ToString());
                model.SetQuantizer(_selectedQuantizer);
                GenerateNewImage();
            }
        }
        #endregion

        #region ColorCountList
        public List<Int32> ColorCountList { get; } = ImageBufferItems.ColorCountList;
        public int ColorCountComboIndex => 4;
        public int SelectedColorCount
        {
            set
            {
                _selectedColorCount = value;
                model.SetColorCount(_selectedColorCount);
                Debug.WriteLine("IQ Color count set "+_selectedColorCount.ToString());
                GenerateNewImage();
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
                    _openCommand = new RelayCommand(param => DialogGetImage());
                }
                return _openCommand;
            }
        }
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand== null)
                {
                    _saveCommand = new RelayCommand(param => DialogSaveImage());
                }
                return _saveCommand;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Opens file and exectues GenerateNewImage if selected item is valid.
        /// </summary>
        private void DialogGetImage()
        {
            //Checks that the path exists and is not repeating itself.
            if (dialogBox.ShowDialog()==true && SelectedPath != dialogBox.FileName)
            {
                SelectedPath = dialogBox.FileName;
                model.SetBitmap(new Bitmap(Image.FromFile(SelectedPath)));
                Task.Run(() => GenerateNewImage());
            }
        }
        /// <summary>
        /// Opens save dialog and saves generated image.
        /// </summary>
        private void DialogSaveImage()
        {
            if (saveDialogBox.ShowDialog() == true)
            {
                string path = saveDialogBox.FileName;
                model.SaveGeneratedImage(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
        }
        /// <summary>
        /// Generates new image and then displays it.
        /// </summary>
        private void GenerateNewImage()
        {
            if (model.GetNewImage())
            {
                GeneratedBitmap = Imageutils.ConvertToBitmapImage(model.GeneratedBitmap);
            }
        }
        #endregion
    }
}
