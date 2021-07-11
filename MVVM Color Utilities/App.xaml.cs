using MVVM_Color_Utilities.ViewModel;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace MVVM_Color_Utilities
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();

            containerRegistry.RegisterForNavigation<ColorsList_Tab.ColorListView, ColorsList_Tab.ColorListViewModel>();
            containerRegistry.RegisterForNavigation<ImageAnalyzer_Tab.ImageAnalyzerView, ImageAnalyzer_Tab.ImageAnalyzerViewModel>();
            containerRegistry.RegisterForNavigation<ImageQuantizer_Tab.ImageQuantizerView, ImageQuantizer_Tab.ImageQuantizerViewModel>();

            containerRegistry.RegisterServices(MVVM_Color_Utilities.Startup.ConfigureServices);
        }
    }
}