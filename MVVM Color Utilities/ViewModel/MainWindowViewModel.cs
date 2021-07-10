using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;

namespace MVVM_Color_Utilities.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private IPageViewModel _currentPageViewModel;

        public MainWindowViewModel(List<IPageViewModel> pageViewModels)
        {
            PageViewModels = pageViewModels;
            CurrentPageViewModel = PageViewModels[0];
            ChangePageCommand = new DelegateCommand<IPageViewModel>(p => ChangeViewModel(p),
                                                          p => p is IPageViewModel);
        }

        public DelegateCommand<IPageViewModel> ChangePageCommand { get; }

        public List<IPageViewModel> PageViewModels { get; }

        public IPageViewModel CurrentPageViewModel
        {
            get => _currentPageViewModel;
            set => SetProperty(ref _currentPageViewModel, value);
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
            {
                PageViewModels.Add(viewModel);
            }
            CurrentPageViewModel = PageViewModels.Find(vm => vm == viewModel);
        }
    }
}