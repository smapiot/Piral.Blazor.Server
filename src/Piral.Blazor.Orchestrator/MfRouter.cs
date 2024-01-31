using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Piral.Blazor.Shared;
using System.Reflection;

namespace Piral.Blazor.Orchestrator;

public class MfRouter : IComponent
{
    private IMfComponentService? _componentService;

    [Inject]
    public IMfComponentService? ComponentService
    {
        get { return _componentService; }
        set { _componentService = value; }
    }

    [Parameter]
    [EditorRequired]
    public Assembly? AppAssembly { get; set; }

    /// <summary>
    /// Gets or sets the content to display when no match is found for the requested route.
    /// </summary>
    [Parameter]
    public RenderFragment? NotFound { get; set; }

    /// <summary>
    /// Gets or sets the content to display when a match is found for the requested route.
    /// </summary>
    [Parameter]
    [EditorRequired]
    public RenderFragment<RouteData>? Found { get; set; }

    private IEnumerable<Assembly> GetAssemblies()
    {
        if (_componentService is not null)
        {
            foreach (var item in _componentService.GetAllRouteComponents().Select(m => m.Component.Assembly).Distinct())
            {
                yield return item;
            }
        }
    }

    private RenderHandle _renderHandle;

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        _renderHandle.Render(builder =>
        {
            builder.OpenComponent(0, typeof(Router));
            builder.AddComponentParameter(0, "AppAssembly", AppAssembly);
            builder.AddComponentParameter(1, "Found", Found);
            builder.AddComponentParameter(2, "NotFound", NotFound);
            builder.AddComponentParameter(3, "AdditionalAssemblies", GetAssemblies());
            builder.CloseComponent();
        });

        return Task.CompletedTask;
    }
}
