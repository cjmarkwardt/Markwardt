namespace Markwardt;

public class DataList(IEnumerable<IDataNode> nodes) : IList<IDataNode>, IDataNode
{
    public DataList()
        : this([]) { }

    private readonly List<IDataNode> nodes = nodes.ToList();

    private bool isDirty;

    public IDataNode this[int index]
    {
        get => nodes[index];
        set
        {
            isDirty = true;
            nodes[index] = value;
        }
    }

    public int Count => nodes.Count;

    public bool IsReadOnly => false;

    public bool PopChanges()
    {
        bool hasChanges = isDirty;

        foreach (IDataNode node in this)
        {
            hasChanges = node.PopChanges();
        }

        isDirty = false;
        return hasChanges;
    }

    public void Add(IDataNode item)
        => nodes.Add(item);

    public void Clear()
    {
        isDirty = true;
        nodes.Clear();
    }

    public bool Contains(IDataNode item)
        => nodes.Contains(item);

    public void CopyTo(IDataNode[] array, int arrayIndex)
        => nodes.CopyTo(array, arrayIndex);

    public int IndexOf(IDataNode item)
        => nodes.IndexOf(item);

    public void Insert(int index, IDataNode item)
    {
        isDirty = true;
        nodes.Insert(index, item);
    }

    public bool Remove(IDataNode item)
    {
        isDirty = true;
        return nodes.Remove(item);
    }

    public void RemoveAt(int index)
    {
        isDirty = true;
        nodes.RemoveAt(index);
    }

    public int RemoveAll(IDataNode item)
        => nodes.RemoveAll(x => x.Equals(item));

    public void ReplaceAll(IEnumerable<IDataNode> items)
    {
        nodes.Clear();
        nodes.AddRange(items);
    }

    public IEnumerator<IDataNode> GetEnumerator()
        => nodes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}