namespace Markwardt;

public interface IKeyLookup<in TKey, out T>
{
    IMaybe<T> KeyedGet(TKey key);
}

public static class KeyLookupExtensions
{
    public static IDictionary<TKey, T> KeyedGet<TKey, T>(this IKeyLookup<TKey, T> lookup, IEnumerable<TKey> keys)
        where TKey : notnull
    {
        Dictionary<TKey, T> items = [];
        foreach (TKey key in keys)
        {
            IMaybe<T> item = lookup.KeyedGet(key);
            if (item.HasValue)
            {
                items[key] = item.Value;
            }
        }

        return items;
    }
}