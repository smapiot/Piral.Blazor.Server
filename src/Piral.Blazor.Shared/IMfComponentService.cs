namespace Piral.Blazor.Shared;

public interface IMfComponentService
{
	event EventHandler ComponentsChanged;

	IEnumerable<string> Styles { get; }

    IEnumerable<string> Scripts { get; }

    IEnumerable<string> ComponentNames { get; }

    IEnumerable<Type> GetComponents(string name);
}
