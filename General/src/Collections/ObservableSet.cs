namespace Markwardt;

public class ObservableSet<T> : ObservableList<T>
    where T : notnull
{
    public ObservableSet(IEnumerable<T>? items = null)
        : base(items)
        => Filter = x => !Contains(x);
}