using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Piral.Blazor.Shared;

namespace Piral.Blazor.Orchestrator;

public class MfRouteView : RouteView
{
    private readonly ConcurrentDictionary<Type, Type?>? _cache;

    [Inject]
    public IMfComponentService? ComponentService { get; set; }

    public MfRouteView()
    {
        _cache = GetType()
            .GetField("_layout", BindingFlags.Static | BindingFlags.NonPublic)!
            .GetValue(null) as ConcurrentDictionary<Type, Type?>;
    }

    protected override void Render(RenderTreeBuilder builder)
    {
        var pageType = RouteData.PageType;
        var origin = ComponentService?.GetComponentOrigin(pageType);

        if (origin is not null)
        {
            _cache?.GetOrAdd(pageType, GetWrapper);
        }

        base.Render(builder);
    }

    private Type GetWrapper(Type pageType)
    {
        var layoutType = pageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType ?? DefaultLayout;
        return Type.MakeGenericSignatureType(typeof(LayoutWrapper<,>), layoutType, pageType);
    }
}

class LayoutWrapper<TLayout, TComponent> : LayoutComponentBase
    where TLayout : LayoutComponentBase, new()
{
    [Inject]
    public IMfComponentService? ComponentService { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenComponent<LayoutView>(0);
        builder.AddAttribute(1, nameof(LayoutView.Layout), typeof(TLayout));
        builder.AddAttribute(2, nameof(LayoutView.ChildContent), (RenderFragment)RenderContent);
        builder.CloseComponent();
    }

    private void RenderContent(RenderTreeBuilder builder)
    {
        var (Route, Microfrontend, _) = ComponentService!.GetAllRouteComponents().First(m => m.Component == typeof(TComponent));
        builder.OpenElement(0, "piral-component");
        builder.AddAttribute(1, "name", $"route:{Route}");
        builder.AddAttribute(2, "origin", Microfrontend);
        builder.AddContent(3, Body);
        builder.CloseElement();
    }
}
