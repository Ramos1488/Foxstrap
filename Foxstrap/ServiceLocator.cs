using Foxstrap.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Foxstrap
{
    public static class ServiceLocator
    {
        private static IServiceProvider? _provider;

        public static IServiceProvider Provider =>
            _provider ?? throw new InvalidOperationException("ServiceLocator not initialized");

        public static void Initialize()
        {
            var services = new ServiceCollection();
            RegisterServices(services);
            _provider = services.BuildServiceProvider();
        }

        private static void RegisterServices(IServiceCollection services)
        {
            // ViewModels
            services.AddTransient<MainViewModel>();

            // Здесь будем добавлять сервисы по мере роста проекта:
            // services.AddSingleton<IUpdateService, UpdateService>();
            // services.AddSingleton<IModService, ModService>();
        }

        public static T Get<T>() where T : notnull =>
            Provider.GetRequiredService<T>();
    }
}