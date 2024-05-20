namespace Markwardt;

public class DataDictionary(IEnumerable<KeyValuePair<string, IDataNode>> nodes) : IDictionary<string, IDataNode>, IDataNode
{
    public DataDictionary()
        : this([]) { }

    private readonly Dictionary<string, IDataNode> nodes = nodes.ToDictionary(x => x.Key, x => x.Value);

    private bool isDirty;

    public ICollection<string> Keys => nodes.Keys;
    public ICollection<IDataNode> Values => nodes.Values;
    public int Count => nodes.Count;
    public bool IsReadOnly => false;

    private string? type;
    public string? Type
    {
        get => type;
        set
        {
            type = value;
            isDirty = true;
        }
    }

    public IDataNode this[string key]
    {
        get => nodes[key];
        set
        {
            isDirty = true;
            nodes[key] = value;
        }
    }

    public IDataNode? Get(string key)
        => TryGetValue(key, out IDataNode? node) ? node : null;

    public void Set(string key, IDataNode? node)
    {
        if (node is null)
        {
            Remove(key);
        }
        else
        {
            this[key] = node;
        }
    }

    public bool PopChanges()
    {
        bool hasChanges = isDirty;
        
        foreach (IDataNode node in Values)
        {
            hasChanges = node.PopChanges();
        }

        isDirty = false;
        return hasChanges;
    }

    public void Add(string key, IDataNode value)
    {
        isDirty = true;
        nodes.Add(key, value);
    }

    public bool ContainsKey(string key)
        => nodes.ContainsKey(key);

    public bool Remove(string key)
    {
        isDirty = true;
        return nodes.Remove(key);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out IDataNode value)
        => nodes.TryGetValue(key, out value);

    public void Add(KeyValuePair<string, IDataNode> item)
    {
        isDirty = true;
        ((ICollection<KeyValuePair<string, IDataNode>>)nodes).Add(item);
    }

    public void Clear()
    {
        isDirty = true;
        nodes.Clear();
    }

    public bool Contains(KeyValuePair<string, IDataNode> item)
        => nodes.Contains(item);

    public void CopyTo(KeyValuePair<string, IDataNode>[] array, int arrayIndex)
        => ((ICollection<KeyValuePair<string, IDataNode>>)nodes).CopyTo(array, arrayIndex);

    public bool Remove(KeyValuePair<string, IDataNode> item)
    {
        isDirty = true;
        return ((ICollection<KeyValuePair<string, IDataNode>>)nodes).Remove(item);
    }

    public IEnumerator<KeyValuePair<string, IDataNode>> GetEnumerator()
        => nodes.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}