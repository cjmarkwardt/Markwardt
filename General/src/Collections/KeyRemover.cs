namespace Markwardt;

public interface IKeyRemover<in TKey>
{
    void KeyedRemove(IEnumerable<TKey> keys);
}

public static class KeyRemoverExtensions
{
    public static void KeyedRemove<TKey>(this IKeyRemover<TKey> remover, params TKey[] keys)
        => remover.KeyedRemove(keys);

    public static void KeyedRemove<TKey>(this IKeyRemover<TKey> remover, TKey key)
        => remover.KeyedRemove([key]);
}