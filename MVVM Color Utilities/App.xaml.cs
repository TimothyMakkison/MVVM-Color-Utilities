using Application.ImageBuffer;
using Application.Palette_Quantizers;
using Application.Palette_Quantizers.Median_Cut;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using MVVM_Color_Utilities.ViewModel;
using Prism.Ioc;
using Prism.Unity;
using StructureMap;
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
            var structureContainer = GetStructureMapContainer();
            containerRegistry.RegisterInstance(structureContainer.GetInstance<MainWindowViewModel>());
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }

        private Container GetStructureMapContainer()
        {
            var defaultImageBuffer = new ImageBuffer(new BitmapScanner(), new MedianCutQuantizer(), 16, new ImageBuilder());
            var container = new Container(_ =>
            {
                _.ForSingletonOf<ColorDataContext>().Use(new ColorDataContext());
                _.For<IFileDialog>().Use<FileDialog>();
                _.For<IImageBuffer>().Use(defaultImageBuffer);
                _.Scan(_ =>
                {
                    _.AssemblyContainingType<IColorQuantizer>();
                    _.AssemblyContainingType<IPageViewModel>();

                    _.AddAllTypesOf<IColorQuantizer>();
                    _.AddAllTypesOf<IPageViewModel>();
                    _.WithDefaultConventions();
                });
            });
            return container;
        }
    }
}