using Microsoft.AspNetCore.Components;
using System.Diagnostics.CodeAnalysis;

namespace Piral.Blazor.Orchestrator;

internal class MfComponentActivator(IModuleContainerService container, ICacheManipulatorService cacheManipulator) : IComponentActivator
{
    private readonly IModuleContainerService _container = container;
    private readonly ICacheManipulatorService _cacheManipulator = cacheManipulator;
    private readonly HashSet<Type> _seen = [];

    public IComponent CreateInstance([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type componentType)
    {
        if (!typeof(IComponent).IsAssignableFrom(componentType))
        {
            throw new ArgumentException($"The type {componentType.FullName} does not implement {nameof(IComponent)}.", nameof(componentType));
        }

        if (_seen.Add(componentType))
        {
            var origin = componentType.Assembly;
            var resolver = _container.GetProvider(origin);

            // local DI has been found - use it
            if (resolver is not null)
            {
                // create wrapper; for the reason see below
                var type = typeof(WrapperComponent<>).MakeGenericType(componentType);
                _cacheManipulator.UpdateComponentCache(componentType, resolver);
                componentType = type;
            }
        }

        return (IComponent)Activator.CreateInstance(componentType)!;
    }

    /// <summary>
    /// This is just an ugly hack to tell Blazor that the
    /// actual component is a different component - forcing
    /// a cache re-validation, which allows us to place the
    /// actual provider in the updated cache, which otherwise
    /// would have been taken out / prepared for us already.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    sealed class WrapperComponent<T> : IComponent
    {
        private RenderHandle _renderHandle;

        public void Attach(RenderHandle renderHandle)
        {
            _renderHandle = renderHandle;
        }

        public Task SetParametersAsync(ParameterView parameters)
        {
            var ps = parameters.ToDictionary();
            _renderHandle.Render(builder =>
            {
                builder.OpenComponent(0, typeof(T));
                builder.AddMultipleAttributes(1, ps!);
                builder.CloseComponent();
            });
            return Task.CompletedTask;
        }
    }
}
