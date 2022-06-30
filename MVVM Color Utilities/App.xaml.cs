using Microsoft.Extensions.DependencyInjection;
using MVVM_Color_Utilities.ImageAnalyzer_Tab;
using MVVM_Color_Utilities.ImageQuantizer_Tab;
using MVVM_Color_Utilities.ViewModel;
using Prism.Ioc;
using Prism.Unity;
using System.Windows;

namespace MVVM_Color_Utilities;

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
        containerRegistry.RegisterForNavigation<ImageAnalyzerView, ImageAnalyzerViewModel>();
        containerRegistry.RegisterForNavigation<ImageQuantizerView, ImageQuantizerViewModel>();

        containerRegistry.RegisterServices(MVVM_Color_Utilities.Startup.ConfigureServices);

        AddConcreteViewModels(containerRegistry);
    }

    private static void AddConcreteViewModels(IContainerRegistry containerRegistry)
    {
        //TODO Fix prism DI
        // Prism DI will not correctly pass all instances of a type into a constructor requiring
        // a collection of a given type -  instead injecting only the last item.
        // I use MS.Extensions.DI to correctly instantiate each view model and the register
        // them with prism.
        var provider = MVVM_Color_Utilities.Startup.BuildService();
        containerRegistry.RegisterInstance(provider.GetService<ImageQuantizerViewModel>());
        containerRegistry.RegisterInstance(provider.GetService<ImageAnalyzerViewModel>());
    }
}
