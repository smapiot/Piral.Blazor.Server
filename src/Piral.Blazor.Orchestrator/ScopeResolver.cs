using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace Piral.Blazor.Orchestrator;

public interface IScopeResolver : IDisposable
{
    IServiceProvider Resolve(IServiceProvider scope);
}

internal class ScopeResolver : IScopeResolver
{
    private readonly IServiceCollection _services;
    private readonly ConditionalWeakTable<IServiceProvider, IServiceProvider> _mapping;

    public ScopeResolver(IServiceCollection services)
    {
        _services = services;
        _mapping = new();
    }

    public void Dispose()
    {
        foreach (var entry in _mapping)
        {
            var scope = entry.Value?.GetService<ILifetimeScope>();
            scope?.Dispose();
        }

        _mapping.Clear();
    }

    public IServiceProvider Resolve(IServiceProvider parentProvider)
    {
        if (!_mapping.TryGetValue(parentProvider, out var childProvider))
        {
            var scope = parentProvider.GetService<ILifetimeScope>()!;
            var childScope = scope.BeginLifetimeScope(newBuilder =>
            {
                newBuilder.Populate(_services);

                foreach (var registration in scope.ComponentRegistry.Registrations)
                {
                    if (registration.Lifetime == CurrentScopeLifetime.Instance)
                    {
                        var serviceType = registration.Services.OfType<IServiceWithType>().Select(m => m.ServiceType).First();

                        if (serviceType != typeof(IServiceProvider) && serviceType != typeof(ILifetimeScope))
                        {
                            var rb = RegistrationBuilder.ForDelegate(serviceType, (_, _) =>
                                parentProvider.GetService(serviceType)!);

                            newBuilder.RegisterCallback(cr => 
                                RegistrationBuilder.RegisterSingleComponent(cr, rb));
                        }
                    }
                }
            });
            childProvider = new AutofacServiceProvider(childScope);
            _mapping.TryAdd(parentProvider, childProvider);
        }

        return childProvider;
    }
}
