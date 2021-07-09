using Application.ImageBuffer;
using Application.Palette_Quantizers;
using Application.Palette_Quantizers.Median_Cut;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using Prism.Commands;
using Prism.Mvvm;
using StructureMap;
using System.Collections.Generic;

namespace MVVM_Color_Utilities.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        private IPageViewModel currentPageViewModel;
        private List<IPageViewModel> pageViewModels;

        public MainWindowViewModel()
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
                    _.AddAllTypesOf<IColorQuantizer>();
                    _.WithDefaultConventions();
                });
            });
            var c = container.GetAllInstances<IColorQuantizer>();

            PageViewModels.Add(container.GetInstance<ColorsList_Tab.ColorListViewModel>());
            PageViewModels.Add(container.GetInstance<ImageQuantizer_Tab.ImageQuantizerViewModel>());
            PageViewModels.Add(container.GetInstance<ImageAnalyzer_Tab.ImageAnalyzerViewModel>());

            CurrentPageViewModel = PageViewModels[0];
            ChangePageCommand = new DelegateCommand<IPageViewModel>(p => ChangeViewModel(p),
                                                          p => p is IPageViewModel);
        }

        public DelegateCommand<IPageViewModel> ChangePageCommand { get; }

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
            set => SetProperty(ref currentPageViewModel, value);
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
            CurrentPageViewModel = PageViewModels.Find(vm => vm == viewModel);
        }
    }
}