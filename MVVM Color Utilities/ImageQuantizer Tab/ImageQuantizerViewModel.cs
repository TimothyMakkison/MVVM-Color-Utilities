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
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Threading.Tasks;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{

    class ImageQuantizerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string _selectedPath;
        private int _quantizerComboIndex, _colorCountComboIndex = 4;

        private ICommand _openCommand;
        private ICommand _saveCommand;

        private readonly ImageQuantizerModel model = new ImageQuantizerModel();
        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;
        private readonly SaveFileDialog saveDialogBox = ImageBufferItems.SaveDialogBox;
        System.Windows.Media.Imaging.BitmapImage _generatedBitmap;
        #endregion

        #region Constructor
        public ImageQuantizerViewModel()
        {
            model.SetColorCount(ColorCountList[_colorCountComboIndex]);
            model.SetQuantizer(QuantizerList[_quantizerComboIndex]);
        }
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.Paint;

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

        public List<BaseColorQuantizer> QuantizerList { get; } = ImageBufferItems.QuantizerOptions;
        public List<Int32> ColorCountList { get; } = ImageBufferItems.ColorCountOptions;
        public int QuantizerComboIndex
        {
            get
            {
                return _quantizerComboIndex;
            }
            set
            {
                _quantizerComboIndex = value;
                MessageBox.Show("Quant setting in new imager");

                model.SetQuantizer(QuantizerList[value]);
            }
        }
        public int ColorCountComboIndex
        {
            get
            {
                return _colorCountComboIndex;
            }
            set
            {
                _colorCountComboIndex = value;
                model.SetColorCount(ColorCountList[value]);
                Task.Run(() => GenerateNewImage());
            }
        }
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
                model.SetBitmap(new Bitmap(Image.FromFile(SelectedPath)));//crashes if file name is null
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
            model.GetNewImage();
            GeneratedBitmap = Imageutils.ConvertToBitmapImage(model.GeneratedBitmap);
        }
        #endregion
    }
}
