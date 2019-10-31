using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;   
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ColorsList_Tab;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using MVVM_Color_Utilities.Helpers;
using System.Text.RegularExpressions;


namespace MVVM_Color_Utilities.ColorsList_Tab
{
    class ColorListViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private readonly Regex _hexCharactersReg = new Regex("^#([0-9a-fA-F]{0,8})?$");
        private readonly Regex _hexColorReg = new Regex("^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$");

        private ListColorClass _selectedItem;

        private bool _addingModeBool = true;
        private int _selectedItemIndex=0;

        private string _inputNameString="";
        private string _inputHexString="";

        private SolidColorBrush _inputBrush = Brushes.White;
        #region ICommands
        private ICommand _addSwitchCommand;
        private ICommand _editSwitchCommand;

        private ICommand _executeCommand;
        private ICommand _sampleColorCommand;
        private ICommand _deleteItemCommand;
        #endregion

        #endregion

        #region Properties

        #region Brushes
        public SolidColorBrush IndicatorBrush
        {
            get
            {
                return _inputBrush;
            }
            set
            {
                _inputBrush = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Strings
        public string InputName
        {
            get
            {
                return _inputNameString;
            }
            set
            {
                _inputNameString = value;
                OnPropertyChanged();
            }
        }
        public string InputHex
        {
            get
            {
                return _inputHexString;
            }
            set
            {
                if (_hexCharactersReg.IsMatch(value)||value=="")//Only allows valid hex charcters ie start with # and the 1-9a-f
                {
                    _inputHexString = value;
                    OnPropertyChanged();
                    IndicatorBrush = _hexColorReg.IsMatch(_inputHexString)
                        ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(_inputHexString)):
                        IndicatorBrush = Brushes.White;
                }
              
            }
        }
        #endregion

        #region Misc
        public PackIconKind Icon => PackIconKind.Palette;
        public bool AddingModeBool
        {
            get
            {
                return _addingModeBool;
            }
            set
            {
                _addingModeBool = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ListColorClass> ColorListSource
        {
            get
            {
                return SharedUtils.ColorClassList;
            }
        }
        public ListColorClass SelectedValue
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                if ( _selectedItem!=null)
                {
                    InputName = _selectedItem.Name;
                    InputHex = _selectedItem.Hex;
                }
                else
                {
                    InputHex = "";
                    InputName = "";
                }
                OnPropertyChanged();
            }
        }
        public int SelectedItemIndex
        {
            get
            {
                return _selectedItemIndex = MathUtils.Clamp(0, ColorListSource.Count - 1, _selectedItemIndex);
            }
            set
            {
                _selectedItemIndex =  MathUtils.Clamp(0, ColorListSource.Count - 1, value);
                OnPropertyChanged();
            }
        } 
        #endregion

        #endregion

        #region Commands

        #region WindowCommands
        public ICommand AddSwitchCommand
        {
            get
            {
                if (_addSwitchCommand == null)
                {
                    _addSwitchCommand = new RelayCommand(param => AddSwitchMethod());
                }
                return _addSwitchCommand;
            }
        }
        public ICommand EditSwitchCommand
        {
            get
            {
                if(_editSwitchCommand == null)
                {
                    _editSwitchCommand = new RelayCommand(param => EditSwitchMethod());
                }
                return _editSwitchCommand;
            }
        }
        #endregion

        #region FunctionalCommands
        public ICommand ExecuteCommand
        {
            get
            {
                if (_executeCommand == null)
                {
                    _executeCommand = new RelayCommand(param => ExecuteMethod());
                }
                return _executeCommand;
            }
        }
        public ICommand SampleColorCommand
        {
            get
            {
                if (_sampleColorCommand == null)
                {
                    _sampleColorCommand = new RelayCommand(param => SampleColorMethod());
                }
                return _sampleColorCommand;
            }

        }
        public ICommand DeleteItem
        {
            get
            {
                if (_deleteItemCommand == null)
                {
                    _deleteItemCommand = new RelayCommand(param => DeleteItemMethod());
                }
                return _deleteItemCommand;
            }
        }
        #endregion
        #endregion

        #region Methods
        void AddSwitchMethod()
        {
            AddingModeBool = true;
        }
        void EditSwitchMethod()
        {
            AddingModeBool = false;
        }
        /// <summary>
        /// Adds or edits item depending on selected setting.
        /// </summary>
        void ExecuteMethod()
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
        void AddNewItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            SharedUtils.AddColorItem(SelectedItemIndex, InputHex, InputName);
            SelectedItemIndex = currentIndex;
        }
        /// <summary>
        /// Edits selected item.
        /// </summary>
        void EditItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            SharedUtils.EditColorItem(SelectedItemIndex, InputHex, InputName);
            SelectedItemIndex = currentIndex;
            if(ColorListSource.Count >0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource[0];
            }
        }
        /// <summary>
        /// Deletes selected item.
        /// </summary>
        void DeleteItemMethod()
        {
            int currentIndex = SelectedItemIndex;
            SharedUtils.DeleteColorItem(SelectedItemIndex);
            SelectedItemIndex = currentIndex;
            if (ColorListSource.Count > 0 && currentIndex == 0)
            {
                SelectedValue = ColorListSource[0];
            }
            }
        /// <summary>
        /// Gets the color of the pixel location.
        /// </summary>
        void SampleColorMethod()
        {
            Color color =  ColorUtils.GetCursorColor();
            InputHex = ColorUtils.ColorToHex(color);
        }
        #endregion
    }
}
