namespace Markwardt;

public enum ItemDisposal
{
    /// <summary> Items are never disposed. </summary>
    None,

    /// <summary> Items are disposed when they are removed from the collection. </summary>
    OnRemoval,

    /// <summary> Items are disposed when the collection is disposed. </summary>
    OnDisposal,

    /// <summary> Items are disposed on removal and on disposal. </summary>
    Full
}