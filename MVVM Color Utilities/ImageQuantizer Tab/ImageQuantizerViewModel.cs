using MaterialDesignThemes.Wpf;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using Microsoft.Win32;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ImageQuantizer_Tab
{

    class ImageQuantizerViewModel : ObservableObject, IPageViewModel
    {
        #region Fields
        private string _selectedPath;
        private ICommand _openCommand;
        private readonly OpenFileDialog dialogBox = new OpenFileDialog()
        { Filter = "Images| *.jpg;*.png;*.jpeg;*.bmp", Title="Browse Images"};
        #endregion

        #region Properties
        public PackIconKind Icon => PackIconKind.Paint;
        public string SelectedPath
        {
            get
            {
                return _selectedPath;
            }
            set
            {
                _selectedPath = value;
                OnPropertyChanged("SelectedPath");
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
            dialogBox.ShowDialog();
            string path = dialogBox.FileName;

            //Checks that the path exists and is not the previous path.
            if (path != "" && SelectedPath != path)
            {
                SelectedPath = path;
                //crashes if file name is null
            }
        }
        #endregion
    }
}
