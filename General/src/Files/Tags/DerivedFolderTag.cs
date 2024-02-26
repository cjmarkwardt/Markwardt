namespace Markwardt;

public abstract class DerivedFolderTag<TParentTag> : SourceTag<TParentTag, IFolder>
{
    protected abstract string Name { get; }

    protected override sealed ValueTask<object> Create(IFolder source)
        => ValueTask.FromResult<object>(source.Descend(Name).AsFolder());
}