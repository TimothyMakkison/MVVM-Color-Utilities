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
        private readonly ColorListModel _colorListModel = new ColorListModel();
        private readonly ColorUtils _colorUtils = new ColorUtils();

        private bool _addingModeBool = true;
        private int _selectedItem;

        //private string _addNameString;
        //private string _editNameString;
        //private string _addHexString ;
        //private string _editHexString;

        private string _inputNameString;
        private string _inputHexString;

        //private SolidColorBrush _addBrush = Brushes.White;
        //private SolidColorBrush _editBrush = Brushes.White;

        private SolidColorBrush _inputBrush = Brushes.White;

        private readonly SolidColorBrush BurgundyBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF5D1A1A"));
        private readonly SolidColorBrush BurgundyLightBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7F2626"));
        #endregion

        #region ICommands
        private ICommand _addSwitchCommand;
        private ICommand _editSwitchCommand;

        private ICommand _addNewItemCommand;
        private ICommand _editItemCommand;
        private ICommand _executeCommand;

        private ICommand _sampleColorCommand;
        private ICommand _deleteItemCommand;

        private ICommand _incrementListCommand;
        private ICommand _decreaseListCommand;
        #endregion

        #endregion

        #region Properties

        #region Brushes
        public SolidColorBrush IndicatorBrush
        {
            get
            {
                //return AddingModeBool ? _addBrush:_editBrush;

                return _inputBrush;
            }
            set
            {
                //depending on what state the menu is in the corresponding color is updated
                //if (AddingModeBool)
                //{
                //    _addBrush = value;
                //}
                //else
                //{
                //    _editBrush = value;
                //}

                _inputBrush = value;

                OnPropertyChanged("IndicatorBrush");
            }
        }
        public SolidColorBrush AddBackground
        {
            get
            {
                return AddingModeBool ? BurgundyLightBrush : BurgundyBrush;
            }
        }

        public SolidColorBrush EditBackground
        {
            get
            {
                return AddingModeBool ? BurgundyBrush : BurgundyLightBrush;
            }
        }
        #endregion

        #region Strings
        public string InputName
        {
            get
            {
                //return AddingModeBool ? _addNameString : _editNameString;
                return _inputNameString;
            }
            set
            {
                //if (AddingModeBool)
                //    _addNameString = value ;
                //else
                //    _editNameString = value;

                _inputNameString = value;

                OnPropertyChanged("InputName");
            }
        }
        public string InputHex
        {
            get
            {
                //return AddingModeBool ? _addHexString :_editHexString;
                return _inputHexString;
            }
            set
            {
                //if (AddingModeBool)
                //    _addHexString = value;
                //else
                //    _editHexString = value;

                _inputHexString = value;

                try
                {
                    //Sets indicator to the new color
                    //if(AddingModeBool)
                    //    _addBrush= new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                    //else
                    //    _editBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));

                    _inputBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));

                    OnPropertyChanged("IndicatorBrush");
                }
                catch { }
                OnPropertyChanged("InputHex");
            }
        }
        #endregion

        #region Misc
        public ColorListViewModel()
        {
            SelectedIndex = 0;
        }

        public bool AddingModeBool
        {
            get
            {
                return _addingModeBool;
            }
            set
            {
                _addingModeBool = value;
                //OnPropertyChanged("IndicatorBrush");
                //OnPropertyChanged("InputName");
                //OnPropertyChanged("InputHex");
                OnPropertyChanged("AddBackground");
                OnPropertyChanged("EditBackground");
            }
        }

        
        public ObservableCollection<ColorClass> ColorListSource
        {
            get
            {
                return _colorListModel.ColorClassList;
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
                InputHex = ColorListSource[value].Hex;
                InputName= ColorListSource[value].Name;
                _selectedItem = value;
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

        #region ListCommands

        public ICommand IncrementListCommand
        {
            get
            {
                if (_incrementListCommand == null)
                {
                    _incrementListCommand = new RelayCommand(param => IncrementListMethod());
                }
                return _incrementListCommand;
            }
        }

        public ICommand DecreaseListCommand
        {
            get
            {
                if (_decreaseListCommand == null)
                {
                    _decreaseListCommand = new RelayCommand(param => DecreaseListMethod());
                }
                return _decreaseListCommand;
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
            OnPropertyChanged("InputName");
            //ColorListSource.Insert(0, new ColorClass(_colorListModel.NextID, _addHexString, _addNameString));
            ColorListSource.Insert(0, new ColorClass(_colorListModel.NextID, _inputHexString, _inputNameString));

            _colorListModel.SaveColorsList();
            SelectedIndex = 0;
            OnPropertyChanged("ColorListSource");
        }
        void EditItemMethod()
        {
            if (ColorListSource.Count > SelectedIndex)
            {
                ColorListSource[SelectedIndex] = new ColorClass(ColorListSource[SelectedIndex].ID, _inputHexString, _inputNameString);
                //ColorListSource[SelectedItem] = new ColorClass(ColorListSource[SelectedItem].ID, _editHexString, _editNameString);
                _colorListModel.SaveColorsList();
                OnPropertyChanged("ColorListSource");
            }
               
        }
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
        void SampleColorMethod()
        {
            var color =  _colorUtils.GetCursorColor();
            InputHex = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }
        void DeleteItemMethod()
        {
            if (ColorListSource.Count > SelectedIndex)
            {
                ColorListSource.RemoveAt(SelectedIndex);
                _colorListModel.SaveColorsList();
                OnPropertyChanged("ColorListSource");
            }
        }
        void IncrementListMethod()
        {
            if (0 <= SelectedIndex && SelectedIndex < ColorListSource.Count - 1)
                SelectedIndex++;
            else
                SelectedIndex = 0;
        }
        void DecreaseListMethod()
        {
            if (0 < SelectedIndex && SelectedIndex <= ColorListSource.Count - 1)
                SelectedIndex--;
            else
                SelectedIndex = ColorListSource.Count - 1;
        }

        #endregion
    }
}
