using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Piral.Blazor.Shared;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

public class MfRouteView : RouteView
{
    private readonly RenderFragment _renderContent;

    [Inject]
    public IMfComponentService? ComponentService { get; set; }

    [Inject]
    public IPiralConfig? Config { get; set; }

    public MfRouteView()
    {
        _renderContent = RenderPageWithParameters;
    }

    protected override void Render(RenderTreeBuilder builder)
    {
        var pageLayoutType = RouteData.PageType.GetCustomAttribute<LayoutAttribute>()?.LayoutType
            ?? DefaultLayout;

        builder.OpenComponent<LayoutView>(0);
        builder.AddAttribute(1, nameof(LayoutView.Layout), pageLayoutType);
        builder.AddAttribute(2, nameof(LayoutView.ChildContent), _renderContent);
        builder.CloseComponent();
    }

    private void RenderPageWithParameters(RenderTreeBuilder builder)
    {
        var pageType = RouteData.PageType;
        var isEmulator = Config!.IsEmulator;
        var (route, origin, _) = ComponentService!.GetAllRouteComponents(isEmulator).FirstOrDefault(m => m.Component == pageType);
        var isContained = route is not null && origin is not null;

        if (isContained)
        {
            builder.OpenElement(0, "piral-component");
            builder.AddAttribute(1, "name", $"route:{route}");
            builder.AddAttribute(2, "origin", origin);
        }

        builder.OpenComponent(3, RouteData.PageType);

        foreach (var kvp in RouteData.RouteValues)
        {
            builder.AddAttribute(1, kvp.Key, kvp.Value);
        }

        builder.CloseComponent();

        if (isContained)
        {
            builder.CloseElement();
        }
    }
}
