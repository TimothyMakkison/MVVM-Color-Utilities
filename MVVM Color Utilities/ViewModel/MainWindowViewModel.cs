using MVVM_Color_Utilities.ColorsList_Tab;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MVVM_Color_Utilities.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            NavigateCommand = new DelegateCommand<string>(Navigate);
            Navigate(nameof(ColorListView));
        }

        public DelegateCommand<string> NavigateCommand { get; }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
            {
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
            }
        }
    }
}