using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Helpers.Extensions;
using MVVM_Color_Utilities.Models;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    internal class ColorListViewModel : ObservableObject, IPageViewModel
    {
        private readonly Regex _hexCharactersReg = new Regex("^#([0-9a-fA-F]{0,8})?$");
        private readonly Regex _hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");

        private Color color;
        private ColorModel _selectedItem;

        private bool _addingModeBool = true;
        private int _selectedItemIndex = 0;

        private string inputName = "";
        private string inputHex = "";

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

        public string InputName
        {
            get => inputName;
            set => Set(ref inputName, value);
        }

        public string InputHex
        {
            get => inputHex;
            set
            {
                if (_hexCharactersReg.IsMatch(value) || value == "")//Only allows valid hex charcters ie start with # and the 1-9a-f
                {
                    Set(ref inputHex, value);

                    if (_hexColorReg.IsMatch(inputHex))
                        Color = ((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(inputHex)).ToDrawingColor();
                }
            }
        }

        public PackIconKind Icon => PackIconKind.Palette;

        public bool AddingModeBool
        {
            get => _addingModeBool;
            set => Set(ref _addingModeBool, value);
        }

        public ObservableCollection<ColorModel> ColorListSource => colorDataContext.Observable;

        public ColorModel SelectedValue
        {
            get => _selectedItem;
            set
            {
                Set(ref _selectedItem, value);
                if (_selectedItem != null)
                {
                    InputName = _selectedItem.Name;
                }
                else
                {
                    InputName = "";
                }
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                if (value != null)
                {
                    Set(ref color, value);
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

        public ICommand SampleColorCommand => PatternHandler.Singleton(ref _sampleColorCommand, SampleColorMethod);

        public ICommand DeleteItem => PatternHandler.Singleton(ref _deleteItemCommand, DeleteItemMethod);

        #region Methods

        private void AddSwitchMethod() => AddingModeBool = true;

        private void EditSwitchMethod() => AddingModeBool = false;

        //TODO Use polymorphism
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
            colorDataContext.InsertAt(0, new ColorModel(Color) { Name = InputName }).Save();
            SelectedItemIndex = currentIndex;
        }

        /// <summary>
        /// Edits selected item.
        /// </summary>
        private void EditItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            colorDataContext.ReplaceAt(SelectedItemIndex, new ColorModel(Color) { Name = InputName }).Save();
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
            if (colorDataContext.Observable.Count <= 0)
                return;

            int currentIndex = SelectedItemIndex;
            colorDataContext.RemoveAt(SelectedItemIndex).Save();
            SelectedItemIndex = currentIndex;
            if (ColorListSource.Count > 0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the color of the pixel location.
        /// </summary>
        private void SampleColorMethod() => Color = ColorUtils.GetCursorColor().ToDrawingColor();

        #endregion Methods
    }
}