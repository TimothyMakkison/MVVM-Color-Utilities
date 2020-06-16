using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;

namespace MVVM_Color_Utilities.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        #region Fields
        private ICommand _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            PageViewModels.Add(new ColorsList_Tab.ColorListViewModel());
            PageViewModels.Add(new ImageQuantizer_Tab.ImageQuantizerViewModel());
            PageViewModels.Add(new ImageAnalyzer_Tab.ImageAnalyzerViewModel());

            CurrentPageViewModel = PageViewModels[0];
        }
        #endregion

        #region Properties
        /// <summary>
        /// Changes page to the relative source
        /// </summary>
        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(p => ChangeViewModel((IPageViewModel)p),
                                                          p => p is IPageViewModel);
                }
                return _changePageCommand;
            }
        }
        /// <summary>
        /// List of all available viewmodels
        /// </summary>
        public List<IPageViewModel> PageViewModels =>
            _pageViewModels ?? (_pageViewModels = new List<IPageViewModel>());
        /// <summary>
        /// Updates or returns current page
        /// </summary>
        public IPageViewModel CurrentPageViewModel
        {
            get => _currentPageViewModel;
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets current viewmodel to given one
        /// </summary>
        /// <param name="viewModel"></param>
        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
            {
                PageViewModels.Add(viewModel);
            }
            CurrentPageViewModel = PageViewModels.FirstOrDefault(vm => vm == viewModel);
        }
        #endregion
    }
}
