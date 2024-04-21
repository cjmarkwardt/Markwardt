namespace Markwardt;

public interface IInjectable
{
    void SetName(string name);

    ValueTask OnReady(CancellationToken cancellation);
    void OnProcess(double delta);
}