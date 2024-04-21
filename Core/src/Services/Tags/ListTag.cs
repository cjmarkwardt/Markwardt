namespace Markwardt;

public class ListTag<T> : DelegateTag
    where T : notnull
{
    protected override ValueTask<object> Create(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments)
    {
        ObservableList<T> list = [];
        Initialize(list);
        return ValueTask.FromResult<object>(list);
    }

    protected virtual void Initialize(IObservableList<T> list) { }
}