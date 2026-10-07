// Copyright (c) 2026 Brianesi
// SPDX-License-Identifier: MIT

using Nesi.UI.Services;

namespace Nesi.UI;

public class ServiceManager : IServiceProvider, IDisposable
{
    private readonly Dictionary<Type, object> _services = new();
    private readonly Dictionary<Type, Type> _registrations = new();
    private bool _disposed;

    public void Register<TInterface, TImplementation>() where TImplementation : TInterface, new()
    {
        _registrations[typeof(TInterface)] = typeof(TImplementation);
    }

    public object? GetService(Type serviceType)
    {
        if (_services.TryGetValue(serviceType, out var service))
            return service;

        if (_registrations.TryGetValue(serviceType, out var implementationType))
        {
            var instance = Activator.CreateInstance(implementationType);
            _services[serviceType] = instance!;
            return instance;
        }

        return null;
    }

    public T? GetService<T>() where T : class
    {
        return GetService(typeof(T)) as T;
    }

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var service in _services.Values)
        {
            if (service is IDisposable disposable)
                disposable.Dispose();
        }

        _services.Clear();
        _registrations.Clear();
        _disposed = true;
    }
}

