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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MVVM_Color_Utilities.Helpers;
using System.Windows.Input;


namespace MVVM_Color_Utilities.ImageAnalyzer_Tab
{
    /// <summary>
    /// ViewModel for ImageAnalyzer, gets the constituent colors of an image.
    /// </summary>
    class ImageAnalyzerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private readonly ImageAnalyzerModel model = new ImageAnalyzerModel();
        private readonly OpenFileDialog dialogBox = ImageBufferItems.OpenDialogBox;

        private BaseColorQuantizer _selectedQuantizer;

        private string _selectedPath;
        private int _selectedColorCount;

        private ICommand _openCommand;
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.PaletteAdvanced;
        /// <summary>
        /// Button displays image from this location.
        /// </summary>
        public string SelectedPath
        {
            get { return _selectedPath; }
            set { _selectedPath = value;OnPropertyChanged("SelectedPath"); }
        }
        public ObservableCollection<ColorClass> SampleColorSource { get; set; } = new ObservableCollection<ColorClass>();

        #region QuantizerList
        public List<BaseColorQuantizer> QuantizerList { get; } = ImageBufferItems.QuantizerList;
        public int QuantizerComboIndex => 0;
        public BaseColorQuantizer SelectedQuantizer
        {
            set
            {
                MessageBox.Show("Quant set");
                _selectedQuantizer = value;
                model.SetQuantizer(_selectedQuantizer);
                Task.Run(() => GetNewPalette());
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
                MessageBox.Show("Color count set " + _selectedColorCount.ToString());
                Task.Run(() => GetNewPalette());
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
            dialogBox.ShowDialog();
            string path = dialogBox.FileName;
            if (path != "" && SelectedPath != path) //Checks that the path exists and is not the previous path.
            {
                SelectedPath = path;
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
        #region Fields
        private ICommand _saveColorCommand;
        #endregion

        #region Constructor
        public ColorClass(Color color)
        {
            Color = ColorUtils.ColorToBrush(color);
            ColorHex = ColorUtils.ColorToHex(color);
        }
        #endregion

        #region Properties
        public System.Windows.Media.SolidColorBrush Color { get; set; }
        public string ColorHex { get; set; }
        #endregion

        #region Commands
        public ICommand SaveColorCommand
        {
            get
            {
                if (_saveColorCommand == null)
                {
                    _saveColorCommand = new RelayCommand(param => SaveColorMethod());
                }
                return _saveColorCommand;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Saves color to shared ColorList.    
        /// </summary>
        /// <returns></returns>
        private bool SaveColorMethod()
        {
            try
            {
                int ID = SharedUtils.NextID;
                SharedUtils.ColorClassList.Insert(0, new Helpers.ColorClass(ID, ColorHex, "Color " + ID.ToString()));
                SharedUtils.SaveColorsList();
                return true;
            }
            catch { return false; }
        }
        #endregion
    }
}
