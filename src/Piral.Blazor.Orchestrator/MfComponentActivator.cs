using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

internal class MfComponentActivator : IComponentActivator
{
	private readonly IModuleContainerService _container;
	private readonly ConcurrentDictionary<Type, Action<IServiceProvider, IComponent>> _cachedInitializers;
    private readonly Func<Type, Action<IServiceProvider, IComponent>> _createInitializer;

    public MfComponentActivator(IModuleContainerService container)
	{
		_container = container;

		var cf = typeof(Renderer).Assembly.GetType("Microsoft.AspNetCore.Components.ComponentFactory")!;
		_cachedInitializers = (ConcurrentDictionary<Type, Action<IServiceProvider, IComponent>>)cf.GetField("_cachedInitializers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		_createInitializer = cf.GetMethod("CreateInitializer", BindingFlags.NonPublic | BindingFlags.Static)!.CreateDelegate<Func<Type, Action<IServiceProvider, IComponent>>>();
	}

	public IComponent CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type componentType)
	{
		if (!typeof(IComponent).IsAssignableFrom(componentType))
		{
			throw new ArgumentException($"The type {componentType.FullName} does not implement {nameof(IComponent)}.", nameof(componentType));
		}

		var child = (IComponent)Activator.CreateInstance(componentType)!;

		if (!_cachedInitializers.ContainsKey(componentType))
        {
            var origin = componentType.Assembly;
            var resolver = _container.GetProvider(origin);
			var initializer = _createInitializer(componentType);

			// local DI has been found - use it
			if (resolver is not null)
            {
				_cachedInitializers.TryAdd(componentType, (scope, cmp) =>
				{
					var provider = resolver.Resolve(scope);
                    initializer(provider, cmp);
				});
            }
        }

		return child;
	}
}
