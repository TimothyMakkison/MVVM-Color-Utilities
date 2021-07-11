using Application.ImageBuffer;
using Application.Palette_Quantizers;
using Application.Palette_Quantizers.Median_Cut;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using MVVM_Color_Utilities.Models;

namespace MVVM_Color_Utilities
{
    public static class Startup
    {
        public static ServiceProvider BuildService()
        {
            var configuration = GetConfigurationBuilder().Build();
            return BuildServiceProvider(configuration);
        }

        public static IConfigurationBuilder GetConfigurationBuilder()
        {
            return new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json");
        }

        public static ServiceProvider BuildServiceProvider(IConfiguration configuration)
        {
            var services = new ServiceCollection();
            services.ConfigureServices();

            return services.BuildServiceProvider();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IDataContext<ColorModel>>(new ColorDataContext());
            services.AddSingleton<IFileDialog, FileDialog>();

            var defaultImageBuffer = new ImageBuffer(new BitmapScanner(), new MedianCutQuantizer(), 16, new ImageBuilder());
            services.AddSingleton<IImageBuffer>(defaultImageBuffer);
            services.AddSingleton<GeneralSettings>();

            services.AddColorQuantizers(typeof(IColorQuantizer));
            services.AddViewModels(typeof(ColorsList_Tab.ColorListView));
        }
    }
}
