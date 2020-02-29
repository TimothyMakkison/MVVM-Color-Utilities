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
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{
    /// <summary>
    /// ViewModel for ImageQuantizer, converts images into lower quality forms.
    /// </summary>
    class ImageQuantizerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string _selectedPath;
        private BaseColorQuantizer _selectedQuantizer = QuantizerList[0];
        private int _selectedColorCount = 16;

        System.Windows.Media.Imaging.BitmapImage _generatedBitmap;

        private ICommand _openCommand;
        private ICommand _saveCommand;

        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;
        private readonly SaveFileDialog saveDialogBox = ImageBufferItems.SaveDialogBox;
        private readonly ImageQuantizerModel model = new ImageQuantizerModel();
        #endregion

        #region Constructor
        public ImageQuantizerViewModel()
        {
            model.SetQuantizer(SelectedQuantizer);
            model.SetColorCount(SelectedColorCount);
        }
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.PaletteAdvanced;
        /// <summary>
        /// Button displays image from this location.
        /// </summary>
        public string SelectedPath
        {
            get => _selectedPath;
            set => SetProperty(ref _selectedPath, value);
        }
        /// <summary>
        /// Displayed by save bitmap button
        /// </summary>
        public System.Windows.Media.Imaging.BitmapImage GeneratedBitmap
        {
            get => _generatedBitmap;
            set => SetProperty(ref _generatedBitmap, value);
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
                Debug.WriteLine("IQ Quantizer set to " + _selectedQuantizer.Name.ToString());
                Task.Run(() => GenerateNewImage());
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
                model.SetColorCount(_selectedColorCount);
                Debug.WriteLine("IQ Color count set to " + _selectedColorCount.ToString());
                Task.Run(() => GenerateNewImage());
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
            if(model.GeneratedBitmap != null)
            {
                GeneratedBitmap = Imageutils.ConvertToBitmapImage(model.GeneratedBitmap);
            }
            //if (model.GetNewImage())
            //{
            //    GeneratedBitmap = Imageutils.ConvertToBitmapImage(model.GeneratedBitmap);
            //}
        }
        #endregion
    }
}
