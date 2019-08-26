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

        private readonly ImageQuantizerModel model = new ImageQuantizerModel();
        private readonly OpenFileDialog dialogBox = new OpenFileDialog()
        { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title = "Browse Images" };
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
                    _openCommand = new RelayCommand(param => OpenFile());
                }
                return _openCommand;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Opens file and exectues GenerateNewImage if selected item is valid.
        /// </summary>
        private void OpenFile()
        {
            dialogBox.ShowDialog();
            string path = dialogBox.FileName;

            //Checks that the path exists and is not repeating itself.
            if (path != "" && SelectedPath != path)
            {
                SelectedPath = path;
                model.SetBitmap(new Bitmap(Image.FromFile(path)));//crashes if file name is null
                Task.Run(() => GenerateNewImage());
            }
        }
        private void GenerateNewImage()
        {
            model.GenerateNewImage();
            GeneratedBitmap = Imageutils.ConvertToBitmapImage(model.GeneratedBitmap);
        }
        #endregion
    }
}
