namespace Markwardt;

public interface ICollectionChange
{
    CollectionChangeType ChangeType { get; }
    IEnumerable<object> Items { get; }
}

public interface ICollectionChange<out T> : ICollectionChange
    where T : notnull
{
    new IEnumerable<T> Items { get; }
}

public static class CollectionChange
{
    public static ICollectionChange<T> Add<T>(IEnumerable<T> items)
        where T : notnull
        => new CollectionChange<T>(CollectionChangeType.Add, items);

    public static ICollectionChange<T> Remove<T>(IEnumerable<T> items)
        where T : notnull
        => new CollectionChange<T>(CollectionChangeType.Remove, items);

    public static ICollectionChange<T> Clear<T>()
        where T : notnull
        => new CollectionChange<T>(CollectionChangeType.Clear, []);
}

public record CollectionChange<T>(CollectionChangeType ChangeType, IEnumerable<T> Items) : ICollectionChange<T>
    where T : notnull
{
    [SuppressMessage("Sonar Code Quality", "S1144")]
    IEnumerable<object> ICollectionChange.Items => Items.Cast<object>();
}