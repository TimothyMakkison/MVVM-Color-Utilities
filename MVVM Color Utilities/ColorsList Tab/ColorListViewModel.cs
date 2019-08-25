using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;   
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ColorsList_Tab;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    class ColorListViewModel : ObservableObject, IPageViewModel
    {
        //"^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$"
        #region Fields

        #region Misc
        private readonly ColorListModel model = new ColorListModel();
        #endregion

        #region Bools + Ints + Strings
        private bool _addingModeBool = true;
        private int _selectedItemIndex = 0;

        private string _inputNameString;
        private string _inputHexString;
        #endregion

        #region Brushes
        private SolidColorBrush _inputBrush = Brushes.White;
        #endregion 

        #region ICommands
        private ICommand _addSwitchCommand;
        private ICommand _editSwitchCommand;

        private ICommand _executeCommand;
        private ICommand _sampleColorCommand;
        private ICommand _deleteItemCommand;
        #endregion

        #endregion

        #region Constructors
        public ColorListViewModel()
        {
            SelectedItemIndex = 0;
        }
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
                OnPropertyChanged("IndicatorBrush");
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
                OnPropertyChanged("InputName");
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
                _inputHexString = value;
                try
                {
                    //Sets indicator to the new color
                    IndicatorBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                }
                catch { }
                OnPropertyChanged("InputHex");
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
                OnPropertyChanged("AddingModeBool");
            }
        }
        public ObservableCollection<ColorClass> ColorListSource
        {
            get
            {
                return model.ColorClassList;
            }
        }
        public int SelectedItemIndex
        {
            get
            {
                if (_selectedItemIndex >= ColorListSource.Count && ColorListSource.Count!=0)
                    _selectedItemIndex = ColorListSource.Count - 1;
                return _selectedItemIndex;
            }
            set
            {
                _selectedItemIndex= MathUtils.Clamp(0, ColorListSource.Count - 1, value);
                if (ColorListSource.Count > 0)
                {
                    InputHex = ColorListSource[_selectedItemIndex].Hex;
                    InputName = ColorListSource[_selectedItemIndex].Name;
                }
                else
                {
                    InputHex = "";
                    InputName = "";
                }
                OnPropertyChanged("SelectedItemIndex");
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
                AddNewItemMethod();
            else
                EditItemMethod();
        }
        /// <summary>
        /// Adds new item.
        /// </summary>
        void AddNewItemMethod()
        {
            model.AddColorItem(SelectedItemIndex, InputHex, InputName);
            SelectedItemIndex = 0;
        }
        /// <summary>
        /// Edits selected item.
        /// </summary>
        void EditItemMethod()
        {
            model.EditColorItem(SelectedItemIndex, InputHex, InputName);
        }
        /// <summary>
        /// Deletes selected item.
        /// </summary>
        void DeleteItemMethod()
        { 
            model.DeleteColorItem(SelectedItemIndex);
        }
        void SampleColorMethod()
        {
            Color color =  ColorUtils.GetCursorColor();
            InputHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        #endregion
    }
}
