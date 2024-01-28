namespace Markwardt;

public abstract class LocalFileNodeTag : SourceTag<LocalFileTreeTag, IFileTree>
{
    public abstract string Path { get; }

    protected override ValueTask<object> Create(IFileTree source)
        => ValueTask.FromResult<object>(source.Descend(Path.SplitPath()).AscendRoot());
}