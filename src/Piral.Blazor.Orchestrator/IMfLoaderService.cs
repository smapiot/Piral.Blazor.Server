namespace Piral.Blazor.Orchestrator;

public interface IMfLoaderService
{
	void ConnectMicrofrontends(CancellationToken cancellationToken);

	Task LoadMicrofrontends(CancellationToken cancellationToken);
}