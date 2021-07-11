using Application.Palette_Quantizers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MVVM_Color_Utilities.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddColorQuantizers(this IServiceCollection services,
            params Type[] assemblies)
        {
            return services.Scan(scan => scan.FromAssembliesOf(assemblies)
            .AddClasses(classes => classes.AssignableTo<IColorQuantizer>())
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        }

        public static IServiceCollection AddViewModels(this IServiceCollection services,
        params Type[] assemblies)
        {
            return services.Scan(scan => scan.FromAssembliesOf(assemblies)
            .AddClasses(classes => classes.Where(t => t.Name.Contains("ViewModel")))
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
        }
    }
}