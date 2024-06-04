namespace Markwardt;

public interface IReadOnlyTwoWayDictionary<TKey, T> : IReadOnlyCollection<KeyValuePair<TKey, T>>
{
    IReadOnlyDictionary<TKey, T> Forward { get; }
    IReadOnlyDictionary<T, TKey> Reverse { get; }

    IEnumerable<TKey> Keys { get; }
    IEnumerable<T> Values { get; }
}

public interface ITwoWayDictionary<TKey, T> : IReadOnlyTwoWayDictionary<TKey, T>, ICollection<KeyValuePair<TKey, T>>
{
    new IDictionary<TKey, T> Forward { get; }
    new IDictionary<T, TKey> Reverse { get; }
}

public static class TwoWayDictionaryExtensions
{
    public static void Add<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, TKey key, T value)
        => dictionary.Forward.Add(key, value);

    public static TKey GetKey<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, T value)
        => dictionary.Reverse.TryGetValue(value, out TKey? key) ? key : throw new InvalidOperationException();

    public static T GetValue<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, TKey key)
        => dictionary.Forward.TryGetValue(key, out T? value) ? value : throw new InvalidOperationException();

    public static bool TryGetKey<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, T value, [MaybeNullWhen(false)] out TKey key)
        => dictionary.Reverse.TryGetValue(value, out key);

    public static bool TryGetValue<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, TKey key, [MaybeNullWhen(false)] out T value)
        => dictionary.Forward.TryGetValue(key, out value);

    public static bool ContainsKey<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, TKey key)
        => dictionary.Forward.ContainsKey(key);

    public static bool ContainsValue<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, T value)
        => dictionary.Reverse.ContainsKey(value);

    public static void RemoveKey<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, TKey key)
        => dictionary.Forward.Remove(key);

    public static void RemoveValue<TKey, T>(this ITwoWayDictionary<TKey, T> dictionary, T value)
        => dictionary.Reverse.Remove(value);
}

public class TwoWayDictionary<TKey, T> : ITwoWayDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public TwoWayDictionary(IEnumerable<KeyValuePair<TKey, T>> items)
    {
        forward = new HandledDictionary<TKey, T>(new ForwardHandler(this));
        reverse = new HandledDictionary<T, TKey>(new ReverseHandler(this));

        items.ForEach(x => forward.Add(x));
    }

    public TwoWayDictionary()
        : this([]) { }

    private readonly HandledDictionary<TKey, T> forward;
    private readonly HandledDictionary<T, TKey> reverse;

    private ICollection<KeyValuePair<TKey, T>> ForwardCollection => forward;

    public IDictionary<TKey, T> Forward => forward;
    public IDictionary<T, TKey> Reverse => reverse;

    public int Count => Forward.Count;
    public bool IsReadOnly => false;

    IReadOnlyDictionary<TKey, T> IReadOnlyTwoWayDictionary<TKey, T>.Forward => forward;
    IReadOnlyDictionary<T, TKey> IReadOnlyTwoWayDictionary<TKey, T>.Reverse => reverse;

    public IEnumerable<TKey> Keys => Forward.Keys;
    public IEnumerable<T> Values => Forward.Values;

    public void Add(KeyValuePair<TKey, T> item)
        => ForwardCollection.Add(item);

    public void Clear()
        => forward.Clear();

    public bool Contains(KeyValuePair<TKey, T> item)
        => ForwardCollection.Contains(item);

    public void CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
        => ForwardCollection.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        => forward.GetEnumerator();

    public bool Remove(KeyValuePair<TKey, T> item)
        => ForwardCollection.Remove(item);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private sealed class ForwardHandler(TwoWayDictionary<TKey, T> dictionary) : HandledDictionary<TKey, T>.IHandler
    {
        public void OnAdd(TKey key, T value)
            => dictionary.reverse.Add(value, key);

        public void OnClear()
            => dictionary.reverse.Clear();

        public void OnRemove(TKey key, T value)
            => dictionary.reverse.Remove(value);

        public void OnSet(TKey key, T value)
            => dictionary.reverse[value] = key;
    }

    private sealed class ReverseHandler(TwoWayDictionary<TKey, T> dictionary) : HandledDictionary<T, TKey>.IHandler
    {
        public void OnAdd(T key, TKey value)
            => dictionary.forward.Add(value, key);

        public void OnClear()
            => dictionary.forward.Clear();

        public void OnRemove(T key, TKey value)
            => dictionary.forward.Remove(value);

        public void OnSet(T key, TKey value)
            => dictionary.forward[value] = key;
    }
}
