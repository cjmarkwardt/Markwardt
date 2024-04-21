namespace Markwardt;

public interface IPluginSource : IExtendedDisposable
{
    IObservableReadOnlyLookupList<string, IPluginModule> Modules { get; }

    ValueTask Refresh(bool purge = true);
}