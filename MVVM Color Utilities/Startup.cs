using Application.ImageBuffer;
using Application.ImageBuffer.BitmapScanner;
using Application.ImageBuffer.ImageBuilder;
using Application.Palette_Quantizers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MVVM_Color_Utilities.Helpers;
using MVVM_Color_Utilities.Infrastructure;
using MVVM_Color_Utilities.Models;
using Serilog;

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
            services.AddSingleton<IDataContext<ColorModel>, ColorDataContext>();
            services.AddSingleton<IFileDialog, FileDialog>();

            services.AddSingleton<IImageBuffer, ImageBuffer>();
            services.AddSingleton<IBitmapScanner, BitmapScanner>();
            services.AddSingleton<IImageBuilder, ImageBuilder>();

            services.AddSingleton<GeneralSettings>();

            services.AddColorQuantizers(typeof(IColorQuantizer));
            services.AddViewModels(typeof(ColorsList_Tab.ColorListViewModel));

            var logger = new LoggerConfiguration().WriteTo
                .Debug()
                .CreateLogger();
            services.AddSingleton<ILogger>(logger);

            services.Decorate<IColorQuantizer, CachingColorQuantizer>();
        }
    }
}