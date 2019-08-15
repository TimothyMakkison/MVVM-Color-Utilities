using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using MaterialDesignThemes.Wpf;
using System.IO;
using System.Windows.Input;
using System.Windows;
using System.Drawing;
using System.Security;
using Microsoft.Win32;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using MVVM_Color_Utilities.Palette_Quantizers.Median_Cut;

namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private readonly OpenFileDialog _dialog = new OpenFileDialog() { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", };
        
        private string _selectedPath ;

        private Image _targetImage;
        private Bitmap _targetBitmap;
        private ICommand _openCommand;
        private readonly ObservableCollection<ColorClass> _sampleColorSource = new ObservableCollection<ColorClass>();
        #endregion

        #region Properties
        public PackIconKind Icon
        {
            get
            {
                return PackIconKind.PaletteAdvanced;
            }
        }
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value; OnPropertyChanged("SelectedPath"); }
        }
        public ObservableCollection<ColorClass> SampleColorSource
        {
            get
            {
                return _sampleColorSource;
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
        private void OpenFile()
        {
            //_dialog.InitialDirectory = SelectedPath;
            _dialog.ShowDialog();
            SelectedPath = _dialog.FileName;

            //Checks to that the path exists
            if (SelectedPath != "")
            {
                _targetImage = Image.FromFile(_dialog.FileName);//crashes if file name is null
                SampleColorSource.Clear();
                GetPalette(16);
            }
        }

        /// <summary>
        /// Returns a palette of x amount of colors
        /// </summary>
        /// <param name="colorCount">Number of palette colors</param>
        private void GetPalette (Int32 colorCount)
        {
            List<Int32> colorList = new List<Int32>();
            _targetBitmap = new Bitmap(_targetImage);

            //Iterates through each pixel adding it to the colorList
            for (int x = 0; x < _targetBitmap.Width; x++)
            {
                for (int y = 0; y < _targetBitmap.Height; y++)
                {
                    Color pixelColor = _targetBitmap.GetPixel(x, y);
                    Int32 key = pixelColor.R << 16 | pixelColor.G << 8 | pixelColor.B;
                    colorList.Add(key);
                }
            }
            MedianCutQuantizer quantizer = new MedianCutQuantizer(colorList);
            List<Color> palette = quantizer.GetPalette(colorCount);

            //adds each color to the displayed list
            foreach (Color color in palette)
            {
                SampleColorSource.Add(new ColorClass(color));
            }

            OnPropertyChanged("SampleColorSource");
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
