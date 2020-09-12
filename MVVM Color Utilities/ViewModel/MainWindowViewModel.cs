using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.ViewModel.Helper_Classes;
using StructureMap;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace MVVM_Color_Utilities.ViewModel
{
    public class MainWindowViewModel : ObservableObject
    {
        private ICommand changePageCommand;
        private IPageViewModel currentPageViewModel;
        private List<IPageViewModel> pageViewModels;

        public MainWindowViewModel()
        {
            var container = new Container(_ =>
            {
                _.ForSingletonOf<ColorDataContext>().Use(new ColorDataContext());
            });
            PageViewModels.Add(container.GetInstance<ColorsList_Tab.ColorListViewModel>());
            PageViewModels.Add(container.GetInstance<ImageQuantizer_Tab.ImageQuantizerViewModel>());
            PageViewModels.Add(container.GetInstance<ImageAnalyzer_Tab.ImageAnalyzerViewModel>());

            CurrentPageViewModel = PageViewModels[0];
        }

        /// <summary>
        /// Changes page to the relative source
        /// </summary>
        public ICommand ChangePageCommand
            => changePageCommand ??= new RelayCommand(p => ChangeViewModel((IPageViewModel)p),
                                                          p => p is IPageViewModel);

        /// <summary>
        /// List of all available viewmodels
        /// </summary>
        public List<IPageViewModel> PageViewModels => pageViewModels ??= new List<IPageViewModel>();

        /// <summary>
        /// Updates or returns current page
        /// </summary>
        public IPageViewModel CurrentPageViewModel
        {
            get => currentPageViewModel;
            set => Set(ref currentPageViewModel, value);
        }

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
    }
}