using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using Microsoft.Win32;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
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
        private string _selectedPath, _generatedImagePath;
        private string _newImagePath = projectPath + "/Resources/Images/NewImage.bmp";
        private readonly static string projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName; //Get Path of ColorItems file
        private int _quantizerComboIndex,_colorCountComboIndex = 4;

        private ICommand _openCommand;

        private readonly ImageQuantizerModel model = new ImageQuantizerModel();
        private readonly OpenFileDialog dialogBox = new OpenFileDialog()
        { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title="Browse Images"};
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

        public string GeneratedImagePath
        {
            get
            {
                return _generatedImagePath;
            }
            set
            {
                _generatedImagePath = value;
                OnPropertyChanged("GeneratedImagePath");
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
            if (path != ""&&SelectedPath != path)
            {
                SelectedPath = path;
                //crashes if file name is null
                model.SetBitmap(new Bitmap(Image.FromFile(path)));
                //Task.Run(() => GenerateNewImage());
                GenerateNewImage();
            }
        }

        public Image TempBMP
        {
            get
            {
                return Image.FromFile(projectPath + "/Resources/Images/TempBMP.bmp");
            }
        }

        private void GenerateNewImage()
        {
            GeneratedImagePath = string.Empty;
            System.Threading.Thread.Sleep(100);
            model.GenerateNewImage();
            model.SaveNewBitmap();
            GeneratedImagePath = _newImagePath;
            MessageBox.Show("finished");
        }
    }
    #endregion

}
