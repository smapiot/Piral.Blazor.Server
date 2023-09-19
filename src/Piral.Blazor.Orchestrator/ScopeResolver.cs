using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Piral.Blazor.Orchestrator;

public interface IScopeResolver : IDisposable
{
    IServiceProvider Resolve(IServiceProvider scope);
}

internal class ScopeResolver : IScopeResolver
{
    private readonly IServiceCollection _services;
    private IServiceProvider? _provider;

    public ScopeResolver(IServiceCollection services)
    {
        _services = services;
    }

    public void Dispose()
    {
        var scope = _provider?.GetService<ILifetimeScope>();
        scope?.Dispose();
    }

    public IServiceProvider Resolve(IServiceProvider provider)
    {
        if (_provider is null)
        {
            var scope = provider.GetService<ILifetimeScope>()!;
            var childScope = scope.BeginLifetimeScope(newBuilder =>
            {
                newBuilder.Populate(_services);

                foreach (var registration in scope.ComponentRegistry.Registrations)
                {
                    if (registration.Lifetime == CurrentScopeLifetime.Instance)
                    {
                        var serviceType = registration.Services.OfType<IServiceWithType>().Select(m => m.ServiceType).First();
                        var instance = provider.GetService(serviceType);

                        if (instance is not null)
                        {
                            newBuilder
                                .RegisterInstance(instance)
                                .As(registration.Services.ToArray());
                        }
                    }
                }
            });
            _provider = new AutofacServiceProvider(childScope);
        }

        return _provider;
    }
}
