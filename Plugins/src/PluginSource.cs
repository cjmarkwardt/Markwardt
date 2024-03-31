namespace Markwardt;

public interface IPluginSource : IExtendedDisposable
{
    IObservableReadOnlyCache<string, IPluginModule> Modules { get; }

    ValueTask Refresh(bool purge = true);
}