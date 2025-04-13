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
    private readonly ConditionalWeakTable<IServiceProvider, IServiceProvider> _mapping = [];

    public ScopeResolver(IServiceProvider parent, IServiceCollection services)
    {
        _provider = new AutofacServiceProvider(CreateRoot(parent, services));
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

    private static ILifetimeScope CreateRoot(IServiceProvider provider, IServiceCollection services)
    {
        var scope = provider.GetService<ILifetimeScope>()!;

        return scope.BeginLifetimeScope(newBuilder =>
        {
            newBuilder.Populate(services);
            SetupScope(newBuilder, scope.ComponentRegistry.Registrations, provider);
        });
    }

    private static ILifetimeScope CreateChild(IServiceProvider globalProvider, IServiceProvider localParent)
    {
        var scope = globalProvider.GetService<ILifetimeScope>()!;

        return scope.BeginLifetimeScope(newBuilder =>
        {
            var registrations = localParent.GetAutofacRoot().ComponentRegistry.Registrations;
            SetupScope(newBuilder, registrations, localParent);
        });
    }

    private static void SetupScope(ContainerBuilder builder, IEnumerable<IComponentRegistration> registrations, IServiceProvider provider)
    {
        foreach (var registration in registrations)
        {
            if (registration.Lifetime == CurrentScopeLifetime.Instance)
            {
                var serviceType = registration.Services.OfType<IServiceWithType>().Select(m => m.ServiceType).First();

                if (serviceType != typeof(IServiceProvider) && serviceType != typeof(ILifetimeScope))
                {
                    var rb = RegistrationBuilder.ForDelegate(serviceType, (_, _) => provider.GetService(serviceType)!);
                    builder.RegisterCallback(cr => RegistrationBuilder.RegisterSingleComponent(cr, rb));
                }
            }
        }
    }
}
