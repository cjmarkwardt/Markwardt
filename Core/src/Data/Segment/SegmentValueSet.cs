namespace Markwardt;

public static class SegmentValueSet
{
    public static object Create(Type type, IDataHandler handler, DataList list)
        => typeof(SegmentValueSet<>).MakeGenericType(type).GetConstructor([typeof(IDataHandler), typeof(DataList)]).NotNull().Invoke([handler, list]);
}

public class SegmentValueSet<T>(IDataHandler handler, DataList data) : ISet<T>
    where T : notnull
{
    private readonly DataReader<T> reader = new(handler);

    public int Count => data.Count;

    public bool IsReadOnly => false;

    public bool Add(T item)
    {
        IDataNode value = reader.Write(item);
        if (!data.Contains(value))
        {
            data.Add(value);
            return true;
        }

        return false;
    }

    public bool Remove(T item)
        => data.RemoveAll(reader.Write(item)) > 0;

    public void Clear()
        => data.Clear();

    public bool Contains(T item)
        => data.Contains(reader.Write(item));

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public void ExceptWith(IEnumerable<T> other)
        => ReplaceAll(x => x.ExceptWith(other));

    public void IntersectWith(IEnumerable<T> other)
        => ReplaceAll(x => x.IntersectWith(other));

    public void SymmetricExceptWith(IEnumerable<T> other)
        => ReplaceAll(x => x.SymmetricExceptWith(other));

    public void UnionWith(IEnumerable<T> other)
        => ReplaceAll(x => x.UnionWith(other));

    public bool IsProperSubsetOf(IEnumerable<T> other)
        => this.ToHashSet().IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<T> other)
        => this.ToHashSet().IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<T> other)
        => this.ToHashSet().IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other)
        => this.ToHashSet().IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other)
        => other.ToHashSet().Overlaps(this);

    public bool SetEquals(IEnumerable<T> other)
        => other.ToHashSet().SetEquals(this);

    void ICollection<T>.Add(T item)
        => Add(item);

    public IEnumerator<T> GetEnumerator()
        => data.Select(reader.Read).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    private void ReplaceAll(Action<HashSet<T>> action)
    {
        HashSet<T> set = this.ToHashSet();
        action(set);
        data.ReplaceAll(set.Select(reader.Write));
    }
}