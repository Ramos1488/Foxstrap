using System;
using System.Collections.Generic;

namespace Foxstrap
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service) where T : notnull
            => _services[typeof(T)] = service;

        public static T Get<T>()
        {
            if (_services.TryGetValue(typeof(T), out var svc))
                return (T)svc;
            throw new InvalidOperationException($"Service {typeof(T).Name} not registered.");
        }
    }
}