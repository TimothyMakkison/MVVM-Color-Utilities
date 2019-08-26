using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MVVM_Color_Utilities.Palette_Quantizers;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
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
using MVVM_Color_Utilities.Helpers;
using System.Windows.Input;


namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private readonly ImageAnalyzerModel model = new ImageAnalyzerModel();
        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;

        private string _selectedPath;

        private int _quantizerComboIndex = 0;
        private int _colorCountComboIndex = 4;

        private ICommand _openCommand;
        #endregion

        #region Constructor
        public ImageAnalyzerViewModel()
        {
            model.SetColorCount(ColorCountList[_colorCountComboIndex]);
            model.SetQuantizer(QuantizerList[_quantizerComboIndex]);
        }
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.PaletteAdvanced;
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value;OnPropertyChanged("SelectedPath"); }
        }
        public ObservableCollection<ColorClass> SampleColorSource { get; set; } = new ObservableCollection<ColorClass>();
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
                MessageBox.Show("IA Quant set");
                model.SetQuantizer(QuantizerList[_quantizerComboIndex]);
                GetNewPalette();
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
                //When changed refresh the palette
                model.SetColorCount(ColorCountList[_colorCountComboIndex]);
                GetNewPalette();
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
        /// Opens a dilog box and if a selection is made, a new palette is created.
        /// </summary>
        private void OpenFile()
        {
            dialogBox.ShowDialog();
            string path = dialogBox.FileName;
            //Checks that the path exists and is not the previous path.
            if (path != "" && SelectedPath != path)
            {
                SelectedPath = path;
                //crashes if file name is null
                model.SetBitmap(new Bitmap(Image.FromFile(path)));
                GetNewPalette();
            }
        }
        /// <summary>
        /// Clears previous palette and gets new colors.
        /// </summary>
        private void GetNewPalette()
        {
            SampleColorSource.Clear();
            foreach (Color color in model.GetPalette())
                SampleColorSource.Add(new ColorClass(color));
        }
        #endregion
    }

    class ColorClass
    {
        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public string ColorHex { get; set; }
        public ColorClass(Color color)
        {
            System.Windows.Media.Color mediaColor = System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
            Color = new System.Windows.Media.SolidColorBrush(mediaColor);
            ColorHex = "#" + color.R.ToString("X2") + color.G.ToString("X2") + color.B.ToString("X2");
        }
    }
}
