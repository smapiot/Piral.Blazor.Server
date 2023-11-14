namespace Piral.Blazor.Shared;

public interface IMfComponentService
{
	event EventHandler ComponentsChanged;

	IEnumerable<string> Styles { get; }

    IEnumerable<string> Scripts { get; }

    IEnumerable<string> ComponentNames { get; }

    IEnumerable<(string Name, string Microfrontend, Type Component)> Components { get; }

    IEnumerable<(string Microfrontend, Type Component)> GetComponents(string name);
}
