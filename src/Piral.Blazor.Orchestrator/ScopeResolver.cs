using Autofac;
using Autofac.Builder;
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
    private readonly IServiceProvider _provider;
    private readonly IServiceCollection _services;
    private readonly ConditionalWeakTable<IServiceProvider, IServiceProvider> _mapping = [];

    public ScopeResolver(IServiceProvider parent, IServiceCollection services)
    {
        _services = services;
        _provider = new AutofacServiceProvider(CreateChild(parent, parent));
        _mapping.Add(parent, _provider);
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
            var childScope = CreateChild(_provider, parentProvider);
            childProvider = new AutofacServiceProvider(childScope);
            _mapping.Add(parentProvider, childProvider);
        }

        return childProvider;
    }

    private ILifetimeScope CreateChild(IServiceProvider globalProvider, IServiceProvider localParent)
    {
        var scope = globalProvider.GetService<ILifetimeScope>()!;

        return scope.BeginLifetimeScope(newBuilder =>
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
                            localParent.GetService(serviceType)!);

                        newBuilder.RegisterCallback(cr => 
                            RegistrationBuilder.RegisterSingleComponent(cr, rb));
                    }
                }
            }
        });
    }
}
