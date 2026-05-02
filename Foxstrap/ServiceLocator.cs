using Foxstrap.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            services.AddTransient<MainViewModel>();
            _provider = services.BuildServiceProvider();
        }

        public static T Get<T>() where T : notnull =>
            Provider.GetRequiredService<T>();
    }
}
