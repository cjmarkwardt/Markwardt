namespace Markwardt;

public interface IPluginSource : IComplexDisposable
{
    IObservableReadOnlyCache<string, IPluginModule> Modules { get; }

    ValueTask Refresh(bool purge = true);
}