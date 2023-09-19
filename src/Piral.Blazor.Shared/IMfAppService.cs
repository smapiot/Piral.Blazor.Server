using Microsoft.AspNetCore.Components;

namespace Piral.Blazor.Shared;

public interface IMfAppService : IMfService
{
    void MapComponent<T>(string name) where T : class, IComponent;

    void MapRoute<T>() where T : class, IComponent;

    void AppendScript(string path);

    void PrependStyleSheet(string path);
}
