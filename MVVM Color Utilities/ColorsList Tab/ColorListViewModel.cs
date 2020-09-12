using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    internal class ColorListViewModel : ObservableObject, IPageViewModel
    {
        private readonly Regex _hexCharactersReg = new Regex("^#([0-9a-fA-F]{0,8})?$");
        private readonly Regex _hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");

        private ColorModel _selectedItem;

        private bool _addingModeBool = true;
        private int _selectedItemIndex = 0;

        private string _inputNameString = "";
        private string _inputHexString = "";

        private SolidColorBrush _inputBrush = Brushes.White;

        private ICommand _addSwitchCommand;
        private ICommand _editSwitchCommand;

        private ICommand _executeCommand;
        private ICommand _sampleColorCommand;
        private ICommand _deleteItemCommand;

        private readonly IDataContext<ColorModel> colorDataContext;

        public ColorListViewModel(IDataContext<ColorModel> dataContext)
        {
            this.colorDataContext = dataContext;
        }

        #region Properties

        #region Brushes

        public SolidColorBrush IndicatorBrush
        {
            get => _inputBrush;
            set => Set(ref _inputBrush, value);
        }

        #endregion Brushes

        public string InputName
        {
            get => _inputNameString;
            set => Set(ref _inputNameString, value);
        }

        public string InputHex
        {
            get => _inputHexString;
            set
            {
                if (_hexCharactersReg.IsMatch(value) || value == "")//Only allows valid hex charcters ie start with # and the 1-9a-f
                {
                    _inputHexString = value;
                    OnPropertyChanged();
                    IndicatorBrush = _hexColorReg.IsMatch(_inputHexString)
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inputHexString)) :
                        IndicatorBrush = Brushes.White;
                }
            }
        }

        public PackIconKind Icon => PackIconKind.Palette;

        public bool AddingModeBool
        {
            get => _addingModeBool;
            set => Set(ref _addingModeBool, value);
        }

        public ObservableCollection<ColorModel> ColorListSource => new ObservableCollection<ColorModel>(colorDataContext.Source);

        public ColorModel SelectedValue
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                if (_selectedItem != null)
                {
                    InputName = _selectedItem.Name;
                    InputHex = _selectedItem.Hex;
                }
                else
                {
                    InputHex = "";
                    InputName = "";
                }
            }
        }

        public int SelectedItemIndex
        {
            get => _selectedItemIndex = MathUtils.Clamp(0, ColorListSource.Count - 1, _selectedItemIndex);
            set => Set(ref _selectedItemIndex, MathUtils.Clamp(0, ColorListSource.Count - 1, value));
        }

        #endregion Properties

        public ICommand AddSwitchCommand => PatternHandler.Singleton(ref _addSwitchCommand, AddSwitchMethod);

        public ICommand EditSwitchCommand => PatternHandler.Singleton(ref _editSwitchCommand, EditSwitchMethod);

        public ICommand ExecuteCommand => PatternHandler.Singleton(ref _executeCommand, ExecuteMethod);

        public ICommand SampleColorCommandExecuteMethod => PatternHandler.Singleton(ref _sampleColorCommand, SampleColorMethod);

        public ICommand DeleteItem => PatternHandler.Singleton(ref _deleteItemCommand, DeleteItemMethod);

        #region Methods

        private void AddSwitchMethod() => AddingModeBool = true;

        private void EditSwitchMethod() => AddingModeBool = false;

        /// <summary>
        /// Adds or edits item depending on selected setting.
        /// </summary>
        private void ExecuteMethod()
        {
            if (AddingModeBool)
            {
                AddNewItemMethod();
            }
            else
            {
                EditItemMethod();
            }
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        private void AddNewItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            colorDataContext.Add(new ColorModel(SelectedItemIndex, InputHex, InputName));
            colorDataContext.Save();

            SelectedItemIndex = currentIndex;
        }

        /// <summary>
        /// Edits selected item.
        /// </summary>
        private void EditItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            //TODO Fix id assignment
            var id = 4;
            colorDataContext.ReplaceAt(SelectedItemIndex, new ColorModel(id, InputHex, InputName));
            colorDataContext.Save();

            SelectedItemIndex = currentIndex;
            if (ColorListSource.Count > 0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource[0];
            }
        }

        /// <summary>
        /// Deletes selected item.
        /// </summary>
        private void DeleteItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            colorDataContext.RemoveAt(SelectedItemIndex);
            colorDataContext.Save();

            SelectedItemIndex = currentIndex;
            if (ColorListSource.Count > 0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the color of the pixel location.
        /// </summary>
        private void SampleColorMethod() => InputHex = ColorUtils.ColorToHex(ColorUtils.GetCursorColor());

        #endregion Methods
    }
}