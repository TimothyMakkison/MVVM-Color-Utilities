using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Helpers.Extensions;
using MVVM_Color_Utilities.Models;
using MVVM_Color_Utilities.ViewModel;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    internal class ColorListViewModel : BindableBase , IPageViewModel
    {
        private readonly Regex _hexCharactersReg = new("^#([0-9a-fA-F]{0,8})?$");
        private readonly Regex _hexColorReg = new("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");

        private Color color;
        private ColorModel selectedItem;

        private bool _addingModeBool = true;
        private int _selectedItemIndex = 0;

        private string _inputName = "";
        private string _inputHex = "";

        private readonly IDataContext<ColorModel> _colorDataContext;

        public ColorListViewModel(IDataContext<ColorModel> dataContext)
        {
            _colorDataContext = dataContext;

            AddSwitchCommand = new DelegateCommand(AddSwitchMethod);
            EditSwitchCommand = new DelegateCommand(EditSwitchMethod);
            ExecuteCommand = new DelegateCommand(ExecuteMethod);
            DeleteItem = new DelegateCommand(DeleteItemMethod);
            SampleColorCommand = new DelegateCommand(GetPixelColor);
        }
        public PackIconKind Icon => PackIconKind.Palette;

        public DelegateCommand AddSwitchCommand { get; }

        public DelegateCommand EditSwitchCommand { get; }

        public DelegateCommand ExecuteCommand { get; }

        public DelegateCommand SampleColorCommand { get; }

        public DelegateCommand DeleteItem { get; }

        public string InputName
        {
            get => _inputName;
            set => SetProperty(ref _inputName, value);
        }

        public string InputHex
        {
            get => _inputHex;
            set
            {
                //Only allows valid hex charcters ie start with # and the 1-9a-f
                if (_hexCharactersReg.IsMatch(value) || value?.Length == 0)
                {
                    SetProperty(ref _inputHex, value);

                    if (_hexColorReg.IsMatch(_inputHex))
                    {
                        Color = ((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(_inputHex)).ToDrawingColor();
                    }
                }
            }
        }


        public bool AddingModeBool
        {
            get => _addingModeBool;
            set => SetProperty(ref _addingModeBool, value);
        }

        public ObservableCollection<ColorModel> ColorListSource => _colorDataContext.Observable;

        public ColorModel SelectedValue
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                if (selectedItem != null)
                {
                    InputName = selectedItem.Name;
                    InputHex = selectedItem.Color.ToHex();
                    Color = selectedItem.Color;
                }
                else
                {
                    InputName = "";
                    InputHex = "";
                    Color = Color.White;
                }
            }
        }

        public Color Color
        {
            get => color;
            set
            {
                if (value != default)
                {
                    SetProperty(ref color, value);
                }
            }
        }

        public int SelectedItemIndex
        {
            get => _selectedItemIndex = MathUtils.Clamp(0, ColorListSource.Count - 1, _selectedItemIndex);
            set => SetProperty(ref _selectedItemIndex, MathUtils.Clamp(0, ColorListSource.Count - 1, value));
        }

        private void AddSwitchMethod()
        {
            AddingModeBool = true;
        }

        private void EditSwitchMethod()
        {
            AddingModeBool = false;
        }

        //TODO Use polymorphism
        /// <summary>
        /// Adds or edits item depending on selected setting.
        /// </summary>
        private void ExecuteMethod()
        {
            if (AddingModeBool)
                AddNewItemMethod();
            else
                EditItemMethod();
        }

        /// <summary>
        /// Adds new item.
        /// </summary>
        private void AddNewItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            _colorDataContext.InsertAt(0, new ColorModel(Color) { Name = InputName })
                .Save();
            SelectedItemIndex = currentIndex;
        }

        /// <summary>
        /// Edits selected item.
        /// </summary>
        private void EditItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            _colorDataContext.ReplaceAt(SelectedItemIndex, new ColorModel(Color) { Name = InputName })
                .Save();
            SelectedItemIndex = currentIndex;
            if (ColorListSource.Count > 0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource[0];
            }
        }

        private void DeleteItemMethod()
        {
            if (_colorDataContext.Observable.Count == 0)
                return;

            int currentIndex = SelectedItemIndex;
            _colorDataContext.RemoveAt(SelectedItemIndex).Save();
            SelectedItemIndex = currentIndex;
            if (ColorListSource.Count > 0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the color of the pixel location.
        /// </summary>
        private void GetPixelColor()
        {
            Color = CursorUtils.GetCursorColor()
                .ToDrawingColor();
        }
    }
}