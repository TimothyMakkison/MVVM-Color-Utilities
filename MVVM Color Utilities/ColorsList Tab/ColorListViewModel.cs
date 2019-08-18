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
using MVVM_Color_Utilities.ViewModel.Helper_Classes;

namespace MVVM_Color_Utilities.ColorsList_Tab
{
    class ColorListViewModel : ObservableObject, IPageViewModel
    {
        //"^#(?:(?:[0-9a-fA-F]{3}){1,2}|(?:[0-9a-fA-F]{4}){1,2})$"
        #region Fields

        #region Misc
        private readonly ColorListModel model = new ColorListModel();
        private readonly ColorUtils colorUtils = new ColorUtils();
        #endregion

        #region Bools + Ints + Strings
        private bool _addingModeBool = true;
        private int _selectedItem = 0;

        private string _inputNameString;
        private string _inputHexString;
        #endregion

        #region Brushes
        private SolidColorBrush _inputBrush = Brushes.White;
        #endregion 

        #region ICommands
        private ICommand _addSwitchCommand;
        private ICommand _editSwitchCommand;

        private ICommand _addNewItemCommand;
        private ICommand _editItemCommand;
        private ICommand _executeCommand;

        private ICommand _sampleColorCommand;
        private ICommand _deleteItemCommand;
        #endregion

        #endregion

        #region Constructors
        public ColorListViewModel()
        {
            SelectedIndex = 0;
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

        public int SelectedIndex
        {
            get
            {
                if (_selectedItem >= ColorListSource.Count && ColorListSource.Count!=0)
                    _selectedItem = ColorListSource.Count - 1;
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                InputHex = ColorListSource[value].Hex;
                InputName= ColorListSource[value].Name;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public PackIconKind Icon
        {
            get 
            {
                return PackIconKind.Palette;
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

        public ICommand AddNewItem
        {
            get
            {
                if (_addNewItemCommand == null)
                {
                    _addNewItemCommand = new RelayCommand(param => AddNewItemMethod());
                }
                return _addNewItemCommand;
            }
        }

        public ICommand EditItem
        {
            get
            {
                if (_editItemCommand == null)
                {
                    _editItemCommand = new RelayCommand(param => EditItemMethod());
                }
                return _editItemCommand;
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
        void AddNewItemMethod()
        {
            model.AddColorItem(SelectedIndex, InputHex, InputName);
            SelectedIndex = 0;
        }
        void EditItemMethod()
        {
            if (ColorListSource.Count > SelectedIndex)
            {
                model.EditColorItem(SelectedIndex, InputHex, InputName);
            }
               
        }
        void ExecuteMethod()
        {
            if (AddingModeBool)
                AddNewItemMethod();
            else
                EditItemMethod();
        }
        void SampleColorMethod()
        {
            var color =  colorUtils.GetCursorColor();
            InputHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        void DeleteItemMethod()
        {
            if (ColorListSource.Count > SelectedIndex)
            {
                ColorListSource.RemoveAt(SelectedIndex);
                model.SaveColorsList();
                OnPropertyChanged("ColorListSource");
            }
        }
        #endregion
    }
}
