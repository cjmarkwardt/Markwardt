namespace Markwardt;

public class PluginSourcesTag : DelegateTag
{
    protected override ValueTask<object> Create(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments)
        => ValueTask.FromResult<object>(new ObservableList<IPluginSource>());
}