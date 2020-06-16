﻿using System;
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
using System.Linq;
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
            get => _inputBrush;
            set => Set(ref _inputBrush, value);
        }
        #endregion

        #region Strings
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
        #endregion

        #region Misc
        public PackIconKind Icon => PackIconKind.Palette;
        public bool AddingModeBool
        {
            get => _addingModeBool;
            set => Set(ref _addingModeBool, value);
        }
        public ObservableCollection<ListColorClass> ColorListSource => SharedUtils.ColorClassList;
        public ListColorClass SelectedValue
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
        #endregion

        #endregion

        #region Commands

        #region WindowCommands
        public ICommand AddSwitchCommand => PatternHandler.Singleton(ref _addSwitchCommand, AddSwitchMethod);

        public ICommand EditSwitchCommand => PatternHandler.Singleton(ref _editSwitchCommand, EditSwitchMethod);

        #endregion

        #region FunctionalCommands
        public ICommand ExecuteCommand => PatternHandler.Singleton(ref _executeCommand, ExecuteMethod);
      
        public ICommand SampleColorCommandExecuteMethod => PatternHandler.Singleton(ref _sampleColorCommand, SampleColorMethod);
      
        public ICommand DeleteItem => PatternHandler.Singleton(ref _deleteItemCommand, DeleteItemMethod);
        #endregion

        #endregion

        #region Methods
        void AddSwitchMethod() => AddingModeBool = true;
        void EditSwitchMethod() => AddingModeBool = false;
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
                SelectedValue = ColorListSource.FirstOrDefault();
            }
        }
        /// <summary>
        /// Gets the color of the pixel location.
        /// </summary>
        void SampleColorMethod() => InputHex = ColorUtils.ColorToHex(ColorUtils.GetCursorColor());
        #endregion
    }
}
