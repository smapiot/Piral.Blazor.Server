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
		var scope = parent.GetService<ILifetimeScope>()!;
		var child = scope.BeginLifetimeScope(newBuilder =>
		{
			newBuilder.Populate(services);

            foreach (var registration in scope.ComponentRegistry.Registrations)
            {
                if (registration.Lifetime == CurrentScopeLifetime.Instance)
                {
                    var serviceType = registration.Services.OfType<IServiceWithType>().Select(m => m.ServiceType).First();

                    if (serviceType != typeof(IServiceProvider) && serviceType != typeof(ILifetimeScope))
                    {
                        var rb = RegistrationBuilder.ForDelegate(serviceType, (_, _) =>
                            parent.GetService(serviceType)!);

                        newBuilder.RegisterCallback(cr => 
                            RegistrationBuilder.RegisterSingleComponent(cr, rb));
                    }
                }
            }
		});
		_provider = new AutofacServiceProvider(child);
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
			var scope = _provider.GetService<ILifetimeScope>()!;
			var childScope = scope.BeginLifetimeScope();
			childProvider = new AutofacServiceProvider(childScope);
			_mapping.Add(parentProvider, childProvider);
        }

        return childProvider;
    }
}
